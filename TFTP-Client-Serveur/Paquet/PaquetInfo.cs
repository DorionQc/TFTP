using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TFTP_Client_Serveur.Paquet
{
    public struct PaquetInfo
    {
        public Type TypeClasse;
        public ChampPaquetAttribute[] tAttribut;
        public TypePaquet TypePaquet;

        public PaquetInfo(Type t, ChampPaquetAttribute[] Attributs, TypePaquet type)
        {
            TypeClasse = t;
            tAttribut = Attributs;
            TypePaquet = type;
        }
    }
}
