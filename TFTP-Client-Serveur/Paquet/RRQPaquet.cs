/***********************************
 * Samuel Goulet
 * Serveur TFTP
 * 2017
 **********************************/

namespace TFTP_Client_Serveur.Paquet
{

    [TypePaquet(TypePaquet.RRQ)]
    public class RRQPaquet : InitPaquet
    {
        public RRQPaquet(string Fichier) : base(Fichier, TypePaquet.RRQ)
        { }

        public RRQPaquet() { }

    }
}
