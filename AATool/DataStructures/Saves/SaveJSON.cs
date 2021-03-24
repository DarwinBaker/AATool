using AATool.Settings;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AATool.DataStructures
{
    public abstract class SaveJSON
    {
        public string CurrentSaveName { get; private set; }

        protected FileStream stream;
        protected StreamReader reader;
        protected dynamic json;
        protected string currentFile;
        protected string folderName;
        protected string latestFile;

        public bool IsOpen => stream != null;

        public void Update()
        {
            latestFile = GetCurrentJSON(folderName);
            Open();
        }

        public void Open()
        {
            if (currentFile == latestFile)
                return;

            //most recently accessed save changed. close old streams and open new ones
            Close();

            if (!File.Exists(latestFile))
                return;

            CurrentSaveName = Directory.GetParent(Directory.GetParent(latestFile).FullName).Name;
            stream = new FileStream(latestFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            reader = new StreamReader(stream);
            currentFile = latestFile;
        }

        public void Close()
        {
            //close all streams and nullify references
            stream?.Close();
            reader?.Close();
            json = null;
            currentFile = null;
            CurrentSaveName = null;
        }

        public void Read()
        {
            try
            {
                if (IsOpen && stream.CanRead)
                {
                    //reset stream to beginning, read all, and deserialize into dynamic JSON
                    stream.Position = 0;
                    json = JsonConvert.DeserializeObject(reader.ReadToEnd());
                }
            }
            catch
            {
                //something went wrong, probably file missing
                if (IsOpen)
                    Close();
            }
        }

        private static string GetCurrentJSON(string folderName)
        {
            if (!Directory.Exists(TrackerSettings.Instance.SavesFolder))
                return null;
            try
            {
                //get most recently accessed save file
                var worldList = new DirectoryInfo(TrackerSettings.Instance.SavesFolder)?.GetDirectories().OrderBy(d => d.LastAccessTime).ToList();
                var directory = new DirectoryInfo(Path.Combine(worldList.Last().FullName, folderName));
                if (worldList.Count > 0 && directory.Exists)
                    return directory.GetFiles().First().FullName;
            }
            catch { }
            return null;
        }
    }
}
