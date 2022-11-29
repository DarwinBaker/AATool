using System;
using Newtonsoft.Json;

namespace AATool.Net
{
    [JsonObject]
    public readonly struct Uuid
    {
        [JsonIgnore] public static readonly Uuid Empty = new (Guid.Empty);

        [JsonProperty] public readonly string String;
        [JsonProperty] public readonly string ShortString;

        [JsonIgnore] private readonly Guid innerID;
 
        [JsonConstructor]
        public Uuid(string stringForm)
        {
            this.String = stringForm ?? string.Empty;
            this.ShortString = this.String?.Replace("-", "");
            Guid.TryParse(this.String, out this.innerID);
        }

        private Uuid(Guid id)
        {
            this.innerID = id;
            this.String = id.ToString();
            this.ShortString = this.String?.Replace("-", "");
        }

        public static bool operator == (Uuid a, Uuid b) => 
            a.innerID == b.innerID;

        public static bool operator != (Uuid a, Uuid b) => 
            a.innerID != b.innerID;

        public static implicit operator Uuid(string value) => new (value);

        public static bool TryParse(string stringForm, out Uuid uuid)
        {
            try
            {
                //try to parse internal id
                if (Guid.TryParse(stringForm, out Guid innerID))
                {
                    uuid = new Uuid(innerID);
                    return true;
                }
            }
            catch { }
            uuid = Empty;
            return false;
        }

        public override bool Equals(object obj) => obj is Uuid uuid && this.String == uuid.String;
        public override int GetHashCode() => this.String?.GetHashCode() ?? 0;
        public override string ToString() => this.String;
    }
}
