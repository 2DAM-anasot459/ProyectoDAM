namespace Agente
{
    partial class Formulario
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.labelTitulo = new System.Windows.Forms.Label();
            this.labelNombreEquipoTitulo = new System.Windows.Forms.Label();
            this.labelNombreEquipo = new System.Windows.Forms.Label();
            this.labelEstadoBDTitulo = new System.Windows.Forms.Label();
            this.labelEstadoBD = new System.Windows.Forms.Label();
            this.buttonProbarConexion = new System.Windows.Forms.Button();
            this.buttonEscanear = new System.Windows.Forms.Button();
            this.textBoxMensajes = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // labelTitulo
            // 
            this.labelTitulo.AutoSize = true;
            this.labelTitulo.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.labelTitulo.Location = new System.Drawing.Point(200, 10);
            this.labelTitulo.Name = "labelTitulo";
            this.labelTitulo.Size = new System.Drawing.Size(216, 15);
            this.labelTitulo.TabIndex = 0;
            this.labelTitulo.Text = "AGENTE  DE INVENTARIO DE EQUIPOS";
            // 
            // labelNombreEquipoTitulo
            // 
            this.labelNombreEquipoTitulo.AutoSize = true;
            this.labelNombreEquipoTitulo.Location = new System.Drawing.Point(40, 50);
            this.labelNombreEquipoTitulo.Name = "labelNombreEquipoTitulo";
            this.labelNombreEquipoTitulo.Size = new System.Drawing.Size(95, 13);
            this.labelNombreEquipoTitulo.TabIndex = 1;
            this.labelNombreEquipoTitulo.Text = "Nombre del equipo:";
            // 
            // labelNombreEquipo
            // 
            this.labelNombreEquipo.AutoSize = true;
            this.labelNombreEquipo.Location = new System.Drawing.Point(160, 50);
            this.labelNombreEquipo.Name = "labelNombreEquipo";
            this.labelNombreEquipo.Size = new System.Drawing.Size(10, 13);
            this.labelNombreEquipo.TabIndex = 2;
            this.labelNombreEquipo.Text = "|";
            // 
            // labelEstadoBDTitulo
            // 
            this.labelEstadoBDTitulo.AutoSize = true;
            this.labelEstadoBDTitulo.Location = new System.Drawing.Point(40, 80);
            this.labelEstadoBDTitulo.Name = "labelEstadoBDTitulo";
            this.labelEstadoBDTitulo.Size = new System.Drawing.Size(60, 13);
            this.labelEstadoBDTitulo.TabIndex = 3;
            this.labelEstadoBDTitulo.Text = "Estado BD:";
            // 
            // labelEstadoBD
            // 
            this.labelEstadoBD.AutoSize = true;
            this.labelEstadoBD.Location = new System.Drawing.Point(160, 80);
            this.labelEstadoBD.Name = "labelEstadoBD";
            this.labelEstadoBD.Size = new System.Drawing.Size(10, 13);
            this.labelEstadoBD.TabIndex = 4;
            this.labelEstadoBD.Text = "|";
            // 
            // buttonProbarConexion
            // 
            this.buttonProbarConexion.Location = new System.Drawing.Point(40, 110);
            this.buttonProbarConexion.Name = "buttonProbarConexion";
            this.buttonProbarConexion.Size = new System.Drawing.Size(130, 35);
            this.buttonProbarConexion.TabIndex = 5;
            this.buttonProbarConexion.Text = "Probar Conexión";
            this.buttonProbarConexion.UseVisualStyleBackColor = true;
            this.buttonProbarConexion.Click += new System.EventHandler(this.buttonProbarConexion_Click);
            // 
            // buttonEscanear
            // 
            this.buttonEscanear.Location = new System.Drawing.Point(40, 160);
            this.buttonEscanear.Name = "buttonEscanear";
            this.buttonEscanear.Size = new System.Drawing.Size(200, 45);
            this.buttonEscanear.TabIndex = 6;
            this.buttonEscanear.Text = "Escanear Equipo";
            this.buttonEscanear.UseVisualStyleBackColor = true;
            this.buttonEscanear.Click += new System.EventHandler(this.buttonEscanear_Click);
            // 
            // textBoxMensajes
            // 
            this.textBoxMensajes.Location = new System.Drawing.Point(40, 230);
            this.textBoxMensajes.Multiline = true;
            this.textBoxMensajes.Name = "textBoxMensajes";
            this.textBoxMensajes.ReadOnly = true;
            this.textBoxMensajes.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxMensajes.Size = new System.Drawing.Size(520, 130);
            this.textBoxMensajes.TabIndex = 7;
            // 
            // Formulario
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(600, 380);
            this.Controls.Add(this.textBoxMensajes);
            this.Controls.Add(this.buttonEscanear);
            this.Controls.Add(this.buttonProbarConexion);
            this.Controls.Add(this.labelEstadoBD);
            this.Controls.Add(this.labelEstadoBDTitulo);
            this.Controls.Add(this.labelNombreEquipo);
            this.Controls.Add(this.labelNombreEquipoTitulo);
            this.Controls.Add(this.labelTitulo);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "Formulario";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Agente de inventario de equipos";
            this.Load += new System.EventHandler(this.Formulario_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelTitulo;
        private System.Windows.Forms.Label labelNombreEquipoTitulo;
        private System.Windows.Forms.Label labelNombreEquipo;
        private System.Windows.Forms.Label labelEstadoBDTitulo;
        private System.Windows.Forms.Label labelEstadoBD;
        private System.Windows.Forms.Button buttonProbarConexion;
        private System.Windows.Forms.Button buttonEscanear;
        private System.Windows.Forms.TextBox textBoxMensajes;
    }
}
