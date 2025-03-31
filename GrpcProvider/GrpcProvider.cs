using Grpc.Core;
using GrpcProvider.Protos;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net;
using System.Reflection;
using static GrpcProvider.Protos.GrpcProvider;

namespace GrpcProvider
{
    public class GrpcProvider : GrpcProviderBase
    {
        private readonly ILogger<GrpcProvider> _logger;
        public GrpcProvider(ILogger<GrpcProvider> logger)
        {
            _logger = logger;
        }

        public override Task<Response> HandleMessage(Request request, ServerCallContext context)
        {
            string fullClassName = request.FullClassName;
            string funcName = request.Funct.ToString();
            string inputData = request.Data.ToString();

            Assembly assembly = Assembly.Load("UserService");
            Type type = assembly.GetType(fullClassName);
            var instance = Activator.CreateInstance(type);
            MethodInfo method = type.GetMethod(funcName);
            if (method != null)
            {
                ParameterInfo[] parameters = method.GetParameters();
                object result;

                if (parameters.Length == 0)
                {
                    result = method.Invoke(null, null);
                }
                else
                {
                    Type paramType = parameters[0].ParameterType;
                    object paramObject = JsonConvert.DeserializeObject(inputData, paramType);
                    result = method.Invoke(instance, new object[] { paramObject });
                    return Task.FromResult(new Response { Status = HttpStatusCode.OK.ToString(), Data = JsonConvert.SerializeObject(result) });
                }

            }
            return Task.FromResult(new Response { Status = HttpStatusCode.InternalServerError.ToString() });
        }
    }
}
