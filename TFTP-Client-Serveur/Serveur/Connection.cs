/***********************************
 * Samuel Goulet
 * Serveur TFTP
 * 2017
 **********************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using System.Net;

using TFTP_Client_Serveur.Paquet;

namespace TFTP_Client_Serveur.Serveur
{
    public abstract class Connection
    {
        private Socket m_Socket;
        private Thread m_Thread;
        private ManualResetEvent m_Event;
        private volatile bool m_Continuer;

        protected Logger logger;

        protected bool Continuer
        {
            get { return m_Continuer; }
            set { m_Continuer = value; }
        }

        protected Socket Socket
        {
            get
            {
                return m_Socket;
            }
            set
            {
                if (m_Event == null)
                    return;
                m_Event.WaitOne();
                m_Socket = value;
            }
        }

        protected Thread Thread
        {
            get
            {
                return m_Thread;
            }
            set
            {
                m_Thread = value;
            }
        }

        protected ManualResetEvent Event
        {
            get
            {
                return m_Event;
            }
        }

        public Connection()
        {
            logger = Logger.INSTANCE;
            m_Event = new ManualResetEvent(false);
            m_Continuer = false;
        }

        public abstract bool Demarrer();

        public abstract void Terminer();

    }
}
