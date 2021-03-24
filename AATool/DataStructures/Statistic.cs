using AATool.Settings;
using System.Xml;

namespace AATool.DataStructures
{
    public class Statistic
    {
        public const string FRAME_COMPLETE   = "frame_count_complete";
        public const string FRAME_INCOMPLETE = "frame_count_incomplete";

        public string ID            { get; private set; }
        public string Name          { get; private set; }
        public int PickedUp         { get; private set; }
        public int Mined            { get; private set; }
        public int TargetCount      { get; private set; }
        public string Icon          { get; private set; }
        public bool IsCompleted     { get; private set; }
        public bool IsEstimate      { get; private set; }
        public string CurrentFrame  => IsCompleted ? FRAME_COMPLETE : FRAME_INCOMPLETE;

        public Statistic(XmlNode node)
        {
            //initialize members from xml 
            ID   = node.Attributes["id"]?.Value;
            Name = node.Attributes["name"]?.Value ?? ID;
            Icon = node.Attributes["icon"]?.Value;
            if (int.TryParse(node.Attributes["target_count"]?.Value, out int parsed)) 
                TargetCount = parsed;
            if (bool.TryParse(node.Attributes["estimate"]?.Value, out bool isEstimate)) 
                IsEstimate = isEstimate;
            if (Icon == null)
            {
                var idParts = ID.Split(':');
                if (idParts.Length > 0)
                    Icon = idParts[idParts.Length - 1];
            }
        }

        public void Update(StatisticsJSON statistics)
        {
            if (TrackerSettings.IsPostExplorationUpdate)
            {
                PickedUp = statistics.PickedUpCount(ID);
                Mined = statistics.MinedCount(ID);
                IsCompleted = PickedUp >= TargetCount;
            }
            else
            {
                PickedUp = statistics.PickedUpCountOld(ID);
                Mined = statistics.MinedCountOld(ID);
                IsCompleted = PickedUp >= TargetCount;
            }
        }
    }
}
