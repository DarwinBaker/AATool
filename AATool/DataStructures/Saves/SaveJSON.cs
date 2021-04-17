using AATool.Settings;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;

namespace AATool.DataStructures
{
    public abstract class SaveJSON
    {
        public string CurrentSaveName      { get; private set; }
        public DateTime CurrentAccessDate  { get; private set; }

        protected FileStream stream;
        protected StreamReader reader;
        protected dynamic json;
        protected string currentFile;
        protected string folderName;
        
        public bool IsOpen => stream != null;

        public bool TryRead()
        {
            //attempt to open stream
            TryOpen(GetCurrentJSON(folderName));

            try
            {
                //check if minecraft has saved changes to this file since last update
                DateTime latestAccessDate = new FileInfo(currentFile).LastWriteTime;
                if (CurrentAccessDate < latestAccessDate)
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
                    CurrentAccessDate = latestAccessDate;
                    return true;
                }
            }
            catch (Exception) { }
            return false;
        }

        private void TryOpen(string latestFile)
        {
            try
            {
                if (currentFile != latestFile)
                {
                    //most recently accessed save changed. close old streams and open new ones
                    Close();
                    if (!File.Exists(latestFile))
                        return;

                    CurrentSaveName = Directory.GetParent(Directory.GetParent(latestFile).FullName).Name;
                    stream          = new FileStream(latestFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    reader          = new StreamReader(stream);
                    currentFile     = latestFile;
                }
            }
            catch (Exception) { }
        }

        private void Close()
        {
            //close all streams and nullify references
            stream?.Close();
            reader?.Close();
            json = null;
            currentFile = null;
            CurrentSaveName = null;
        }

        private static string GetCurrentJSON(string folderName)
        {
            //make sure save path actually exists before attempting world list collection
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
