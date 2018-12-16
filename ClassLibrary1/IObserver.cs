using System;
using System.Collections.Generic;

namespace Interface
{

    public interface IObserver
    {
        void addMessage(string text);
        void Login(string username);
        void Logout(string username);
        List<string> GetMembers();
        List<string> GetMessages();
    }

}
