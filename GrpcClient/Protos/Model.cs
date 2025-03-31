
using System.Net;

namespace GrpcProvider.Protos.Model
{
    public class Request
    {
        public string Funct;
        public string Data;
    }
    public class Response
    {
        public HttpStatusCode Status;
        public string Data;
    }
}
