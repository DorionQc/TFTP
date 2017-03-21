/***********************************
 * Samuel Goulet
 * Serveur TFTP
 * 2017
 **********************************/

using System;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using System.IO;

using TFTP_Client_Serveur.Paquet;

namespace TFTP_Client_Serveur.Serveur
{
    /// <summary>
    /// Classe abstraite, parent des deux types de serveur
    /// </summary>
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

        /// <summary>
        /// Indique que le thread doit continuer
        /// </summary>
        protected bool Continuer
        {
            get { return m_Continuer; }
            set { m_Continuer = value; }
        }

        /// <summary>
        /// Socket utilisé pour envoyer et recevoir les informations
        /// </summary>
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

        /// <summary>
        /// Thread utilisé par le socket
        /// </summary>
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

        /// <summary>
        /// Événement qui indique l'état du thread
        /// </summary>
        protected ManualResetEvent Event
        {
            get
            {
                return m_Event;
            }
        }

        /// <summary>
        /// Constructeur protégé! :D
        /// </summary>
        protected Connection()
        {
            logger = Logger.INSTANCE;
            m_Event = new ManualResetEvent(false);
            m_Continuer = false;
        }

        /// <summary>
        /// Méthode à définir par le serveur, selon son rôle
        /// </summary>
        protected abstract void Communication();

        /// <summary>
        /// Démarre le serveur
        /// </summary>
        /// <returns>True si le serveur est démarré</returns>
        public bool Demarrer()
        {
            if (Socket != null)
                return false;
            Event.Set();
            try
            {
                Socket = new Socket(AddressFamily.InterNetwork,
                    SocketType.Dgram, ProtocolType.Udp);
                Socket.Bind(m_LocalEP);
            }
            catch(Exception e)
            {
                logger.Log(ConsoleSource.Serveur, e.Message);
                return false;
            }
            Thread = new Thread(Communication);
            Continuer = true;
            Thread.Start();
            return true;
        }

        /// <summary>
        /// Termine le thread de manière sécure
        /// </summary>
        public void Terminer()
        {
            Continuer = false;
            Event.WaitOne();
            m_br?.Close();
            m_fs?.Close();

            TFTPServeur.INSTANCE.EnleverConnection(this);
            logger.Log(ConsoleSource.Serveur, "La connection vers " + m_DistantEP + " est terminée");
        }

        /// <summary>
        /// Sépare le fichier en paquet de 512 octets
        /// </summary>
        /// <param name="NoPaquet">Indice du paquet à prendre</param>
        /// <returns>Un tableau d'octets contenant la partie du fichier désirée</returns>
        protected byte[] SeparerFichier(long NoPaquet)
        {
            NoPaquet--;
            // Out of bound
            if (m_fs.Length < NoPaquet * 512)
                return null;
            // Exactement la fin du fichier
            if (m_fs.Length == NoPaquet * 512)
                return new byte[0];
            
            m_fs.Seek(NoPaquet * 512, SeekOrigin.Begin);

            // 512 bytes complet
            if (m_fs.Length >= (NoPaquet + 1) * 512)
                return m_br.ReadBytes(512);
            // 512 bytes incomplets
            return m_br.ReadBytes((int)(m_fs.Length - (NoPaquet * 512)));
        }

        /// <summary>
        /// Envoie un paquet en utilisant le socket courant
        /// </summary>
        /// <param name="paquet"></param>
        protected void Envoyer(absPaquet paquet)
        {
            byte[] Data;
            paquet.Encode(out Data);
            Socket.SendTo(Data, m_DistantEP);
            logger.Log(ConsoleSource.Serveur, "Envoi du packet " + paquet.ToString());
        }

    }
}
