using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using AATool.Data.Progress;
using AATool.Net;
using AATool.Settings;
using AATool.Utilities;

namespace AATool.Data
{
    public class Advancement : IAchievable
    {
        public string Id                { get; protected set; }
        public Uuid FirstCompletionist  { get; protected set; }
        public Uuid DesignatedPlayer    { get; protected set; }
        public bool DesignationLinked   { get; protected set; }
        public CriteriaSet Criteria     { get; protected set; }

        public readonly List<Uuid> Completionists;     
        public readonly string Name;
        public readonly string ShortName;
        public readonly string Icon;
        public readonly bool HiddenWhenRelaxed;
        public readonly bool HiddenWhenCompact;
        public readonly FrameType Type;

        public Advancement(XmlNode node)
        {
            //initialize members from xml 
            this.Completionists = new();

            this.Id        = XmlObject.ParseAttribute(node, "id", string.Empty);
            this.Icon      = XmlObject.ParseAttribute(node, "icon", string.Empty);
            this.Name      = XmlObject.ParseAttribute(node, "name", this.Id);
            this.ShortName = XmlObject.ParseAttribute(node, "short_name", this.Name);

            if (string.IsNullOrEmpty(this.Icon))
            {
                string[] idParts = this.Id.Split('/');
                if (idParts.Length > 0)
                    this.Icon = idParts[idParts.Length - 1];
            }

            if (Enum.TryParse(node.Attributes["type"]?.Value, true, out FrameType parsed))
                this.Type = parsed;

            string hideMode = node.Attributes["hidden"]?.Value;
            if (bool.TryParse(hideMode, out bool hidden))
            {
                this.HiddenWhenRelaxed = hidden;
                this.HiddenWhenCompact = true;
            }
            else
            {
                this.HiddenWhenRelaxed = hideMode is "relaxed";
                this.HiddenWhenCompact = hideMode is "compact";
            }

            this.ParseCriteria(node);
            if (this.HasCriteria && Peer.IsServer && Peer.TryGetLobby(out Lobby lobby))
            {
                if (lobby.Designations.TryGetValue(this.Id, out Uuid player))
                    this.Designate(player);
            }
        }

        public bool HasCriteria => this.Criteria is not null;

        public bool TryGetCriteria(out CriteriaSet criteria)
        {
            criteria = this.Criteria;
            return this.HasCriteria;
        }

        public bool CompletedByAnyone() => this.Completionists.Any();

        public bool CompletedBy(Uuid player) => this.Completionists.Contains(player);

        public void Designate(Uuid id)
        {
            if (id == Uuid.Empty)
                return;

            this.DesignatedPlayer = id;
            if (Server.TryGet(out Server server))
                server.DesignatePlayer(this.Id, this.DesignatedPlayer);
        }
         
        public void LinkDesignation()   => this.DesignationLinked = true;
        public void UnlinkDesignation() => this.DesignationLinked = false;

        public Uuid GetDesignatedPlayer()
        {
            if (this.DesignationLinked && Peer.IsClient && Peer.IsConnected)
            {
                if (Peer.TryGetLobby(out Lobby lobby))
                {
                    //auto-sync with server
                    lobby.Designations.TryGetValue(this.Id, out Uuid serverDesignation);
                    return serverDesignation;
                }
            }
            return this.DesignatedPlayer;
        }

        public void Update(ProgressState progress)
        {
            this.Completionists.Clear();
            this.Completionists.AddRange(progress.CompletionistsOf(this));
            this.Criteria?.Update(progress);
            this.UpdateFirstCompletionist();

            if (!this.HasCriteria)
                return;

            //handle auto-designation
            if (!Tracker.Invalidated && !Peer.StateChangedFlag)
                return;

            if (Peer.IsConnected)
            {
                if (Peer.IsServer)
                {
                    //server just started
                    this.Designate(this.DesignatedPlayer);
                }
                else
                {
                    //client just started
                    this.LinkDesignation();
                }
            }
            else
            {
                //assign to player with most progress so far
                this.Designate(this.Criteria.ClosestToCompletion);
            }
        }

        private void UpdateFirstCompletionist()
        {
            //attempt to reset first completionist
            if (this.FirstCompletionist != Uuid.Empty)
            {
                bool clearFirstCompletionist = Config.Tracker.UseDefaultPathChanged();
                clearFirstCompletionist |= !Config.Tracker.UseDefaultPath && Config.Tracker.CustomPathChanged();
                clearFirstCompletionist |= Tracker.WorldFolderChanged;
                if (clearFirstCompletionist)
                    this.FirstCompletionist = Uuid.Empty;
            }

            //lock in first player to give credit to
            if (this.FirstCompletionist == Uuid.Empty && this.Completionists.Count > 0)
                this.FirstCompletionist = this.Completionists.First();
        }

        protected void ParseCriteria(XmlNode advancementNode)
        {
            //initialize criteria if this advancement has any
            XmlNode criteriaNode = advancementNode.SelectSingleNode("criteria");
            if (criteriaNode is null)
                return;

            this.Criteria = new CriteriaSet(criteriaNode, this);
            this.Designate(Uuid.Empty);
        }
    }
}