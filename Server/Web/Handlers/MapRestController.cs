using Server.Web.Structures;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using uhttpsharp;
using uhttpsharp.Handlers;

namespace Server.Web.Handlers
{
    public class MapRestController : IRestController<MapData>
    {
        public Task<IEnumerable<MapData>> Get(IHttpRequest request)
        {
            return Task.FromResult(new[] { WebServer.MapData }.AsEnumerable());
        }

        public Task<MapData> GetItem(IHttpRequest request)
        {
            return Task.FromResult(WebServer.MapData);
        }

        public Task<MapData> Create(IHttpRequest request)
        {
            throw new HttpException(HttpResponseCode.MethodNotAllowed, "The method is not allowed");
        }
        public Task<MapData> Upsert(IHttpRequest request)
        {
            throw new HttpException(HttpResponseCode.MethodNotAllowed, "The method is not allowed");
        }
        public Task<MapData> Delete(IHttpRequest request)
        {
            throw new HttpException(HttpResponseCode.MethodNotAllowed, "The method is not allowed");
        }
    }
}