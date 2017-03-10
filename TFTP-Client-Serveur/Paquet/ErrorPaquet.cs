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
using System.Runtime.InteropServices;

namespace TFTP_Client_Serveur.Paquet
{
    public enum CodeErreur : short
    {
        Undefined = 0,
        FileNotFound = 1,
        AccessViolation = 2,
        DiskFull = 3,
        IllegalOperation = 4,
        UnknownTransferID = 5,
        FileExists = 6,
        NoSuchUser = 7
    };

    [ChampPaquet("ErrorCode", 1, typeof(short))]
    [ChampPaquet("ErrorMessage", 2, typeof(byte[]), EndValue: (byte)0)]
    [TypePaquet(TypePaquet.ERROR)]
    public class ErrorPaquet : absPaquet
    {
        
        private CodeErreur m_CodeErreur;
        private string m_MessageErreur;

        public ErrorPaquet() { }

        public ErrorPaquet(CodeErreur CodeErreur, string MessageErreur) : base(TypePaquet.ERROR)
        {
            m_CodeErreur = CodeErreur;
            m_MessageErreur = MessageErreur;
        }

        public CodeErreur CodeErreur
        {
            get { return m_CodeErreur; }
        }

        public string MessageErreur
        {
            get { return m_MessageErreur; }
        }

        public override bool Decode(byte[] Data)
        {
            StringBuilder sb;
            int i;
            short CodeErreur;
            if (Data.Length < 4)
                return false;
            if (Data[0] != 0 || Data[1] != (byte)TypePaquet.ERROR)
                return false;
            CodeErreur = BitConverter.ToInt16(Data, 2);
            if (CodeErreur < 0 || CodeErreur > 7)
                return false;
            i = 4;
            sb = new StringBuilder();
            while (i < Data.Length && Data[i] != 0)
            {
                sb.Append((char)Data[i]);
                i++;
            }
            if (sb.ToString() == "")
                return false;
            m_CodeErreur = (CodeErreur)CodeErreur;
            m_MessageErreur = sb.ToString();
            this.Type = TypePaquet.ERROR;
            return true;
        }

        public override bool Encode(out byte[] Data)
        {
            byte[] bytesErreur = Encoding.ASCII.GetBytes(m_MessageErreur);
            Data = new byte[5 + bytesErreur.Length];

            Data[0] = 0;
            Data[1] = (byte)this.Type;
            Data[2] = (byte)(((short)m_CodeErreur & 0xff00) >> 8);
            Data[3] = (byte)((short)m_CodeErreur & 0xff);
            Array.Copy(bytesErreur, 0, Data, 4, bytesErreur.Length);
            Data[Data.Length - 1] = 0;
            return true;

        }


    }
}
