using System;
using MySqlConnector;
using System.Threading.Tasks;
using InventarioActivos.Models.DatosActivos;
using InventarioActivos.Data;

namespace InventarioActivos.Services;

public class ActivosService 
{
    //Consulta para obtener una lista de activos con un resumen items de hardware y software
	public async Task<List<ItemActivos>> ObtenerActivosResumenAsync(string filtroNombre)
	{
		List<ItemActivos> lista = new List<ItemActivos>();
		
		if (filtroNombre == null) filtroNombre = "";
		filtroNombre = filtroNombre.Trim();

		try
		{
			using(MySqlConnection connection = DbConnectionFactory.CreateConnection())
			{
				await connection.OpenAsync();

				string sql = "SELECT a.ID_ACTIVO, a.NOMBRE_EQUIPO, a.USUARIO_ACTUAL, l.NOMBRE AS LOCALIZACION, " +
                             "h.CPU_NOMBRE, h.RAM_NUMERO_MODULOS, h.RAM_TIPO_MODULOS, h.DISCO_DURO_MODELO, h.DISCO_DUROS_ESPACIO_LIBRE, " +
                             "s.SO_NOMBRE, s.SO_VERSION, s.SEGURIDAD_ESTADO_WINDOWS_DEFENDER " +
                             "FROM ACTIVOS a " +
                             "LEFT JOIN LOCALIZACION l ON a.ID_LOCALIZACION = l.ID_LOCALIZACION " +
                             "LEFT JOIN ( " +
                             "   SELECT ID_ACTIVO, " +
                             "          MAX(CPU_NOMBRE) CPU_NOMBRE, " +
                             "          MAX(RAM_NUMERO_MODULOS) RAM_NUMERO_MODULOS, " +
                             "          MAX(RAM_TIPO_MODULOS) RAM_TIPO_MODULOS, " +
                             "          MAX(DISCO_DURO_MODELO) DISCO_DURO_MODELO, " +
                             "          MAX(DISCO_DUROS_ESPACIO_LIBRE) DISCO_DUROS_ESPACIO_LIBRE " +
                             "   FROM ACTIVOS_HARDWARE " +
                             "   GROUP BY ID_ACTIVO " +
                             ") h ON a.ID_ACTIVO = h.ID_ACTIVO " +
                             "LEFT JOIN ( " +
                             "   SELECT ID_ACTIVO, " +
                             "          MAX(SO_NOMBRE) SO_NOMBRE, " +
                             "          MAX(SO_VERSION) SO_VERSION, " +
                             "          MAX(SEGURIDAD_ESTADO_WINDOWS_DEFENDER) SEGURIDAD_ESTADO_WINDOWS_DEFENDER " +
                             "   FROM ACTIVOS_SOFTWARE " +
                             "   GROUP BY ID_ACTIVO " +
                             ") s ON a.ID_ACTIVO = s.ID_ACTIVO " +
                             "WHERE (@Filtro = '' OR a.NOMBRE_EQUIPO LIKE CONCAT('%', @Filtro, '%')) " +
                             "ORDER BY a.NOMBRE_EQUIPO;";

                using (MySqlCommand command = new MySqlCommand(sql, connection))
				{
					command.Parameters.AddWithValue("@Filtro", filtroNombre);

					using(var reader = await command.ExecuteReaderAsync())
					{
						while (await reader.ReadAsync())
						{
							ItemActivos item = new ItemActivos();

							item.IdActivo = reader.GetInt32(0);

							if (reader.IsDBNull(1)) item.NombreEquipo = "";
							else item.NombreEquipo = reader.GetString(1);

							if (reader.IsDBNull(2)) item.UsuarioActual = "";
							else item.UsuarioActual = reader.GetString(2);

							if (reader.IsDBNull(3)) item.NombreLocalizacion = "";
							else item.NombreLocalizacion = reader.GetString(3);

							if (reader.IsDBNull(4)) item.CpuNombre = "";
							else item.CpuNombre = reader.GetString(4);

                            if (reader.IsDBNull(5)) item.RanurasRam = 0;
                            else item.RanurasRam = reader.GetInt32(5);

                            if (reader.IsDBNull(6)) item.TipoRam = "";
                            else item.TipoRam = reader.GetString(6);

                            if (reader.IsDBNull(7)) item.ModeloDisco = "";
                            else item.ModeloDisco = reader.GetString(7);

                            if (reader.IsDBNull(8)) item.CapacidadDisco = 0;
                            else item.CapacidadDisco = reader.GetInt32(8);

                            if (reader.IsDBNull(9)) item.SistemaOperativo = "";
                            else item.SistemaOperativo = reader.GetString(9);

                            if (reader.IsDBNull(10)) item.VersionSO = "";
                            else item.VersionSO = reader.GetString(10);

                            if (reader.IsDBNull(11)) item.EstadoDefender = "";
                            else item.EstadoDefender = reader.GetString(11);

							lista.Add(item);

                        }
					}
				}
			}
		}catch(Exception ex)
		{
			throw new Exception("Error al obtener activos resumen: " + ex.Message);
		}
		return lista;
	}
    //Consulta para obtener todos los items hardware y software de un activo
    public async Task<ItemActivos> ObtenerFichaActivoAsync(int idActivo)
    {
        if (idActivo <= 0) return  null;
        ItemActivos item = null;
        try
        {
            using (MySqlConnection connection = DbConnectionFactory.CreateConnection())
            {
                await connection.OpenAsync();

                string sql = "SELECT a.ID_ACTIVO, a.NOMBRE_EQUIPO, a.USUARIO_ACTUAL, l.NOMBRE AS LOCALIZACION, " +
                "h.CPU_NOMBRE, h.CPU_NUMERO_NUCLEOS, h.CPU_FABRICANTE, " +
                "h.RAM_CAPACIDAD_TOTAL, h.RAM_NUMERO_MODULOS, h.RAM_TIPO_MODULOS, h.RAM_NUMERO_RANURAS, " +
                "h.PLACA_MODELO, h.PLACA_VERSION, " +
                "h.DISCO_DURO_MODELO, h.DISCO_DURO_CAPACIDAD, h.DISCO_DUROS_ESPACIO_LIBRE, " +
                "h.RED_TIPO, " +
                "s.SO_NOMBRE, s.SO_VERSION, s.SO_ULTIMO_ARRANQUE, s.USUARIO_NOMBRE, s.SEGURIDAD_ESTADO_WINDOWS_DEFENDER " +
                "FROM ACTIVOS a " +
                "LEFT JOIN LOCALIZACION l ON a.ID_LOCALIZACION = l.ID_LOCALIZACION " +
                "LEFT JOIN ( " +
                "   SELECT ID_ACTIVO, " +
                "          MAX(CPU_NOMBRE) CPU_NOMBRE, " +
                "          MAX(CPU_NUMERO_NUCLEOS) CPU_NUMERO_NUCLEOS, " +
                "          MAX(CPU_FABRICANTE) CPU_FABRICANTE, " +
                "          MAX(RAM_CAPACIDAD_TOTAL) RAM_CAPACIDAD_TOTAL, " +
                "          MAX(RAM_NUMERO_MODULOS) RAM_NUMERO_MODULOS, " +
                "          MAX(RAM_TIPO_MODULOS) RAM_TIPO_MODULOS, " +
                "          MAX(RAM_NUMERO_RANURAS) RAM_NUMERO_RANURAS, " +
                "          MAX(PLACA_MODELO) PLACA_MODELO, " +
                "          MAX(PLACA_VERSION) PLACA_VERSION, " +
                "          MAX(DISCO_DURO_MODELO) DISCO_DURO_MODELO, " +
                "          MAX(DISCO_DURO_CAPACIDAD) DISCO_DURO_CAPACIDAD, " +
                "          MAX(DISCO_DUROS_ESPACIO_LIBRE) DISCO_DUROS_ESPACIO_LIBRE, " +
                "          MAX(RED_TIPO) RED_TIPO " +
                "   FROM ACTIVOS_HARDWARE " +
                "   GROUP BY ID_ACTIVO " +
                ") h ON a.ID_ACTIVO = h.ID_ACTIVO " +
                "LEFT JOIN ( " +
                "   SELECT ID_ACTIVO, " +
                "          MAX(SO_NOMBRE) SO_NOMBRE, " +
                "          MAX(SO_VERSION) SO_VERSION, " +
                "          MAX(SO_ULTIMO_ARRANQUE) SO_ULTIMO_ARRANQUE, " +
                "          MAX(USUARIO_NOMBRE) USUARIO_NOMBRE, " +
                "          MAX(SEGURIDAD_ESTADO_WINDOWS_DEFENDER) SEGURIDAD_ESTADO_WINDOWS_DEFENDER " +
                "   FROM ACTIVOS_SOFTWARE " +
                "   GROUP BY ID_ACTIVO " +
                ") s ON a.ID_ACTIVO = s.ID_ACTIVO " +
                "WHERE a.ID_ACTIVO = @Id;";

                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Id", idActivo);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if(await reader.ReadAsync())
                        {
                            item = new ItemActivos();

                            item.IdActivo = reader.GetInt32(0);

                            if (reader.IsDBNull(1)) item.NombreEquipo = "";
                            else item.NombreEquipo = reader.GetString(1);

                            if (reader.IsDBNull(2)) item.UsuarioActual = "";
                            else item.UsuarioActual = reader.GetString(2);

                            if (reader.IsDBNull(3)) item.NombreLocalizacion = "";
                            else item.NombreLocalizacion = reader.GetString(3);

                            //Hardware
                            if (reader.IsDBNull(4)) item.CpuNombre = "";
                            else item.CpuNombre = reader.GetString(4);

                            if (reader.IsDBNull(5)) item.CpuNumeroNucleos = 0;
                            else item.CpuNumeroNucleos = reader.GetInt32(5);

                            if (reader.IsDBNull(6)) item.CpuFabricante = "";
                            else item.CpuFabricante = reader.GetString(6);

                            if (reader.IsDBNull(7)) item.RamCapacidad = 0;
                            else item.RamCapacidad = reader.GetInt32(7);

                            if (reader.IsDBNull(8)) item.RanurasRam = 0;
                            else item.RanurasRam = reader.GetInt32(8);

                            if (reader.IsDBNull(9)) item.TipoRam = "";
                            else item.TipoRam = reader.GetString(9);

                            if (reader.IsDBNull(10)) item.RamNumeroRarunaras = 0;
                            else item.RamNumeroRarunaras = reader.GetInt32(10);

                            if (reader.IsDBNull(11)) item.PlacaModelo = "";
                            else item.PlacaModelo = reader.GetString(11);

                            if (reader.IsDBNull(12)) item.PlacaVersion = "";
                            else item.PlacaVersion = reader.GetString(12);

                            if (reader.IsDBNull(13)) item.ModeloDisco = "";
                            else item.ModeloDisco = reader.GetString(13);

                            if (reader.IsDBNull(14)) item.CapacidadDisco = 0;
                            else item.CapacidadDisco = reader.GetInt32(14);

                            if (reader.IsDBNull(15)) item.DiscoDuroCapacidadTotal = 0;
                            else item.DiscoDuroCapacidadTotal = reader.GetInt32(15);

                            if (reader.IsDBNull(16)) item.RedTipo = "";
                            else item.RedTipo = reader.GetString(16);

                            //Software
                            if (reader.IsDBNull(17)) item.SistemaOperativo = "";
                            else item.SistemaOperativo = reader.GetString(17);

                            if (reader.IsDBNull(18)) item.VersionSO = "";
                            else item.VersionSO = reader.GetString(18);

                            if (reader.IsDBNull(19)) item.SOUltimoArranque = DateTime.MinValue;
                            else item.SOUltimoArranque = reader.GetDateTime(19);

                            if (reader.IsDBNull(21)) item.EstadoDefender = "";
                            else item.EstadoDefender = reader.GetString(21);
                        }
                    }
                }

                if (item == null) return null;

                //Consulta para filtrar los programas que el activo tenga instalados en la carpeta ProgramFiles
                string sqlProgramas = "SELECT DISTINCT TRIM(SI_NOMBRE) " +
                "FROM ACTIVOS_SOFTWARE " +
                "WHERE ID_ACTIVO = @Id " +
                "AND SI_NOMBRE IS NOT NULL AND TRIM(SI_NOMBRE) <> '' " +
                "AND SI_RUTA IS NOT NULL AND TRIM(SI_RUTA) <> '' " +
                "AND ( " +
                "   SI_RUTA LIKE 'C:\\\\Program Files\\\\%' ESCAPE '|' " +
                "   OR SI_RUTA LIKE 'C:\\\\Program Files (x86)\\\\%' ESCAPE '|' " +
                ") " +
                "ORDER BY TRIM(SI_NOMBRE);";

                using (MySqlCommand command2 = new MySqlCommand(sqlProgramas, connection))
                {
                    command2.Parameters.AddWithValue("@Id", idActivo);

                    using (var reader2 = await command2.ExecuteReaderAsync())
                    {
                        item.ProgramasInstalados.Clear();

                        while (await reader2.ReadAsync())
                        {
                            string nombrePrograma = "";
                            if (!reader2.IsDBNull(0)) nombrePrograma = reader2.GetString(0);

                            nombrePrograma = nombrePrograma.Trim();
                            if (nombrePrograma.Length > 0)
                            {
                                item.ProgramasInstalados.Add(nombrePrograma);
                            }
                        }
                    }
                }
                item.TotalProgramas = item.ProgramasInstalados.Count;
            }
        }
        catch (Exception ex)
        {

            throw new Exception("Error al obtener los elementos del activo: " + ex.Message);
        }
        return item;
    }
}