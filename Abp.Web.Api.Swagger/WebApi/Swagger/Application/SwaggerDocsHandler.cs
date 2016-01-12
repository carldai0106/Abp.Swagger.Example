using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Abp.WebApi.Swagger.Application
{
    public class SwaggerDocsHandler : HttpMessageHandler
    {
        private readonly SwaggerDocsConfig _config;

        public SwaggerDocsHandler(SwaggerDocsConfig config)
        {
            _config = config;
        }
      
        private Task<HttpResponseMessage> TaskFor(HttpResponseMessage response)
        {
            var tsc = new TaskCompletionSource<HttpResponseMessage>();
            tsc.SetResult(response);
            return tsc.Task;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            try
            {
                return TaskFor(new HttpResponseMessage());
            }
            catch (Exception ex)
            {
                return TaskFor(request.CreateErrorResponse(HttpStatusCode.NotFound, ex));
            }
        }
    }
}
