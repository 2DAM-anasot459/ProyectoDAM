namespace InventarioActivos.Models.GestionIncidencias;

public class EstadoIncidencias
{
	public enum EstadoIncidencia
	{
		Terminada,
		EnProgreso,
        Pendiente
    }


		public int IdIncidencia { get; set; }
		public string Titulo { get; set; }
		public string Descripcion { get; set; }
		public string UsuarioAsignado { get; set; }
		public EstadoIncidencia Estado { get; set; }
		public string ActivoNombre { get; set; }
		public DateTime FechaCreacion { get; set; }
		public DateTime FechaFinalizacion { get; set; }
		public int IdeUsuarioAsignado { get; set; }

    public EstadoIncidencias()
		{
			Titulo = "";
			Descripcion = "";
			UsuarioAsignado = "";
			ActivoNombre = "";
			Estado = EstadoIncidencia.Pendiente;
			FechaCreacion = DateTime.Today;
			FechaFinalizacion = DateTime.MinValue;
			IdeUsuarioAsignado = 0;

		}
	public Color EstadoColor
	{
		get
		{
			if (Estado == EstadoIncidencia.Terminada)
			{
				return Colors.Green;
			}
			if (Estado == EstadoIncidencia.EnProgreso)
			{
				return Colors.Orange;
            }
			return Colors.Red;
        }
	}

    
}