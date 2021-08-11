using AATool.Data.Progress;
using System.Xml;

namespace AATool.Data
{
    public class Statistic
    {
        public const string FRAME_COMPLETE   = "frame_count_complete";
        public const string FRAME_INCOMPLETE = "frame_count_incomplete";

        public string ID        { get; private set; }
        public string Name      { get; private set; }
        public int PickedUp     { get; private set; }
        public int TargetCount  { get; private set; }
        public string Icon      { get; private set; }
        public bool IsCompleted { get; private set; }
        public bool IsEstimate  { get; private set; }

        public string CurrentFrame  => this.IsCompleted ? FRAME_COMPLETE : FRAME_INCOMPLETE;

        public Statistic(XmlNode node)
        {
            //initialize members from xml 
            this.ID   = node.Attributes["id"]?.Value;
            this.Name = node.Attributes["name"]?.Value ?? this.ID;
            this.Icon = node.Attributes["icon"]?.Value;
            if (int.TryParse(node.Attributes["target_count"]?.Value, out int parsed))
                this.TargetCount = parsed;
            if (bool.TryParse(node.Attributes["estimate"]?.Value, out bool isEstimate))
                this.IsEstimate = isEstimate;
            if (this.Icon is null)
            {
                string[] idParts = this.ID.Split(':');
                if (idParts.Length > 0)
                    this.Icon = idParts[idParts.Length - 1];
            }
        }

        public void Update(ProgressState progress)
        {
            this.PickedUp = progress.ItemCount(this.ID);
            this.IsCompleted = this.PickedUp >= this.TargetCount;
        }
    }
}
