using InventarioActivos.Data;
using InventarioActivos.Models.GestionLocalizaciones;
using MySqlConnector;
using System.Data.Common;
namespace InventarioActivos.Services;

public class LocalizacionService 
{
	public async Task<List<ActivoLocalizacionItem>> ObtenerActivosConLocalizacionAsync()
	{
		List<ActivoLocalizacionItem> lista = new List<ActivoLocalizacionItem>();

		try
		{
			using (MySqlConnection connection = DbConnectionFactory.CreateConnection())
			{
				await connection.OpenAsync();
				//Consulta para obtener los activos que tengan un id de localizacion
				string sql =
					"SELECT a.ID_ACTIVO, a.NOMBRE_EQUIPO, " +
                    "l.ID_LOCALIZACION, l.NOMBRE, l.LATITUD, l.LONGITUD " +
					"FROM ACTIVOS a " +
					"INNER JOIN LOCALIZACION l ON a.ID_LOCALIZACION = l.ID_LOCALIZACION " +
					"ORDER BY a.NOMBRE_EQUIPO;";

				using (MySqlCommand command = new MySqlCommand(sql, connection))
				using (var reader = await command.ExecuteReaderAsync())
				{
					while (await reader.ReadAsync())
					{
						ActivoLocalizacionItem item = new ActivoLocalizacionItem();

						item.IdActivo = reader.GetInt32(0);

						if (reader.IsDBNull(1)) item.NombreEquipo = "";
						else item.NombreEquipo = reader.GetString(1);

						if (reader.IsDBNull(2)) item.IdLocalizacion = 0;
						else item.IdLocalizacion = reader.GetInt32(2);

						if (reader.IsDBNull(3)) item.NombreLocalizacion = "";
						else item.NombreLocalizacion = reader.GetString(3);

                        if (reader.IsDBNull(4)) item.Latitud = 0;
                        else item.Latitud = reader.GetDouble(4);

                        if (reader.IsDBNull(5)) item.Longitud = 0;
                        else item.Longitud = reader.GetDouble(5);

                        lista.Add(item);
					}
				}
			}
		}
		catch (Exception ex)
		{
			throw new Exception("Error al obtener activos con localizacion: " + ex.Message);
		}

		return lista;

	}

