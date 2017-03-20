/***********************************
 * Samuel Goulet
 * Serveur TFTP
 * 2017
 **********************************/
 
using System;
using System.Text;

namespace TFTP_Client_Serveur.Paquet
{
    public enum CodeErreur : ushort
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

    [TypePaquet(TypePaquet.ERROR)]
    public class ErrorPaquet : absPaquet
    {
        /*// Ce champ doit OBLIGATOIREMENT exister pour que le paquet soit reconnu.
        public static TypePaquet TYPEPAQUET = TypePaquet.ERROR;*/

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
            ushort CodeErreur;
            if (Data.Length < 4)
                return false;
            if (Data[0] != 0 || Data[1] != (byte)TypePaquet.ERROR)
                return false;
            CodeErreur = BitConverter.ToUInt16(Data, 2);
            if (CodeErreur > 7)
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
            Type = TypePaquet.ERROR;
            return true;
        }

        public override bool Encode(out byte[] Data)
        {
            byte[] bytesErreur = Encoding.ASCII.GetBytes(m_MessageErreur);
            Data = new byte[5 + bytesErreur.Length];

            Data[0] = 0;
            Data[1] = (byte)Type;
            Data[2] = (byte)(((ushort)m_CodeErreur & 0xff00) >> 8);
            Data[3] = (byte)((ushort)m_CodeErreur & 0xff);
            Array.Copy(bytesErreur, 0, Data, 4, bytesErreur.Length);
            Data[Data.Length - 1] = 0;
            return true;

        }


    }
}
