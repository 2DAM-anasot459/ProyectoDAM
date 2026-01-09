using InventarioActivos.Data;
using InventarioActivos.Models.GestionIncidencias;
using MySqlConnector;

namespace InventarioActivos.Services;

public class IncidenciaService
{
    public async Task<int> CrearIncidenciaAsync(string titulo, string descripcion, DateTime fechaCreacion, DateTime fechaFinalizacion ,int idActivo, int idEstado, int idUsuarioAsignado)
    {
        try
        {
            using (MySqlConnection connection = DbConnectionFactory.CreateConnection())
            {
                await connection.OpenAsync();
                string sql =
                    //Consulta para crear una nueva incidencia 
                    "INSERT INTO INCIDENCIA (TITULO, DESCRIPCION, FECHA_CREACION, FECHA_FINALIZACION, ID_ACTIVO, ID_ESTADO, ID_USUARIO_ASIGNADO) " +
                    "VALUES (@Titulo, @Descripcion, @FechaCreacion, @FechaFinalizacion, @IdActivo, @IdEstado, @IdUsuario); " +
                    "SELECT LAST_INSERT_ID();";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Titulo", titulo);
                    command.Parameters.AddWithValue("@Descripcion", descripcion);
                    command.Parameters.AddWithValue("@FechaCreacion", fechaCreacion);
                    if (fechaFinalizacion == DateTime.MinValue)
                    {
                        command.Parameters.Add("@FechaFinalizacion", MySqlConnector.MySqlDbType.DateTime).Value = DBNull.Value;
                    }
                    else
                    {
                        command.Parameters.Add("@FechaFinalizacion", MySqlConnector.MySqlDbType.DateTime).Value = fechaFinalizacion;
                    }
                    command.Parameters.AddWithValue("@IdActivo", idActivo);
                    command.Parameters.AddWithValue("@IdEstado", idEstado);
                    command.Parameters.AddWithValue("@IdUsuario", idUsuarioAsignado);
                    object result = await command.ExecuteScalarAsync();
                    return Convert.ToInt32(result);
                }
            }
        }
        catch (Exception ex)
        {
            throw new Exception("Error al crear la incidencia: " + ex.Message);
        }
    }

    public async Task<int> ObtenerIdEstadoPendienteAsync()
    {

        try
        {
            using (MySqlConnection connection = DbConnectionFactory.CreateConnection())
            {
                await connection.OpenAsync();
                //Consulta para obtener el estado pendiente
                string sql = "SELECT ID_ESTADO FROM ESTADO WHERE TIPO = 'Pendiente' LIMIT 1;";

                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    object obj = await command.ExecuteScalarAsync();
                    if (obj == null) return 0;

                    return Convert.ToInt32(obj);
                }
            }
        }catch (Exception ex)
        {
            throw new Exception("Error al obtener el id del estado: " + ex.Message);
        }
    }

    public async Task<List<EstadoIncidencias>> ObtenerIncidenciaAsync()
	{
		var lista = new List<EstadoIncidencias>();
        try
        {
            using (MySqlConnection connection = DbConnectionFactory.CreateConnection())
            {
                await connection.OpenAsync();

                string sql =
                    //Consulta para obtener una lista de incidencias
                    "SELECT i.NUMERO_INCIDENCIA, i.TITULO, i.DESCRIPCION, i.FECHA_CREACION, " +
                    "e.TIPO AS ESTADO_TIPO, a.NOMBRE_EQUIPO AS ACTIVO_NOMBRE, " +
                    "u.NOMBRE_USUARIO AS USUARIO_ASIGNADO " +
                    "FROM INCIDENCIA i " +
                    "INNER JOIN ESTADO e ON i.ID_ESTADO = e.ID_ESTADO " +
                    "INNER JOIN ACTIVOS a ON i.ID_ACTIVO = a.ID_ACTIVO " +
                    "LEFT JOIN USUARIO u ON i.ID_USUARIO_ASIGNADO = u.ID_USUARIO " +
                    "ORDER BY i.FECHA_CREACION DESC;";

                using (MySqlCommand command = new MySqlCommand(sql, connection))
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        EstadoIncidencias estadoIncidencias = new EstadoIncidencias();

                        estadoIncidencias.IdIncidencia = reader.GetInt32(0);
                        if (reader.IsDBNull(1))
                        {
                            estadoIncidencias.Titulo = "";
                        }
                        else
                        {
                            estadoIncidencias.Titulo = reader.GetString(1);
                        }

                        if (reader.IsDBNull(2))
                        {
                            estadoIncidencias.Descripcion = "";
                        }
                        else
                        {
                            estadoIncidencias.Descripcion = reader.GetString(2);
                        }

                        if (!reader.IsDBNull(3))
                        {
                            estadoIncidencias.FechaCreacion = reader.GetDateTime(3);

                        }
                        else
                        {
                            estadoIncidencias.FechaCreacion = DateTime.Today;
                        }

                        string estadoTexto;
                        if (reader.IsDBNull(4))
                        {
                            estadoTexto = "";
                        }
                        else
                        {
                            estadoTexto = reader.GetString(4);
                        }

                        estadoIncidencias.Estado = ConvertirEstado(estadoTexto);

                        if (reader.IsDBNull(5))
                        {
                            estadoIncidencias.ActivoNombre = "";
                        }
                        else
                        {
                            estadoIncidencias.ActivoNombre = reader.GetString(5);
                        }

                        if (reader.IsDBNull(6))
                        {
                            estadoIncidencias.UsuarioAsignado = "";
                        }
                        else
                        {
                            estadoIncidencias.UsuarioAsignado = reader.GetString(6);
                        }

                        lista.Add(estadoIncidencias);
                    }
                }

            }
        }
        catch (Exception ex)
        {
            throw new Exception("Error al obtener las incidencias: " + ex.Message);
        }
		return lista;
    }

    public async Task<List<EstadoIncidencias>> ObtenerIncidenciasAsignadasAsync(int idTecnico)
    {
        var lista = new List<EstadoIncidencias>();
        try
        {
            using (MySqlConnection connection = DbConnectionFactory.CreateConnection())
            {
                await connection.OpenAsync();

                string sql =
                    //Consulta para obtener las incidencais que están asignadas al usuario logeado
                    "SELECT i.NUMERO_INCIDENCIA, i.TITULO, i.DESCRIPCION, " +
                    "i.FECHA_CREACION, i.FECHA_FINALIZACION, e.TIPO, a.NOMBRE_EQUIPO " +
                    "FROM INCIDENCIA i " +
                    "INNER JOIN ACTIVOS a ON i.ID_ACTIVO = a.ID_ACTIVO " +
                    "INNER JOIN ESTADO e ON i.ID_ESTADO = e.ID_ESTADO " +
                    "WHERE i.ID_USUARIO_ASIGNADO = @IdTecnico " +
                    "ORDER BY i.FECHA_CREACION DESC;";

                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@IdTecnico", idTecnico);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            EstadoIncidencias estadoIncidencias = new EstadoIncidencias();

                            estadoIncidencias.IdIncidencia = reader.GetInt32(0);
                            if (reader.IsDBNull(1))
                            {
                                estadoIncidencias.Titulo = "";
                            }
                            else
                            {
                                estadoIncidencias.Titulo = reader.GetString(1);
                            }

                            if (reader.IsDBNull(2))
                            {
                                estadoIncidencias.Descripcion = "";
                            }
                            else
                            {
                                estadoIncidencias.Descripcion = reader.GetString(2);
                            }

                            if (!reader.IsDBNull(3))
                            {
                                estadoIncidencias.FechaCreacion = reader.GetDateTime(3);

                            }
                            else
                            {
                                estadoIncidencias.FechaCreacion = DateTime.Today;
                            }

                            if (!reader.IsDBNull(4))
                            {
                                estadoIncidencias.FechaFinalizacion = reader.GetDateTime(4);
                            }
                            else
                            {
                                estadoIncidencias.FechaFinalizacion = DateTime.MinValue;
                            }

                            string estadoTexto;
                            if (reader.IsDBNull(5))
                            {
                                estadoTexto = "";
                            }
                            else
                            {
                                estadoTexto = reader.GetString(5);
                            }

                            estadoIncidencias.Estado = ConvertirEstado(estadoTexto);

                            if (reader.IsDBNull(6))
                            {
                                estadoIncidencias.ActivoNombre = "";
                            }
                            else
                            {
                                estadoIncidencias.ActivoNombre = reader.GetString(6);
                            }

                            lista.Add(estadoIncidencias);
                        }
                    }
                }                    
                

            }
        }
        catch (Exception ex)
        {
            throw new Exception("Error al obtener las incidencias del técnico: " + ex.Message);
        }
        return lista;
    }   

    //Permite cambiar el estado de la incidencia segun el seleccionado
	private EstadoIncidencias.EstadoIncidencia ConvertirEstado(string estadoTexto)
	{
		if(estadoTexto == "Terminada")
		{
			return EstadoIncidencias.EstadoIncidencia.Terminada;
		}
		else if (estadoTexto == "En Progreso")
		{
			return EstadoIncidencias.EstadoIncidencia.EnProgreso;
		}
		else
		{
			return EstadoIncidencias.EstadoIncidencia.Pendiente;
        }
    }

    public async Task<bool> EliminarIncidenciaAsync(int idIncidencia)
    {
        try
        {
            using (MySqlConnection connection = DbConnectionFactory.CreateConnection())
            {
                await connection.OpenAsync();
                //Consulta para borrar una incidencia
                string sql = "DELETE FROM INCIDENCIA WHERE NUMERO_INCIDENCIA = @IdIncidencia;";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@IdIncidencia", idIncidencia);
                    int filas = await command.ExecuteNonQueryAsync();
                    return filas > 0;
                }
            }
        }
        catch (Exception ex)
        {
            throw new Exception("Error al eliminar la incidencia: " + ex.Message);
        }
    }

    public async Task<EstadoIncidencias> ObtenerIncidenciaPorIdAsync(int idIncidencia)
    {
        EstadoIncidencias incidencia = null;
        try
        {
            using (MySqlConnection connection = DbConnectionFactory.CreateConnection())
            {
                await connection.OpenAsync();
                string sql =
                    "SELECT i.NUMERO_INCIDENCIA, i.TITULO, i.DESCRIPCION, i.FECHA_CREACION, i.FECHA_FINALIZACION, " +
                    "i.ID_ACTIVO, i.ID_ESTADO, i.ID_USUARIO_ASIGNADO " +
                    "FROM INCIDENCIA i " +                   
                    "WHERE i.NUMERO_INCIDENCIA = @Id;";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Id", idIncidencia);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            incidencia = new EstadoIncidencias();

                            incidencia.IdIncidencia = reader.GetInt32(0);

                            if (reader.IsDBNull(1))
                            {
                                incidencia.Titulo = "";
                            }
                            else
                            {
                                incidencia.Titulo = reader.GetString(1);
                            }

                            if (reader.IsDBNull(2))
                            {
                                incidencia.Descripcion = "";
                            }
                            else
                            {
                                incidencia.Descripcion = reader.GetString(2);
                            }

                            if (reader.IsDBNull(3))
                            {
                                incidencia.FechaCreacion = DateTime.Today;
                            }
                            else
                            {
                                incidencia.FechaCreacion = reader.GetDateTime(3);
                            }

                            if (!reader.IsDBNull(4))
                            {
                                incidencia.FechaFinalizacion = reader.GetDateTime(4);
                            }
                            else
                            {
                                incidencia.FechaFinalizacion = DateTime.MinValue;
                            }

                            if (!reader.IsDBNull(7))
                            {
                                incidencia.IdeUsuarioAsignado = reader.GetInt32(7);
                            }
                            else
                            {
                                incidencia.IdeUsuarioAsignado = 0;
                            }
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            throw new Exception("Error al obtener la incidencia por ID: " + ex.Message);
        }
        return incidencia;
    }
    public async Task<List<ActivoItem>> ObtenerActivosAsync()
    {
        List<ActivoItem> listaActivo = new List<ActivoItem>();
        try
        {
            using (MySqlConnection connection = DbConnectionFactory.CreateConnection())
            {
                await connection.OpenAsync();
                string sql =
                    "SELECT ID_ACTIVO, NOMBRE_EQUIPO " +
                    "FROM ACTIVOS " +
                    "ORDER BY NOMBRE_EQUIPO;";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        ActivoItem activoItem = new ActivoItem();
                        activoItem.IdActivo = reader.GetInt32(0);
                        if (reader.IsDBNull(1))
                        {
                            activoItem.NombreActivo = "";
                        }
                        else
                        {
                            activoItem.NombreActivo = reader.GetString(1);
                        }
                        listaActivo.Add(activoItem);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            throw new Exception("Error al obtener activos: " + ex.Message);
        }
        return listaActivo;
    }
    public async Task<List<EstadoItem>> ObtenerEstadosAsync()
    {
        List<EstadoItem> listaEstado = new List<EstadoItem>();
        try
        {
            using (MySqlConnection connection = DbConnectionFactory.CreateConnection())
            {
                await connection.OpenAsync();
                string sql =
                    "SELECT ID_ESTADO, TIPO " +
                    "FROM ESTADO " +
                    "ORDER BY ID_ESTADO;";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        EstadoItem estadoItem = new EstadoItem();
                        estadoItem.IdEstado = reader.GetInt32(0);
                        if (reader.IsDBNull(1))
                        {
                            estadoItem.Tipo = "";
                        }
                        else
                        {
                            estadoItem.Tipo = reader.GetString(1);
                        }
                       
                        listaEstado.Add(estadoItem);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            throw new Exception("Error al obtener estados: " + ex.Message);
        }
        return listaEstado;
    }

    public async Task<List<UsuarioItem>> ObtenerUsuariosAsync()
    {
        List<UsuarioItem> listaUsuario = new List<UsuarioItem>();
        try
        {
            using (MySqlConnection connection = DbConnectionFactory.CreateConnection())
            {
                await connection.OpenAsync();
                string sql =
                    "SELECT ID_USUARIO, NOMBRE_USUARIO " +
                    "FROM USUARIO " +
                    "ORDER BY NOMBRE_USUARIO;";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        UsuarioItem usuarioItem = new UsuarioItem();
                        usuarioItem.IdUsuario = reader.GetInt32(0);
                        if (reader.IsDBNull(1))
                        {
                            usuarioItem.NombreUsuario = "";
                        }
                        else
                        {
                            usuarioItem.NombreUsuario = reader.GetString(1);
                        }
                        listaUsuario.Add(usuarioItem);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            throw new Exception("Error al obtener usuarios: " + ex.Message);
        }
        return listaUsuario;
    }

    public async Task<bool> ActualizarIncidenciaAsync(int idIncidencia, string titulo, string descripcion, int idActivo, int idEstado, int idUsuarioAsignado, DateTime fechaCreacion, DateTime fechaFinalizacion)
    {
        try
        {
            using (MySqlConnection connection = DbConnectionFactory.CreateConnection())
            {
                await connection.OpenAsync();
                string sql =
                    "UPDATE INCIDENCIA " +
                    "SET TITULO = @Titulo, DESCRIPCION = @Descripcion, FECHA_CREACION = @FechaCreacion, FECHA_FINALIZACION = @FechaFinalizacion, ID_ACTIVO = @IdActivo, ID_ESTADO = @IdEstado, ID_USUARIO_ASIGNADO = @IdUsuario " +
                    "WHERE NUMERO_INCIDENCIA = @Id;";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Titulo", titulo);
                    command.Parameters.AddWithValue("@Descripcion", descripcion);
                    command.Parameters.AddWithValue("@FechaCreacion", fechaCreacion);
                    if (fechaFinalizacion == DateTime.MinValue)
                    {
                        command.Parameters.Add("@FechaFinalizacion", MySqlConnector.MySqlDbType.DateTime).Value = DBNull.Value;
                    }
                    else
                    {
                        command.Parameters.Add("@FechaFinalizacion", MySqlConnector.MySqlDbType.DateTime).Value = fechaFinalizacion;
                    }
                    command.Parameters.AddWithValue("@IdActivo", idActivo);
                    command.Parameters.AddWithValue("@IdEstado", idEstado);   
                    command.Parameters.AddWithValue("@IdUsuario", idUsuarioAsignado);
                    command.Parameters.AddWithValue("@Id", idIncidencia);
                    int filas = await command.ExecuteNonQueryAsync();
                    return filas > 0;
                }
            }
        }
        catch (Exception ex)
        {
            throw new Exception("Error al actualizar la incidencia: " + ex.Message);
        }

    }

   
} 