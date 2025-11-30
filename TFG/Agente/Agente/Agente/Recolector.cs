using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using Microsoft.Win32;
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
            ManagementObjectSearcher cpu = new ManagementObjectSearcher(@"root\CIMV2", "select Name, NumberOfCores, Manufacturer from Win32_Processor");

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
                if (cpuObj["NumberOfCores"] == null)
                {
                    dhw.CpuNucleos = 0;
                }
                else
                {
                    dhw.CpuNucleos = Convert.ToInt32(cpuObj["NumberOfCores"]);
                }
                if (cpuObj["Manufacturer"] == null)
                {
                    dhw.CpuFabricante = "";
                }
                else
                {
                    dhw.CpuFabricante = Convert.ToString(cpuObj["Manufacturer"]);
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
                if (ramObj["Capacity"] != null) 
                ramTotal += Convert.ToInt64(ramObj["Capacity"]);
                if (ramObj["SMBIOSMemoryType"] != null)
                {
                    int codigoTipo = Convert.ToInt32(ramObj["SMBIOSMemoryType"]);
                    string tipoActual = TipoRamNombre(codigoTipo);
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

            //Función para obtener el nombre del tipo de RAM
            static string TipoRamNombre(int codigo)
            {
                switch(codigo)
                {
                    case 20:
                        return "DDR";
                    case 21:
                        return "DDR2";
                    case 24:
                        return "DDR3";
                    case 26:
                        return "DDR4";
                    case 34:
                        return "DDR5";
                    default:
                        return "Desconocido (" + codigo.ToString() + ")";
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
            dsw.ModeloEquipo = "";
            dsw.RedIp = "";
            dsw.RedEstado = "Desconectado";

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
                    if (!string.IsNullOrEmpty(lastBoot))
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
            dsw.EstadoDefender = "Desconocido";
            try
            {
                ManagementObjectSearcher defender = new ManagementObjectSearcher(@"root\SecurityCenter2", "select displayName from AntiVirusProduct");
                ManagementObjectCollection defenderCol = defender.Get();
                ManagementObjectCollection.ManagementObjectEnumerator defenderEnum = defenderCol.GetEnumerator();

                if (defenderEnum.MoveNext())
                {
                    ManagementObject defenderObj = (ManagementObject)defenderEnum.Current;
                    if (defenderObj["displayName"] != null)
                    {
                        string nombreDefender = defenderObj["displayName"].ToString();
                        string nombreMinusculas = nombreDefender.ToLower();
                        if (nombreDefender.ToLower().Contains("windows defender") || nombreMinusculas.Contains("microsoft defender"))
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
            catch (System.Management.ManagementException)
            {
                dsw.EstadoDefender = "Desconocido";
            }
            catch (System.UnauthorizedAccessException)
            {
                dsw.EstadoDefender = "Desconocido";
            }
            catch (Exception)
            {
                dsw.EstadoDefender = "Desconocido";
            }

            //Modelo Equipo
            ManagementObjectSearcher modelo = new ManagementObjectSearcher("select Model from Win32_ComputerSystem");
            ManagementObjectCollection modeloCol = modelo.Get();
            ManagementObjectCollection.ManagementObjectEnumerator modeloEnum = modeloCol.GetEnumerator();
            if (modeloEnum.MoveNext())
            {
                ManagementObject modeloObj = (ManagementObject)modeloEnum.Current;
                if (modeloObj["Model"] != null)
                {
                    dsw.ModeloEquipo = modeloObj["Model"].ToString();
                }
            }

            //IP y Estado Red
            try
            {
                ManagementObjectSearcher red = new ManagementObjectSearcher("select IPAddress from Win32_NetworkAdapterConfiguration where IPEnabled=true");
                ManagementObjectCollection redCol = red.Get();
                ManagementObjectCollection.ManagementObjectEnumerator redEnum = redCol.GetEnumerator();

                if(redEnum.MoveNext())
                {
                    ManagementObject redObj = (ManagementObject)redEnum.Current;
                    if (redObj["IPAddress"] != null)
                    {
                        object ipAddressObj = redObj["IPAddress"];
                        if(ipAddressObj is Array)
                        {
                            string[] ipAddresses = (string[])ipAddressObj;
                            if (ipAddresses.Length > 0)
                            {
                                dsw.RedIp = ipAddresses[0];
                                
                            }
                        }
                        if(dsw.RedIp != "")
                        {
                            dsw.RedEstado = "Conectado";
                        }
                    }
                }
            }
            catch(Exception)
            {
                dsw.RedIp = "";
                dsw.RedEstado = "Desconocido";
            }       

            return dsw;

        }

        //Recolector de Programas Instalados
        public static List<DatosProgramas> recolectorProgramas()
        {
            List<DatosProgramas> listaProgramas = new List<DatosProgramas>();
            //Programas de 64 bits
            LeerProgramasDeRegistro(listaProgramas, Registry.LocalMachine, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall");
            //Programas de 32 bits
            LeerProgramasDeRegistro(listaProgramas, Registry.LocalMachine, @"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall");

            return listaProgramas;
        }

        private static void LeerProgramasDeRegistro(List<DatosProgramas> listaProgramas, RegistryKey localMachine, string ruta)
        {
            RegistryKey clave = localMachine.OpenSubKey(ruta);
            if (clave == null)
            {
                return;
            }
            string[] subclaves = clave.GetSubKeyNames();
            int i = 0;

            while (i < subclaves.Length)
            {
                string nombreSubclave = subclaves[i];
                RegistryKey appKey = clave.OpenSubKey(subclaves[i]);
                if (appKey != null)
                {
                    object nombrePrograma = appKey.GetValue("DisplayName");
                    if (nombrePrograma != null)
                    {
                        DatosProgramas dp = new DatosProgramas();
                        dp.NombrePrograma = nombrePrograma.ToString();


                        object rutaProg = appKey.GetValue("InstallLocationº");
                        if (rutaProg != null)
                        {
                            dp.RutaPrograma = rutaProg.ToString();
                        }
                        else
                        {
                            dp.RutaPrograma = "";
                        }
                        listaProgramas.Add(dp);
                    }
                    appKey.Close();
                }
                i = i + 1;
            }
            clave.Close();
        }



    }
}

