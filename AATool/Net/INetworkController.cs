using System.Collections.Generic;

namespace AATool.Net
{
    public interface INetworkController
    {
        public void SetControlStates(string buttonText, bool enableButton, bool enableDropDown);
        public void WriteToConsole(string text);
        public void SyncConsole();
        public void SyncUserList(IEnumerable<User> users);
    }
}
