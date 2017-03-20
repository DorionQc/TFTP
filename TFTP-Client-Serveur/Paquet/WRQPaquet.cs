/***********************************
 * Samuel Goulet
 * Serveur TFTP
 * 2017
 **********************************/

namespace TFTP_Client_Serveur.Paquet
{

    [TypePaquet(TypePaquet.WRQ)]
    public class WRQPaquet : InitPaquet
    {

        public WRQPaquet(string Fichier) : base(Fichier, TypePaquet.WRQ)
        { }

        public WRQPaquet() { }

    }
}
