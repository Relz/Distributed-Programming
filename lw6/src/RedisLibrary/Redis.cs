using System;
using StackExchange.Redis;

namespace RedisLibrary
{
	public class Redis
	{
		public static readonly Redis Instance = new Redis();

		public IDatabase Database { get; }

		private Redis()
		{
			Database = ConnectionMultiplexer.Connect(ConstantLibrary.Redis.ConnectionString).GetDatabase();
		}
	}
}
