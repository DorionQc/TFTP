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
    [TypePaquet(TypePaquet.ACK)]
    public class AckPaquet : absPaquet
    {
        private ushort m_NoBlock;

        public AckPaquet() { }

        public AckPaquet(ushort NoBlock) : base(TypePaquet.ACK)
        {
            m_NoBlock = NoBlock;
        }

        public ushort NoBlock
        {
            get { return m_NoBlock; }
        }


        public override bool Decode(byte[] Data)
        {
            if (Data.Length < 4)
                return false;
            if (Data[0] != 0 || Data[1] != (byte)TypePaquet.ACK)
                return false;
            Type = TypePaquet.ACK;

            m_NoBlock = (ushort)((Data[2] << 8) + Data[3]);
            return true;
        }

        public override bool Encode(out byte[] Data)
        {
            Data = new byte[4];
            Data[0] = 0;
            Data[1] = (byte)Type;
            Data[2] = (byte)((m_NoBlock & 0xff00) >> 8);
            Data[3] = (byte)(m_NoBlock & 0xff);
            return true;
        }
    }
}
