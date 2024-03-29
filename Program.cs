using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure.Identity;

namespace AzureAppDynamicConfig
{
   public class Program
   {
      public static void Main(string[] args)
      {
         CreateHostBuilder(args).Build().Run();
      }

      public static IHostBuilder CreateHostBuilder(string[] args) =>
          Host.CreateDefaultBuilder(args)
              .ConfigureWebHostDefaults(webBuilder =>
                                        {
                                           webBuilder.ConfigureAppConfiguration(builder =>
                                                                                {
                                                                                   var config = builder.Build();
                                                                                   builder.AddAzureAppConfiguration(
                                                                                      az =>
                                                                                      {
                                                                                         //Connect using local conn string
                                                                                         az.Connect(config.GetConnectionString("AppConfig"));
                                                                                         //Config auto refresh
                                                                                         az.ConfigureRefresh(rf =>
                                                                                                             {
                                                                                                                rf.Register("CHYA:WebApp:Dynamic:Signal", true);
                                                                                                                rf.SetCacheExpiration(new TimeSpan(0,0,15));
                                                                                                             });

                                                                                         //Config KV reference
                                                                                         //Setup service principle to access key vault
                                                                                         Environment.SetEnvironmentVariable("AZURE_CLIENT_ID ", "971c306c-8ea5-4247-8a07-7732facc8d58");
                                                                                         Environment.SetEnvironmentVariable("AZURE_CLIENT_SECRET", "UQF-z_kFJRr~lCAWrwHslg0f1Q75-4rvMw");
                                                                                         Environment.SetEnvironmentVariable("AZURE_TENANT_ID", "4e6f57dc-a3d9-4a0c-818b-a7c1bb2b79f6");
                                                                                         az.ConfigureKeyVault(kv => {
                                                                                            kv.SetCredential(new DefaultAzureCredential());
                                                                                         });
                                                                                      });
                                                                                   
                                                                                });
                 webBuilder.UseStartup<Startup>();
              });
   }
}
