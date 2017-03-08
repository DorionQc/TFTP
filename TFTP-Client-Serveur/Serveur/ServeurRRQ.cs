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
using System.IO;
using System.Threading;

using TFTP_Client_Serveur.Paquet;

namespace TFTP_Client_Serveur.Serveur
{
    class ServeurRRQ : Connection
    {

        public ServeurRRQ(string Fichier, EndPoint Distant, EndPoint Local) : base()
        {
            m_NomFichier = Fichier;
            m_DistantEP = Distant;
            m_LocalEP = Local;
            Demarrer();
        }

        protected override void Communication()
        {
            int len;
            byte[] buffer = new byte[516];
            short NumeroPaquet = 0;


            // Au cas où...
            this.Event.WaitOne();
            this.Event.Reset();

            logger.Log(ConsoleSource.Serveur, "Nouvelle connection RRQ vers " + m_DistantEP.ToString());


            if (!File.Exists(m_NomFichier))
            {
                new ErrorPaquet(CodeErreur.FileNotFound, "Impossible de trouver le fichier").Encode(out buffer);
                this.Socket.SendTo(buffer, m_DistantEP);
                return;
            }
            try {
                m_fs = new FileStream(m_NomFichier, FileMode.Open, FileAccess.Read);
                m_br = new BinaryReader(m_fs);
            }
            catch (Exception ex)
            {
                new ErrorPaquet(CodeErreur.AccessViolation, "Erreur lors de la lecture du fichier : " + ex.Message).Encode(out buffer);
                this.Socket.SendTo(buffer, m_DistantEP);
                return;
            }
            BinaryReader br = new BinaryReader(m_fs);


            // Envoi du premier paquet
            new DataPaquet(NumeroPaquet, SeparerFichier(NumeroPaquet)).Encode(out buffer);
            this.Socket.SendTo(buffer, m_DistantEP);
            NumeroPaquet++;

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
