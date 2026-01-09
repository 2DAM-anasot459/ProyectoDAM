namespace InventarioActivos.Data;

public static class DbConfig
{
	public static string Server = "qaon980.islamagica.info";
	public static int Port = 3306;
	public static string Database = "qaon980";
	public static string User = "qaon980";
	public static string Password = "4n4.S0t0TFg";

	public static string GetConnectionString()
	{
		string cs = "";
		cs = cs + "Server=" + Server + ";";
		cs = cs + "Port=" + Port.ToString() + ";";
		cs = cs + "Database=" + Database + ";";
		cs = cs + "User Id=" + User + ";";
		cs = cs + "Password=" + Password + ";";
		cs = cs + "SslMode=Required;";
		return cs;
    }

}