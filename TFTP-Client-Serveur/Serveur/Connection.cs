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
using System.IO;

using TFTP_Client_Serveur.Paquet;

namespace TFTP_Client_Serveur.Serveur
{
    public abstract class Connection
    {
        protected Socket m_Socket;
        protected Thread m_Thread;
        protected ManualResetEvent m_Event;
        protected volatile bool m_Continuer;
        protected string m_NomFichier;
        protected EndPoint m_DistantEP;
        protected EndPoint m_LocalEP;
        protected FileStream m_fs;
        protected BinaryReader m_br;
        protected BinaryWriter m_bw;

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

        protected Connection()
        {
            logger = Logger.INSTANCE;
            m_Event = new ManualResetEvent(false);
            m_Continuer = false;
        }

        protected abstract void Communication();

        public bool Demarrer()
        {
            if (Socket != null)
                return false;
            Event.Set();
            Socket = new Socket(AddressFamily.InterNetwork,
                SocketType.Dgram, ProtocolType.Udp);
            Socket.Bind(m_LocalEP);
            Thread = new Thread(Communication);
            Continuer = true;
            Thread.Start();
            return true;
        }

        public void Terminer()
        {
            Continuer = false;
            Event.WaitOne();
            m_br?.Close();
            m_fs?.Close();

            TFTPServeur.INSTANCE.EnleverConnection(this);
            logger.Log(ConsoleSource.Serveur, "La connection vers " + m_DistantEP + " est terminée");
        }

        protected byte[] SeparerFichier(long NoPaquet)
        {
            NoPaquet--;
            if (m_fs.Length < NoPaquet * 512)
                return null;
            if (m_fs.Length == NoPaquet * 512)
                return new byte[0];
            m_fs.Seek(NoPaquet * 512, SeekOrigin.Begin);
            if (m_fs.Length >= (NoPaquet + 1) * 512)
            {
                return m_br.ReadBytes(512);
            }
            return m_br.ReadBytes((int)(m_fs.Length - (NoPaquet * 512)));
        }

        protected void Envoyer(absPaquet paquet)
        {
            byte[] Data;
            paquet.Encode(out Data);
            Socket.SendTo(Data, m_DistantEP);
            //logger.Log(ConsoleSource.Serveur, "Envoi du packet " + paquet.ToString());
        }

    }
}
