using System;
using System.Collections.Generic;
using System.Linq;

#if NETFRAMEWORK
using System.Management;
#endif 

using System.Runtime.InteropServices;
using DotNetHelper_DeviceInformation.Models;

namespace DotNetHelper_DeviceInformation
{
    /// <summary>
    /// A static class used to determine the device/machine system information
    /// </summary>
    public static class DeviceInformation
    {

        /// <summary>
        /// Gets the device identifier.
        /// </summary>
        /// <value>The device identifier.</value>
        public static string DeviceId { get; } = null;
        /// <summary>
        /// Gets The Device Model.
        /// </summary>
        /// <value>The model.</value>
        public static string Model { get; }
        /// <summary>
        /// Gets the name of the machine.
        /// </summary>
        /// <value>The name of the machine.</value>
        public static string MachineName { get; } = Environment.MachineName;
        /// <summary>
        /// Gets the network drivers.
        /// </summary>
        /// <value>The network drivers.</value>
        public static List<NetworkDriver> NetworkDrivers { get; } = GetNetworkDrivers();
        /// <summary>
        /// Gets the disk drivers.
        /// </summary>
        /// <value>The disk drivers.</value>
        public static List<DiskDriver> DiskDrivers { get; } = GetDiskDrivers();
        /// <summary>
        /// Gets the device os.
        /// </summary>
        /// <value>The device os.</value>
        public static DeviceOs DeviceOS { get; } = GetDeviceOs();
        /// <summary>
        /// Gets the platform.
        /// </summary>
        /// <value>The platform.</value>
        public static string Platform { get; } = GetDeviceOs().ToString();
        /// <summary>
        /// Gets or sets the version.
        /// </summary>
        /// <value>The version.</value>
        public static string Version { get; set; } = GetPcInfo("Caption");
        /// <summary>
        /// Gets the framework description.
        /// </summary>
        /// <value>The framework description.</value>
        internal static string FrameworkDescription { get; } = GetFrameworkDescription();
        /// <summary>
        /// Gets the os description.
        /// </summary>
        /// <value>The os description.</value>
        public static string OsDescription { get; } = GetOsDescription();
        /// <summary>
        /// Gets the CPU architecture. 
        /// </summary>
        /// <value>The architecture.</value>
        public static string Architecture { get; } = GetArchitecture();
        /// <summary>
        /// Gets or sets the manufacturer.
        /// </summary>
        /// <value>The manufacturer.</value>
        public static string Manufacturer { get; set; } = GetPcInfo("Manufacturer");
        /// <summary>
        /// Gets the processor count.
        /// </summary>
        /// <value>The processor count.</value>
        internal static int ProcessorCount { get; } = Environment.ProcessorCount;
        /// <summary>
        /// Gets the height of the device.
        /// </summary>
        /// <value>The height of the device.</value>
        public static int DeviceHeight { get; } = 0;
        /// <summary>
        /// Gets the width of the device.
        /// </summary>
        /// <value>The width of the device.</value>
        public static int DeviceWidth { get; } = 0;



        // disable resharper 
        /// <summary>
        /// Enum DeviceOs
        /// </summary>
        public enum DeviceOs
        {
            /// <summary>
            /// The unknown
            /// </summary>
            Unknown = 0,
            /// <summary>
            /// The windows
            /// </summary>
            Windows = 1,
            /// <summary>
            /// The mac os
            /// </summary>
            MacOs = 2,
            /// <summary>
            /// The i os
            /// </summary>
            iOS = 3,
            /// <summary>
            /// The win phone
            /// </summary>
            WinPhone = 4,
            /// <summary>
            /// The android
            /// </summary>
            Android = 5,
            /// <summary>
            /// The uwp
            /// </summary>
            UWP = 6,
            /// <summary>
            /// The linux
            /// </summary>
            Linux = 7,
        }


       

        /// <summary>
        /// Gets the pc information.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>System.String.</returns>
        private static string GetPcInfo(string name)
        {


#if NETFRAMEWORK
                try
                {

                    var scope = new ManagementScope($@"\\{Environment.MachineName}\root\cimv2",new ConnectionOptions());
                    scope.Connect();
                    //Query system for Operating System information
                    var query = new ObjectQuery("SELECT * FROM Win32_OperatingSystem");
                    var searcher = new ManagementObjectSearcher(scope, query);
                    var queryCollection = searcher.Get();
                    return (from ManagementBaseObject m in queryCollection select m[name].ToString()).FirstOrDefault();
                }
                catch (Exception)
                {
                    // ignore
                }
#endif
            return RuntimeInformation.OSDescription;
            

        }



