using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PeakSWC;
using PeakSWC.Configuration;
using Radzen;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PeakSWC.ConfigurationEditor
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");

            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            builder.Services.AddScoped<DialogService>();
            builder.Services.AddScoped<NotificationService>();

            // Register services
           

            var serializer = new MemoryConfigurationSerializer();
            var root = new RootComponent { Id = "1", Name = "The First Root", StringProp="First string", IntProp=1 };
            //if(serializer.Roots.Count == 0)
                serializer.Roots.Add(root);
            serializer.Roots.Add(new RootComponent { Id = "2", Name = "The Second Root", StringProp="Second string", IntProp=2 });
            await serializer.Write();

            builder.Services.AddSingleton<IConfigurationSerializer>(serializer);
            await builder.Build().RunAsync();
        }
    }
}
