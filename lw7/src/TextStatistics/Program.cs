using System;
using RabbitMqLibrary;
using RedisLibrary;

namespace TextStatistics
{
	class Program
	{
		private const string _listeningExchangeName = "text-rank-calc";

		private static int _totalTextCount = 0;
		private static int _highRankCount = 0;
		private static double _totalRank = 0;

		static void Main()
		{
			LoadStatistics();
			var rabbitMq = new RabbitMq();
			rabbitMq.QueueDeclare();
			rabbitMq.ExchangeDeclare(_listeningExchangeName, ExchangeType.Fanout);
			rabbitMq.BindQueueToExchange(_listeningExchangeName);
			rabbitMq.ConsumeQueue(textId =>
			{
				Redis.Instance.SetDatabase(Redis.Instance.CalculateDatabase(textId));
				double rank = Double.Parse(Redis.Instance.Database.StringGet($"{ConstantLibrary.Redis.Prefix.Rank}{textId}"));
				Console.WriteLine($"'{ConstantLibrary.Redis.Prefix.Rank}{textId}: {rank}' from redis database({Redis.Instance.Database.Database})");
				Console.WriteLine($"{textId}: {rank}");

				++_totalTextCount;
				if (rank > 0.5)
				{
					++_highRankCount;
				}
				_totalRank += rank;

				Redis.Instance.SetDatabase(ConstantLibrary.Redis.StatisticsDatabaseId);
				Redis.Instance.Database.StringSet(ConstantLibrary.Redis.Prefix.Statistics.TotalTextCount, _totalTextCount);
				Redis.Instance.Database.StringSet(ConstantLibrary.Redis.Prefix.Statistics.HighRankCount, _highRankCount);
				Redis.Instance.Database.StringSet(ConstantLibrary.Redis.Prefix.Statistics.TotalRank, _totalRank);
			});

			Console.WriteLine("TextStatistics has started");
			Console.WriteLine("Press [enter] to exit.");
			Console.ReadLine();
		}

		private static void LoadStatistics()
		{
			Redis.Instance.SetDatabase(ConstantLibrary.Redis.StatisticsDatabaseId);
			if (Redis.Instance.Database.KeyExists(ConstantLibrary.Redis.Prefix.Statistics.TotalTextCount))
			{
				_totalTextCount = Int32.Parse(Redis.Instance.Database.StringGet(ConstantLibrary.Redis.Prefix.Statistics.TotalTextCount));
			}
			if (Redis.Instance.Database.KeyExists(ConstantLibrary.Redis.Prefix.Statistics.HighRankCount))
			{
				_highRankCount = Int32.Parse(Redis.Instance.Database.StringGet(ConstantLibrary.Redis.Prefix.Statistics.HighRankCount));
			}
			if (Redis.Instance.Database.KeyExists(ConstantLibrary.Redis.Prefix.Statistics.TotalRank))
			{
				_totalRank = Double.Parse(Redis.Instance.Database.StringGet(ConstantLibrary.Redis.Prefix.Statistics.TotalRank));
			}
		}
	}
}