        /// <summary>
        /// Gets the network drivers.
        /// </summary>
        /// <returns>List&lt;NetworkDriver&gt;.</returns>
        private static List<NetworkDriver> GetNetworkDrivers()
        {
            var mak = new List<NetworkDriver>() { };
#if NETFRAMEWORK
            try {
            var scope = new ManagementScope($@"\\{Environment.MachineName}\root\cimv2", new ConnectionOptions()); scope.Connect();
            var objectSearcher = new ManagementObjectSearcher(scope, new ObjectQuery("SELECT * FROM Win32_NetworkAdapterConfiguration where IPEnabled=True"));

            foreach (var o in objectSearcher.Get())
            {
                var mo = (ManagementObject)o;
                var item = new NetworkDriver();
                mak.Add(item);
                item.MacAddress = mo["MacAddress"] as string;
                item.DhcpEnabled = (bool)mo["DHCPEnabled"];
                item.IpAddress = mo["IPAddress"] as string[];
                item.DefaultIpGateway = mo["DefaultIPGateway"] as string[];
                item.DhcpServer = mo["DHCPServer"] as string;
                item.Description = mo["Description"] as string;
                item.ServiceName = mo["ServiceName"] as string;
                item.Caption = mo["Caption"] as string;
            }
            }catch(Exception){
               // ignore
            }
#endif
            return mak;

        }

        /// <summary>
        /// Gets the disk drivers.
        /// </summary>
        /// <returns>List&lt;DiskDriver&gt;.</returns>
        private static List<DiskDriver> GetDiskDrivers()
        {
            var list = new List<DiskDriver>();
#if NETFRAMEWORK
              try { 
            var scope = new ManagementScope($@"\\{Environment.MachineName}\root\cimv2", new ConnectionOptions()); scope.Connect();
            var disks = new ManagementObjectSearcher(scope, new ObjectQuery("SELECT * FROM Win32_DiskDrive"));

            foreach (var o in disks.Get())
            {
                var disk = (ManagementObject) o;
                var diskdev = new DiskDriver();
                list.Add(diskdev);

                var sn = disk.GetPropertyValue("SerialNumber") as string;
                diskdev.SerialNumber = string.IsNullOrEmpty(sn) ? ParseSerialFromDeviceID(disk["PNPDeviceID"].ToString()) : sn;
                diskdev.Model = disk.GetPropertyValue("Model") as string;
                diskdev.InterfaceType = disk.GetPropertyValue("InterfaceType") as string;

                if (!string.IsNullOrEmpty(diskdev.SerialNumber)) diskdev.SerialNumber = diskdev.SerialNumber.Trim();
                if (!string.IsNullOrEmpty(diskdev.Model)) diskdev.Model = diskdev.Model.Trim();
                if (!string.IsNullOrEmpty(diskdev.InterfaceType)) diskdev.InterfaceType = diskdev.InterfaceType.Trim();
            }
            return list;
              }catch(Exception){
               // ignore
            }
#endif

            return list;

        }


        /// <summary>
        /// Parses the serial from device identifier.
        /// </summary>
        /// <param name="deviceId">The device identifier.</param>
        /// <returns>System.String.</returns>
        private static string ParseSerialFromDeviceID(string deviceId)
        {
            var splitDeviceId = deviceId.Split('\\');
            var arrayLen = splitDeviceId.Length - 1;

            var serialArray = splitDeviceId[arrayLen].Split('&');
            var serial = serialArray[0];

            return serial;
        }



        /// <summary>
        /// Gets the framework description.
        /// </summary>
        /// <returns>System.String.</returns>
        private static string GetFrameworkDescription()
        {
            try
            {
                return RuntimeInformation.FrameworkDescription;
            }
            catch (Exception)
            {
                return null;
            }
        }
        /// <summary>
        /// Gets the os description.
        /// </summary>
        /// <returns>System.String.</returns>
        private static string GetOsDescription()
        {
            try
            {
                return RuntimeInformation.OSDescription;
            }
            catch (Exception)
            {
                return null;
            }
        }
        /// <summary>
        /// Gets the architecture.
        /// </summary>
        /// <returns>System.String.</returns>
        private static string GetArchitecture()
        {
            try
            {
                //  return Environment.GetEnvironmentVariable("PROCESSOR_ARCHITECTURE");
                return RuntimeInformation.OSArchitecture.ToString();
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the device os.
        /// </summary>
        /// <returns>DeviceOs.</returns>
        public static DeviceOs GetDeviceOs()
        {

            try
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    if (Environment.CommandLine.Contains(".apk")) // TODO :: Do you know da way
                    {
                        return DeviceOs.Android;
                    }
                    return DeviceOs.Linux;
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    return DeviceOs.MacOs;
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    return DeviceOs.Windows;
                }

            }
            catch (Exception)
            {
                return DeviceOs.Windows;
            }
            return DeviceOs.Unknown;
        }
    }
}
