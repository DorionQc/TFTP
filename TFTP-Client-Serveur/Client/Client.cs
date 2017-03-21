using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using TFTP_Client_Serveur.Paquet;

namespace TFTP_Client_Serveur.Client
{
    /// <summary>
    /// Classe Client TFTP
    /// </summary>
    public class ClientTFTP
    {
        protected int PortServeurTFTP;
        private string IPServeurTFTP = "";
        protected Logger logger = Logger.INSTANCE;



        /// <summary>
        /// Telecharge le fichier distant specifie
        /// </summary>
        /// <param name="FichierDistantClient">The remote file.</param>
        /// <param name="FichierLocalClient">The local file.</param>
        public void RRQ(string FichierDistantClient, string FichierLocalClient)
        {
            int len;
            ushort NumeroPaquet = 1;
            byte[] TamponDEnvoi;
            IPEndPoint serverEP;
            byte[] TamponDeReception = new byte[516];
            if (!new RRQPaquet(FichierDistantClient).Encode(out TamponDEnvoi))
            {
                return;
            }

            BinaryWriter fs =
                new BinaryWriter(new FileStream(FichierLocalClient, FileMode.Create, FileAccess.Write, FileShare.Read));



            serverEP = new IPEndPoint(IPAddress.Parse(IPServeurTFTP), PortServeurTFTP);



            EndPoint donnesEP = serverEP;
            Socket SocketTFTP = new Socket(serverEP.Address.AddressFamily, SocketType.Dgram, ProtocolType.Udp);

            //Demande le premier packet de data
            SocketTFTP.SendTo(TamponDEnvoi, TamponDEnvoi.Length, SocketFlags.None, serverEP);
            SocketTFTP.ReceiveTimeout = 15000;
            //Dans un monde ideal, recoit le premier packet de data
            len = SocketTFTP.ReceiveFrom(TamponDeReception, ref donnesEP);

            serverEP.Port = ((IPEndPoint)donnesEP).Port;

            while (TamponDeReception[1] != (byte)TypePaquet.ERROR && len == 516)
            {

                // S'attend a recevoir le packet suivant
                if ((((TamponDeReception[2] << 8) & 0xff00) | TamponDeReception[3]) == NumeroPaquet)
                {
                    // Ecris les donnes dans le fichier
                    fs.Write(TamponDeReception, 4, len - 4);

                    // Envoi un Ack correspondant au packet recu
                    
                }
                // check si le c'etait le dernier packet
                if (len == 516)
                {
                    // Receive Next Data Packet From TFTP Server
                    len = SocketTFTP.ReceiveFrom(TamponDeReception, ref donnesEP);
                    new AckPaquet(NumeroPaquet++).Encode(out TamponDEnvoi);
                    SocketTFTP.SendTo(TamponDEnvoi, TamponDEnvoi.Length, SocketFlags.None, serverEP);
                }
            }

            // Close Socket and release resources
            if (TamponDeReception[1] == (byte)TypePaquet.ERROR)
            {
                logger.Log(ConsoleSource.Client, Encoding.GetEncoding(437).GetString(TamponDeReception, 4, TamponDeReception.Length - 5).Trim('\0'));
            }
            SocketTFTP.Close();
            fs.Close();
        }

        /// <summary>
        /// Televerse le fichier distant specifie
        /// </summary>
        /// <param name="FichierDistantClient">The remote file.</param>
        /// <param name="FichierLocalClient">The local file.</param>
        public void WRQ(string FichierDistantClient, string FichierLocalClient)
        {
            ushort NumeroPaquet = 0;
            int len = 516;
            IPEndPoint serverEP;
            byte[] TamponDEnvoi;
            byte[] TamponDeReception = new byte[516];

            if (!new WRQPaquet(FichierDistantClient).Encode(out TamponDEnvoi))
            {
                return;
            }

            BinaryReader fs = new BinaryReader(new FileStream(FichierLocalClient, FileMode.Open, FileAccess.Read, FileShare.ReadWrite));
            serverEP = new IPEndPoint(IPAddress.Parse(IPServeurTFTP), PortServeurTFTP);
            EndPoint donnesEP = serverEP;
            Socket SocketTFTP = new Socket(serverEP.Address.AddressFamily, SocketType.Dgram, ProtocolType.Udp);

            SocketTFTP.SendTo(TamponDEnvoi, TamponDEnvoi.Length, SocketFlags.None, serverEP);
            SocketTFTP.ReceiveTimeout = 1000;
            SocketTFTP.ReceiveFrom(TamponDeReception, ref donnesEP);

            serverEP.Port = ((IPEndPoint)donnesEP).Port;

            while (TamponDeReception[1] != (byte)TypePaquet.ERROR && len == 516)
            {
                if ((TamponDeReception[1] == (byte)TypePaquet.ACK) && (((TamponDeReception[2] << 8) & 0xff00) | TamponDeReception[3]) == NumeroPaquet)
                {
                    new DataPaquet(++NumeroPaquet, fs.ReadBytes(512)).Encode(out TamponDEnvoi);

                    len = SocketTFTP.SendTo(TamponDEnvoi, TamponDEnvoi.Length, SocketFlags.None, serverEP);
                }

                SocketTFTP.ReceiveFrom(TamponDeReception, ref donnesEP);
            }

            // Close Socket and release resources
            SocketTFTP.Close();
            fs.Close();
        }


        /// <summary>
        /// Initialize une nouvelle instance de la <see cref="ClientTFTP"/>.
        /// </summary>
        /// <param name="server"><see cref="Server"/></param>
        /// <param name="port"><see cref="Port"/></param>
        public ClientTFTP(string server, int port = 69)
        {
            Server = server;
            Port = port;
        }

        /// <summary>
        /// Retourne le port utilisé
        /// </summary>
        /// <value>Port.</value>
        public int Port
        {
            get { return PortServeurTFTP; }
            private set { PortServeurTFTP = value; }
        }

        /// <summary>
        /// Retourne l'IP du serveur TFTP
        /// </summary>
        /// <value>IP</value>
        public string Server
        {
            get { return IPServeurTFTP; }
            private set { IPServeurTFTP = value; }
        }

    }
}