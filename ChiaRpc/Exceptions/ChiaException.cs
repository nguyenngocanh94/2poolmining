using System.Net.Http;

namespace ChiaRpc.Exceptions
{
    public class ChiaHttpException : HttpRequestException
    {
        public ChiaHttpException(string mess) : base(mess)
        {
            
        }
    }
    
    public class ChiaConnectionException : HttpRequestException
    {
        public ChiaConnectionException(string mess) : base(mess)
        {
            
        }
    }
}