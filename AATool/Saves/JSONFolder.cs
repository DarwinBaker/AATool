using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using AATool.Configuration;
using AATool.Data.Progress;
using AATool.Net;
using AATool.Net.Requests;

namespace AATool.Saves
{
    public abstract class JsonFolder
    {
        private const string JsonPattern = "*.json";

        public readonly Dictionary<Uuid, JsonStream> Players;

        private DirectoryInfo folder;

        public JsonFolder()
        {
            this.Players = new ();
        }

        public void SetPath(string path)
        {
            if (this.folder?.FullName != path)
            {
                if (string.IsNullOrEmpty(path))
                {
                    this.folder = null;
                    return;
                }

                this.Players.Clear();
                try
                {
                    this.folder = new DirectoryInfo(path);
                }
                catch
                {
                    this.folder = null;
                }
            }
        }

        public bool TryRefresh()
        {
            //get all uuid json files in folder
            Dictionary<Uuid, FileInfo> players = this.GetPlayerJsons();
            bool modified = this.TryRemoveDeadFiles(players) | this.TryAddNewFiles(players);

            //update jsons
            foreach (JsonStream json in this.Players.Values)
                modified |= json.TryRefresh(Tracker.ObjectivesChanged);

            return modified;
        }

        public void Update(WorldState state)
        {
            foreach (KeyValuePair<Uuid, JsonStream> player in this.Players)
            {
                if (!state.Players.TryGetValue(player.Key, out Contribution contribution))
                    state.Players[player.Key] = contribution = new Contribution(player.Key);
                this.Update(player.Value, state, contribution);
            }
        }

        protected abstract void Update(JsonStream json, WorldState state, Contribution contribution);

        private Dictionary<Uuid, FileInfo> GetPlayerJsons()
        {
            var players = new Dictionary<Uuid, FileInfo>();
            this.folder?.Refresh();
            if (this.folder?.Exists is not true)
                return players;

            try
            {
                //get all json files in folder
                FileInfo[] files = this.folder.GetFiles(JsonPattern, SearchOption.TopDirectoryOnly);
                foreach (FileInfo file in files)
                {
                    string fileName = Path.GetFileNameWithoutExtension(file.Name);
                    if (Uuid.TryParse(fileName, out Uuid id))
                    {
                        //filename is uuid
                        players[id] = file;
                        Player.FetchIdentityAsync(id);
                    }
                }
            }
            catch (IOException) { }
            catch (ArgumentException) { }
            catch (SecurityException) { }

            return players;
        }

        private bool TryRemoveDeadFiles(Dictionary<Uuid, FileInfo> newFiles = null)
        {
            bool modified = false;
            foreach (Uuid id in this.Players.Keys.ToArray())
            {
                if (!this.Players.TryGetValue(id, out JsonStream json))
                    continue;

                if (newFiles is null || !newFiles.ContainsKey(id))
                {
                    this.Players.Remove(id);
                    modified = true;
                }
            }
            return modified;
        }

        private bool TryAddNewFiles(Dictionary<Uuid, FileInfo> newFiles)
        {
            bool modified = false;
            foreach (KeyValuePair<Uuid, FileInfo> file in newFiles)
            {
                try
                {
                    string fileName = Path.GetFileNameWithoutExtension(file.Value.Name);
                    if (this.Players.ContainsKey(file.Key))
                        continue;

                    var stream = new JsonStream(file.Value.FullName, file.Key);
                    this.Players[file.Key] = stream;
                    modified = true;
                }
                catch (ArgumentException) { }
            }
            return modified;
        }
    }
}
