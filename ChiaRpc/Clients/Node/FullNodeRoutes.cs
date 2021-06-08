using System;

namespace Chia.NET.Clients
{
    internal static class FullNodeRoutes
    {
        public static Uri GetBlockchainState(string apiUrl)
            => new Uri(apiUrl + "get_blockchain_state");

        public static Uri GetBlock(string apiUrl)
            => new Uri(apiUrl + "get_block");

        public static Uri GetBlocks(string apiUrl)
            => new Uri(apiUrl + "get_blocks");
    }
}
