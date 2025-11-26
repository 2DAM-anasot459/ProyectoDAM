using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace Agente
{
    public class Recolector
    {
        //Recolector de Datos Hardware
        public static DatosHardware recolectorHardware()
        {
            DatosHardware dhw = new DatosHardware();

            //CPU
            ManagementObjectSearcher cpu = new ManagementObjectSearcher("select Name, NumberOfCore, Manufacturer from Win32_Processor");

            ManagementObjectCollection cpuCol = cpu.Get();
            ManagementObjectCollection.ManagementObjectEnumerator cpuEnum = cpuCol.GetEnumerator();
            ManagementObject cpuObj = null;

            if (cpuEnum.MoveNext())
            {
                cpuObj = (ManagementObject)cpuEnum.Current;

            }

            if (cpuObj != null)
            {
                if (cpuObj["Name"] == null)
                {
                    dhw.CpuNombre = "";
                }
                else
                {
                    dhw.CpuNombre = cpuObj["Name"].ToString();

                }
                if (cpuObj["NumberOfCore"] == null)
                {
                    dhw.CpuNucleos = 0;
                }
                else
                {
                    dhw.CpuNucleos = Convert.ToInt32(cpuObj["NumberOfCore"]);
                }
                if (cpuObj["Manufacturer"] == null)
                {
                    dhw.CpuFabricante = 0;
                }
                else
                {
                    dhw.CpuFabricante = Convert.ToInt32(cpuObj["Manufacturer"]);
                }
            }

            //RAM
            ManagementObjectSearcher ram = new ManagementObjectSearcher("select Capacity, SMBIOSMemoryType from Win32_PhysicalMemory");

            ManagementObjectCollection ramCol = ram.Get();
            ManagementObjectCollection.ManagementObjectEnumerator ramEnum = ramCol.GetEnumerator();

            long ramTotal = 0;
            int modulos = 0;
            string tipoRam = "";
            bool tipoRamDiferente = false;

            while (ramEnum.MoveNext())
            {
                ManagementObject ramObj = (ManagementObject)ramEnum.Current;
                modulos++;
                if (ramObj["Capacity"] != null) ;
                ramTotal += Convert.ToInt64(ramObj["Capacity"]);
                if (ramObj["SMBIOSMemoryType"] != null)
                {
                    string tipoActual = ramObj["SMBIOSMemoryType"].ToString();
                    if (tipoRam == "")
                    {
                        tipoRam = tipoActual;
                    }
                    else
                    {
                        if (tipoRam != tipoActual)
                        {
                            tipoRamDiferente = true;
                        }
                    }
                }
                dhw.RamModulos = modulos;
                dhw.RamTotalGb = (int)(ramTotal / (1024d * 1024 * 1024));

                if (tipoRamDiferente)
                {
                    dhw.RamTipo = "Variedad";
                }
                else
                {
                    dhw.RamTipo = tipoRam;
                }

                //Ranuras Ram
                ManagementObjectSearcher ranurasRam = new ManagementObjectSearcher("select MemoryDevices from Win32_PhysicalMemoryArray");
                ManagementObjectCollection ranurasRamCol = ranurasRam.Get();
                ManagementObjectCollection.ManagementObjectEnumerator ranurasRamEnum = ranurasRamCol.GetEnumerator();

                dhw.RamRanuaras = 0;
                while (ranurasRamEnum.MoveNext())
                {
                    ManagementObject ranuraObj = (ManagementObject)ranurasRamEnum.Current;
                    if (ranuraObj["MemoryDevices"] != null)
                    {
                        dhw.RamRanuaras += Convert.ToInt32(ranuraObj["MemoryDevices"]);
                    }
                }
            }

            //Placa Base
            ManagementObjectSearcher placa = new ManagementObjectSearcher("select Product, Version from Win32_BaseBoard");
            ManagementObjectCollection placaCol = placa.Get();
            ManagementObjectCollection.ManagementObjectEnumerator placaEnum = placaCol.GetEnumerator();

            while (placaEnum.MoveNext())
            {
                ManagementObject placaObj = (ManagementObject)placaEnum.Current;
                if (placaObj["Product"] == null)
                {
                    dhw.PlacaModelo = "";
                }
                else
                {
                    dhw.PlacaModelo = placaObj["Product"].ToString();
                }
                if (placaObj["Version"] == null)
                {
                    dhw.PlacaVersion = "";
                }
                else
                {
                    dhw.PlacaVersion = placaObj["Version"].ToString();
                }
            }

            //Disco Duro
            ManagementObjectSearcher disco = new ManagementObjectSearcher("select Model, Size from Win32_DiskDrive");
            ManagementObjectCollection discoCol = disco.Get();    
            ManagementObjectCollection.ManagementObjectEnumerator discoEnum = discoCol.GetEnumerator();
            if (discoEnum.MoveNext())
            {
                ManagementObject discoObj = (ManagementObject)discoEnum.Current;

                if (discoObj["Model"] != null)
                    dhw.DiscoModelo = discoObj["Model"].ToString();
                if (discoObj["Size"] != null)
                {
                    double capacidadGB = Convert.ToDouble(discoObj["Size"]) / (1024d * 1024 * 1024);
                    dhw.DiscoCapacidadGb = (int)Math.Round(capacidadGB);
                }
            }

            //Espacio libre en C
            ManagementObjectSearcher espacioC = new ManagementObjectSearcher("select DeviceID, FreeSpace from Win32_LogicalDisk where DriveType=3");
            ManagementObjectCollection CCol = espacioC.Get();
            ManagementObjectCollection.ManagementObjectEnumerator CEnum = CCol.GetEnumerator();

            while (CEnum.MoveNext())
            {
                ManagementObject CObj = (ManagementObject)CEnum.Current;
                if (CObj["DeviceID"] != null && CObj["DeviceID"].ToString() == "C:")
                {
                    if (CObj["FreeSpace"] != null)
                    {
                        double libreGB = Convert.ToDouble(CObj["FreeSpace"]) / (1024d * 1024 * 1024);
                        dhw.DiscoLibreGb = (int)Math.Round(libreGB);
                    }
                }
            }

            //Red
            ManagementObjectSearcher red = new ManagementObjectSearcher("select AdapterType from Win32_NetworkAdapter where NetEnabled=true");
            ManagementObjectCollection redCol = red.Get();
            ManagementObjectCollection.ManagementObjectEnumerator redEnum = redCol.GetEnumerator();

            if (redEnum.MoveNext())
            {
                ManagementObject redObj = (ManagementObject)redEnum.Current;
                if (redObj["AdapterType"] != null)
                {
                    dhw.TipoRed = redObj["AdapterType"].ToString();
                }
            }

            return dhw;





        }

        //Recolector de Datos Software
        public static DatosSoftware recolectorSoftware()
        {
            DatosSoftware dsw = new DatosSoftware();
            dsw.SistemaOperativo = "";
            dsw.SOVersion = "";
            dsw.UltimoArranque = DateTime.UtcNow;
            dsw.NombreUsuario = "";
            
            //Sistema Operativo
            ManagementObjectSearcher so = new ManagementObjectSearcher("select Caption, Version, LastBootUpTime from Win32_OperatingSystem");
            ManagementObjectCollection soCol = so.Get();
            ManagementObjectCollection.ManagementObjectEnumerator soEnum = soCol.GetEnumerator();

            while (soEnum.MoveNext())
            {
                ManagementObject soObj = (ManagementObject)soEnum.Current;
                if (soObj["Caption"] != null)
                {
                    dsw.SistemaOperativo = soObj["Caption"].ToString();
                }
                if (soObj["Version"] != null)
                {
                    dsw.SOVersion = soObj["Version"].ToString();
                }
                if (soObj["LastBootUpTime"] != null)
                {
                    string lastBoot = soObj["LastBootUpTime"].ToString();
                    if(!string.IsNullOrEmpty(lastBoot))
                        dsw.UltimoArranque = ManagementDateTimeConverter.ToDateTime(lastBoot);
                }                               
            }

            //Nombre Usuario
            ManagementObjectSearcher usuario = new ManagementObjectSearcher("select UserName from Win32_ComputerSystem");
            ManagementObjectCollection usuarioCol = usuario.Get();
            ManagementObjectCollection.ManagementObjectEnumerator usuarioEnum = usuarioCol.GetEnumerator();

            while (usuarioEnum.MoveNext())
            {
                ManagementObject usuarioObj = (ManagementObject)usuarioEnum.Current;
                if (usuarioObj["UserName"] != null)
                {
                    dsw.NombreUsuario = usuarioObj["UserName"].ToString();
                }
            }

            //Estado Windows Defender
            try
            {
                ManagementObjectSearcher defender = new ManagementObjectSearcher("select displayName from AntiVirusProduct");
                ManagementObjectCollection defenderCol = defender.Get();
                ManagementObjectCollection.ManagementObjectEnumerator defenderEnum = defenderCol.GetEnumerator();

                if (defenderEnum.MoveNext())
                {
                    ManagementObject defenderObj = (ManagementObject)defenderEnum.Current;
                    if(defenderObj["displayName"] != null)
                    {
                        string nombreDefender = defenderObj["displayName"].ToString();
                        if (nombreDefender.ToLower().Contains("windows defender"))
                        {
                            dsw.EstadoDefender = "Activo";
                        }
                        else
                        {
                            dsw.EstadoDefender = "Inactivo";
                        }
                    }
                }
            }
            catch
            {
                dsw.EstadoDefender = "Desconocido";
            }
            


            return dsw;

        }

        //Recolector de Programas Instalados
        public static List<DatosProgramas> recolectorProgramas()
        {
            List<DatosProgramas> listaProgramas = new List<DatosProgramas>();
            ManagementObjectSearcher programas = new ManagementObjectSearcher("select Name, InstallLocation from Win32_Product");
            ManagementObjectCollection programasCol = programas.Get();
            ManagementObjectCollection.ManagementObjectEnumerator programasEnum = programasCol.GetEnumerator();
            while (programasEnum.MoveNext())
            {
                ManagementObject programaObj = (ManagementObject)programasEnum.Current;
                DatosProgramas dp = new DatosProgramas();
                if (programaObj["Name"] != null)
                {
                    dp.NombrePrograma = programaObj["Name"].ToString();
                }
                else
                {
                    dp.NombrePrograma = "";
                }
                if (programaObj["InstallLocation"] != null)
                {
                    dp.RutaPrograma = programaObj["InstallLocation"].ToString();
                }
                else
                {
                    dp.RutaPrograma = "";
                }
                listaProgramas.Add(dp);
            }
            return listaProgramas;
        }
    }
}
