/***********************************
 * Samuel Goulet
 * Serveur TFTP
 * 2017
 **********************************/

 /*

    #TODO
    Vérification de la validité des paquets
    Code d'erreur
    Serveur
    Client

 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;

using TFTP_Client_Serveur.Paquet;

namespace TFTP_Client_Serveur.Serveur
{
    public class TFTPServeur
    {

        // Singleton!

        private static TFTPServeur __instance;
        public static TFTPServeur INSTANCE
        {
            get
            {
                if (__instance == null)
                {
                    __instance = new TFTPServeur();
                }
                return __instance;
            }
        }

        public bool Initialised
        {
            get { return m_Initialized; }
        }

        // Port à utiliser (TFTP utilise 69 par défaut)
        private const int PORT = 69;
        // Longueur du timeout, avant d'abandonner une connection, en ms
        private const int TIMEOUT = 2000;
        // Nombre de connections limite
        private const int LIMITECONNECTION = 30;


        // Socket d'écoute
        private Socket m_ListenerSocket;
        // Thread d'écoute
        private Thread m_ThreadEcoute;

        // Indique si le serveur a été initialisé
        private bool m_Initialized;
        // Indique si le thread doit continuer de s'exécuter
        private volatile bool m_Continue;
        // EndPoint local
        private EndPoint m_LocalEP;
        // Dossier dans lequel enregistrer les fichiers
        private DirectoryInfo m_Dossier;
        // Console pour afficher les messages
        private Logger logger;

        // Mutex, pour assurer que le socket se ferme bien, que le thread se termine, etc.
        private ManualResetEvent m_Running;

        // Liste de threads et de sockets pour les différentes connections
        private List<Connection> m_lConnection;

        private TFTPServeur()
        {
            logger = Logger.INSTANCE;
            m_Initialized = false;
            m_LocalEP = null;
            m_Dossier = null;
            m_lConnection = null;
            m_ListenerSocket = null;
            m_ThreadEcoute = null;
            m_Continue = false;
            m_Running = new ManualResetEvent(true);


            
        }

        /// <summary>
        /// Initialise le serveur pour le rendre prêt à recevoir des connections
        /// </summary>
        /// <param name="Adresse">Adresse locale à utiliser</param>
        /// <param name="Dossier">Dossier dans lequel aller chercher ou enregistrer les fichiers</param>
        public void Init(IPAddress Adresse, DirectoryInfo Dossier)
        {
            if (m_Initialized)
            {
                logger.Log(ConsoleSource.Serveur, "Réinitialisation");
                m_Continue = false;
                m_Running.WaitOne();
                m_ListenerSocket.Close();
                m_ListenerSocket.Dispose();
            }
            else
            {
                logger.Log(ConsoleSource.Serveur, "Initialisation");
            }
            m_LocalEP = new IPEndPoint(Adresse, PORT);
            m_Dossier = Dossier;
            m_lConnection = new List<Connection>();
            m_ListenerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            m_ListenerSocket.Bind(m_LocalEP);

            m_Continue = true;
            // Crée et commence le thread
            m_ThreadEcoute = new Thread(new ThreadStart(Ecoute));
            m_ThreadEcoute.Start();

            m_Initialized = true;

        }

        private void Ecoute()
        {
            m_Running.WaitOne();
            // Indique que le serveur est en fonction
            m_Running.Reset();

            // Tampon de réception
            byte[] buffer = new byte[64];
            int len = 0;

            absPaquet paquet;

            // Hôte distant
            EndPoint DistantEP = new IPEndPoint(0, 0);
            logger.Log(ConsoleSource.Serveur, "En écoute...");
            while (m_Continue)
            {
                // Si il n'y a rien, ne perdons pas notre temps et laissons les autres threads faire leurs actions! Fuck les boucles inutiles.
                if (m_ListenerSocket.Available == 0)
                    Thread.Sleep(0);
                else
                {
                    len = m_ListenerSocket.ReceiveFrom(buffer, 0, 64, SocketFlags.None, ref DistantEP);

                    if (len != 0)
                    {
                        if (m_lConnection.Count >= LIMITECONNECTION)
                        {
                            logger.Log(ConsoleSource.Serveur, "Une connection a tenté d'être établie, mais le serveur est surchargé.");
                        }
                        else
                        {
                            if (absPaquet.Decoder(buffer, out paquet))
                            {
                                if (paquet.Type == TypePaquet.RRQ)
                                {
                                    m_lConnection.Add(new ServeurRRQ(((RRQPaquet)paquet).Fichier,
                                        DistantEP,
                                        new IPEndPoint(((IPEndPoint)m_LocalEP).Address, 0)));
                                }
                                else if (paquet.Type == TypePaquet.WRQ)
                                {

                                }
                                else
                                {
                                    logger.Log(ConsoleSource.Serveur, "Un message illisible a été transmis.");
                                }
                            }
                            else
                            {
                                logger.Log(ConsoleSource.Serveur, "Un message illisible a été transmis.");
                            }
                        }
                    }
                    
                }
            }
            // Indique que le serveur a terminé et peut être disposé
            m_Running.Set();
        }

        public void Arret()
        {
            // Arrête le serveur.
            if (!m_Continue)
                return;
            m_Continue = false;
            m_Running.WaitOne();
            logger.Log(ConsoleSource.Serveur, "Serveur arrêté");
        }
    }
}
