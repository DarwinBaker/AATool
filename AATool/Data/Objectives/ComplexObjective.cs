using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml;
using AATool.Data.Progress;
using AATool.Utilities;

namespace AATool.Data.Objectives
{
    public abstract class ComplexObjective : Objective
    {   
        //static dictionary to hold all control types for dynamic instantiation
        public static readonly Dictionary<string, Type> Types = (
            from t in Assembly.GetExecutingAssembly().GetTypes()
            where t.IsClass && t.Namespace == "AATool.Data.Objectives.Complex"
            select t).ToDictionary(t => t.Name.ToString().ToLower(),
            t => t, StringComparer.OrdinalIgnoreCase);

        public static bool TryCreateInstance(string type, out ComplexObjective objective)
        {
            //if type name is valid, create instance of type
            if (Types.TryGetValue(type, out Type realType))
            {
                objective = Activator.CreateInstance(realType) as ComplexObjective;
                return true;
            }
            objective = null;
            return false;
        }

        private string fullStatus;
        private string shortStatus;

        public sealed override string GetFullCaption() => this.fullStatus;
        public sealed override string GetShortCaption() => this.shortStatus;

        public ComplexObjective(XmlNode node = null) : base(node)
        {
            this.Id = XmlObject.Attribute(node, "id", string.Empty);
            this.Name = XmlObject.Attribute(node, "name", string.Empty);

            this.Icon = XmlObject.Attribute(node, "icon", string.Empty);
            if (string.IsNullOrEmpty(this.Icon))
                this.Icon = this.Id.Split(':').LastOrDefault();

            this.fullStatus = this.GetLongStatus();
            this.shortStatus = this.GetShortStatus();
        }

        protected abstract string GetLongStatus();
        protected abstract string GetShortStatus();
        protected abstract void UpdateAdvancedState(ProgressState progress);
        protected abstract void ClearAdvancedState();

        public sealed override void UpdateState(ProgressState progress)
        {
            if (Tracker.WorldChanged || Tracker.SavesFolderChanged || !Tracker.IsWorking)
                this.ManuallyChecked = false;

            if (progress is not null)
            { 
                this.UpdateAdvancedState(progress);         
            }
            else
            { 
                this.ClearAdvancedState();
                this.CompletionOverride = this.ManuallyChecked;
            }

            this.fullStatus = this.GetLongStatus();
            this.shortStatus = this.GetShortStatus();
        }
    }
}
