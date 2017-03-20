namespace TFTP_Client_Serveur
{
    partial class frmTFTP
    {
        /// <summary>
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Nettoyage des ressources utilisées.
        /// </summary>
        /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Code généré par le Concepteur Windows Form

        /// <summary>
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            this.gpServeur = new System.Windows.Forms.GroupBox();
            this.btnRafraichirServeur = new System.Windows.Forms.Button();
            this.cbIPServeur = new System.Windows.Forms.ComboBox();
            this.btnArreterServeur = new System.Windows.Forms.Button();
            this.btnDemarrerServeur = new System.Windows.Forms.Button();
            this.btnDossierServeur = new System.Windows.Forms.Button();
            this.txtDossierServeur = new System.Windows.Forms.TextBox();
            this.lblDossierServeur = new System.Windows.Forms.Label();
            this.lblPortServeur = new System.Windows.Forms.Label();
            this.lblIPServeur = new System.Windows.Forms.Label();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.gpClient = new System.Windows.Forms.GroupBox();
            this.btnRecevoirClient = new System.Windows.Forms.Button();
            this.btnEnvoyerClient = new System.Windows.Forms.Button();
            this.txtFichierDistantClient = new System.Windows.Forms.TextBox();
            this.lblFichierDistantClient = new System.Windows.Forms.Label();
            this.lblIPClient = new System.Windows.Forms.Label();
            this.txtFichierLocalClient = new System.Windows.Forms.TextBox();
            this.txtIPClient = new System.Windows.Forms.TextBox();
            this.lblPortClient = new System.Windows.Forms.Label();
            this.lblFichierLocalClient = new System.Windows.Forms.Label();
            this.txtDossierClient = new System.Windows.Forms.TextBox();
            this.btnDossierClient = new System.Windows.Forms.Button();
            this.lblDossierClient = new System.Windows.Forms.Label();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.txtConsole = new System.Windows.Forms.TextBox();
            this.gpServeur.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.gpClient.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.SuspendLayout();
            // 
            // gpServeur
            // 
            this.gpServeur.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.gpServeur.Controls.Add(this.btnRafraichirServeur);
            this.gpServeur.Controls.Add(this.cbIPServeur);
            this.gpServeur.Controls.Add(this.btnArreterServeur);
            this.gpServeur.Controls.Add(this.btnDemarrerServeur);
            this.gpServeur.Controls.Add(this.btnDossierServeur);
            this.gpServeur.Controls.Add(this.txtDossierServeur);
            this.gpServeur.Controls.Add(this.lblDossierServeur);
            this.gpServeur.Controls.Add(this.lblPortServeur);
            this.gpServeur.Controls.Add(this.lblIPServeur);
            this.gpServeur.Location = new System.Drawing.Point(26, 41);
            this.gpServeur.Name = "gpServeur";
            this.gpServeur.Size = new System.Drawing.Size(427, 155);
            this.gpServeur.TabIndex = 3;
            this.gpServeur.TabStop = false;
            this.gpServeur.Text = "Serveur";
            // 
            // btnRafraichirServeur
            // 
            this.btnRafraichirServeur.Location = new System.Drawing.Point(249, 15);
            this.btnRafraichirServeur.Name = "btnRafraichirServeur";
            this.btnRafraichirServeur.Size = new System.Drawing.Size(75, 23);
            this.btnRafraichirServeur.TabIndex = 11;
            this.btnRafraichirServeur.Text = "Rafraichir";
            this.btnRafraichirServeur.UseVisualStyleBackColor = true;
            this.btnRafraichirServeur.Click += new System.EventHandler(this.btnRafraichirServeur_Click);
            // 
            // cbIPServeur
            // 
            this.cbIPServeur.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbIPServeur.FormattingEnabled = true;
            this.cbIPServeur.Location = new System.Drawing.Point(91, 17);
            this.cbIPServeur.Name = "cbIPServeur";
            this.cbIPServeur.Size = new System.Drawing.Size(121, 21);
            this.cbIPServeur.TabIndex = 10;
            this.cbIPServeur.SelectedIndexChanged += new System.EventHandler(this.cbIPServeur_SelectedIndexChanged);
            // 
            // btnArreterServeur
            // 
            this.btnArreterServeur.Enabled = false;
            this.btnArreterServeur.Location = new System.Drawing.Point(346, 69);
            this.btnArreterServeur.Name = "btnArreterServeur";
            this.btnArreterServeur.Size = new System.Drawing.Size(75, 23);
            this.btnArreterServeur.TabIndex = 7;
            this.btnArreterServeur.Text = "Arrêter";
            this.btnArreterServeur.UseVisualStyleBackColor = true;
            this.btnArreterServeur.Click += new System.EventHandler(this.btnArreterServeur_Click);
            // 
            // btnDemarrerServeur
            // 
            this.btnDemarrerServeur.Location = new System.Drawing.Point(10, 69);
            this.btnDemarrerServeur.Name = "btnDemarrerServeur";
            this.btnDemarrerServeur.Size = new System.Drawing.Size(75, 23);
            this.btnDemarrerServeur.TabIndex = 6;
            this.btnDemarrerServeur.Text = "Démarrer";
            this.btnDemarrerServeur.UseVisualStyleBackColor = true;
            this.btnDemarrerServeur.Click += new System.EventHandler(this.btnDemarrerServeur_Click);
            // 
            // btnDossierServeur
            // 
            this.btnDossierServeur.BackgroundImage = global::TFTP_Client_Serveur.Properties.Resources.ico41;
            this.btnDossierServeur.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btnDossierServeur.Location = new System.Drawing.Point(352, 41);
            this.btnDossierServeur.Name = "btnDossierServeur";
            this.btnDossierServeur.Size = new System.Drawing.Size(25, 22);
            this.btnDossierServeur.TabIndex = 5;
            this.btnDossierServeur.UseVisualStyleBackColor = true;
            this.btnDossierServeur.Click += new System.EventHandler(this.btnDossierServeur_Click);
            // 
            // txtDossierServeur
            // 
            this.txtDossierServeur.Location = new System.Drawing.Point(89, 43);
            this.txtDossierServeur.Name = "txtDossierServeur";
            this.txtDossierServeur.Size = new System.Drawing.Size(257, 20);
            this.txtDossierServeur.TabIndex = 4;
            // 
            // lblDossierServeur
            // 
            this.lblDossierServeur.AutoSize = true;
            this.lblDossierServeur.Location = new System.Drawing.Point(32, 46);
            this.lblDossierServeur.Name = "lblDossierServeur";
            this.lblDossierServeur.Size = new System.Drawing.Size(51, 13);
            this.lblDossierServeur.TabIndex = 3;
            this.lblDossierServeur.Text = "Dossier =";
            // 
            // lblPortServeur
            // 
            this.lblPortServeur.AutoSize = true;
            this.lblPortServeur.Location = new System.Drawing.Point(218, 20);
            this.lblPortServeur.Name = "lblPortServeur";
            this.lblPortServeur.Size = new System.Drawing.Size(25, 13);
            this.lblPortServeur.TabIndex = 2;
            this.lblPortServeur.Text = ": 69";
            // 
            // lblIPServeur
            // 
            this.lblIPServeur.AutoSize = true;
            this.lblIPServeur.Location = new System.Drawing.Point(16, 20);
            this.lblIPServeur.Name = "lblIPServeur";
            this.lblIPServeur.Size = new System.Drawing.Size(69, 13);
            this.lblIPServeur.TabIndex = 0;
            this.lblIPServeur.Text = "IP Serveur = ";
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.gpServeur);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.gpClient);
            this.splitContainer1.Size = new System.Drawing.Size(993, 236);
            this.splitContainer1.SplitterDistance = 479;
            this.splitContainer1.TabIndex = 4;
            // 
            // gpClient
            // 
            this.gpClient.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.gpClient.Controls.Add(this.btnRecevoirClient);
            this.gpClient.Controls.Add(this.btnEnvoyerClient);
            this.gpClient.Controls.Add(this.txtFichierDistantClient);
            this.gpClient.Controls.Add(this.lblFichierDistantClient);
            this.gpClient.Controls.Add(this.lblIPClient);
            this.gpClient.Controls.Add(this.txtFichierLocalClient);
            this.gpClient.Controls.Add(this.txtIPClient);
            this.gpClient.Controls.Add(this.lblPortClient);
            this.gpClient.Controls.Add(this.lblFichierLocalClient);
            this.gpClient.Controls.Add(this.txtDossierClient);
            this.gpClient.Controls.Add(this.btnDossierClient);
            this.gpClient.Controls.Add(this.lblDossierClient);
            this.gpClient.Location = new System.Drawing.Point(42, 41);
            this.gpClient.Name = "gpClient";
            this.gpClient.Size = new System.Drawing.Size(427, 155);
            this.gpClient.TabIndex = 15;
            this.gpClient.TabStop = false;
            this.gpClient.Text = "Client";
            // 
            // btnRecevoirClient
            // 
            this.btnRecevoirClient.Location = new System.Drawing.Point(346, 117);
            this.btnRecevoirClient.Name = "btnRecevoirClient";
            this.btnRecevoirClient.Size = new System.Drawing.Size(75, 23);
            this.btnRecevoirClient.TabIndex = 11;
            this.btnRecevoirClient.Text = "Recevoir";
            this.btnRecevoirClient.UseVisualStyleBackColor = true;
            this.btnRecevoirClient.Click += new System.EventHandler(this.btnRecevoirClient_Click);
            // 
            // btnEnvoyerClient
            // 
            this.btnEnvoyerClient.Location = new System.Drawing.Point(6, 117);
            this.btnEnvoyerClient.Name = "btnEnvoyerClient";
            this.btnEnvoyerClient.Size = new System.Drawing.Size(75, 23);
            this.btnEnvoyerClient.TabIndex = 10;
            this.btnEnvoyerClient.Text = "Envoyer";
            this.btnEnvoyerClient.UseVisualStyleBackColor = true;
            this.btnEnvoyerClient.Click += new System.EventHandler(this.btnEnvoyerClient_Click);
            // 
            // txtFichierDistantClient
            // 
            this.txtFichierDistantClient.Location = new System.Drawing.Point(81, 91);
            this.txtFichierDistantClient.Name = "txtFichierDistantClient";
            this.txtFichierDistantClient.Size = new System.Drawing.Size(287, 20);
            this.txtFichierDistantClient.TabIndex = 15;
            // 
            // lblFichierDistantClient
            // 
            this.lblFichierDistantClient.AutoSize = true;
            this.lblFichierDistantClient.Location = new System.Drawing.Point(-3, 94);
            this.lblFichierDistantClient.Name = "lblFichierDistantClient";
            this.lblFichierDistantClient.Size = new System.Drawing.Size(81, 13);
            this.lblFichierDistantClient.TabIndex = 14;
            this.lblFichierDistantClient.Text = "Fichier distant =";
            // 
            // lblIPClient
            // 
            this.lblIPClient.AutoSize = true;
            this.lblIPClient.Location = new System.Drawing.Point(6, 16);
            this.lblIPClient.Name = "lblIPClient";
            this.lblIPClient.Size = new System.Drawing.Size(69, 13);
            this.lblIPClient.TabIndex = 6;
            this.lblIPClient.Text = "IP Serveur = ";
            // 
            // txtFichierLocalClient
            // 
            this.txtFichierLocalClient.Location = new System.Drawing.Point(81, 65);
            this.txtFichierLocalClient.Name = "txtFichierLocalClient";
            this.txtFichierLocalClient.Size = new System.Drawing.Size(287, 20);
            this.txtFichierLocalClient.TabIndex = 13;
            // 
            // txtIPClient
            // 
            this.txtIPClient.Location = new System.Drawing.Point(81, 13);
            this.txtIPClient.Name = "txtIPClient";
            this.txtIPClient.Size = new System.Drawing.Size(287, 20);
            this.txtIPClient.TabIndex = 7;
            // 
            // lblPortClient
            // 
            this.lblPortClient.AutoSize = true;
            this.lblPortClient.Location = new System.Drawing.Point(371, 16);
            this.lblPortClient.Name = "lblPortClient";
            this.lblPortClient.Size = new System.Drawing.Size(25, 13);
            this.lblPortClient.TabIndex = 8;
            this.lblPortClient.Text = ": 69";
            // 
            // lblFichierLocalClient
            // 
            this.lblFichierLocalClient.AutoSize = true;
            this.lblFichierLocalClient.Location = new System.Drawing.Point(3, 68);
            this.lblFichierLocalClient.Name = "lblFichierLocalClient";
            this.lblFichierLocalClient.Size = new System.Drawing.Size(72, 13);
            this.lblFichierLocalClient.TabIndex = 12;
            this.lblFichierLocalClient.Text = "Fichier local =";
            // 
            // txtDossierClient
            // 
            this.txtDossierClient.Location = new System.Drawing.Point(81, 39);
            this.txtDossierClient.Name = "txtDossierClient";
            this.txtDossierClient.Size = new System.Drawing.Size(287, 20);
            this.txtDossierClient.TabIndex = 10;
            // 
            // btnDossierClient
            // 
            this.btnDossierClient.BackgroundImage = global::TFTP_Client_Serveur.Properties.Resources.ico41;
            this.btnDossierClient.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btnDossierClient.Location = new System.Drawing.Point(371, 37);
            this.btnDossierClient.Name = "btnDossierClient";
            this.btnDossierClient.Size = new System.Drawing.Size(25, 22);
            this.btnDossierClient.TabIndex = 11;
            this.btnDossierClient.UseVisualStyleBackColor = true;
            this.btnDossierClient.Click += new System.EventHandler(this.btnDossierClient_Click);
            // 
            // lblDossierClient
            // 
            this.lblDossierClient.AutoSize = true;
            this.lblDossierClient.Location = new System.Drawing.Point(24, 42);
            this.lblDossierClient.Name = "lblDossierClient";
            this.lblDossierClient.Size = new System.Drawing.Size(51, 13);
            this.lblDossierClient.TabIndex = 9;
            this.lblDossierClient.Text = "Dossier =";
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.txtConsole);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.splitContainer1);
            this.splitContainer2.Size = new System.Drawing.Size(993, 450);
            this.splitContainer2.SplitterDistance = 210;
            this.splitContainer2.TabIndex = 5;
            // 
            // txtConsole
            // 
            this.txtConsole.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.txtConsole.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtConsole.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtConsole.ForeColor = System.Drawing.SystemColors.Window;
            this.txtConsole.Location = new System.Drawing.Point(0, 0);
            this.txtConsole.MaxLength = 65535;
            this.txtConsole.Multiline = true;
            this.txtConsole.Name = "txtConsole";
            this.txtConsole.ReadOnly = true;
            this.txtConsole.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtConsole.Size = new System.Drawing.Size(993, 210);
            this.txtConsole.TabIndex = 0;
            this.txtConsole.TabStop = false;
            // 
            // frmTFTP
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(993, 450);
            this.Controls.Add(this.splitContainer2);
            this.Name = "frmTFTP";
            this.Text = "Client-Serveur TFTP | Samuel Goulet";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmTFTP_FormClosing);
            this.gpServeur.ResumeLayout(false);
            this.gpServeur.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.gpClient.ResumeLayout(false);
            this.gpClient.PerformLayout();
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel1.PerformLayout();
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gpServeur;
        private System.Windows.Forms.Button btnArreterServeur;
        private System.Windows.Forms.Button btnDemarrerServeur;
        private System.Windows.Forms.Button btnDossierServeur;
        private System.Windows.Forms.TextBox txtDossierServeur;
        private System.Windows.Forms.Label lblDossierServeur;
        private System.Windows.Forms.Label lblPortServeur;
        private System.Windows.Forms.Label lblIPServeur;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.TextBox txtConsole;
        private System.Windows.Forms.Button btnDossierClient;
        private System.Windows.Forms.TextBox txtDossierClient;
        private System.Windows.Forms.Label lblDossierClient;
        private System.Windows.Forms.Label lblPortClient;
        private System.Windows.Forms.TextBox txtIPClient;
        private System.Windows.Forms.Label lblIPClient;
        private System.Windows.Forms.GroupBox gpClient;
        private System.Windows.Forms.TextBox txtFichierLocalClient;
        private System.Windows.Forms.Label lblFichierLocalClient;
        private System.Windows.Forms.TextBox txtFichierDistantClient;
        private System.Windows.Forms.Label lblFichierDistantClient;
        private System.Windows.Forms.Button btnRecevoirClient;
        private System.Windows.Forms.Button btnEnvoyerClient;
        private System.Windows.Forms.ComboBox cbIPServeur;
        private System.Windows.Forms.Button btnRafraichirServeur;
    }
}