    public async Task<List<ActivoLocalizacionItem>> ObtenerActivosConYSinLocalizacionAsync()
    {
        List<ActivoLocalizacionItem> lista = new List<ActivoLocalizacionItem>();

        try
        {
            using (MySqlConnection connection = DbConnectionFactory.CreateConnection())
            {
                await connection.OpenAsync();

				//Consulta para obtener la lista de todos los activos con y sin id de localizacion
                string sql =
                    "SELECT a.ID_ACTIVO, a.NOMBRE_EQUIPO, " +
                    "l.ID_LOCALIZACION, l.NOMBRE, l.LATITUD, l.LONGITUD " +
                    "FROM ACTIVOS a " +
                    "LEFT JOIN LOCALIZACION l ON a.ID_LOCALIZACION = l.ID_LOCALIZACION " +
                    "ORDER BY a.NOMBRE_EQUIPO;";

                using (MySqlCommand command = new MySqlCommand(sql, connection))
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        ActivoLocalizacionItem item = new ActivoLocalizacionItem();

                        item.IdActivo = reader.GetInt32(0);

                        if (reader.IsDBNull(1)) item.NombreEquipo = "";
                        else item.NombreEquipo = reader.GetString(1);

                        if (reader.IsDBNull(2)) item.IdLocalizacion = 0;
                        else item.IdLocalizacion = reader.GetInt32(2);

                        if (reader.IsDBNull(3)) item.NombreLocalizacion = "";
                        else item.NombreLocalizacion = reader.GetString(3);

                        if (reader.IsDBNull(4)) item.Latitud = 0;
                        else item.Latitud = reader.GetDouble(4);

                        if (reader.IsDBNull(5)) item.Longitud = 0;
                        else item.Longitud = reader.GetDouble(5);

                        lista.Add(item);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            throw new Exception("Error al obtener activos con localizacion: " + ex.Message);
        }

        return lista;

    }


    public async Task<List<LocalizacionItem>> ObtenerLocalizacionesAsync()
	{
		List<LocalizacionItem> listaLocalizaciones = new List<LocalizacionItem>();

		try
		{
			using (MySqlConnection connection = DbConnectionFactory.CreateConnection())
			{
				await connection.OpenAsync();

				//Consulta para obtener una lista con todas las localizaciones
				string sql = "SELECT ID_LOCALIZACION, NOMBRE, LATITUD, LONGITUD FROM LOCALIZACION ORDER BY NOMBRE;";

				using (MySqlCommand command = new MySqlCommand(sql, connection))
				using (var reader = await command.ExecuteReaderAsync())
				{
					while (await reader.ReadAsync())
					{
						LocalizacionItem item = new LocalizacionItem();

						item.IdLocalizacion = reader.GetInt32(0);

						if (reader.IsDBNull(1)) item.Nombre = "";
						else item.Nombre = reader.GetString(1);

						if (reader.IsDBNull(2)) item.Latitud = 0;
						else item.Latitud = reader.GetDouble(2);

						if (reader.IsDBNull(3)) item.Longitud = 0;
						else item.Longitud = reader.GetDouble(3);

						listaLocalizaciones.Add(item);
					}
				}
			}
		}
		catch (Exception ex)
		{
			throw new Exception("Error al obtener localizaciones: " + ex.Message);
		}
		return listaLocalizaciones;
	}

	public async Task<LocalizacionItem> ObtenerLocalizacionPorIdAsync(int idLocalizacion)
	{
		LocalizacionItem item = null;

		try
		{
			using (MySqlConnection connection = DbConnectionFactory.CreateConnection())
			{
				await connection.OpenAsync();

				//Consulta para obtener una lista de localizaciones mediante el id
				string sql = "SELECT ID_LOCALIZACION, NOMBRE, LATITUD, LONGITUD FROM LOCALIZACION WHERE ID_LOCALIZACION = @Id;";

				using (MySqlCommand command = new MySqlCommand(sql, connection))
				{
					command.Parameters.AddWithValue("@Id", idLocalizacion);

					using (var reader = await command.ExecuteReaderAsync())
					{
						if (await reader.ReadAsync())
						{
							item = new LocalizacionItem();
							item.IdLocalizacion = reader.GetInt32(0);

							if (reader.IsDBNull(1)) item.Nombre = "";
							else item.Nombre = reader.GetString(1);

							if (reader.IsDBNull(2)) item.Latitud = 0;
							else item.Latitud = reader.GetDouble(2);

							if (reader.IsDBNull(3)) item.Longitud = 0;
							else item.Longitud = reader.GetDouble(3);

						}
					}
				}
			}
		}
		catch (Exception ex) 
		{
            throw new Exception("Error al obtener localizaciones: " + ex.Message);
        }

		return item;
	}

	public async Task<List<ActivoLocalizacionItem>> ObtenerActivosSinLocalizacionAsync()
	{
		List<ActivoLocalizacionItem> lista = new List<ActivoLocalizacionItem>();

		try
		{
			using (MySqlConnection connection = DbConnectionFactory.CreateConnection())
			{
				await connection.OpenAsync();

				//Consulta para obtener activos sin id de localizacion
				string sql = "SELECT ID_ACTIVO, NOMBRE_EQUIPO " +
					"FROM ACTIVOS " +
					"WHERE ID_LOCALIZACION IS NULL " +
					"ORDER BY NOMBRE_EQUIPO;";

				using (MySqlCommand command = new MySqlCommand(sql, connection))
				using (var reader = await command.ExecuteReaderAsync())
				{
					while (await reader.ReadAsync())
					{
						ActivoLocalizacionItem item = new ActivoLocalizacionItem();
						item.IdActivo = reader.GetInt32(0);

						if (reader.IsDBNull(1)) item.NombreEquipo = "";
						else item.NombreEquipo = reader.GetString(1);

						item.IdLocalizacion = 0;
						item.NombreLocalizacion = "";

						lista.Add(item);
					}
				}
			}
		}
		catch (Exception ex)
		{
			throw new Exception("Error al obtener activos sin localización: " + ex.Message);
		}
		return lista;
	}

	public async Task<int> CrearLocalizacionAsync(string nombre, double latitud, double longitud)
	{
		if (nombre == null) nombre = "";
		nombre = nombre.Trim();
		if(nombre.Length == 0) return 0;

		try
		{
			using (MySqlConnection connection = DbConnectionFactory.CreateConnection())
			{
				await connection.OpenAsync();

				//Consulta para crear una nueva localizacion
				string sql = "INSERT INTO LOCALIZACION (NOMBRE, LATITUD, LONGITUD) VALUES (@Nombre, @Lat, @Lon);";

				using (MySqlCommand command = new MySqlCommand(sql, connection))
				{
					command.Parameters.AddWithValue("@Nombre", nombre);
                    command.Parameters.AddWithValue("@Lat", latitud);
                    command.Parameters.AddWithValue("@Lon", longitud);

					await command.ExecuteNonQueryAsync();
					return (int)command.LastInsertedId;
                }
			}

		}
		catch (Exception ex)
		{

			throw new Exception("Error al crear localización: " + ex.Message);
		}
	}

	public async Task<bool> CrearLocalizacionYAsignarAsync(int idActivo, string nombre, double latitud, double longitud)
	{
		if(idActivo <= 0) return false;

		if (nombre == null) nombre = "";
		nombre = nombre.Trim();
		if (nombre.Length == 0) return false;

		try
		{
            using (MySqlConnection connection = DbConnectionFactory.CreateConnection())
			{
				await connection.OpenAsync();

				using (var reader = await connection.BeginTransactionAsync())
				{
					int idNuevaLoc = 0;

					//Consulta para crear una localizacion
					string sqlInsert = "INSERT INTO LOCALIZACION (NOMBRE, LATITUD, LONGITUD) " +
						"VALUES (@Nombre, @Lat, @Lon);";

					using (MySqlCommand command1 = new MySqlCommand(sqlInsert, connection))
					{
						command1.Transaction = reader;

						command1.Parameters.AddWithValue("@Nombre", nombre);
						command1.Parameters.AddWithValue("@Lat", latitud);
                        command1.Parameters.AddWithValue("@Lon", longitud);

						await command1.ExecuteNonQueryAsync();
						idNuevaLoc = (int)command1.LastInsertedId;
                    }

					if (idNuevaLoc <= 0)
					{
						await reader.RollbackAsync();
						return false;
					}

					//Consulta para actualizar el id de la localizacion en un activo
					string sqlUpdate = "UPDATE ACTIVOS SET ID_LOCALIZACION = @IdLoc WHERE ID_ACTIVO = @IdActivo;";

                    using (MySqlCommand command2 = new MySqlCommand(sqlUpdate, connection))
					{
						command2.Transaction = reader;

                        command2.Parameters.AddWithValue("@IdLoc", idNuevaLoc);
                        command2.Parameters.AddWithValue("@IdActivo", idActivo);

						//Si alguno de las consultas falla se hace un rollback para evitar fallos en las tablas
						int filas = await command2.ExecuteNonQueryAsync();
						if(filas <= 0)
						{
							await reader.RollbackAsync();
							return false;
						}
                    }

					await reader.CommitAsync();
					return true;

                }
			}

        }
		catch(Exception ex)
		{
			throw new Exception("Error al crear y asignar localización: " + ex.Message);
		}
	}

	public async Task<bool> ActualizarLocalizacionAsync(int idLocalizacion, string nombre, double latitud, double longitud)
	{
		if (idLocalizacion <= 0) return false;

		if (nombre == null) nombre = "";
		nombre = nombre.Trim();
		if (nombre.Length == 0) return false;

		try
		{
			using (MySqlConnection connection = DbConnectionFactory.CreateConnection())
			{
				await connection.OpenAsync();

				string sql = "UPDATE LOCALIZACION SET NOMBRE = @Nombre, LATITUD = @Lat, LONGITUD = @Lon " +
					"WHERE ID_LOCALIZACION = @Id;";

				using (MySqlCommand command = new MySqlCommand(sql, connection))
				{
					command.Parameters.AddWithValue("@Id", idLocalizacion);
					command.Parameters.AddWithValue("@Nombre", nombre);
					command.Parameters.AddWithValue("@Lat", latitud);
					command.Parameters.AddWithValue("@Lon", longitud);

					int filas = await command.ExecuteNonQueryAsync();
					return filas > 0;
				}
			}
		}
		catch (Exception ex)
		{
			throw new Exception("Error al actualizar localización: " + ex.Message);
		}
	}

	public async Task<bool> AsignarLocalizacionActivoAsync(int idActivo, int idLocalizacion)
	{
		if(idActivo <= 0) return false;
		if (idLocalizacion <= 0) return false;

		try
		{
			using (MySqlConnection connection = DbConnectionFactory.CreateConnection())
			{
				await connection.OpenAsync();

				string sql = "UPDATE ACTIVOS SET ID_LOCALIZACION = @IdLoc WHERE ID_ACTIVO = @IdActivo;";

				using (MySqlCommand command = new MySqlCommand(sql, connection))
				{
					command.Parameters.AddWithValue("@IdActivo", idActivo);
					command.Parameters.AddWithValue("@IdLoc", idLocalizacion);

					int filas = await command.ExecuteNonQueryAsync();
					return filas > 0;
				}
			}
		}
		catch (Exception ex) 
		{
			throw new Exception("Error al asignar localización al activo: " + ex.Message);
		}
	}

	//Metodo para quitar la localización de un activo. Si esa localizacion ya no es usado por otro activo se elimina de la base de datos
	public async Task<bool> QuitarYBorrarLocalizacionDeActivoAsync(int idActivo)
	{
        if (idActivo <= 0) return false;

        try
        {
            using (MySqlConnection connection = DbConnectionFactory.CreateConnection())
            {
                await connection.OpenAsync();

                using (var tx = await connection.BeginTransactionAsync())
                {
                    int idLoc = 0;
					
                    string sqlGet = "SELECT ID_LOCALIZACION FROM ACTIVOS WHERE ID_ACTIVO = @IdActivo;";
                    using (MySqlCommand cmdGet = new MySqlCommand(sqlGet, connection))
                    {
                        cmdGet.Transaction = tx;
                        cmdGet.Parameters.AddWithValue("@IdActivo", idActivo);

                        object valor = await cmdGet.ExecuteScalarAsync();
                        if (valor == null || valor == DBNull.Value)
                        {
                            await tx.RollbackAsync();
                            return false;
                        }
                        idLoc = Convert.ToInt32(valor);
                        if (idLoc <= 0)
                        {
                            await tx.RollbackAsync();
                            return false;
                        }
                    }

                    string sqlUpd = "UPDATE ACTIVOS SET ID_LOCALIZACION = NULL WHERE ID_ACTIVO = @IdActivo;";
                    using (MySqlCommand cmdUpd = new MySqlCommand(sqlUpd, connection))
                    {
                        cmdUpd.Transaction = tx;
                        cmdUpd.Parameters.AddWithValue("@IdActivo", idActivo);
                        int filas = await cmdUpd.ExecuteNonQueryAsync();
                        if (filas <= 0)
                        {
                            await tx.RollbackAsync();
                            return false;
                        }
                    }

                    string sqlCount = "SELECT COUNT(*) FROM ACTIVOS WHERE ID_LOCALIZACION = @IdLoc;";
                    int usados = 0;
                    using (MySqlCommand cmdCount = new MySqlCommand(sqlCount, connection))
                    {
                        cmdCount.Transaction = tx;
                        cmdCount.Parameters.AddWithValue("@IdLoc", idLoc);
                        object c = await cmdCount.ExecuteScalarAsync();
                        usados = Convert.ToInt32(c);
                    }

                    if (usados == 0)
                    {
                        string sqlDel = "DELETE FROM LOCALIZACION WHERE ID_LOCALIZACION = @IdLoc;";
                        using (MySqlCommand cmdDel = new MySqlCommand(sqlDel, connection))
                        {
                            cmdDel.Transaction = tx;
                            cmdDel.Parameters.AddWithValue("@IdLoc", idLoc);
                            await cmdDel.ExecuteNonQueryAsync();
                        }
                    }

                    await tx.CommitAsync();
                    return true;
                }
            }
        }
        catch (Exception ex)
        {
            throw new Exception("Error al quitar y borrar localización: " + ex.Message);
        }
    }
}