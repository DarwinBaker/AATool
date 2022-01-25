using Newtonsoft.Json;
using System;
using System.IO;

namespace AATool.Saves
{
    public class JsonStream
    {
        private DateTime lastReadTime;
        private dynamic jsonData;
        private bool isAlive;

        protected readonly string FullName;

        public dynamic this[string key] => this.jsonData?[key];

        public JsonStream(string fullName)
        {
            this.FullName = fullName;
        }

        private void Close(StreamReader reader) => reader?.Close();

        public bool TryUpdate(bool force)
        {
            //handle file timestamps and attempt to read
            bool modified = false;
            if (this.NeedsRefresh() || force)
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
            if (this.TryGetLastWriteTime(out DateTime lastWriteTime))
            {
                if (this.lastReadTime != lastWriteTime)
                {
                    this.lastReadTime = lastWriteTime;
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
