using System.Net.Http;

namespace Chia2Pool.Pool
{
    public class PoolHttpException : HttpRequestException
    {
        public PoolHttpException(string mess) :base(mess)
        {
            
        }
    }
}