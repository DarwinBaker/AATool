using AATool.Net;
using Newtonsoft.Json;
using System;
using System.IO;

namespace AATool.Saves
{
    public class JsonStream
    {
        public readonly Uuid Player;
        public readonly string FullName;

        public DateTime LastWriteTime { get; private set; }

        private dynamic jsonData;
        private bool isAlive;

        public dynamic this[string key] => this.jsonData?[key];

        public override string ToString() => this.jsonData?.ToString() ?? string.Empty;

        public JsonStream(string fullName, Uuid player)
        {
            this.FullName = fullName;
            this.Player = player;
        }

        private void Close(StreamReader reader) => reader?.Close();

        public bool TryRefresh(bool ignoreTimestamps)
        {
            //handle file timestamps and attempt to read
            bool modified = false;
            if (this.NeedsRefresh() || ignoreTimestamps)
            {
                if (this.TryOpen(this.FullName, out StreamReader stream) && this.TryRead(stream))
                {
                    this.isAlive = true;
                    modified = true;
                }
                else if (this.isAlive)
                {
                    this.isAlive = false;
                    modified = true;
                }
            }
            return modified;
        }

        private bool TryRead(StreamReader stream)
        {
            try
            {
                //read all json file contents and deserialize into dynamic json
                string fileContents = stream.ReadToEnd();
                this.jsonData = JsonConvert.DeserializeObject(fileContents);
                return true;
            }
            catch
            {
                this.jsonData = null;
                return false;
            }
            finally
            {
                stream?.Close();
            }
        }

        private bool NeedsRefresh()
        {
            if (this.TryGetLastWriteTime(out DateTime latestWriteTime))
            {
                if (this.LastWriteTime != latestWriteTime)
                {
                    this.LastWriteTime = latestWriteTime;
                    return true;
                }
            }
            else
            {
                this.isAlive = false;
            }
            return false;
        }

        private bool TryGetLastWriteTime(out DateTime lastWriteTime)
        {
            lastWriteTime = default;
            if (File.Exists(this.FullName))
            {
                try
                {
                    lastWriteTime = File.GetLastWriteTime(this.FullName);
                    return true;
                }
                catch { }
            }
            return false;
        }

        private bool TryOpen(string path, out StreamReader reader)
        {
            reader = null;
            if (File.Exists(path))
            {
                try
                {
                    var stream = new FileStream(path,
                        FileMode.Open,
                        FileAccess.Read,
                        FileShare.ReadWrite | FileShare.Delete);

                    reader = new StreamReader(stream);
                    return true;
                }
                catch
                {
                    this.isAlive = false;
                    this.Close(reader);
                }
            }
            return false;
        }
    }
}
