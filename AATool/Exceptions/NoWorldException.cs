using System;
using System.IO;
using AATool.Configuration;
using AATool.Utilities;

namespace AATool.Exceptions
{
    [Serializable]
    public class NoWorldException : IOException
    {
        public NoWorldException() : base(GetMessage())
        {

        }

        private static string GetMessage()
        {
            if (Config.Tracking.Source == TrackerSource.ActiveInstance)
            {
                return ActiveInstance.HasNumber
                    ? $"No worlds in instance {ActiveInstance.Number}"
                    : "No worlds in active instance";
            }

            if (Config.Tracking.Source == TrackerSource.SpecificWorld)
            {
                try
                {
                    string name = new DirectoryInfo(Config.Tracking.CustomWorldPath).Name;
                    return $"Specified world \"{name}\" doesn't exist";
                }
                catch
                {
                    return $"Specified world invalid";
                }
            }
            return "No worlds in custom save path";
        }
    }
}
