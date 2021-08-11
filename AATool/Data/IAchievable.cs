using AATool.Data.Progress;
using AATool.Net;

namespace AATool.Data
{
    public interface IAchievable
    {
        public bool CompletedByAnyone();
        public bool CompletedBy(Uuid player);
        public void Update(ProgressState progress);
    }
}
