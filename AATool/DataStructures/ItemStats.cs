using System;
using System.Xml;

namespace AATool.DataStructures
{
    public class ItemStats
    {
        public static string FrameComplete      = "frame_count_complete";
        public static string FrameIncomplete    = "frame_count_incomplete";

        public string ID                { get; private set; }
        public string Name              { get; private set; }
        public int PickedUp             { get; private set; }
        public int Mined                { get; private set; }
        public int TargetCount          { get; private set; }
        public string Icon              { get; private set; }
        public bool IsCompleted         { get; private set; }
        public bool IsEstimate          { get; private set; }
        public string CurrentFrame      => IsCompleted ? FrameComplete : FrameIncomplete;

        public ItemStats(XmlNode node)
        {
            //initialize members from xml 
            ID   = node.Attributes["id"]?.Value;
            Name = node.Attributes["name"]?.Value ?? ID;
            Icon = node.Attributes["icon"]?.Value;
            if (Icon == null)
            {
                var idParts = ID.Split(':');
                if (idParts.Length > 0)
                    Icon = idParts[idParts.Length - 1];
            }
            if (int.TryParse(node.Attributes["target_count"]?.Value, out int parsed)) 
                TargetCount = parsed;
            if (bool.TryParse(node.Attributes["estimate"]?.Value, out bool isEstimate)) 
                IsEstimate = isEstimate;
        }

        public void Update(StatisticsJSON statistics)
        {
            PickedUp = statistics.PickedUpCount(ID);
            Mined    = statistics.MinedCount(ID);
            IsCompleted = PickedUp >= TargetCount;
        }
    }
}
