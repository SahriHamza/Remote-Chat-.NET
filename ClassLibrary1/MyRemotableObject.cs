using System;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Collections.Generic;

namespace Interface
{

    public class MyRemotableObject : MarshalByRefObject
    {

        public MyRemotableObject()
        {

        }

        public void SetMessage(string message)
        {
            Cache.GetInstance().MessageString = message;
        }

        public void Login(string username) {
            Cache.GetInstance().newUsername = username;
        }

        public void Logout(string username) {
            Cache.GetInstance().removeUsername = username;
        }

        public List<string> GetUsernames() {
            return Cache.GetInstance().members;
        }

        public List<string> GetMessages() {
            return Cache.GetInstance().messages;
        }
    }
}
