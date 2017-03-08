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
using System.Reflection;

namespace TFTP_Client_Serveur.Paquet
{
    public enum TypePaquet : byte
    {
        NULL = 0,
        RRQ = 1,
        WRQ = 2,
        DATA = 3,
        ACK = 4,
        ERROR = 5
    };

    public abstract class absPaquet
    {
        public TypePaquet Type;
        
        public absPaquet() { }

        public absPaquet(TypePaquet Type)
        {
            this.Type = Type;
        }

        public static bool Decoder(byte[] Data, out absPaquet Paquet)
        {
            Paquet = null;
            object paquetInst;
            TypePaquet type;
            Type t;

            if (Data.Length < 2)
            {
                return false;
            }

            type = (TypePaquet)Data[1];

            IEnumerator<Type> Enumerator = s_lClassesEnfant.GetEnumerator();
            while (Enumerator.MoveNext() && Paquet == null)
            {
                t = Enumerator.Current;
                if (getTypePaquet(t) == type)
                {
                    paquetInst = Activator.CreateInstance(t);
                    if (paquetInst is absPaquet)
                    {
                        Paquet = (absPaquet)paquetInst;
                        if (!Paquet.Decode(Data))
                            return false;
                        return true;
                    }
                }
            }

            return false;
        }

        public static bool Encoder(absPaquet Paquet, out byte[] Data)
        {
            return Paquet.Encode(out Data);
        }

        /// <summary>
        /// Modifie l'instance actuelle pour correspondre à la trame reçue.
        /// </summary>
        /// <param name="Data"></param>
        /// <returns>True si la modification s'est bien effectuée</returns>
        public abstract bool Decode(byte[] Data);

        /// <summary>
        /// Crée la tableau de bytes correspondant au paquet
        /// </summary>
        /// <param name="Data"></param>
        /// <returns>Un tableau de bytes contenant le paquet</returns>
        public abstract bool Encode(out byte[] Data);


        #region CodeObscure

        static List<Type> s_lClassesEnfant;

        // Eh oui, un constructeur statique. On veut que ce code soit exécuté 
        // dès qu'une classe de ce type existe
        static absPaquet()
        {
            // Recherche toutes les classes qui héritent de absPaquet
            s_lClassesEnfant = new List<Type>();
            foreach(Type t in Assembly.GetAssembly(typeof(absPaquet)).GetTypes()
                .Where(type => type.IsClass && !type.IsAbstract && type.IsSubclassOf(typeof(absPaquet))))
            {
                s_lClassesEnfant.Add(t);
            }
        }
        
        /// <summary>
        /// Trouve le type de paquet, déterminé par le champs statique TYPEPAQUET
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static TypePaquet getTypePaquet(Type t)
        {
            // Vérifie que la classe hérite bien de absPaquet, et est bien une classe instanciable.
            if (!(!t.IsAbstract && t.IsSubclassOf(typeof(absPaquet)) && t.IsClass))
                return TypePaquet.NULL;

            Attribute att = t.GetCustomAttribute(typeof(TypePaquetAttribute));

            if (att is TypePaquetAttribute)
                return ((TypePaquetAttribute)att).Type;
            return TypePaquet.NULL;

        }

        #endregion

    }
}
