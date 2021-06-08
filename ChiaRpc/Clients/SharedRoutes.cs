using System;

namespace Chia.NET.Clients
{
    internal static class SharedRoutes
    {
        public static Uri GetConnections(string apiUrl)
            => new Uri(apiUrl + "get_connections");
        
        public static Uri StopNode(string apiUrl)
            => new Uri(apiUrl + "stop_node");
    }
}
