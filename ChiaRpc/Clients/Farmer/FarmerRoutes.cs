using System;

namespace Chia.NET.Clients.Farmer
{
    public static class FarmerRoutes
    {
        public static Uri SetRewardTargets(string apiUrl)
            => new Uri(apiUrl + "set_reward_targets");
    }
}
