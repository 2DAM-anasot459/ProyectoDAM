using InventarioActivos.Data;
using InventarioActivos.Models.GestionUsuarios;
using MySqlConnector;
namespace InventarioActivos.Services;

public class UsuarioService
{
	
	public async Task<List<UsuarioListadoItem>> ObtenerUsuariosAsync()
	{
		List<UsuarioListadoItem> listaUsuarios = new List<UsuarioListadoItem>();

		try
		{
			//Consulta para obtemer una lista de usuarios 
			using (MySqlConnection connection = DbConnectionFactory.CreateConnection())
			{
				await connection.OpenAsync();

				string sql = "SELECT u.ID_USUARIO, u.NOMBRE, u.APELLIDOS, r.NOMBRE AS ROL " +
					"FROM USUARIO u " +
					"LEFT JOIN USUARIO_ROL ur ON u.ID_USUARIO = ur.ID_USUARIO " +
					"LEFT JOIN ROL r ON ur.ID_ROL = r.ID_ROL " +
					"ORDER BY u.NOMBRE, u.APELLIDOS;";

				using (MySqlCommand command = new MySqlCommand(sql, connection))
				using (var reader = await command.ExecuteReaderAsync())
				{
					while (await reader.ReadAsync())
					{
						UsuarioListadoItem item = new UsuarioListadoItem();

						item.IdUsuario = reader.GetInt32(0);

						if (reader.IsDBNull(1)) item.Nombre = "";
						else item.Nombre = reader.GetString(1);

						if (reader.IsDBNull(2)) item.Apellidos = "";
						else item.Apellidos = reader.GetString(2);

						if (reader.IsDBNull(3)) item.TipoUsuario = "";
						else item.TipoUsuario = reader.GetString(3);

						listaUsuarios.Add(item);
					}
				}


			}
		}
		catch (Exception ex)
		{
			throw new Exception("Error al obtener usuarios: " + ex.Message);
		}

		return listaUsuarios;

	}

	public async Task<bool> EliminarUsuarioAsync(int idUsuario)
	{
		try
		{
			using (MySqlConnection connection = DbConnectionFactory.CreateConnection())
			{
				await connection.OpenAsync();
				//Consulta para actualizar la incidencia y dejar null al usuairo asignado
				string sqlNullIncidencias = "UPDATE INCIDENCIA SET ID_USUARIO_ASIGNADO = NULL WHERE ID_USUARIO_ASIGNADO = @Id;";

				using (MySqlCommand command1 = new MySqlCommand(sqlNullIncidencias, connection))
				{
					command1.Parameters.AddWithValue("@Id", idUsuario);
					await command1.ExecuteNonQueryAsync();
				}
				//Consulta para eliminar el usuario de la tabla usuario_rol
				string sqlDeleteRol = "DELETE FROM USUARIO_ROL WHERE ID_USUARIO = @Id;";
				using (MySqlCommand command2 = new MySqlCommand(sqlDeleteRol, connection))
				{
					command2.Parameters.AddWithValue("@Id", idUsuario);
					await command2.ExecuteNonQueryAsync();
				}
				//Consulta para eliminar al usuario
				string sqlDeteleUsuario = "DELETE FROM USUARIO WHERE ID_USUARIO = @Id;";
				using (MySqlCommand command3 = new MySqlCommand(sqlDeteleUsuario, connection))
				{
					command3.Parameters.AddWithValue("@Id", idUsuario);
					int fila = await command3.ExecuteNonQueryAsync();
					return fila > 0;
				}

			}

		} catch (Exception ex)
		{
			throw new Exception("Error añ eliminar usuario: " + ex.Message);
		}
	}

	public async Task<List<RolItem>> ObtenerRolesAsync()
	{
		List<RolItem> listaRoles = new List<RolItem>();
		try
		{
			using (MySqlConnection connection = DbConnectionFactory.CreateConnection())
			{
				//Consulta para obtener los roles
				await connection.OpenAsync();
				string sql = "SELECT ID_ROL, NOMBRE FROM ROL ORDER BY ID_ROL;";
				using (MySqlCommand command = new MySqlCommand(sql, connection))
				using (var reader = await command.ExecuteReaderAsync())
				{
					while (await reader.ReadAsync())
					{
						RolItem item = new RolItem();
						item.IdRol = reader.GetInt32(0);
						if (reader.IsDBNull(1)) item.NombreRol = "";
						else item.NombreRol = reader.GetString(1);
						listaRoles.Add(item);
					}
				}
			}
		}
		catch (Exception ex)
		{
			throw new Exception("Error al obtener roles: " + ex.Message);
		}
		return listaRoles;
	} 

