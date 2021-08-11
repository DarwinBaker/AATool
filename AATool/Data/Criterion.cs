using AATool.Data.Progress;
using AATool.Net;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml;

namespace AATool.Data
{
    public class Criterion : IAchievable
    {
        public Advancement ParentAdvancement    { get; private set; }
        public HashSet<Uuid> Completionists     { get; private set; }
        public string ID                        { get; private set; }
        public string Name                      { get; private set; }
        public string ShortName                 { get; private set; }
        public string Icon                      { get; private set; }
        public string ParentID                  { get; private set; }

        public Uuid DesignatedPlayer     => this.ParentAdvancement.DesignatedPlayer;

        public bool CompletedByAnyone()  => this.Completionists.Any();
        public bool CompletedBy(Uuid id) => this.Completionists.Contains(id);

        public Criterion(XmlNode node, Advancement advancement)
        {
            //initialize members from xml 
            this.Completionists    = new();
            this.ParentAdvancement = advancement;
            this.ID                = node.Attributes["id"]?.Value;

            string[] idParts = this.ID.Split(':');    
            string shortID   = idParts.Length > 0 
                ? idParts[idParts.Length - 1] 
                : null;

            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
            this.Name = node.Attributes["name"]?.Value 
                ?? textInfo.ToTitleCase(shortID.Replace('_', ' '));

            this.ShortName = node.Attributes["short_name"]?.Value 
                ?? this.Name;

            this.Icon = node.Attributes["icon"]?.Value 
                ?? shortID.Replace(' ', '_').ToLower();
        }

        public void Update(ProgressState progress)
        {
            this.Completionists.Clear();
            this.Completionists.UnionWith(progress.CompletionistsOf(this));
        }
    }
}
