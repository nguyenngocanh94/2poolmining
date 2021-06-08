using Chia.NET.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Chia.NET.Clients
{
    public sealed class FullNodeClient : ChiaApiClient
    {
        private const string ApiUrl = "https://localhost:8555/";

        public FullNodeClient(string ssl)
            : base(ssl,"full_node", ApiUrl)
        {
            InitializeAsync();
        }

        public async Task<BlockchainState> GetBlockchainStateAsync()
        {
            var result = await PostAsync<GetBlockchainStateResult>(FullNodeRoutes.GetBlockchainState(ApiUrl));
            return result.BlockchainState;
        }
        
        public BlockchainState GetBlockchainState()
        {
            var result = Post<GetBlockchainStateResult>(FullNodeRoutes.GetBlockchainState(ApiUrl));
            return result.BlockchainState;
        }
        public async Task<Block> GetBlockAsync(string headerHash)
        {
            var result = await PostAsync<GetBlockResult>(FullNodeRoutes.GetBlock(ApiUrl), new Dictionary<string, string>()
            {
                ["header_hash"] = headerHash,
            });
            return result.Block;
        }
        public async Task<Block[]> GetBlocksAsync(int startHeight, int endHeight)
        {
            var result = await PostAsync<GetBlocksResult>(FullNodeRoutes.GetBlocks(ApiUrl), new Dictionary<string, string>()
            {
                ["start"] = $"{startHeight}",
                ["end"] = $"{endHeight}",
            });
            return result.Blocks;
        }
    }
}
