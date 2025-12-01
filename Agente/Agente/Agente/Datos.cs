using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agente
{
    public class DatosHardware
    {
        //Datos Hardware del activo
        public string CpuNombre { get; set; }
        public int CpuNucleos { get; set; }
        public string CpuFabricante { get; set; }
        public int RamTotalGb { get; set; }
        public int RamModulos { get; set; }
        public string RamTipo { get; set; }
        public int RamRanuaras { get; set; }
        public string PlacaModelo { get; set; }
        public string PlacaVersion { get; set; }
        public string DiscoModelo { get; set; }
        public int DiscoCapacidadGb { get; set; }
        public int DiscoLibreGb { get; set; }
        public string TipoRed { get; set; }
    }

    public class DatosSoftware
    {
        //Datos Software del activo
        public string SistemaOperativo { get; set; }
        public string SOVersion { get; set; }
        public DateTime UltimoArranque { get; set; }
        public string NombreUsuario { get; set; }
        public string EstadoDefender { get; set; }

        //Datos para tabla Activos
        public string ModeloEquipo { get; set; }
        public string RedIp { get; set; }
        public string RedEstado { get; set; }

    }


    public class DatosProgramas
    { //Datos Programas instalados
        public string NombrePrograma { get; set; }
        public string RutaPrograma { get; set; }
    }
}
