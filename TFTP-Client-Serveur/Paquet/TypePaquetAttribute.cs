/***********************************
 * Samuel Goulet
 * Serveur TFTP
 * 2017
 **********************************/
 
using System;

namespace TFTP_Client_Serveur.Paquet
{
    // Attribut de classe qui définit le type d'un paquet (ex. [TypePaquet(TypePaquet.WRQ)])
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
