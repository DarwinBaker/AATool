using Newtonsoft.Json;
using System;
using System.IO;

namespace AATool.Saves
{
    public class JSONStream
    {
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
                    modified = true;
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
                return true;
            }
            catch (Exception exception)
            {
                this.JsonData = null;
                if (exception is not (IOException or JsonReaderException))
                    throw exception;

                //malformed json. ignore and try to open again later
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
                if (this.LastReadTime != lastWriteTime)
                {
                    this.LastReadTime = lastWriteTime;
                    return true;
                }
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
            if (!File.Exists(path))
                return false;

            try
            {
                var stream = new FileStream(path,
                        FileMode.Open,
                        FileAccess.Read,
                        FileShare.ReadWrite | FileShare.Delete);
                reader = new StreamReader(stream);
                return true;
            }
            catch (IOException)
            {
                //file unavailable, ignore and try to open again later
                reader?.Close();
                return false;
            }
        }
    }
}
