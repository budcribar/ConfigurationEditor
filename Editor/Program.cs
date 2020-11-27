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
            builder.Services.AddSingleton<IConfigurationSerializer>(new JSONConfigurationSerializer());

            var serializer = new JSONConfigurationSerializer();
            var root = new RootComponent { Id = "1", Name = "TheRoot" };
            if(serializer.Roots.Count == 0)
                serializer.Roots.Add(root);
            await serializer.Write();


            await builder.Build().RunAsync();
        }
    }
}
