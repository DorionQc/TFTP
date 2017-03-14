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

        protected abstract void Communication();

        public bool Demarrer()
        {
            if (this.Socket != null)
                return false;
            this.Event.Set();
            this.Socket = new Socket(AddressFamily.InterNetwork,
                SocketType.Dgram, ProtocolType.Udp);
            this.Socket.Bind(m_LocalEP);
            this.Thread = new Thread(new ThreadStart(Communication));
            this.Continuer = true;
            this.Thread.Start();
            return true;
        }

        public void Terminer()
        {
            if (!this.Continuer)
                return;
            this.Continuer = false;
            this.Event.WaitOne();
            if (m_br != null) m_br.Close();
            if (m_fs != null) m_fs.Close();

            TFTPServeur.INSTANCE.EnleverConnection(this);
            logger.Log(ConsoleSource.Serveur, "La connection vers " + m_DistantEP.ToString() + " est terminée");
        }

        protected byte[] SeparerFichier(long NoPaquet)
        {
            NoPaquet--;
            if (m_fs.Length < NoPaquet * 512)
                return null;
            else if (m_fs.Length == NoPaquet * 512)
                return new byte[0];
            else
            {
                m_fs.Seek(NoPaquet * 512, SeekOrigin.Begin);
                if (m_fs.Length >= (NoPaquet + 1) * 512)
                {
                    return m_br.ReadBytes(512);
                }
                else
                {
                    return m_br.ReadBytes((int)(m_fs.Length - (NoPaquet * 512)));
                }
            }
        }

        protected void Envoyer(absPaquet paquet)
        {
            byte[] Data;
            paquet.Encode(out Data);
            this.Socket.SendTo(Data, m_DistantEP);
            //logger.Log(ConsoleSource.Serveur, "Envoi du packet " + paquet.ToString());
        }

    }
}
