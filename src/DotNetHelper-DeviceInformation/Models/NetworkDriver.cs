namespace DotNetHelper_DeviceInformation.Models
{
    public class NetworkDriver
    {
        /// <summary>
        /// 
        /// </summary>
        public string MacAddress { get; protected internal set; }
        /// <summary>
        /// 
        /// </summary>
        public string Description { get; protected internal set; }
        /// <summary>
        /// 
        /// </summary>
        public bool DhcpEnabled { get; protected internal set; }
        /// <summary>
        /// 
        /// </summary>
        public string DhcpServer { get; protected internal set; }
        /// <summary>
        /// 
        /// </summary>
        public string ServiceName { get; protected internal set; }
        /// <summary>
        /// 
        /// </summary>
        public string Caption { get; protected internal set; }
        /// <summary>
        /// 
        /// </summary>
        public string[] IpAddress { get; protected internal set; }
        /// <summary>
        /// 
        /// </summary>
        public string[] DefaultIpGateway { get; protected internal set; }
    }
}
