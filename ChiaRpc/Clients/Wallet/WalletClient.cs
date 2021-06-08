using Chia.NET.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Chia.NET.Clients
{
    public sealed class WalletClient : ChiaApiClient
    {
        private const string ApiUrl = "https://localhost:9256/";

        public WalletClient(string ssl)
            : base(ssl,"wallet", ApiUrl)
        {
            InitializeAsync();
        }

        /// <summary>
        /// Retrieves balances for a wallet.
        /// </summary>
        /// <returns></returns>
        public async Task<Wallet> GetWalletBalance(int walletId)
        {
            var result = await PostAsync<GetWalletBalanceResult>(WalletRoutes.GetWalletBalance(ApiUrl), new Dictionary<string, string>()
            {
                ["wallet_id"] = $"{walletId}"
            });
            return result.Wallet;
        }

        /// <summary>
        /// Retrieves the address of the wallet at the given id for the current key.
        /// </summary>
        /// <returns></returns>
        public async Task<string> GetWalletAddressAsync(int walletId, bool generateAddress)
        {
            var result = await PostAsync<GetWalletAddressResult>(WalletRoutes.GetWalletAddress(ApiUrl), new Dictionary<string, string>()
            {
                ["wallet_id"] = $"{walletId}",
                ["new_address"] = $"{generateAddress}"
            });
            return result.Address;
        }
    }
}
