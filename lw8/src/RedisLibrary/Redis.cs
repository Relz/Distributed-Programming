using System;
using StackExchange.Redis;

namespace RedisLibrary
{
	public class Redis
	{
		public static readonly Redis Instance = new Redis();

		public IDatabase Database { get; private set; }

		private Redis()
		{
			SetDatabase(-1);
		}

		public void SetDatabase(int db)
		{
			Database = ConnectionMultiplexer.Connect(ConstantLibrary.Redis.ConnectionString).GetDatabase(db);
		}

		public int CalculateDatabase(string data)
		{
			int digitCount = 0;
			foreach (char ch in data)
			{
				if (Char.IsDigit(ch))
				{
					++digitCount;
				}
			}

			return digitCount % ConstantLibrary.Redis.RankCalcDatabaseCount;
		}
	}
}
