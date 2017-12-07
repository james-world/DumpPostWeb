using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace DumpPostWeb
{
    public class Startup
    {
        private static readonly CircularBuffer Posts = new CircularBuffer(3);

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.Run(async (context) =>
            {
                if (context.Request.Method == "GET" && context.Request.Path == "/show" )
                {
                    var response = new StringBuilder();
                    response.AppendLine("<html><body><ul>");
                    foreach (var post in Posts.Reverse())
                    {
                        response.AppendLine($"<li>{post}</li>");
                    }
                    response.AppendLine("</ul></body></html>");
                    await context.Response.WriteAsync(response.ToString());
                }
                else if (context.Request.Method == "GET" && context.Request.Path == "/favicon.ico")
                {
                    context.Response.StatusCode = 200;
                    await context.Response.SendFileAsync("favicon.ico");
                }
                else
                {
                    var content = new StringBuilder();
                    content.AppendFormat("DateTime:{0}<br>", DateTime.UtcNow);
                    content.AppendFormat("Path:{0}<br>", context.Request.Path);
                    foreach (var header in context.Request.Headers)
                    {
                        content.AppendFormat("{0}:{1}<br>", header.Key, header.Value);
                    }

                    var stream = new StreamReader(context.Request.Body);

                    content.AppendLine("Body:<br>");
                    var body = await stream.ReadToEndAsync();
                    var encodedBody = HttpUtility.HtmlEncode(body);
                    content.AppendLine(encodedBody);

                    Posts.Add(content.ToString());

                    context.Response.StatusCode = 200;
                    await context.Response.WriteAsync("Thanks");
                }
            });
        }
    }
}
