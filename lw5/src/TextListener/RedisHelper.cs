using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;

namespace TextListener
{
	public sealed class RedisHelper
	{
		private static readonly string REDIS_CONNECTIONSTRING = "REDIS_CONNECTIONSTRING";
		private static IDatabase _database;
		
		private RedisHelper()
		{
			var config = new ConfigurationBuilder()
				.AddEnvironmentVariables()
				.Build();

			string connectionString = config[REDIS_CONNECTIONSTRING];

			if (connectionString == null)
			{
				throw new KeyNotFoundException($"Environment variable for {REDIS_CONNECTIONSTRING} was not found.");
			}

			ConfigurationOptions options = ConfigurationOptions.Parse(connectionString);

			_database = ConnectionMultiplexer.Connect(options).GetDatabase();
		}

		public static RedisHelper Instance { get; } = new RedisHelper();
	
		public string Get(string key)
		{
			return _database.StringGet(key);
		}

		public void Set(string key, string value)
		{
			_database.StringSet(key, value);
		}

		public void Delete(string key)
		{
			_database.KeyDelete(key);
		}
	}
}
