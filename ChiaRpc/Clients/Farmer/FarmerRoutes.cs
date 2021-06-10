using System;

namespace Chia.NET.Clients.Farmer
{
    public static class FarmerRoutes
    {
        public static Uri SetRewardTargets(string apiUrl)
            => new Uri(apiUrl + "set_reward_targets");
        
        public static Uri GetRewardTarget(string apiUrl)
            => new Uri(apiUrl + "get_reward_targets");
    }
}
