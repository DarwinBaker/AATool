using AATool.Data.Progress;

namespace AATool.Data
{
    public interface IManifest
    {
        public void UpdateReference();
        public void Update(ProgressState progress);
        public void ClearProgress();
    }
}
