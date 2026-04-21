using Server.Web.Structures;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using uhttpsharp;
using uhttpsharp.Handlers;

namespace Server.Web.Handlers
{
    public class TelemetryRestController : IRestController<TelemetryData>
    {
        public Task<IEnumerable<TelemetryData>> Get(IHttpRequest request)
        {
            return Task.FromResult(Enumerable.Repeat(WebServer.TelemetryData, 1));
        }

        public Task<TelemetryData> GetItem(IHttpRequest request)
        {
            return Task.FromResult(WebServer.TelemetryData);
        }

        public Task<TelemetryData> Create(IHttpRequest request)
        {
            throw new HttpException(HttpResponseCode.MethodNotAllowed, "POST not supported");
        }

        public Task<TelemetryData> Upsert(IHttpRequest request)
        {
            throw new HttpException(HttpResponseCode.MethodNotAllowed, "PUT not supported");
        }

        public Task<TelemetryData> Delete(IHttpRequest request)
        {
            throw new HttpException(HttpResponseCode.MethodNotAllowed, "DELETE not supported");
        }
    }
}