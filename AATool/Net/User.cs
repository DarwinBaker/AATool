using System.Threading.Tasks;
using AATool.Net.Requests;
using Newtonsoft.Json;

namespace AATool.Net
{
    [JsonObject]
    public readonly struct User
    {
        private const int MAX_NAME_LENGTH = 16;
        private const string ELLIPSES = "...";

        public static readonly User Nobody = new (Uuid.Empty, string.Empty, string.Empty);

        [JsonProperty] public readonly Uuid Id;
        [JsonProperty] public readonly string Pronouns;
        [JsonProperty] private readonly string preferredName;

        [JsonConstructor]
        public User(Uuid id, string pronouns, string preferredName = null)
        {
            this.Id = id;
            this.Pronouns = pronouns;
            this.preferredName = preferredName;

            //abbreviate name if too long
            if (preferredName is not null && preferredName.Length > MAX_NAME_LENGTH)
                this.preferredName = preferredName.Substring(0, MAX_NAME_LENGTH - ELLIPSES.Length) + ELLIPSES;
        }

        public static bool operator ==(User a, User b) =>
            a.Id == b.Id;

        public static bool operator !=(User a, User b) =>
            a.Id != b.Id;

        public override int GetHashCode() => this.Id.GetHashCode();
        public override bool Equals(object obj) => obj is User user && this == user;

        public string Name => string.IsNullOrEmpty(this.preferredName) && Player.TryGetName(this.Id, out string name)
            ? name
            : this.preferredName;
    }
}
