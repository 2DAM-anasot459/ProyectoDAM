using MySqlConnector;
namespace InventarioActivos.Data;

public static class DbConnectionFactory
{
	public static MySqlConnection CreateConnection()
	{
		string cs = DbConfig.GetConnectionString();
		MySqlConnection conn = new MySqlConnection(cs);
		return conn;
    }

}