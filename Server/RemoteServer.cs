using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using Interface;

namespace Server
{
    class RemoteServer : IObserver
    {
        private MyRemotableObject remotableObject; // remote object used to contact clients
        private List<string> usernames = new List<string>(); // list of connected members
        private List<string> messages = new List<string>(); // list of messages

        public RemoteServer()
        {
            remotableObject = new MyRemotableObject();

            //************************************* TCP *************************************//
            // using TCP protocol
            TcpChannel channel = new TcpChannel(8080);
            ChannelServices.RegisterChannel(channel);
            // Show the name of the channel.
            Console.WriteLine("The name of the channel is {0}.",
                channel.ChannelName);
            RemotingConfiguration.RegisterWellKnownServiceType(typeof(MyRemotableObject), "RemoteChat", WellKnownObjectMode.Singleton);
            //************************************* TCP *************************************//
            Interface.Cache.Attach(this);
        }

        [STAThread]
        static void Main()
        {
            RemoteServer serv = new Server.RemoteServer();
            // Wait for the user prompt.
            Console.WriteLine("Press ENTER to exit the server.");
            Console.ReadLine();
        }

        #region IObserver Members

        /// <summary>
        ///  Adds a message to the messages List
        /// </summary>
        /// <param name="text"></param>
        public void addMessage(string text)
        {
            messages.Add(text);
        }

        /// <summary>
        /// Logs in a new member
        /// </summary>
        /// <param name="username"></param>
        public void Login(string username) {
            usernames.Add(username);
            Console.WriteLine(String.Concat("Received Login from : ",username));
            remotableObject.SetMessage(String.Concat(username," has joined the conversation"));
        }

        /// <summary>
        /// Logs out a member
        /// </summary>
        /// <param name="username"></param>

        public void Logout(string username) {
            usernames.Remove(username);
            Console.WriteLine(String.Concat("Received Logout from : ", username));
            remotableObject.SetMessage(String.Concat(username, " has left the conversation"));
        }

        /// <summary>
        /// returns the list of members
        /// </summary>
        /// <returns></returns>
        public List<string> GetMembers()
        {
            return usernames;
        }

        /// <summary>
        /// returns the list of messages
        /// </summary>
        /// <returns></returns>
        public List<string> GetMessages() {
            return messages;
        }
        #endregion
    }
}
