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
using System.Runtime.InteropServices;

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



    [ChampPaquet("Type", 0, typeof(short))]
    public abstract class absPaquet
    {
        /// <summary>
        /// Liste des types dérivés de cette classe
        /// </summary>
        private static List<Type> s_lClassesEnfant;

        /// <summary>
        /// Décode un paquet et retourne une trame du type approprié
        /// </summary>
        /// <param name="Data"></param>
        /// <param name="Paquet"></param>
        /// <returns></returns>
        public static bool Decoder(byte[] Data, out absPaquet Paquet)
        {
            // Quelques variables pour commencer
            Paquet = null;
            object paquetInst;
            TypePaquet type;
            Type t = null;
            IEnumerator<Type> Enumerator;

            // Vérification de longueur minimale
            if (Data.Length < 2)
                return false;

            // Type du paquet
            type = (TypePaquet)Data[1];

            // Trouver la bonne classe pour ce type de paquet (par l'attribut de classe TypePaquetAttribute)
            Enumerator = s_lClassesEnfant.GetEnumerator();
            while (Enumerator.MoveNext() && Paquet == null)
            {
                t = Enumerator.Current;
                if (getTypePaquet(t) == type)
                {
                    paquetInst = Activator.CreateInstance(t);
                    if (paquetInst is absPaquet)
                    {
                        Paquet = (absPaquet)paquetInst;
                    }
                }
            }

            // Si on a pas trouvé de type approprié
            if (Paquet == null || t == null)
                return false;

            // Quelques autres variables
            object[] Valeurs;
            ChampPaquetAttribute[] Attribs;
            ChampPaquetAttribute att;
            int Pos = 0;
            int i = 0;
            int Max = Data.Length;
            int Taille;



            // Recherche des champs de la trame
            Attribs = getPaquetAttributes(t).OrderBy(a => a.Position).ToArray();
            Valeurs = new object[Attribs.Length];

            while (Pos < Max && i < Attribs.Length)
            {
                att = Attribs[i];

                // Taille de la mémoire à allouer
                Taille = Attribs[i].Size;
                if (Taille == -1 && att.EndValue == null)
                {
                    if (i != Attribs.Length - 1)
                        throw new IndexOutOfRangeException("L'attribut calculé n'a pas de fin définie.");
                    else
                        Taille = Data.Length - Pos;
                }
                Logger.INSTANCE.Log(ConsoleSource.Interface, att.ToString() + " ||| Taille = " + Taille.ToString());
                i++;
                Pos++;
            }

            return false;
        }

        public static bool Encoder(absPaquet Paquet, out byte[] Data)
        {
            return Paquet.Encode(out Data);
        }


        // Eh oui, un constructeur statique. On veut que ce code soit exécuté 
        // dès qu'une classe de ce type existe
        static absPaquet()
        {
            // Recherche toutes les classes qui héritent de absPaquet
            s_lClassesEnfant = new List<Type>();
            foreach (Type t in Assembly.GetAssembly(typeof(absPaquet)).GetTypes()
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
        private static TypePaquet getTypePaquet(Type t)
        {
            // Vérifie que la classe hérite bien de absPaquet, et est bien une classe instanciable.
            if (!(!t.IsAbstract && t.IsSubclassOf(typeof(absPaquet)) && t.IsClass))
                return TypePaquet.NULL;

            Attribute att = t.GetCustomAttribute(typeof(TypePaquetAttribute));

            if (att is TypePaquetAttribute)
                return ((TypePaquetAttribute)att).Type;
            return TypePaquet.NULL;
        }


        /// <summary>
        /// Donne la liste des ChampPaquetAttributes d'une trame
        /// </summary>
        /// <param name="t">Type de la trame</param>
        /// <returns>La liste des ChampPaquetAttributes d'une trame</returns>
        private static ChampPaquetAttribute[] getPaquetAttributes(Type t)
        {
            if (!(!t.IsAbstract && t.IsSubclassOf(typeof(absPaquet)) && t.IsClass))
                return null;

            return (ChampPaquetAttribute[])t.GetCustomAttributes(inherit: true, attributeType: typeof(ChampPaquetAttribute));
        }

        /// <summary>
        /// Donne l'attribut d'un nom spécifié d'une trame
        /// </summary>
        /// <param name="paquet"></param>
        /// <param name="Nom"></param>
        /// <returns>L'attribut ayant le nom spécifié</returns>
        private ChampPaquetAttribute getPaquetAttribute(absPaquet paquet, string Nom)
        {
            return getPaquetAttributes(paquet.GetType()).Where(t => t.Nom == Nom).FirstOrDefault();
        }














        protected TypePaquet m_Type;
        public TypePaquet Type
        {
            get { return m_Type; }
            set { m_Type = value; }
        }
        public absPaquet() { }
        public absPaquet(TypePaquet t) { }

        public abstract bool Decode(byte[] Data);
        public abstract bool Encode(out byte[] Data);

    }
}
