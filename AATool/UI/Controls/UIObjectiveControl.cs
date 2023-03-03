using System;
using System.Xml;
using AATool.Data.Objectives;

namespace AATool.UI.Controls
{
    public abstract class UIObjectiveControl : UIControl
    {
        public Objective Objective { get; private set; }
        public string ObjectiveId { get; private set; }
        public string ObjectiveOwnerId { get; private set; }

        protected Type ObjectiveType { get; private set; }

        public bool ObjectiveCompleted => this.Objective?.IsComplete() is true;

        public virtual void SetObjective<T>(string objectiveId, string objectiveOwnerId = null)
        {
            this.ObjectiveId = objectiveId;
            this.ObjectiveOwnerId = objectiveOwnerId ?? string.Empty;
            this.ObjectiveType = typeof(T);
            this.AutoSetObjective();
        }

        public virtual void SetObjective(Objective objective)
        {
            this.Objective = objective;
            this.ObjectiveId = objective?.Id;
            this.ObjectiveType = objective?.GetType();
            this.ObjectiveOwnerId = objective is Criterion criterion ? criterion.OwnerId : string.Empty;
        }

        public virtual void AutoSetObjective()
        {
            if (this.ObjectiveType == typeof(Advancement))
            {
                if (Tracker.TryGetAdvancement(this.ObjectiveId, out Advancement objective))
                    this.SetObjective(objective);
            }
            else if (this.ObjectiveType == typeof(Criterion))
            {
                if (Tracker.TryGetCriterion(this.ObjectiveOwnerId, this.ObjectiveId, out Criterion criterion))
                    this.SetObjective(criterion);
            }
            else if (this.ObjectiveType == typeof(ComplexObjective) || this.ObjectiveType == typeof(Pickup))
            {
                if (Tracker.TryGetComplexObjective(this.ObjectiveId, out ComplexObjective objective))
                    this.SetObjective(objective);
            }
            else if (this.ObjectiveType == typeof(Block))
            {
                if (Tracker.TryGetBlock(this.ObjectiveId, out Block block))
                    this.SetObjective(block);
            }
            else if (this.ObjectiveType == typeof(Death))
            {
                if (Tracker.TryGetDeath(this.ObjectiveId, out Death death))
                    this.SetObjective(death);
            }
        }

        public override void ReadNode(XmlNode node)
        {
            base.ReadNode(node);

            //check if this frame contains an advancement
            this.ObjectiveId = Attribute(node, "advancement", string.Empty);
            if (!string.IsNullOrEmpty(this.ObjectiveId))
            {
                this.ObjectiveType = typeof(Advancement);
                return;
            }

            //check if this frame contains an achievement
            this.ObjectiveId = Attribute(node, "achievement", string.Empty);
            if (!string.IsNullOrEmpty(this.ObjectiveId))
            {
                this.ObjectiveType = typeof(Advancement);
                return;
            }

            //check if this frame contains a criterion
            this.ObjectiveId = Attribute(node, "criterion", string.Empty);
            if (!string.IsNullOrEmpty(this.ObjectiveId))
            {
                this.ObjectiveType = typeof(Criterion);
                this.ObjectiveOwnerId = Attribute(node, "owner", string.Empty);
                return;
            }

            //check if this frame contains a pickup counter
            this.ObjectiveId = Attribute(node, "complex", string.Empty);
            if (!string.IsNullOrEmpty(this.ObjectiveId))
            {
                this.ObjectiveType = typeof(ComplexObjective);
                return;
            }

            //check if this frame contains a block
            this.ObjectiveId = Attribute(node, "block", string.Empty);
            if (!string.IsNullOrEmpty(this.ObjectiveId))
            {
                this.ObjectiveType = typeof(Block);
                return;
            }

            //check if this frame contains a death message
            this.ObjectiveId = Attribute(node, "death", string.Empty);
            if (!string.IsNullOrEmpty(this.ObjectiveId))
            {
                this.ObjectiveType = typeof(Death);
                return;
            }

            //check if this frame contains a death message
            this.ObjectiveId = Attribute(node, "pickup", string.Empty);
            if (!string.IsNullOrEmpty(this.ObjectiveId))
            {
                this.ObjectiveType = typeof(Pickup);
                return;
            }
        }
    }
}
