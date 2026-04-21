using Server.Web.Structures;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using uhttpsharp;
using uhttpsharp.Handlers;

namespace Server.Web.Handlers
{
    public class ChatRestController : IRestController<ChatData>
    {
        public Task<IEnumerable<ChatData>> Get(IHttpRequest request)
        {
            return Task.FromResult(Enumerable.Repeat(WebServer.ChatData, 1));
        }

        public Task<ChatData> GetItem(IHttpRequest request)
        {
            return Task.FromResult(WebServer.ChatData);
        }

        public Task<ChatData> Create(IHttpRequest request)
        {
            string text;
            if (!request.Post.Parsed.TryGetByName("text", out text) || string.IsNullOrWhiteSpace(text))
                throw new HttpException(HttpResponseCode.BadRequest, "Missing 'text' field");

            WebServer.ChatData.AddMessage("[Server]", text);
            ChatData.Broadcast("[Server]", text);

            return Task.FromResult(WebServer.ChatData);
        }

        public Task<ChatData> Upsert(IHttpRequest request)
        {
            throw new HttpException(HttpResponseCode.MethodNotAllowed, "The method is not allowed");
        }
        public Task<ChatData> Delete(IHttpRequest request)
        {
            throw new HttpException(HttpResponseCode.MethodNotAllowed, "The method is not allowed");
        }
    }
}