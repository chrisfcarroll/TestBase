﻿using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace GuineaPig.AspNetCore2
{
    public class Program
    {
        public static void Main(string[] args) { BuildWebHost(args).Run(); }

        public static IWebHost BuildWebHost(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
                          .UseStartup<Startup>()
                          .Build();
        }
    }
}
