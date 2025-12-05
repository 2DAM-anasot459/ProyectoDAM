<<<<<<< HEAD

namespace Agente
{
    public partial class Formulario : Form
    {
        // Cadena de conexión a la base de datos MySQL
        private string cadenaConexion = "Server=qaon980.islamagica.info;Port=3306;Database=qaon980;User ID=qaon980;Password=4n4.S0t0TFg;SslMode=Required;";                                             
        public Formulario()
        {
            InitializeComponent();
        }

        private void Formulario_Load(object sender, EventArgs e)
        {
            // Obtener y mostrar el nombre del equipo               
            string nombreEquipo = Environment.MachineName;
            labelNombreEquipo.Text = nombreEquipo;

            // Estado inicial de la conexión a la base de datos
            labelEstadoBD.Text = "Sin comprobar";
            labelEstadoBD.ForeColor = System.Drawing.Color.Black;

            // Limpiar el área de mensajes
            textBoxMensajes.Text = "";
        }

        //Botón para probar la conexión a la base de datos
        private void buttonProbarConexion_Click(object sender, EventArgs e)
        {
            textBoxMensajes.Text = "";
            EscribirMensaje("Probando conexión con la base de datos...");

            try
            {
                Conexion conexion = new Conexion(cadenaConexion);
                conexion.ProbarConexion();
                labelEstadoBD.Text = "Conectado";
                labelEstadoBD.ForeColor = System.Drawing.Color.DarkGreen;
                EscribirMensaje("Conexión correcta con MySQL.");
            }
            catch (Exception ex)
            {
                labelEstadoBD.Text = "Error";
                labelEstadoBD.ForeColor = System.Drawing.Color.Red;
                EscribirMensaje("Error al conectar: " + ex.Message);
            }
        }

        //Botón para escanear el equipo y guardar los datos en la base de datos
        private void buttonEscanear_Click(object sender, EventArgs e)
        {
            textBoxMensajes.Text = "";
            EscribirMensaje("Iniciando escaneo del equipo...");

            try
            {
                DatosHardware datosHardware = Recolector.recolectorHardware();
                DatosSoftware datosSoftware = Recolector.recolectorSoftware();
                List<DatosProgramas> listaProgramas = Recolector.recolectorProgramas();


                EscribirMensaje("Escaneo completado. Guardando datos en la base de datos...");
                Conexion conexion = new Conexion(cadenaConexion);

                string nombreEquipo = labelNombreEquipo.Text;
                int idActivo = conexion.ObtenerIdActivoPorNombre(nombreEquipo);

                EscribirMensaje("Guardando datos de hardware...");
                conexion.InsertarDatosHardware(idActivo, datosHardware);
                EscribirMensaje("Guardando datos de software y programas instalados...");
                conexion.InsertarDatosSoftware(idActivo, datosSoftware, listaProgramas);
                conexion.ActualizarDatosActivo(idActivo, datosSoftware);

                EscribirMensaje("Proceso completado correctamente.");
                MessageBox.Show("Escaneo y guardado completados.",
                    "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                EscribirMensaje("ERROR: " + ex.Message);
                MessageBox.Show("Se ha producido un error durante el proceso.\n\n" + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void EscribirMensaje(string texto)
        {
            if (textBoxMensajes.Text == "")
                textBoxMensajes.Text = texto;
            else
                textBoxMensajes.Text = textBoxMensajes.Text + Environment.NewLine + texto;
        }
    }
       
}

=======

namespace Agente
{
    public partial class Formulario : Form
    {
        // Cadena de conexión a la base de datos MySQL
        private string cadenaConexion = "Server=qaon980.islamagica.info;Port=3306;Database=qaon980;User ID=qaon980;Password=4n4.S0t0TFg;SslMode=Required;";                                             
        public Formulario()
        {
            InitializeComponent();
        }

        private void Formulario_Load(object sender, EventArgs e)
        {
            // Obtener y mostrar el nombre del equipo               
            string nombreEquipo = Environment.MachineName;
            labelNombreEquipo.Text = nombreEquipo;

            // Estado inicial de la conexión a la base de datos
            labelEstadoBD.Text = "Sin comprobar";
            labelEstadoBD.ForeColor = System.Drawing.Color.Black;

            // Limpiar el área de mensajes
            textBoxMensajes.Text = "";
        }

        //Botón para probar la conexión a la base de datos
        private void buttonProbarConexion_Click(object sender, EventArgs e)
        {
            textBoxMensajes.Text = "";
            EscribirMensaje("Probando conexión con la base de datos...");

            try
            {
                Conexion conexion = new Conexion(cadenaConexion);
                conexion.ProbarConexion();
                labelEstadoBD.Text = "Conectado";
                labelEstadoBD.ForeColor = System.Drawing.Color.DarkGreen;
                EscribirMensaje("Conexión correcta con MySQL.");
            }
            catch (Exception ex)
            {
                labelEstadoBD.Text = "Error";
                labelEstadoBD.ForeColor = System.Drawing.Color.Red;
                EscribirMensaje("Error al conectar: " + ex.Message);
            }
        }

        //Botón para escanear el equipo y guardar los datos en la base de datos
        private void buttonEscanear_Click(object sender, EventArgs e)
        {
            textBoxMensajes.Text = "";
            EscribirMensaje("Iniciando escaneo del equipo...");

            try
            {
                DatosHardware datosHardware = Recolector.recolectorHardware();
                DatosSoftware datosSoftware = Recolector.recolectorSoftware();
                List<DatosProgramas> listaProgramas = Recolector.recolectorProgramas();


                EscribirMensaje("Escaneo completado. Guardando datos en la base de datos...");
                Conexion conexion = new Conexion(cadenaConexion);

                string nombreEquipo = labelNombreEquipo.Text;
                int idActivo = conexion.ObtenerIdActivoPorNombre(nombreEquipo);

                EscribirMensaje("Guardando datos de hardware...");
                conexion.InsertarDatosHardware(idActivo, datosHardware);
                EscribirMensaje("Guardando datos de software y programas instalados...");
                conexion.InsertarDatosSoftware(idActivo, datosSoftware, listaProgramas);
                conexion.ActualizarDatosActivo(idActivo, datosSoftware);

                EscribirMensaje("Proceso completado correctamente.");
                MessageBox.Show("Escaneo y guardado completados.",
                    "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                EscribirMensaje("ERROR: " + ex.Message);
                MessageBox.Show("Se ha producido un error durante el proceso.\n\n" + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void EscribirMensaje(string texto)
        {
            if (textBoxMensajes.Text == "")
                textBoxMensajes.Text = texto;
            else
                textBoxMensajes.Text = textBoxMensajes.Text + Environment.NewLine + texto;
        }
    }
       
}

>>>>>>> 18d7fc4ac011a640e8e17d9c713255b8c92b9a23
