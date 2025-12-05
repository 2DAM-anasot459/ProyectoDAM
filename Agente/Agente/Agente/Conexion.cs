
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySqlConnector;

namespace Agente
{
    public class Conexion
    {
        private string cadena;
        public Conexion(string cadenaConexion)
        {
            cadena = cadenaConexion;
        }

        // Método para probar la conexión a la base de datos
        public void ProbarConexion()
        {
            MySqlConnection conex = new MySqlConnection(cadena);
            conex.Open();
            conex.Close();
        }

        //Método para obtener o crear el Id del activo por nombre
        public int ObtenerIdActivoPorNombre(string nombreEquipo)
        {
            int idActivo = -1;
            MySqlConnection conex = new MySqlConnection(cadena);

            string colsultaBusqueda = "SELECT ID_ACTIVO FROM ACTIVOS WHERE NOMBRE_EQUIPO = @nombreEquipo";
            MySqlCommand comandoBusqueda = new MySqlCommand(colsultaBusqueda, conex);
            comandoBusqueda.Parameters.AddWithValue("@nombreEquipo", nombreEquipo);

            conex.Open();

            object resultado = comandoBusqueda.ExecuteScalar();

            if (resultado != null)
            {
                idActivo = Convert.ToInt32(resultado);
            }
            else
            {
                string consultaInsercion = "INSERT INTO ACTIVOS (NOMBRE_EQUIPO, MODELO_EQUIPO, USUARIO_ACTUAL, RED_IP, RED_ESTADO, SO_NOMBRE, SO_VERSION, ID_LOCALIZACION)" +
                "VALUES(@nombreEquipo, @modeloequipo, @usuarioactual, @redip, @redestado, @sonombre, @soversion, @idlocalizacion)";
                MySqlCommand comandoInsercion = new MySqlCommand(consultaInsercion, conex);
                comandoInsercion.Parameters.AddWithValue("@nombreEquipo", nombreEquipo);
                comandoInsercion.Parameters.AddWithValue("@modeloequipo", "");
                comandoInsercion.Parameters.AddWithValue("@usuarioactual", "");
                comandoInsercion.Parameters.AddWithValue("@redip", "");
                comandoInsercion.Parameters.AddWithValue("@redestado", "");
                comandoInsercion.Parameters.AddWithValue("@sonombre", "");
                comandoInsercion.Parameters.AddWithValue("@soversion", "");
                comandoInsercion.Parameters.AddWithValue("@idlocalizacion", null);
                comandoInsercion.ExecuteNonQuery();

                idActivo = (int)comandoInsercion.LastInsertedId;
            }
            conex.Close();
            return idActivo;
        }

        //Método para insertar datos hardware
        public void InsertarDatosHardware(int idActivo, DatosHardware datosHardware)
        {
            MySqlConnection conex = new MySqlConnection(cadena);
            conex.Open();

            string consultaEliminacion = "DELETE FROM ACTIVOS_HARDWARE WHERE ID_ACTIVO = @idActivo";
            MySqlCommand comandoEliminacion = new MySqlCommand(consultaEliminacion, conex);
            comandoEliminacion.Parameters.AddWithValue("@idActivo", idActivo);
            comandoEliminacion.ExecuteNonQuery();

            string consultaInsercion = "INSERT INTO ACTIVOS_HARDWARE (ID_ACTIVO, CPU_NOMBRE, CPU_NUMERO_NUCLEOS, CPU_FABRICANTE, RAM_CAPACIDAD_TOTAL, RAM_NUMERO_MODULOS, RAM_TIPO_MODULOS, RAM_NUMERO_RANURAS, PLACA_MODELO, PLACA_VERSION, DISCO_DURO_MODELO, DISCO_DURO_CAPACIDAD, DISCO_DUROS_ESPACIO_LIBRE, RED_TIPO)" +
                "VALUES(@idActivo, @cpuNombre, @cpuNucleos, @cpuFabricante, @ramTotalGb, @ramModulos, @ramTipo, @ramRanuras, @placaModelo, @placaVersion, @discoModelo, @discoCapacidadGb, @discoLibreGb, @tipoRed)";
            MySqlCommand comandoInsercion = new MySqlCommand(consultaInsercion, conex);
            comandoInsercion.Parameters.AddWithValue("@idActivo", idActivo);
            comandoInsercion.Parameters.AddWithValue("@cpuNombre", datosHardware.CpuNombre);
            comandoInsercion.Parameters.AddWithValue("@cpuNucleos", datosHardware.CpuNucleos);
            comandoInsercion.Parameters.AddWithValue("@cpuFabricante", datosHardware.CpuFabricante);
            comandoInsercion.Parameters.AddWithValue("@ramTotalGb", datosHardware.RamTotalGb);
            comandoInsercion.Parameters.AddWithValue("@ramModulos", datosHardware.RamModulos);
            comandoInsercion.Parameters.AddWithValue("@ramTipo", datosHardware.RamTipo);
            comandoInsercion.Parameters.AddWithValue("@ramRanuras", datosHardware.RamRanuaras);
            comandoInsercion.Parameters.AddWithValue("@placaModelo", datosHardware.PlacaModelo);
            comandoInsercion.Parameters.AddWithValue("@placaVersion", datosHardware.PlacaVersion);
            comandoInsercion.Parameters.AddWithValue("@discoModelo", datosHardware.DiscoModelo);
            comandoInsercion.Parameters.AddWithValue("@discoCapacidadGb", datosHardware.DiscoCapacidadGb);
            comandoInsercion.Parameters.AddWithValue("@discoLibreGb", datosHardware.DiscoLibreGb);
            comandoInsercion.Parameters.AddWithValue("@tipoRed", datosHardware.TipoRed);
            
            comandoInsercion.ExecuteNonQuery();
            conex.Close();
        }

