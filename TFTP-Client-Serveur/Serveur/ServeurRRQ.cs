/***********************************
 * Samuel Goulet
 * Serveur TFTP
 * 2017
 **********************************/
 
using System;
using System.Net;
using System.IO;
using System.Threading;

using TFTP_Client_Serveur.Paquet;

namespace TFTP_Client_Serveur.Serveur
{
    class ServeurRRQ : Connection
    {

        public ServeurRRQ(string Fichier, EndPoint Distant, EndPoint Local)// : base()
        {
            m_NomFichier = Fichier;
            m_DistantEP = Distant;
            m_LocalEP = Local;
            Demarrer();
        }

        // Méthode principale du thread
        protected override void Communication()
        {
            int len;
            bool AckRecu;
            byte[] TamponReception = new byte[516];
            long NumeroPaquet = 1;
            byte NbEssais = 0;
            Continuer = true;
            // Paquet reçu
            absPaquet recu;
            DataPaquet data;
            // Temps de l'envoi du paquet
            DateTime temps;


            // Au cas où...
            Event.WaitOne();
            Event.Reset();

            logger.Log(ConsoleSource.Serveur, "Nouvelle connection RRQ vers " + m_DistantEP);

            // Si le fichier n'existe pas, on abandonne
            if (!File.Exists(m_NomFichier))
            {
                Envoyer(new ErrorPaquet(CodeErreur.FileNotFound, "Impossible de trouver le fichier"));
                logger.Log(ConsoleSource.Serveur, "Impossible de trouver le fichier");
                Event.Set();
                Terminer();
                return;
            }

            // Si le fichier n'est pas ouvrable, on abandonne
            try {
                m_fs = new FileStream(m_NomFichier, FileMode.Open, FileAccess.Read);
                m_br = new BinaryReader(m_fs);
            }
            catch (Exception ex)
            {
                Envoyer(new ErrorPaquet(CodeErreur.AccessViolation, "Erreur lors de la lecture du fichier : " + ex.Message));
                logger.Log(ConsoleSource.Serveur, "Erreur lors de la lecture du fichier");
                Event.Set();
                Terminer();
                return;
            }

            while (Continuer)
            {
                // Envoi des données
                data = new DataPaquet((ushort)NumeroPaquet, SeparerFichier(NumeroPaquet));
                Envoyer(data);
                temps = DateTime.Now;
                AckRecu = false;
                NbEssais++;

                // Timeout (manuel), on attend pour le ACK pendant ~5 secondes
                while (!AckRecu && DateTime.Now.Ticks - temps.Ticks < 50000000)
                {
                    if (Socket.Available == 0)
                        Thread.Sleep(0);
                    else
                    {
                        // On a reçu quelque chose!
                        try
                        {
                            len = Socket.ReceiveFrom(TamponReception, ref m_DistantEP);
                        }
                        catch (Exception ex)
                        {
                            Continuer = false;
                            logger.Log(ConsoleSource.Serveur, ex.Message);
                            break;
                        }
                        if (len != 0 && absPaquet.Decoder(TamponReception, out recu))
                        {
                            // Paquet valide et de type ACK
                            if (recu.Type == TypePaquet.ACK && ((AckPaquet)recu).NoBlock == (ushort)NumeroPaquet)
                            {
                                logger.Log(ConsoleSource.Serveur, "Réception du ACK du paquet #" + ((AckPaquet)recu).NoBlock.ToString());
                                AckRecu = true;
                                NbEssais = 0;
                                NumeroPaquet++;
                                if (data.EstDernier)
                                    Continuer = false;
                            }
                            // Paquet valide et de type ERROR
                            else if (recu.Type == TypePaquet.ERROR)
                            {
                                logger.Log(ConsoleSource.Serveur, "Erreur reçue.");
                                logger.Log(ConsoleSource.Serveur, ((ErrorPaquet)recu).MessageErreur);
                                Continuer = false;
                            }
                        }
                    }
                }
                // Timeout dépassé
                if (NbEssais == 10 && !AckRecu)
                    Continuer = false;
            }
            // Fin du thread, fermeture du socket et du fichier
            Event.Set();
            Terminer();
        }
    }
}
