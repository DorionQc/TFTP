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
using System.Windows.Forms;

namespace TFTP_Client_Serveur
{
    // Différents acteurs pouvant utiliser la console.
    public enum ConsoleSource
    {
        Serveur,
        Client,
        Interface
    };

    public class Logger
    {

        // Singleton!

        private static Logger __instance;
        public static Logger INSTANCE
        {
            get
            {
                if (__instance == null)
                    __instance = new Logger();
                return __instance;
            }
        }

        // Propriété pour le textbox à utiliser
        public TextBox TextBox
        {
            get
            {
                return m_Console;
            }

            set
            {
                m_Console = value;
            }
        }

        private TextBox m_Console;

        // Constructeur privé
        private Logger() { }

        /// <summary>
        /// Ajoute un message à la console
        /// </summary>
        /// <param name="source">Acteur utilisant la console</param>
        /// <param name="Message">Message à afficher</param>
        public void Log(ConsoleSource source, string Message)
        {
            m_Console.Invoke(new Action(() =>
            {
                m_Console.AppendText("[" + source.ToString() + "] " + Message + "\r\n");
            }));
        }

        /// <summary>
        /// Vide la console
        /// </summary>
        public void Clear()
        {
            m_Console.Invoke(new Action(() =>
            {
                m_Console.Text = "";
            }));
        }

    }
}
