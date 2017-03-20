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
        
        /*// Ce champ doit OBLIGATOIREMENT exister pour que le paquet soit reconnu.
        public static TypePaquet TYPEPAQUET = TypePaquet.RRQ;*/

        public RRQPaquet(string Fichier) : base(Fichier, TypePaquet.RRQ)
        { }

        public RRQPaquet() { }

    }
}
