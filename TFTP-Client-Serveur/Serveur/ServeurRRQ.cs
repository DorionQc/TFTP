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
            bool AckRecu;
            byte[] buffer = new byte[516];
            long NumeroPaquet = 1;
            byte NbEssais = 0;
            this.Continuer = true;
            absPaquet recu;
            AckPaquet ack;
            DataPaquet data;
            DateTime temps;


            // Au cas où...
            this.Event.WaitOne();
            this.Event.Reset();

            logger.Log(ConsoleSource.Serveur, "Nouvelle connection RRQ vers " + m_DistantEP.ToString());


            if (!File.Exists(m_NomFichier))
            {
                Envoyer(new ErrorPaquet(CodeErreur.FileNotFound, "Impossible de trouver le fichier"));
                logger.Log(ConsoleSource.Serveur, "Impossible de trouver le fichier");
                return;
            }
            try {
                m_fs = new FileStream(m_NomFichier, FileMode.Open, FileAccess.Read);
                m_br = new BinaryReader(m_fs);
            }
            catch (Exception ex)
            {
                Envoyer(new ErrorPaquet(CodeErreur.AccessViolation, "Erreur lors de la lecture du fichier : " + ex.Message));
                logger.Log(ConsoleSource.Serveur, "Erreur lors de la lecture du fichier");
                return;
            }
            BinaryReader br = new BinaryReader(m_fs);

            while (this.Continuer)
            {
                // Envoi des données
                data = new DataPaquet((short)NumeroPaquet, SeparerFichier(NumeroPaquet));
                Envoyer(data);
                temps = DateTime.Now;
                AckRecu = false;
                NbEssais++;

                while (!AckRecu && DateTime.Now.Ticks - temps.Ticks < 5000000)
                {
                    if (this.Socket.Available == 0)
                        Thread.Sleep(0);
                    else
                    {
                        len = this.Socket.ReceiveFrom(buffer, ref m_DistantEP);
                        if (len != 0 && absPaquet.Decoder(buffer, out recu))
                        {
                            if (recu.Type == TypePaquet.ACK && ((AckPaquet)recu).NoBlock == (short)NumeroPaquet)
                            {
                                //logger.Log(ConsoleSource.Serveur, "Réception du ACK du paquet #" + ((AckPaquet)recu).NoBlock.ToString());
                                AckRecu = true;
                                NbEssais = 0;
                                NumeroPaquet++;
                                if (data.EstDernier)
                                    this.Continuer = false;
                            }
                            else if (recu.Type == TypePaquet.ERROR)
                            {
                                logger.Log(ConsoleSource.Serveur, "Erreur reçue.");
                                logger.Log(ConsoleSource.Serveur, ((ErrorPaquet)recu).MessageErreur);
                                this.Continuer = false;
                            }
                        }
                    }
                }

                if (NbEssais == 10)
                    this.Continuer = false;
            }
            this.Event.Set();
            Terminer();
        }
    }
}
