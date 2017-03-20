/***********************************
 * Samuel Goulet
 * Serveur TFTP
 * 2017
 **********************************/
 
using System;
using System.Windows.Forms;

namespace TFTP_Client_Serveur
{
    static class pgmTFTP
    {
        /// <summary>
        /// Point d'entrée principal de l'application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new frmTFTP());
        }
    }
}
