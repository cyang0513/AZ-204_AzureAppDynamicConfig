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
                                                                                         az.ConfigureKeyVault(kv =>
                                                                                                              {
                                                                                                                 kv.SetCredential(new AzureCliCredential());
                                                                                                              }
                                                                                         );
                                                                                      });
                                                                                   
                                                                                });
                 webBuilder.UseStartup<Startup>();
              });
   }
}
