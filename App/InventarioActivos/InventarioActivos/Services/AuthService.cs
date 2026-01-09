using System;
using MySqlConnector;
using System.Threading.Tasks;
using InventarioActivos.Models.Auth;
using InventarioActivos.Data;

namespace InventarioActivos.Services;

public class AuthService 
{
	

	public async Task<LoginResult> LoginAsync(string usuario, string contrasena)
	{
		LoginResult result = new LoginResult();

		if(usuario == null) usuario = "";
		if(contrasena == null) contrasena = "";

		usuario = usuario.Trim();
		contrasena = contrasena.Trim();

		if(usuario.Length == 0 || contrasena.Length == 0)
		{
			return result;
        }
		try
		{
            using (MySqlConnection connection = DbConnectionFactory.CreateConnection())
            {
                await connection.OpenAsync();

                string sql =
                    "SELECT u.ID_USUARIO, r.NOMBRE " +
                    "FROM USUARIO u " +
                    "INNER JOIN USUARIO_ROL ur ON u.ID_USUARIO = ur.ID_USUARIO " +
                    "INNER JOIN ROL r ON ur.ID_ROL = r.ID_ROL " +
                    "WHERE u.NOMBRE_USUARIO = @user AND u.CONTRASENA = @pass" +
                    " LIMIT 1;";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@user", usuario);
                    command.Parameters.AddWithValue("@pass", contrasena);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            result.Ok = true;
                            result.IdUsuario = reader.GetInt32(0);

                            string rol = reader.GetString(1);
                            if (rol == null) rol = "";
                            result.Rol = rol;
                        }
                    }
                }
            }
        }catch (Exception ex)
        {
            await Console.Out.WriteLineAsync("Error en AuthService.LoginAsync: " + ex.Message);
            result.Ok = false;  
        }



        return result;
    }

    public async Task<bool> CambiarContrasenaAsync(string usuario, string contrasenaActual,string nuevaContrasena)
    {
        if (usuario == null) usuario = "";
        if (contrasenaActual == null) contrasenaActual = "";
        if (nuevaContrasena == null) nuevaContrasena = "";

        usuario = usuario.Trim();
        contrasenaActual = contrasenaActual.Trim();
        nuevaContrasena = nuevaContrasena.Trim();

        if (usuario.Length == 0) return false;
        if (contrasenaActual.Length == 0) return false;
        if (nuevaContrasena.Length == 0) return false;
        try
        {
            using (MySqlConnection connection = DbConnectionFactory.CreateConnection())
            {
                await connection.OpenAsync();
                string sql =
                    "UPDATE USUARIO " +
                    "SET CONTRASENA = @newPass " +
                    "WHERE NOMBRE_USUARIO = @user AND CONTRASENA = @oldPass;";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@newPass", nuevaContrasena);
                    command.Parameters.AddWithValue("@user", usuario);
                    command.Parameters.AddWithValue("@oldPass", contrasenaActual);
                    int filas = await command.ExecuteNonQueryAsync();
                    
                    if (filas > 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            await Console.Out.WriteLineAsync("Error en AuthService.ChangePasswordAsync: " + ex.Message);
            return false;
        }
    }
}