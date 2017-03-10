using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace TFTP_Client_Serveur.Paquet
{

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public sealed class ChampPaquetAttribute : Attribute
    {
        private readonly string m_Nom;
        private readonly int m_Position;
        private readonly Type m_Type;
        private readonly object m_EndValue;
        private readonly int m_MaxLength;
        private readonly Type m_endValueType;
        private readonly bool m_isCollection;

        public ChampPaquetAttribute(string Nom, int Position, Type type, object EndValue = null, int MaxLength = -1)
        {
            m_Nom = Nom;
            m_Position = Position;
            m_Type = type;
            m_endValueType = this.getElementType();
            m_isCollection = this.Type.GetInterface("ICollection") != null;
            if (!m_endValueType.IsPrimitive)
                throw new InvalidCastException("Le type de " + m_Nom + " n'est pas primitif");
            if (m_endValueType == typeof(string))
                throw new InvalidCastException("Utilisez le type byte[] ou char[] pour les tableaux de caractères");
            if (m_endValueType == typeof(DateTime))
                throw new InvalidCastException("Le type DateTime n'est pas supporté");
            if (EndValue != null)
            {
                if (EndValue.GetType() == m_endValueType)
                    m_EndValue = EndValue;
                else
                    throw new InvalidCastException("La valeur de fin de l'attribut " + m_Nom + " ne correspond pas à son type");
            }
            if (MaxLength != -1)
            {
                m_MaxLength = MaxLength;
            }
            

        }

        private Type getElementType()
        {
            Type t;
            t = this.Type.GetElementType();
            if (t != null) return t;
            t = this.Type.GetGenericArguments().FirstOrDefault();
            if (t != null) return t;
            return this.Type;
        }

        public string Nom
        {
            get { return m_Nom; }
        }
        public int Position
        {
            get { return m_Position; }
        }

        public Type Type
        {
            get { return m_Type; }
        }

        public object EndValue
        {
            get { return m_EndValue; }
        }

        public int MaxLength
        {
            get { return m_MaxLength; }
        }

        public override string ToString()
        {
            return this.Nom + " at " + m_Position.ToString() + " of type " + m_Type.ToString();
        }

        public bool isCollection
        {
            get { return m_isCollection; }
        }

        public int Size
        {
            get
            {
                Type t = m_endValueType;
                if (m_isCollection)
                {
                    if (this.MaxLength != -1)
                        return Marshal.SizeOf(t) * MaxLength;
                    return -1; // Si c'est une collection sans taille maximale, il est impossible de déterminer de manière statique la taille de la valeur contenue
                }
                return Marshal.SizeOf(t);
            }
        }

        public Type SubType
        {
            get { return m_endValueType; }
        }
    }
}
