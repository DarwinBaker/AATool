using System.Threading.Tasks;
using AATool.Utilities;

namespace AATool.Net
{
    public abstract partial class NetRequest
    {
        public string Url  { get; private set; }

        private int failures;
        private readonly Timer cooldown;

        private bool IsStillTimedOut => this.cooldown.IsRunning;

        public NetRequest(string url)
        {
            this.Url = url;
            this.cooldown = new Timer();
        }

        public abstract Task<bool> RunAsync();

        private void UpdateCooldown(Time time)
        {
            if (this.cooldown.IsRunning)
                this.cooldown.Update(time);
        }

        private async void StartAsync()
        {
            try
            {
                //try to process request
                bool success = await this.RunAsync();
                if (success)
                    this.Complete();
                else
                    this.Fail();
            }
            catch
            {
                //network error
                this.Fail();
            }
            Active.Remove(this.Url);
        }

        private void Complete()
        {
            Completed.Add(this.Url);
        }

        private void Fail()
        {
            //request failed
            this.failures++;
            if (this.failures < Protocol.REQUEST_MAX_ATTEMPTS)
            {
                //set cooldown to try again later
                TimedOut.Add(this);
                this.cooldown.SetAndStart(Protocol.REQUEST_RETY_COOLDOWN);
            }
            else
            {
                //request has failed too many times. stop trying
                Abandoned.Add(this.Url);
            }
        }
    }
}
