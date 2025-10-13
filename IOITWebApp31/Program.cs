using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System.IO;

namespace IOITWebApp31
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        //public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
        //   WebHost.CreateDefaultBuilder(args)
        //  .UseStartup<Startup>();//
        //  //.UseUrls("http://localhost:5002"); sử dụng khi chạy trên linux
        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            var builder = new ConfigurationBuilder()
                   .SetBasePath(Directory.GetCurrentDirectory())
                   .AddJsonFile("appsettings.json");

            var configuration = builder.Build();
            string con = configuration.GetConnectionString("urls"); // cấu hinh build linux

            return WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
            .UseUrls(con);
        }
    }
}
