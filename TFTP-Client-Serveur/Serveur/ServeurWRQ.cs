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
    class ServeurWRQ : Connection
    {

        public ServeurWRQ(string Fichier, EndPoint Distant, EndPoint Local)// : base()
        {
            m_NomFichier = Fichier;
            m_DistantEP = Distant;
            m_LocalEP = Local;
            Demarrer();
        }

        protected override void Communication()
        {
            int len;
            bool PacketRecu;
            byte[] buffer = new byte[516];
            long NumeroPaquet = 0;
            byte NbEssais = 0;
            Continuer = true;
            absPaquet recu;
            AckPaquet ack;
            DateTime temps;


            // Au cas où...
            Event.WaitOne();
            Event.Reset();

            logger.Log(ConsoleSource.Serveur, "Nouvelle connection WRQ vers " + m_DistantEP);


            if (!File.Exists(m_NomFichier))
            {

                try
                {
                    m_fs = File.Create(m_NomFichier);
                    m_bw = new BinaryWriter(m_fs);
                    m_bw.Seek(0, SeekOrigin.Begin);
                }
                catch (Exception ex)
                {
                    Envoyer(new ErrorPaquet(CodeErreur.AccessViolation, "Erreur lors de l'écriture du fichier : " + ex.Message));
                    //logger.Log(ConsoleSource.Serveur, "Erreur lors de l'écriture du fichier : " + ex.Message);
                    Event.Set();
                    Terminer();
                    return;
                }
            }
            else
            {
                Envoyer(new ErrorPaquet(CodeErreur.FileExists, "Le fichier existe déjà"));
                //logger.Log(ConsoleSource.Serveur, "Le fichier existe déjà");
                Event.Set();
                Terminer();
                return;
            }
            
            // Envoi du  ack
            ack = new AckPaquet((ushort)NumeroPaquet);
            Envoyer(ack);
            while (Continuer)
            {
                
                
                temps = DateTime.Now;
                PacketRecu = false;
                NbEssais++;

                while (!PacketRecu && DateTime.Now.Ticks - temps.Ticks < 50000000)
                {
                    if (Socket.Available == 0)
                        Thread.Sleep(0);
                    else
                    {
                        try
                        {
                            len = Socket.ReceiveFrom(buffer, ref m_DistantEP);
                        }
                        catch (Exception ex)
                        {
                            Continuer = false;
                            logger.Log(ConsoleSource.Serveur, ex.Message);
                            break;
                        }

                        if (len != 0 && absPaquet.Decoder(buffer, out recu))
                        {
                            NumeroPaquet++;
                            if (recu.Type == TypePaquet.DATA && ((DataPaquet) recu).NoBlock == (ushort) NumeroPaquet)
                            {
                                //logger.Log(ConsoleSource.Serveur, "Réception du paquet #" + ((DataPaquet)recu).NoBlock.ToString());
                                PacketRecu = true;
                                NbEssais = 0;
                               // NumeroPaquet++;
                                //   m_bw.Seek(0, SeekOrigin.Current); //duh

                                m_bw.Write(((DataPaquet) recu).Data,0,len-4);
                                m_bw.Flush();
                                if (len < 516)
                                    Continuer = false;
                            }
                            else
                            {
                                NumeroPaquet--;

                                if (recu.Type == TypePaquet.ERROR)
                                {
                                    logger.Log(ConsoleSource.Serveur, "Erreur reçue.");
                                    logger.Log(ConsoleSource.Serveur, ((ErrorPaquet) recu).MessageErreur);
                                    Continuer = false;
                                }
                            }
                        }
                    }
                }

                if (NbEssais == 10)
                    Continuer = false;
                // Envoi des acks
                ack = new AckPaquet((ushort)NumeroPaquet);
                //logger.Log(ConsoleSource.Serveur, "Envoi du ACK #" + NumeroPaquet);
                Envoyer(ack);
            }
            m_bw.Close();
            Event.Set();
            Terminer();
        }
    }
}
