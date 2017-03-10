﻿/***********************************
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
    [ChampPaquet("Block", 1, typeof(short))]
    [ChampPaquet("Data", 2, typeof(byte[]), MaxLength: 512)]
    [TypePaquet(TypePaquet.DATA)]
    public class DataPaquet : absPaquet
    {
        
        private short m_NoBlock;
        private byte[] m_Data;
        private bool m_Dernier;

        public DataPaquet() { }

        public DataPaquet(short NoBlock, byte[] Data) : base(TypePaquet.DATA)
        {
            m_NoBlock = NoBlock;
            if (Data.Length > 512)
            {
                Logger.INSTANCE.Log(ConsoleSource.Serveur, "[ERREUR] Le paquet #" + m_NoBlock.ToString() + " contient plus de 512 octets!!!");
                m_Data = Data.Take(512).ToArray();
            }
            else
                m_Data = Data;
        }

        public bool EstDernier
        {
            get { return m_Data.Length < 512; }
        }

        public short NoBlock
        {
            get { return m_NoBlock; }
        }

        public byte[] Data
        {
            get { return m_Data; }
        }

        public override bool Decode(byte[] Data)
        {
            if (Data.Length < 4)
                return false;
            if (Data[0] != 0 || Data[1] != (byte)TypePaquet.DATA)
                return false;
            m_NoBlock = BitConverter.ToInt16(Data, 2);
            if (Data.Length > 516)
            {
                Logger.INSTANCE.Log(ConsoleSource.Serveur, "[ERREUR] Le paquet #" + m_NoBlock.ToString() + " contient plus de 512 octets!!!");
                m_Data = new byte[512];
                Array.Copy(Data, 4, m_Data, 0, 512);
            }
            else if (Data.Length > 4)
            {
                m_Data = new byte[Data.Length - 4];
                Array.Copy(Data, 4, m_Data, 0, Data.Length - 4);
            }
            else if (Data.Length == 4)
            {
                m_Data = new byte[0];
            }
            this.Type = TypePaquet.DATA;
            return true;


        }

        public override bool Encode(out byte[] Data)
        {
            Data = new byte[4 + m_Data.Length];
            Data[0] = 0;
            Data[1] = (byte)this.Type;
            Data[2] = (byte)((m_NoBlock & 0xff00) >> 8);
            Data[3] = (byte)(m_NoBlock & 0xff);
            Array.Copy(m_Data, 0, Data, 4, m_Data.Length);
            return true;
        }
    }
}
