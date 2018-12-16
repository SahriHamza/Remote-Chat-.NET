using System;
using System.Collections.Generic;

namespace Interface
{

    public class Cache
    {
        /// <summary>
        /// this class Implements a caching system to access the server variables and methods (list of messages .... ) 
        /// </summary>
        private static Cache myInstance;
        public static IObserver Observer;

        private Cache()
        {

        }

        public static void Attach(IObserver observer)
        {
            Observer = observer;
        }


        public static Cache GetInstance()
        {
            if (myInstance == null)
            {
                myInstance = new Cache();
            }
            return myInstance;
        }


        public string MessageString
        {
            set
            {
                Observer.addMessage(value);
            }
        }

        public List<string> messages {
            get
            {
                return Observer.GetMessages();
            }
        }

        public string newUsername{
            set {
                Observer.Login(value);
            }
        }

        public string removeUsername {
            set {
                Observer.Logout(value);
            }
        }

        public List<string> members {
            get  {
                return Observer.GetMembers();
            }
        }
    }
}
