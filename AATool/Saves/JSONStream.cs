using Newtonsoft.Json;
using System;
using System.IO;

namespace AATool.Saves
{
    public class JSONStream
    {
        public bool IsAlive               { get; private set; }

        protected DateTime LastReadTime  { get; private set; }
        protected dynamic JsonData       { get; private set; }

        protected readonly string FullName;

        public dynamic this[string key] => this.JsonData?[key];

        public JSONStream(string fullName)
        {
            this.FullName = fullName;
        }

        public bool TryUpdate(bool force)
        {
            //handle file timestamps and attempt to read
            bool modified = false;
            if (this.NeedsRefresh() || force)
            {
                if (this.TryOpen(this.FullName, out StreamReader stream) && this.TryRead(stream))
                {
                    this.IsAlive = true;
                    modified = true;
                }
                else if (this.IsAlive)
                {
                    this.IsAlive = false;
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
                this.JsonData = JsonConvert.DeserializeObject(fileContents);
                this.Close(stream);
                return true;
            }
            catch
            {
                this.JsonData = null;
                this.Close(stream);
                return false;
            }
        }

        private void Close(StreamReader reader)
        {
            //dereference json and close stream
            reader?.Close();
        }

        private bool NeedsRefresh()
        {
            if (this.TryGetLastWriteTime(out DateTime lastWriteTime))
            {
                if (this.LastReadTime != lastWriteTime)
                {
                    this.LastReadTime = lastWriteTime;
                    return true;
                }
            }
            else
            {
                this.IsAlive = false;
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
                        FileShare.ReadWrite);
                    reader = new StreamReader(stream);
                    return true;
                }
                catch
                {
                    this.IsAlive = false;
                    this.Close(reader);
                }
            }
            return false;
        }
    }
}
