using Newtonsoft.Json;

namespace AATool.Configuration
{
    public interface ISetting
    {
        public void Set(object value);
        public void ApplyDefault();
        public void ClearFlag();
    }

    [JsonObject]
    public class Setting<T> : ISetting
    {
        [JsonProperty] public T Value   { get; private set; }

        [JsonIgnore] public bool Changed { get; private set; }

        [JsonIgnore] public readonly T Default;

        public static implicit operator T(Setting<T> setting) => setting.Value;

        public Setting(T defaultValue)
        {
            this.Default = defaultValue;
            this.ApplyDefault();
        }

        public void Set(object newValue)
        {
            if (newValue is T valid && !valid.Equals(this.Value))
            {
                this.Value = valid;
                this.Changed = true;
            }
        }

        public void ApplyDefault() => this.Set(this.Default);

        public void ClearFlag() => this.Changed = false;

        public void InvokeChange() => this.Changed = true;
    }
}
