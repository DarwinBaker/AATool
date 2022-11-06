using System;
using System.Linq;
using System.Xml;
using AATool.Configuration;
using AATool.Data.Progress;
using AATool.Net;
using AATool.Utilities;

namespace AATool.Data.Objectives
{
    public class Advancement : Objective
    {
        public Uuid DesignatedPlayer   { get; protected set; }
        public bool DesignationLinked  { get; protected set; }
        public CriteriaSet Criteria    { get; private set; }
        
        public readonly bool HiddenWhenRelaxed;
        public readonly bool HiddenWhenCompact;
        public readonly bool UsedInHalfPercent;

        public override string FullStatus => this.Name;
        public override string TinyStatus => this.ShortName;

        public bool HasCriteria => this.Criteria.Any;

        public Advancement(XmlNode node) : base(node)
        {
            this.Criteria = new CriteriaSet(node?.SelectSingleNode("criteria"), this);
            this.UsedInHalfPercent = XmlObject.Attribute(node, "half", true);

            string hideMode = XmlObject.Attribute(node, "hidden", "false");
            if (bool.TryParse(hideMode, out bool hidden))
            {
                this.HiddenWhenRelaxed = hidden;
                this.HiddenWhenCompact = hidden;
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
            this.LinkDesignation();
        }

        public void LinkDesignation() => this.DesignationLinked = true;
        public void UnlinkDesignation() => this.DesignationLinked = false;

        public void Designate(Uuid id)
        {
            if (id == Uuid.Empty)
                id = new Uuid(id.String);
            if (id == Uuid.Empty)
                return;

            if (id != this.DesignatedPlayer || this.DesignationLinked)
            {
                this.DesignatedPlayer = id;
                Tracker.InvalidateDesignations();

                if (Server.TryGet(out Server server))
                    server.DesignatePlayer(this.Id, this.DesignatedPlayer);
                else
                    Server.PrepareDesignation(this.Id, this.DesignatedPlayer);
            }
        }

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
            return this.DesignatedPlayer != Uuid.Empty 
                ? this.DesignatedPlayer 
                : Tracker.GetMainPlayer();
        }

        public override void UpdateState(ProgressState progress)
        {
            base.UpdateState(progress);
            this.Criteria.UpdateStates(Tracker.State);

            if (!this.HasCriteria)
                return;

            //handle auto-designation
            if (!(Tracker.Invalidated || Peer.StateChanged || Config.Tracking.FilterChanged))
                return;

            if (Peer.IsConnected)
            {
                if (Peer.IsServer)
                {
                    if (this.DesignatedPlayer != Uuid.Empty)
                        this.Designate(this.DesignatedPlayer);
                    else
                        this.Designate(this.Criteria.ClosestToCompletion);
                }
                else if (this.DesignationLinked 
                    && Peer.TryGetLobby(out Lobby lobby) 
                    && lobby.Designations.TryGetValue(this.Id, out Uuid player))
                {
                    this.DesignatedPlayer = player;
                }
            }
            else
            {
                //assign a default based on filter mode
                if (Config.Tracking.Filter == ProgressFilter.Combined)
                    this.Designate(this.Criteria.ClosestToCompletion);
                else if (Player.TryGetUuid(Config.Tracking.SoloFilterName, out Uuid player))
                    this.Designate(player);
            }
        }

        protected void ParseCriteria(XmlNode advancementNode)
        {
            //initialize criteria if this advancement has any
            XmlNode criteriaNode = advancementNode.SelectSingleNode("criteria");
            if (criteriaNode is not null)
            {
                this.Criteria = new CriteriaSet(criteriaNode, this);
                this.Designate(Uuid.Empty);
            }
        }
    }
}