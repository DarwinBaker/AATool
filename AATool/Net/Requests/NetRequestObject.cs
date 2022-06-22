using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using AATool.Utilities;

namespace AATool.Net
{
    public abstract partial class NetRequest
    {
        public const string Incoming = "->";
        public const string Outgoing = "<-";

        protected string Url;

        private int failures;
        private Stopwatch stopwatch;
        private readonly Timer cooldown;

        private bool IsOnCooldown => this.cooldown.IsRunning;

        protected string ResponseTime => $"{this.stopwatch?.ElapsedMilliseconds ?? 0} ms";

        public NetRequest(string url)
        {
            this.Url = url;
            this.cooldown = new Timer();
        }

        public void EnqueueOnce() => Enqueue(this);

        public abstract Task<bool> DownloadAsync();

        private void Complete() => Completed.Add(this.Url);

        protected void BeginTiming()
        {
            this.stopwatch = new Stopwatch();
            this.stopwatch.Start();
        }

        protected void EndTiming() => this.stopwatch.Stop();

        private void UpdateCooldown(Time time)
        {
            if (this.cooldown.IsRunning)
                this.cooldown.Update(time);
        }

        public async void SendAsync()
        {
            try
            {
                if (await this.DownloadAsync())
                    this.Complete();
                else
                    this.Fail();
            }
            catch (Exception e)
            {
                //safely ignore and move on if network error ocurred
                if (e is HttpRequestException or OperationCanceledException)
                    this.Fail();
                else
                    throw;
            }
            finally 
            {
                Active.Remove(this.Url);
            }
        }

        private void Fail()
        {
            //request failed
            this.failures++;
            if (this.failures < Protocol.Requests.MaxRetries)
            {
                //set cooldown to try again later
                TimedOut.Add(this);
                this.cooldown.SetAndStart(Protocol.Requests.RetryCooldown);
            }
            else
            {
                //request has failed too many times. stop trying
                Abandoned.Add(this.Url);
            }
        }
    }
}
