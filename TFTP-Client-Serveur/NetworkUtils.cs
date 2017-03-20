/***********************************
 * Samuel Goulet
 * Serveur TFTP
 * 2017
 **********************************/

using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace TFTP_Client_Serveur
{
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
