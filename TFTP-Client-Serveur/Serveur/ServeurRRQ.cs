/***********************************
 * Samuel Goulet
 * Serveur TFTP
 * 2017
 **********************************/
 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace TFTP_Client_Serveur.Serveur
{
    class ServeurRRQ : Connection
    {
        private string m_Fichier;
        private EndPoint m_DistantEP;
        private EndPoint m_LocalEP;



        public ServeurRRQ(string Fichier, EndPoint Distant, EndPoint Local) : base()
        {
            m_Fichier = Fichier;
            m_DistantEP = Distant;
            m_LocalEP = Local;
            Demarrer();
        }


        public override bool Demarrer()
        {
            if (this.Socket != null)
                return false;
            this.Socket = new Socket(AddressFamily.InterNetwork,
                SocketType.Dgram, ProtocolType.Udp);
            this.Socket.Bind(m_LocalEP);
            this.Thread = new Thread(new ThreadStart(Communication));
            this.Event.Set();
            this.Continuer = true;
            this.Thread.Start();
            return true;

        }

        public override void Terminer()
        {
            throw new NotImplementedException();
        }

        private void Communication()
        {
            this.Event.WaitOne();
            this.Event.Reset();
            logger.Log(ConsoleSource.Serveur, "Nouvelle connection RRQ vers " + m_DistantEP.ToString());
            while (this.Continuer)
            {
                if (this.Socket.Available == 0)
                    Thread.Sleep(0);
                else
                {

                }
            }
        }
    }
}
