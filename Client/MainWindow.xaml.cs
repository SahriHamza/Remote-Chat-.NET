using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using Interface;
using System.Threading;
using System.ComponentModel;

namespace Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MyRemotableObject remoteObject; //remote Object used to contact the server
        List<string> usernames = new List<string>(); // list of usernames of members connected
        Thread listenThread; 
        string pseudo; // the username of the current client
        List<string> messages = new List<string>(); // list of messages
        bool loggedIn = false; 
        TcpChannel chan;

        public MainWindow()
        {
            InitializeComponent();
            var messageToSend = (TextBox)this.FindName("message");
            messageToSend.IsEnabled = false;
        }


        private void Login()
        {
            var host = (TextBox)this.FindName("host");
            var port = (TextBox)this.FindName("port");
            String uri = String.Concat("tcp://", host.Text, ":", port.Text, "/RemoteChat");
            //************************************* TCP *************************************//
            // using TCP protocol
            // running both client and server on same machines
            chan = new TcpChannel();
            ChannelServices.RegisterChannel(chan,false);
            // Create an instance of the remote object
            remoteObject = (MyRemotableObject)Activator.GetObject(typeof(MyRemotableObject), uri);
            
            //************************************* TCP *************************************//


            //create listen thread
            listenThread = new Thread(new ThreadStart(delegate ()
            {
                Application app = System.Windows.Application.Current;
                for (;;)
                {
                    app.Dispatcher.Invoke(
                         System.Windows.Threading.DispatcherPriority.SystemIdle,
                         new Action(
                             delegate ()
                             {
                                 // get conversation and connected members from server
                                 this.setConversation();
                                 this.setMembers();

                             }
                      ));
                }
            }));
            listenThread.Start();
        }

        private void setConversation() {
            //get messages from server
            List<string> message = remoteObject.GetMessages();
            // check if there is a new message
            if (messages.Count != message.Count)
            {
                var conversation = (TextBlock)this.FindName("conversation");
                messages = message;
                string messageList = "";
                foreach (string s in messages) {
                    messageList = String.Concat(messageList, s, "\n");
                }
                conversation.Text = messageList;
                }
            }
        /// <summary>
        /// this function gets the list of members from the server
        /// </summary>
        private void setMembers() {
            this.usernames = remoteObject.GetUsernames();
            var members = (TextBlock)this.FindName("members");
            string membersList = "";

            foreach (string member in this.usernames)
            {
                membersList = String.Concat(membersList, member, "\n");
            }
            members.Text = membersList;
        }


        /// <summary>
        /// login button Call Back
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void login_Click(object sender, RoutedEventArgs e)
        {
            if (!loggedIn) {
                Login();
                var username = (TextBox)this.FindName("username");
                var port = (TextBox)this.FindName("port");
                var host = (TextBox)this.FindName("host");
                var messageToSend = (TextBox)this.FindName("message");
                pseudo = username.Text;
                remoteObject.Login(pseudo);
                loggedIn = true;
                username.IsEnabled = false;
                port.IsEnabled = false;
                host.IsEnabled = false;
                messageToSend.IsEnabled = true;
            }
            
        }

        /// <summary>
        /// Logout button Call Back
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void logout_Click(object sender, RoutedEventArgs e) {
            if (loggedIn) {
                remoteObject.Logout(pseudo);
                loggedIn = false;
                ChannelServices.UnregisterChannel(chan);
                listenThread.Abort();
                var username = (TextBox)this.FindName("username");
                var port = (TextBox)this.FindName("port");
                var host = (TextBox)this.FindName("host");
                var messageToSend = (TextBox)this.FindName("message");
                username.IsEnabled = true;
                port.IsEnabled = true;
                host.IsEnabled = true;
                messageToSend.IsEnabled = false;
            }
        }

        /// <summary>
        /// Send button CallBack
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void send_Click(object sender, RoutedEventArgs e)
        {
            var messageToSend = (TextBox)this.FindName("message");
            if (loggedIn)
            {  
                remoteObject.SetMessage(String.Concat(pseudo, " : ", messageToSend.Text));
                messageToSend.Text = "";
            }
            else {
                messageToSend.Text = "CLIENT NOT CONNECTED";
            }

        }
        /// <summary>
        /// closing the listen thread before exiting the client
        /// </summary>
        private void DataWindow_Closing(object sender, CancelEventArgs e)
        {
            listenThread.Abort();
        }
    }
}
