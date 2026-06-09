

using Microsoft.AspNetCore.Builder;

using Microsoft.AspNetCore.Http;

using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

using System.Reflection;
using System.Text;

namespace BaseCore.Common.Sockets
{

    // ════════════════════════════════════════════════════════════
    // EXTENSION QUẢN LÝ WEBSOCKET (DI & PIPELINE)
    // ════════════════════════════════════════════════════════════

    public static class WebSocketManagerExtensions
    {

        

        // ════════════════════════════════════════════════════════════
        // ĐĂNG KÝ DỊCH VỤ
        // ════════════════════════════════════════════════════════════

        public static IServiceCollection AddWebSocketService(this IServiceCollection services)
        {

            

            services.AddTransient<WebSocketConnectionManager>();

            

            
            
            foreach (var type in Assembly.GetEntryAssembly().ExportedTypes)
            {

                if (type.GetTypeInfo().BaseType == typeof(WebSocketHandler))
                {
                    services.AddSingleton(type);
                }
            }
            return services;
        }

        

        

        

        
        // ════════════════════════════════════════════════════════════
        // ÁNH XẠ MIDDLEWARE
        // ════════════════════════════════════════════════════════════

        public static IApplicationBuilder MapWebSocketManager(this IApplicationBuilder app, PathString path, WebSocketHandler handler)
        {

            return app.Map(path, (_app) => {

                _app.UseMiddleware<WebSocketManagerMiddleware>(handler);
            });
        }
    }
}
