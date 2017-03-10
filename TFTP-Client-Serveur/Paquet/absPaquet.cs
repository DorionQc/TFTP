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
        private static List<PaquetInfo> s_lClassesEnfant;

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
            PaquetInfo pi;
            IEnumerator<PaquetInfo> Enumerator;

            // Vérification de longueur minimale
            if (Data.Length < 2)
                return false;

            // Type du paquet
            type = (TypePaquet)Data[1];

            // Trouver la bonne classe pour ce type de paquet (par l'attribut de classe TypePaquetAttribute)
            Enumerator = s_lClassesEnfant.GetEnumerator();
            pi = Enumerator.Current; // Juste pour que le compilateur ne chiale pas que la variable soit potentiellement pas assignée
            while (Enumerator.MoveNext() && Paquet == null)
            {
                pi = Enumerator.Current;
                if (pi.TypePaquet == type)
                {
                    paquetInst = Activator.CreateInstance(pi.TypeClasse);
                    if (paquetInst is absPaquet)
                    {
                        Paquet = (absPaquet)paquetInst;
                    }
                }
            }

            // Si on a pas trouvé de type approprié
            if (Paquet == null)
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
            Attribs = pi.tAttribut.OrderBy(a => a.Position).ToArray();
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

                Valeurs[i] = getValue(att, Data, ref Pos);

                Logger.INSTANCE.Log(ConsoleSource.Interface, att.ToString() + " value = " + Valeurs[i].ToString());
                if (Valeurs[i] is Array)
                {
                    Array a = (Array)Valeurs[i];
                    for (int j = 0; j < a.Length; j++)
                    {
                        Logger.INSTANCE.Log(ConsoleSource.Interface, att.ToString() + " value at index " + j.ToString() + " = " + a.GetValue(j).ToString());
                    }
                    if (att.SubType == typeof(byte))
                    {
                        Logger.INSTANCE.Log(ConsoleSource.Interface, att.ToString() + " string value = " + Encoding.ASCII.GetString((byte[])a));
                    }
                }
                i++;
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
            s_lClassesEnfant = new List<PaquetInfo>();
            IEnumerable<Type> types = Assembly.GetAssembly(typeof(absPaquet)).GetTypes();
            types = types.Where(type => type.IsClass && !type.IsAbstract && type.IsSubclassOf(typeof(absPaquet))).ToArray();
            IEnumerator<Type> Enumerator = types.GetEnumerator();
            Type t;
            while (Enumerator.MoveNext())
            {
                t = Enumerator.Current;
                s_lClassesEnfant.Add(new PaquetInfo(t, getPaquetAttributes(t), getTypePaquet(t)));
            }
        }


        private static object getValue(ChampPaquetAttribute att, byte[] Data, ref int Position)
        {
            if (att.isCollection)
            {
                Type t = att.SubType;
                int size = Marshal.SizeOf(t);
                int init = Position;
                int i = 0;
                Array a;
                if (att.EndValue != null)
                {
                    bool Fin = false;
                    object val;
                    a = Array.CreateInstance(t, 16);
                    
                    while (Position + size <= Data.Length && i < a.Length && !Fin)
                    {
                        val = getValue(t, Data, ref Position);
                        if ((byte)val == (byte)att.EndValue)
                            Fin = true;
                        else
                        {
                            if (i == a.Length - 1)
                            {
                                if (a.Length == 512)
                                    Fin = true;
                                else
                                {
                                    Array tmp = Array.CreateInstance(t, a.Length << 1);
                                    Buffer.BlockCopy(a, 0, tmp, 0, a.Length);
                                    a = tmp;
                                }
                            }
                            a.SetValue(val, i);
                            i++;
                        }
                    }
                }
                else
                {
                    a = Array.CreateInstance(t, att.MaxLength);

                    while (Position + size <= Data.Length && i < att.MaxLength)
                    {
                        a.SetValue(getValue(t, Data, ref Position), i);
                        i++;
                    }
                }
                Array b = Array.CreateInstance(t, i);
                Buffer.BlockCopy(a, 0, b, 0, i);
                return b;
            }
            else
            {
                return getValue(att.Type, Data, ref Position);
            }
        }

        private static object getValue(Type t, byte[] Data, ref int Position)
        {
            int size = Marshal.SizeOf(t);
            if (Data.Length < Position + size)
                return null;

            Array a = Array.CreateInstance(t, 1);
            byte[] bytes = new byte[size];
            Buffer.BlockCopy(Data, Position, bytes, 0, size);
            byte b;
            // Swap, because BlockCopy would change the Endian :I
            for (int i = 0; i < size / 2; i++)
            {
                b = bytes[bytes.Length - 1 - i];
                bytes[bytes.Length - 1 - i] = bytes[i];
                bytes[i] = b;
            }
            Buffer.BlockCopy(bytes, 0, a, 0, size);

            Position += size;
            return a.GetValue(0);
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