	public async Task<bool> CrearUsuarioAsync(string nombre, string apellidos, string nombreUsuario, string contrasena, int idRol)
	{
		if(nombre == null) nombre = "";
		if(apellidos == null) apellidos = "";
		if(nombreUsuario == null) nombreUsuario = "";
        if (contrasena == null) contrasena = "";

		nombre = nombre.Trim();
		apellidos = apellidos.Trim();
		nombreUsuario = nombreUsuario.Trim();
		contrasena = contrasena.Trim();

		if(nombre.Length == 0) return false;
		if(apellidos.Length == 0) return false;
		if(nombreUsuario.Length == 0) return false;
		if(contrasena.Length == 0) return false;
		if(idRol <= 0) return false;

        try
		{
			using (MySqlConnection connection = DbConnectionFactory.CreateConnection())
			{
				await connection.OpenAsync();
				//Consulta para crear un nuevo usuario
				string sqlInsertUsuario = "INSERT INTO USUARIO (NOMBRE, APELLIDOS, NOMBRE_USUARIO, CONTRASENA) " +
					"VALUES (@Nombre, @Apellidos, @NombreUsuario, @Contrasena);";
				int nuevoIdUsuario = 0;
				using (MySqlCommand command1 = new MySqlCommand(sqlInsertUsuario, connection))
				{
					command1.Parameters.AddWithValue("@Nombre", nombre);
					command1.Parameters.AddWithValue("@Apellidos", apellidos);
					command1.Parameters.AddWithValue("@NombreUsuario", nombreUsuario);
                    command1.Parameters.AddWithValue("@Contrasena", contrasena);
					
					await command1.ExecuteNonQueryAsync();
					nuevoIdUsuario = (int)command1.LastInsertedId;
                }
				//Consulta para asignar el rol al nuevo ususario creado
				string sqlInsertRol = "INSERT INTO USUARIO_ROL (ID_USUARIO, ID_ROL) " +
					"VALUES (@IdUsuario, @IdRol);";
				using (MySqlCommand command2 = new MySqlCommand(sqlInsertRol, connection))
				{
					command2.Parameters.AddWithValue("@IdUsuario", nuevoIdUsuario);
					command2.Parameters.AddWithValue("@IdRol", idRol);
					await command2.ExecuteNonQueryAsync();
				}
				return true;
			}
		}
		catch (Exception ex)
		{
			throw new Exception("Error al crear usuario: " + ex.Message);
		}
    }

	public async Task<UsuarioListadoItem> ObtenerUsuarioPorIdAsync(int idUsuario)
	{
		UsuarioListadoItem usuario = null;
		try
		{
			using (MySqlConnection connection = DbConnectionFactory.CreateConnection())
			{
				//Consulta para obtener la información de un usuario por su id
				await connection.OpenAsync();
				string sql = "SELECT u.ID_USUARIO, u.NOMBRE, u.APELLIDOS, u.NOMBRE_USUARIO, u.CONTRASENA, r.ID_ROL " +
					"FROM USUARIO u " +
					"LEFT JOIN USUARIO_ROL ur ON u.ID_USUARIO = ur.ID_USUARIO " +
					"LEFT JOIN ROL r ON ur.ID_ROL = r.ID_ROL " +
					"WHERE u.ID_USUARIO = @IdUsuario;";
				using (MySqlCommand command = new MySqlCommand(sql, connection))
				{
					command.Parameters.AddWithValue("@IdUsuario", idUsuario);
					using (var reader = await command.ExecuteReaderAsync())
					{
						if (await reader.ReadAsync())
						{
							usuario = new UsuarioListadoItem();

							usuario.IdUsuario = reader.GetInt32(0);
							if (reader.IsDBNull(1)) usuario.Nombre = "";
							else usuario.Nombre = reader.GetString(1);

							if (reader.IsDBNull(2)) usuario.Apellidos = "";
							else usuario.Apellidos = reader.GetString(2);

							if (reader.IsDBNull(3)) usuario.NombreUsuario = "";
							else usuario.NombreUsuario = reader.GetString(3);

							if (reader.IsDBNull(4)) usuario.Contrasena = "";
							else usuario.Contrasena = reader.GetString(4);

							if (reader.IsDBNull(5)) usuario.IdRol = 0;
							else usuario.IdRol = reader.GetInt32(5);
						}
					}
				}
			}
		}
		catch (Exception ex)
		{
			throw new Exception("Error al obtener usuario por ID: " + ex.Message);
		}
		return usuario;
    }

	public async Task<bool> ActualizarUsuarioAsync(int idUsuario, string nombre, string apellidos, string nombreUsuario, string contrasena, int idRol)
	{
		try
		{
			using (MySqlConnection connection = DbConnectionFactory.CreateConnection())
			{
				await connection.OpenAsync();
				//Consulta para actualizar la información de un usuario
				string sqlUpdateUsuario = "UPDATE USUARIO SET NOMBRE = @Nombre, APELLIDOS = @Apellidos, " +
					"NOMBRE_USUARIO = @NombreUsuario, CONTRASENA = @Contrasena WHERE ID_USUARIO = @IdUsuario;";
				using (MySqlCommand command1 = new MySqlCommand(sqlUpdateUsuario, connection))
				{
					command1.Parameters.AddWithValue("@Nombre", nombre);
					command1.Parameters.AddWithValue("@Apellidos", apellidos);
					command1.Parameters.AddWithValue("@NombreUsuario", nombreUsuario);
					command1.Parameters.AddWithValue("@Contrasena", contrasena);
					command1.Parameters.AddWithValue("@IdUsuario", idUsuario);
					
					int filas = await command1.ExecuteNonQueryAsync();
					if (filas <= 0) return false;
                }

				//Consulta para borrar el rol del usuario y actualizarlo si se cambia el rol del usuario
				string sqlDelete = "DELETE FROM USUARIO_ROL WHERE ID_USUARIO = @IdUsuario;";
				using (MySqlCommand commandDelete = new MySqlCommand(sqlDelete, connection))
				{
					commandDelete.Parameters.AddWithValue("@IdUsuario", idUsuario);
					await commandDelete.ExecuteNonQueryAsync();
                }
				
				if(idRol > 0)
				{
                    string sqlInsert= "INSERT INTO USUARIO_ROL (ID_USUARIO, ID_ROL) VALUES (@IdUsuario, @IdRol);";
                    using (MySqlCommand command2 = new MySqlCommand(sqlInsert, connection))
                    {
                        command2.Parameters.AddWithValue("@IdUsuario", idUsuario);
                        command2.Parameters.AddWithValue("@IdRol", idRol);                        
                        await command2.ExecuteNonQueryAsync();
                    }
                    
                }
                return true;

            }
        }catch(Exception ex)
		{
			throw new Exception("Error al actualizar usuario: " + ex.Message);
        }

    }
}