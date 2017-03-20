/***********************************
 * Samuel Goulet
 * Serveur TFTP
 * 2017
 **********************************/

using System;
using System.Windows.Forms;
using System.IO;
using System.Net;
using TFTP_Client_Serveur.Client;

//using TFTP_Client_Serveur.Client;
using TFTP_Client_Serveur.Serveur;

namespace TFTP_Client_Serveur
{
    public partial class frmTFTP : Form
    {

        private DirectoryInfo m_DossierLocalServeur;
        private DirectoryInfo m_DossierLocalClient;
        private IPAddress m_AdresseLocale;
        private string m_DossierParDefaut;
        private Logger logger;

        private TFTPServeur m_Serveur;
        //private ClientTFTP m_Client;

        public frmTFTP()
        {
            InitializeComponent();
            // Pour savoir quand le textbox est prêt pour la console
            txtConsole.HandleCreated += TxtConsole_HandleCreated;

            // Création de la console
            logger = Logger.INSTANCE;
            logger.TextBox = txtConsole;

            // Dossier par défaut
            m_DossierParDefaut = Path.Combine(Directory.GetCurrentDirectory(), "TFTP");

            // Création du dossier si nécessaire
            if (!Directory.Exists(m_DossierParDefaut))
            {
                m_DossierLocalClient = Directory.CreateDirectory(m_DossierParDefaut);
            }
            else
            {
                m_DossierLocalClient = new DirectoryInfo(m_DossierParDefaut);
            }

            // Affichage du dossier par défaut
            m_DossierLocalServeur = m_DossierLocalClient;
            txtDossierClient.Text = m_DossierLocalClient.FullName;
            txtDossierServeur.Text = m_DossierLocalServeur.FullName;
        }

        private void TxtConsole_HandleCreated(object sender, EventArgs e)
        {
            // Indique que le textbox est prêt à recevoir la console
            // Vidage du group box d'IPs
            cbIPServeur.Items.Clear();
            // Recherche des IPs locales
            foreach (IPAddress ip in NetworkUtils.getLocalIPs())
            {
                cbIPServeur.Items.Add(ip.ToString());
                logger.Log(ConsoleSource.Interface, "IP Trouvée - " + ip);
            }
            // Sélection de la première IP
            if (cbIPServeur.Items.Count != 0)
                cbIPServeur.SelectedIndex = 0;
        }


        private void btnDemarrerServeur_Click(object sender, EventArgs e)
        {
            // Démarre le serveur!
            // Lecture de l'IP
            if (m_AdresseLocale == null)
            {
                logger.Log(ConsoleSource.Interface, "Veuillez choisir une adresse IP avant de créer le serveur.");
                return;
            }
            // Lecture du dossier
            if (m_DossierLocalServeur == null || !m_DossierLocalServeur.Exists)
            {
                logger.Log(ConsoleSource.Interface, "Le dossier désigné pour le serveur n'est pas valide.");
                return;
            }

            // Instanciation du singleton de serveur (qui ne devrait pas être un singleton, honnêtement)
            m_Serveur = TFTPServeur.INSTANCE;
            m_Serveur.Init(m_AdresseLocale, m_DossierLocalServeur);

            btnDemarrerServeur.Enabled = false;
            btnArreterServeur.Enabled = true;
            cbIPServeur.Enabled = false;
            btnRafraichirServeur.Enabled = false;
            btnDossierServeur.Enabled = false;
            txtDossierServeur.Enabled = false;

        }

        private void btnArreterServeur_Click(object sender, EventArgs e)
        {
            // Arrêt du serveur
            if (m_Serveur != null && m_Serveur.Initialised)
            {
                btnDemarrerServeur.Enabled = true;
                btnArreterServeur.Enabled = false;
                cbIPServeur.Enabled = true;
                btnRafraichirServeur.Enabled = true;
                btnDossierServeur.Enabled = true;
                txtDossierServeur.Enabled = true;
                m_Serveur.Arret();
            }
        }

        private void btnEnvoyerClient_Click(object sender, EventArgs e)
        {
            

            ClientTFTP client = new ClientTFTP(txtIPClient.Text);
            client.WRQ(txtFichierDistantClient.Text,txtDossierClient.Text + "/" + txtFichierLocalClient.Text);
        }

        private void btnRecevoirClient_Click(object sender, EventArgs e)
        {
            ClientTFTP client = new ClientTFTP(txtIPClient.Text);
            client.RRQ(txtFichierDistantClient.Text, txtDossierClient.Text + "/" + txtFichierLocalClient.Text);
        }

        private void btnDossierClient_Click(object sender, EventArgs e)
        {
            // Bouton pour choisir le dossier du côté client
            DirectoryInfo temp;
            if (DialogueDossier(out temp))
            {
                txtDossierClient.Text = temp.FullName;
                m_DossierLocalClient = temp;
                logger.Log(ConsoleSource.Client, "Utilisation de " + temp.FullName + " comme dossier.");
            }
        }

        private void btnDossierServeur_Click(object sender, EventArgs e)
        {
            // Bouton pour choisir le dossier du côté serveur
            DirectoryInfo temp;
            if (DialogueDossier(out temp))
            {
                txtDossierServeur.Text = temp.FullName;
                m_DossierLocalServeur = temp;
                logger.Log(ConsoleSource.Serveur, "Utilisation de " + temp.FullName + " comme dossier.");
            }
        }

        private bool DialogueDossier(out DirectoryInfo dir)
        {
            // Sélection du dossier par dialogue
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            dir = null;
            if (fbd.ShowDialog() != DialogResult.OK)
                return false;
            else
            {
                dir = new DirectoryInfo(fbd.SelectedPath);
                return true;
            }
            

        }

        private void cbIPServeur_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Changement d'IP
            if (cbIPServeur.SelectedIndex < 0)
            {
                cbIPServeur.Text = "";
                return;
            }

            int Indice = cbIPServeur.SelectedIndex;

            if (!IPAddress.TryParse(cbIPServeur.Items[Indice].ToString(), out m_AdresseLocale))
            {
                // IP Invalide
                logger.Log(ConsoleSource.Interface, "Impossible de lire l'IP - " + cbIPServeur.Items[Indice]);
                cbIPServeur.Items.RemoveAt(Indice);
                cbIPServeur.SelectedIndex = cbIPServeur.Items.Count - 1;
                cbIPServeur_SelectedIndexChanged(null, null);
                m_AdresseLocale = IPAddress.None;
                return;
            }

            if (!m_AdresseLocale.Equals(IPAddress.None))
            {
                // IP lisible et non-nulle
                logger.Log(ConsoleSource.Serveur, "Utilisation de l'adresse IP " + m_AdresseLocale);
            }


        }

        private void btnRafraichirServeur_Click(object sender, EventArgs e)
        {
            // Rafraichir la liste d'IP locales
            TxtConsole_HandleCreated(null, null);
        }

        private void frmTFTP_FormClosing(object sender, FormClosingEventArgs e)
        {
            btnArreterServeur_Click(null, null);
        }
    }
}
