/***********************************
 * Samuel Goulet
 * Serveur TFTP
 * 2017
 **********************************/
 
using System;
using System.Text;

namespace TFTP_Client_Serveur.Paquet
{
    // Classe abstraite de laquelle hérite RRQ et WRQ
    public abstract class InitPaquet : absPaquet
    {
        private string m_Fichier;
        private string m_Mode;

        protected InitPaquet() { }

        protected InitPaquet(string Fichier, TypePaquet type) : base(type)
        {
            m_Fichier = Fichier;
            m_Mode = "octet";
        }

        public string Fichier
        {
            get { return m_Fichier; }
        }

        public string Mode
        {
            get { return m_Mode; }
        }

        public override bool Decode(byte[] Data)
        {
            
            if (Data.Length < 2)
                return false;
            if (Data[0] != 0 || Data[1] > 2 || Data[1] < 1)
                return false;
            int i = 2;
            StringBuilder sb = new StringBuilder();
            string Fichier;

            while (i < Data.Length && Data[i] != 0)
            {
                sb.Append((char)Data[i]);
                i++;
            }
            Fichier = sb.ToString();
            sb.Clear();
            i++;
            while (i < Data.Length && Data[i] != 0)
            {
                sb.Append((char)Data[i]);
                i++;
            }
            if (sb.ToString() != "octet" || Fichier == "")
                return false;

            m_Fichier = Fichier;
            m_Mode = sb.ToString();

            Type = (TypePaquet)Data[1];

            return true;
        }

        public override bool Encode(out byte[] Data)
        {
            byte[] FileBytes = Encoding.ASCII.GetBytes(m_Fichier);
            byte[] ModeBytes = Encoding.ASCII.GetBytes(m_Mode);

            Data = new byte[4 + FileBytes.Length + ModeBytes.Length];

            Data[0] = 0;
            Data[1] = (byte)Type;
            Array.Copy(FileBytes, 0, Data, 2, FileBytes.Length);
            Data[FileBytes.Length + 2] = 0;
            Array.Copy(ModeBytes, 0, Data, FileBytes.Length + 3, ModeBytes.Length);
            Data[Data.Length - 1] = 0;

            return true;

        }


    }
}
