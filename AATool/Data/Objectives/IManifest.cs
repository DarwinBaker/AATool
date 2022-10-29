using AATool.Data.Progress;

namespace AATool.Data.Objectives
{
    public interface IManifest
    {
        public void ClearObjectives();
        public void RefreshObjectives();
        public void UpdateState(ProgressState progress);
    }
}
