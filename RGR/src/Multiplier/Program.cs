using System;
using System.Linq;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace Multiplier
{
	public static class Program
	{
		public static void Main(string[] args)
		{
			if (args.Length != 1)
			{
				Console.WriteLine("Error: port isn't specified");
				return;
			}
			BuildWebHost(args).Run();
		}

		private static IWebHost BuildWebHost(string[] args) =>
			WebHost.CreateDefaultBuilder()
				.UseStartup<Startup>()
				.UseUrls($"http://localhost:{args.ElementAt(0)}")
				.Build();
	}
}