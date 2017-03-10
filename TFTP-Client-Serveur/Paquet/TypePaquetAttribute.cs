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

namespace TFTP_Client_Serveur.Paquet
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class TypePaquetAttribute : Attribute
    {
        private readonly TypePaquet m_Type;

        public TypePaquetAttribute(TypePaquet Type)
        {
            m_Type = Type;
        }

        public TypePaquet Type
        {
            get { return m_Type; }
        }
    }
}
