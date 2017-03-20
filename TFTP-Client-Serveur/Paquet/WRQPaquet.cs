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

        /*// Ce champ doit OBLIGATOIREMENT exister pour que le paquet soit reconnu.
        public static TypePaquet TYPEPAQUET = TypePaquet.WRQ;*/

        public WRQPaquet(string Fichier) : base(Fichier, TypePaquet.WRQ)
        { }

        public WRQPaquet() { }

    }
}
