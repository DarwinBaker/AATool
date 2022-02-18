using System;
using System.IO;
using AATool.Configuration;
using AATool.Utilities;

namespace AATool.Exceptions
{
    [Serializable]
    public class NoSavesFolderException : IOException
    {
        public readonly string MissingPath;

        public NoSavesFolderException(string missingPath) : base(GetMessage())
        {
            this.MissingPath = missingPath;
        }

        private static string GetMessage()
        {
            if (Config.Tracking.Source == TrackerSource.ActiveInstance)
            {
                return ActiveInstance.HasNumber
                    ? $"Instance {ActiveInstance.Number} saves folder doesn't exist"
                    : "Active instance missing saves folder";
            }

            return Config.Tracking.Source == TrackerSource.DefaultAppData
                ? "Default .minecraft saves folder doesn't exist"
                : "Custom saves path doesn't exist";
        }
    }
}
