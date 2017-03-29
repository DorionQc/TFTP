using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
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
            BinaryWriter fs;
            IPEndPoint serverEP;
            byte[] TamponDeReception = new byte[516];
            if (!new RRQPaquet(FichierDistantClient).Encode(out TamponDEnvoi))
            {
                return;
            }
            try
            {
               fs =
                    new BinaryWriter(new FileStream(FichierLocalClient, FileMode.Create, FileAccess.Write,
                        FileShare.Read));
            }
            catch (Exception e)
            {
                logger.Log(ConsoleSource.Client, "Erreur: " + e.Message);
                return;
            }


            serverEP = new IPEndPoint(IPAddress.Parse(IPServeurTFTP), PortServeurTFTP);



            EndPoint donnesEP = serverEP;
            Socket SocketTFTP = new Socket(serverEP.Address.AddressFamily, SocketType.Dgram, ProtocolType.Udp);
            SocketTFTP.Bind(new IPEndPoint(0, 0));
            //Demande le premier packet de data
            SocketTFTP.SendTo(TamponDEnvoi, TamponDEnvoi.Length, SocketFlags.None, serverEP);
            SocketTFTP.ReceiveTimeout = 15000;
            //Dans un monde ideal, recoit le premier packet de data
            try
            {
                len = SocketTFTP.ReceiveFrom(TamponDeReception, ref donnesEP);
            }
            catch (Exception e)
            {
                logger.Log(ConsoleSource.Client, "Erreur: " + e.Message);
                fs.Close();
                return;
            }
            serverEP.Port = ((IPEndPoint)donnesEP).Port;

            while (TamponDeReception[1] != (byte)TypePaquet.ERROR && len == 516)
            {

                // S'attend a recevoir le packet suivant
                if ((((TamponDeReception[2] << 8) & 0xff00) | TamponDeReception[3]) == NumeroPaquet)
                {
                    // Ecris les donnes dans le fichier
                    fs.Write(TamponDeReception, 4, len - 4);

                    // Envoi un Ack correspondant au packet recu
                    new AckPaquet(NumeroPaquet).Encode(out TamponDEnvoi);
                    SocketTFTP.SendTo(TamponDEnvoi, TamponDEnvoi.Length, SocketFlags.None, serverEP);
                    NumeroPaquet++;
                }
                // check si le c'etait le dernier packet
                if (len == 516)
                {
                    // Receive Next Data Packet From TFTP Server
                    try
                    {
                        len = SocketTFTP.ReceiveFrom(TamponDeReception, ref donnesEP);
                    }
                    catch (Exception e)
                    {
                        logger.Log(ConsoleSource.Client, "Erreur: " + e.Message);
                        fs.Close();
                        return;
                    }
                }
                new AckPaquet((ushort)(((TamponDeReception[2] << 8) & 0xff00) | TamponDeReception[3])).Encode(out TamponDEnvoi);
                SocketTFTP.SendTo(TamponDEnvoi, TamponDEnvoi.Length, SocketFlags.None, serverEP);
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
            BinaryReader fs;
            IPEndPoint serverEP;
            byte[] TamponDEnvoi;
            byte[] TamponDeReception = new byte[516];

            if (!new WRQPaquet(FichierDistantClient).Encode(out TamponDEnvoi))
            {
                return;
            }
            try
            {
                fs = new BinaryReader(new FileStream(FichierLocalClient, FileMode.Open, FileAccess.Read, FileShare.ReadWrite));
            }
            catch (Exception e)
            {
                logger.Log(ConsoleSource.Client, "Erreur: "+e.Message);
                return;
            }
            
            serverEP = new IPEndPoint(IPAddress.Parse(IPServeurTFTP), PortServeurTFTP);


            EndPoint donnesEP = new IPEndPoint(IPAddress.Any, 0);
            Socket SocketTFTP = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            SocketTFTP.Bind(donnesEP);
            SocketTFTP.ReceiveTimeout = 15000;
            SocketTFTP.SendTo(TamponDEnvoi, TamponDEnvoi.Length, SocketFlags.None, serverEP);
           


            try
            {
                SocketTFTP.ReceiveFrom(TamponDeReception, ref donnesEP);
            }
            catch (Exception e)
            {
                logger.Log(ConsoleSource.Client, "Erreur: " + e.Message);
                fs.Close();
                return;
            }
            
            while (TamponDeReception[1] != (byte)TypePaquet.ERROR && len == 516)
            {
                if ((TamponDeReception[1] == (byte) TypePaquet.ACK) &&
                    (((TamponDeReception[2] << 8) & 0xff00) | TamponDeReception[3]) == NumeroPaquet)
                {
                    new DataPaquet(++NumeroPaquet, fs.ReadBytes(512)).Encode(out TamponDEnvoi);
                    try
                    {
                        len = SocketTFTP.SendTo(TamponDEnvoi, TamponDEnvoi.Length, SocketFlags.None, donnesEP);
                    }
                    catch (Exception e)
                    {
                        logger.Log(ConsoleSource.Client, "Erreur: " + e.Message);
                        fs.Close();
                        return;
                    }
                }

                try
                {
                    SocketTFTP.ReceiveFrom(TamponDeReception, ref donnesEP);
                }
                catch (Exception e)
                {
                    logger.Log(ConsoleSource.Client, "Erreur: " + e.Message);
                    fs.Close();
                    return;
                }
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