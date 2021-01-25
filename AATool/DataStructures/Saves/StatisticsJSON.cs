
namespace AATool.DataStructures
{
    public class StatisticsJSON : SaveJSON
    {
        public int PickedUpCount(string item)   => (int)(json?["stats"]?["minecraft:picked_up"]?[item]?.Value ?? 0);
        public int DroppedCount(string item)    => (int)(json?["stats"]?["minecraft:dropped"]?[item]?.Value ?? 0);
        public int MinedCount(string item)      => (int)(json?["stats"]?["minecraft:mined"]?[item]?.Value ?? 0);
        public int TotalCount(string item)      => PickedUpCount(item) - DroppedCount(item);
        public bool HasPickedUp(string item)    => PickedUpCount(item) > 0;

        public StatisticsJSON()
        {
            folderName = "stats";
        }
    }
}
