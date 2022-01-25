using System.Collections.Generic;

namespace AATool.Data.Categories
{
    public interface ICategory
    {
        public bool TrySetVersion(string version);

        public IEnumerable<string> GetSupportedVersions();

        public bool IsComplete();
        public int GetCompletionPercent();
        public string GetCompletionMessage();
    }
}