        //Método para insertar datos software(SO y programas)
        public void InsertarDatosSoftware(int idActivo, DatosSoftware datosSoftware, List<DatosProgramas> programas)
        {
            MySqlConnection conex = new MySqlConnection(cadena);
            conex.Open();

            string consultaEliminacion = "DELETE FROM ACTIVOS_SOFTWARE WHERE ID_ACTIVO = @idActivo";
            MySqlCommand comandoEliminacion = new MySqlCommand(consultaEliminacion, conex);
            comandoEliminacion.Parameters.AddWithValue("@idActivo", idActivo);
            comandoEliminacion.ExecuteNonQuery();

            int i = 0;

            while (i < programas.Count)
            {
                DatosProgramas programa = programas[i];
                string consultaInsercionSO = "INSERT INTO ACTIVOS_SOFTWARE (ID_ACTIVO, SO_NOMBRE, SO_VERSION, SO_ULTIMO_ARRANQUE, SI_NOMBRE, SI_RUTA, USUARIO_NOMBRE, SEGURIDAD_ESTADO_WINDOWS_DEFENDER)" +
               "VALUES(@idActivo, @soNombre, @soVersion, @ultimoArranque, @nombrePrograma, @rutaPrograma, @usuarioActual, @estadoDefender)";
                MySqlCommand comandoInsercionSO = new MySqlCommand(consultaInsercionSO, conex);
                comandoInsercionSO.Parameters.AddWithValue("@idActivo", idActivo);
                comandoInsercionSO.Parameters.AddWithValue("@soNombre", datosSoftware.SistemaOperativo);
                comandoInsercionSO.Parameters.AddWithValue("@soVersion", datosSoftware.SOVersion);
                comandoInsercionSO.Parameters.AddWithValue("@ultimoArranque", datosSoftware.UltimoArranque);
                comandoInsercionSO.Parameters.AddWithValue("@nombrePrograma", programa.NombrePrograma);
                comandoInsercionSO.Parameters.AddWithValue("@rutaPrograma", programa.RutaPrograma);                
                comandoInsercionSO.Parameters.AddWithValue("@usuarioActual", datosSoftware.NombreUsuario);
                comandoInsercionSO.Parameters.AddWithValue("@estadoDefender", datosSoftware.EstadoDefender);                
                comandoInsercionSO.ExecuteNonQuery();
                
                i = i + 1;
            }          
           
            conex.Close();            

        }

        //Método para actualizar datos del activo
        public void ActualizarDatosActivo(int idActivo, DatosSoftware ds)
        {
            MySqlConnection conex = new MySqlConnection(cadena);
            string consultaActualizacion = "UPDATE ACTIVOS SET MODELO_EQUIPO = @modeloEquipo, USUARIO_ACTUAL = @usuarioActual, RED_IP = @redIp, RED_ESTADO = @redEstado, SO_NOMBRE = @soNombre, SO_VERSION = @soVersion WHERE ID_ACTIVO = @idActivo";
            MySqlCommand comandoActualizacion = new MySqlCommand(consultaActualizacion, conex);
            comandoActualizacion.Parameters.AddWithValue("@modeloEquipo", ds.ModeloEquipo);
            comandoActualizacion.Parameters.AddWithValue("@usuarioActual", ds.NombreUsuario);
            comandoActualizacion.Parameters.AddWithValue("@redIp", ds.RedIp);   
            comandoActualizacion.Parameters.AddWithValue("@redEstado", ds.RedEstado);
            comandoActualizacion.Parameters.AddWithValue("@soNombre", ds.SistemaOperativo);
            comandoActualizacion.Parameters.AddWithValue("@soVersion", ds.SOVersion);
            comandoActualizacion.Parameters.AddWithValue("@idActivo", idActivo);
            conex.Open();
            comandoActualizacion.ExecuteNonQuery();
            conex.Close();
        }
    }
}
