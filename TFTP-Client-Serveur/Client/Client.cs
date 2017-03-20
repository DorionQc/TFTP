using System;
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
        /// Initialize une nouvelle instance de la <see cref="ClientTFTP"/>.
        /// </summary>
        /// <param name="server">The server.</param>
        /// <param name="port">The port.</param>
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

        /// <summary>
        /// Gets the specified remote file.
        /// </summary>
        /// <param name="FichierDistantClient">The remote file.</param>
        /// <param name="FichierLocalClient">The local file.</param>
        public void RRQ(string FichierDistantClient, string FichierLocalClient)
        {
            int len = 0;
            ushort packetNr = 1;
            byte[] sndBuffer;
            if (!new RRQPaquet(FichierDistantClient).Encode(out sndBuffer))
            {
                return;
            }
            byte[] rcvBuffer = new byte[516];


            BinaryWriter fileStream =
                new BinaryWriter(new FileStream(FichierLocalClient, FileMode.Create, FileAccess.Write, FileShare.Read));
            IPHostEntry hostEntry;
            IPEndPoint serverEP;
            
            
            serverEP = new IPEndPoint(IPAddress.Parse(IPServeurTFTP), PortServeurTFTP);
            
            

            EndPoint dataEP = (EndPoint)serverEP;
            Socket tftpSocket = new Socket(serverEP.Address.AddressFamily, SocketType.Dgram, ProtocolType.Udp);

            // Request and Receive first Data Packet From TFTP Server
            tftpSocket.SendTo(sndBuffer, sndBuffer.Length, SocketFlags.None, serverEP);
            tftpSocket.ReceiveTimeout = 1000;
            len = tftpSocket.ReceiveFrom(rcvBuffer, ref dataEP);

            // keep track of the TID 
            serverEP.Port = ((IPEndPoint)dataEP).Port;

            while (rcvBuffer[1] != (byte)TypePaquet.ERROR)
            {
               
                // expect the next packet
                if ((((rcvBuffer[2] << 8) & 0xff00) | rcvBuffer[3]) == packetNr)
                {
                    // Store to local file
                    fileStream.Write(rcvBuffer, 4, len - 4);

                    // Send Ack Packet to TFTP Server
                    new AckPaquet(packetNr++).Encode(out sndBuffer);
                    tftpSocket.SendTo(sndBuffer, sndBuffer.Length, SocketFlags.None, serverEP);
                }
                // Was ist the last packet ?
                if (len < 516)
                {
                    break;
                }
                else
                {
                    // Receive Next Data Packet From TFTP Server
                    len = tftpSocket.ReceiveFrom(rcvBuffer, ref dataEP);
                }
            }

            // Close Socket and release resources
            logger.Log(ConsoleSource.Client, Encoding.ASCII.GetString(rcvBuffer, 4, rcvBuffer.Length - 5).Trim('\0'));
            tftpSocket.Close();
            fileStream.Close();
        }

        /// <summary>
        /// Puts the specified remote file.
        /// </summary>
        /// <param name="remoteFile">The remote file.</param>
        /// <param name="localFile">The local file.</param>
      
        /// <summary>
        /// Puts the specified remote file.
        /// </summary>
        /// <param name="remoteFile">The remote file.</param>
        /// <param name="localFile">The local file.</param>
        /// <param name="tftpMode">The TFTP mode.</param>
        /// <remarks>What if the ack does not come !</remarks>
        public void WRQ(string remoteFile, string localFile)
        {
            int len = 0;
            int packetNr = 0;
            byte[] sndBuffer = CreateRequestPacket(TypePaquet.WRQ, remoteFile);
            byte[] rcvBuffer = new byte[516];

            BinaryReader fileStream = new BinaryReader(new FileStream(localFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite));
            IPHostEntry hostEntry = Dns.GetHostEntry(IPServeurTFTP);
            IPEndPoint serverEP = new IPEndPoint(hostEntry.AddressList[0], PortServeurTFTP);
            EndPoint dataEP = (EndPoint)serverEP;
            Socket tftpSocket = new Socket(serverEP.Address.AddressFamily, SocketType.Dgram, ProtocolType.Udp);

            // Request Writing to TFTP Server
            tftpSocket.SendTo(sndBuffer, sndBuffer.Length, SocketFlags.None, serverEP);
            tftpSocket.ReceiveTimeout = 1000;
            len = tftpSocket.ReceiveFrom(rcvBuffer, ref dataEP);

            // keep track of the TID 
            serverEP.Port = ((IPEndPoint)dataEP).Port;

            while (true)
            {
                // handle any kind of error 
                if (rcvBuffer[1] == (byte)TypePaquet.ERROR)
                {
                    fileStream.Close();
                    tftpSocket.Close();
           //         throw new TFTPException(((rcvBuffer[2] << 8) & 0xff00) | rcvBuffer[3], Encoding.ASCII.GetString(rcvBuffer, 4, rcvBuffer.Length - 5).Trim('\0'));
                }

                // expect the next packet ack
                if ((rcvBuffer[1] == (byte)TypePaquet.ACK) && (((rcvBuffer[2] << 8) & 0xff00) | rcvBuffer[3]) == packetNr)
                {
                    sndBuffer = CreateDataPacket(++packetNr, fileStream.ReadBytes(512));
                    tftpSocket.SendTo(sndBuffer, sndBuffer.Length, SocketFlags.None, serverEP);
                }

                // we are done
                if (sndBuffer.Length < 516)
                {
                    break;
                }
                else
                {
                    len = tftpSocket.ReceiveFrom(rcvBuffer, ref dataEP);
                }
            }

            // Close Socket and release resources
            tftpSocket.Close();
            fileStream.Close();
        }

        /// <summary>
        /// Creates the request packet.
        /// </summary>
        /// <param name="opCode">The op code.</param>
        /// <param name="remoteFile">The remote file.</param>
        /// <param name="tftpMode">The TFTP mode.</param>
        /// <returns>the ack packet</returns>
        private byte[] CreateRequestPacket(TypePaquet Type, string remoteFile)
        {
            // Create new Byte array to hold Initial 
            // Read Request Packet
            int pos = 0;
            string modeAscii = "octet";
            byte[] ret = new byte[modeAscii.Length + remoteFile.Length + 4];

            // Set first Opcode of packet to indicate
            // if this is a read request or write request
            ret[pos++] = 0;
            ret[pos++] = (byte)Type;

            // Convert Filename to a char array
            pos += Encoding.ASCII.GetBytes(remoteFile, 0, remoteFile.Length, ret, pos);
            ret[pos++] = 0;
            pos += Encoding.ASCII.GetBytes(modeAscii, 0, modeAscii.Length, ret, pos);
            ret[pos] = 0;

            return ret;
        }

        /// <summary>
        /// Creates the data packet.
        /// </summary>
        /// <param name="packetNr">The packet nr.</param>
        /// <param name="data">The data.</param>
        /// <returns>the data packet</returns>
        private byte[] CreateDataPacket(int blockNr, byte[] data)
        {
            // Create Byte array to hold ack packet
            byte[] ret = new byte[4 + data.Length];

            // Set first Opcode of packet to TFTP_ACK
            ret[0] = 0;
            ret[1] = (byte)TypePaquet.DATA;
            ret[2] = (byte)((blockNr >> 8) & 0xff);
            ret[3] = (byte)(blockNr & 0xff);
            Array.Copy(data, 0, ret, 4, data.Length);
            return ret;
        }
        
    }
}
