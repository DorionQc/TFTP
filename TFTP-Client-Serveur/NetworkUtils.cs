/***********************************
 * Samuel Goulet
 * Serveur TFTP
 * 2017
 **********************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace TFTP_Client_Serveur
{
    public enum TypePaquet : byte
    {
        NULL = 0,
        RRQ = 1,
        WRQ = 2,
        DATA = 3,
        ACK = 4,
        ERROR = 5
    };
    public static class NetworkUtils
    {
        /// <summary>
        /// Retourne la liste d'IP IPv4 locales
        /// </summary>
        /// <returns>Tableau contenant les IP IPv4 locales</returns>
        public static IPAddress[] getLocalIPs()
        {
            return Dns.GetHostAddresses(Dns.GetHostName()).Where(ip => ip.AddressFamily == AddressFamily.InterNetwork).ToArray();
        }


    }
}
