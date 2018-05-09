using System;
using System.Globalization;
using ModelLibrary;
using RabbitMqLibrary;
using RedisLibrary;

namespace TextStatistics
{
	class Program
	{
		private const string _listeningExchangeName = "text-success-marker";

		private static Statistics _statistics = new Statistics();

		static void Main()
		{
			LoadStatistics();
			var rabbitMq = new RabbitMq();
			rabbitMq.QueueDeclare();
			rabbitMq.ExchangeDeclare(_listeningExchangeName, ExchangeType.Fanout);
			rabbitMq.BindQueueToExchange(_listeningExchangeName);
			rabbitMq.ConsumeQueue(textIdAndStatus =>
			{
				Console.WriteLine($"New message from {_listeningExchangeName}: \"{textIdAndStatus}\"");

				string[] splittedMessage = textIdAndStatus.Split(ConstantLibrary.RabbitMq.Delimiter);
				string textId = splittedMessage[0];
				bool isSucceededText = splittedMessage[1] == ConstantLibrary.RabbitMq.TextSuccessMarker.Status.True;

				Redis.Instance.SetDatabase(Redis.Instance.CalculateDatabase(textId));
				double rank = Double.Parse(Redis.Instance.Database.StringGet($"{ConstantLibrary.Redis.Prefix.Rank}{textId}"));
				Console.WriteLine($"'{ConstantLibrary.Redis.Prefix.Rank}{textId}: {rank}' from redis database({Redis.Instance.Database.Database})");

				++_statistics.TotalTextCount;
				if (isSucceededText)
				{
					++_statistics.HighRankCount;
				}
				_statistics.TotalRank += rank;

				Redis.Instance.SetDatabase(ConstantLibrary.Redis.Statistics.DatabaseId);
				IncreaseStatisticsVersion();
				Redis.Instance.Database.StringSet(ConstantLibrary.Redis.Statistics.TotalTextCount, _statistics.TotalTextCount);
				Console.WriteLine($"'{ConstantLibrary.Redis.Statistics.TotalTextCount}: {_statistics.TotalTextCount}' to redis database({Redis.Instance.Database.Database})");
				Redis.Instance.Database.StringSet(ConstantLibrary.Redis.Statistics.HighRankCount, _statistics.HighRankCount);
				Console.WriteLine($"'{ConstantLibrary.Redis.Statistics.HighRankCount}: {_statistics.HighRankCount}' to redis database({Redis.Instance.Database.Database})");
				Redis.Instance.Database.StringSet(ConstantLibrary.Redis.Statistics.TotalRank, _statistics.TotalRank);
				Console.WriteLine($"'{ConstantLibrary.Redis.Statistics.TotalRank}: {_statistics.TotalRank}' to redis database({Redis.Instance.Database.Database})");

				Console.WriteLine("----------");
			});

			Console.WriteLine("TextStatistics has started");
			Console.WriteLine("Press [enter] to exit.");
			Console.ReadLine();
		}

		private static void LoadStatistics()
		{
			Redis.Instance.SetDatabase(ConstantLibrary.Redis.Statistics.DatabaseId);
			if (Redis.Instance.Database.KeyExists(ConstantLibrary.Redis.Statistics.TotalTextCount))
			{
				_statistics.TotalTextCount = Int32.Parse(Redis.Instance.Database.StringGet(ConstantLibrary.Redis.Statistics.TotalTextCount));
				Console.WriteLine($"'{ConstantLibrary.Redis.Statistics.TotalTextCount}: {_statistics.TotalTextCount}' from redis database({Redis.Instance.Database.Database})");
			}
			if (Redis.Instance.Database.KeyExists(ConstantLibrary.Redis.Statistics.HighRankCount))
			{
				_statistics.HighRankCount = Int32.Parse(Redis.Instance.Database.StringGet(ConstantLibrary.Redis.Statistics.HighRankCount));
				Console.WriteLine($"'{ConstantLibrary.Redis.Statistics.HighRankCount}: {_statistics.HighRankCount}' from redis database({Redis.Instance.Database.Database})");
			}
			if (Redis.Instance.Database.KeyExists(ConstantLibrary.Redis.Statistics.TotalRank))
			{
				_statistics.TotalRank = Double.Parse(Redis.Instance.Database.StringGet(ConstantLibrary.Redis.Statistics.TotalRank), CultureInfo.InvariantCulture);
				Console.WriteLine($"'{ConstantLibrary.Redis.Statistics.TotalRank}: {_statistics.TotalRank}' from redis database({Redis.Instance.Database.Database})");
			}
		}

		private static void IncreaseStatisticsVersion()
		{
			int currentStatisticsVersion = Redis.Instance.Database.KeyExists(ConstantLibrary.Redis.Statistics.Version)
				? Int32.Parse(Redis.Instance.Database.StringGet(ConstantLibrary.Redis.Statistics.Version))
				: 0;
			Redis.Instance.Database.StringSet(ConstantLibrary.Redis.Statistics.Version, currentStatisticsVersion + 1);
			Console.WriteLine($"'{ConstantLibrary.Redis.Statistics.Version}: {currentStatisticsVersion + 1}' to redis database({Redis.Instance.Database.Database})");
		}
	}
}
