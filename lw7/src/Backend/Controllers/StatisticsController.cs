using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ModelLibrary;
using RabbitMqLibrary;
using RedisLibrary;

namespace Backend.Controllers
{
	[Route("api/[controller]")]
	public class StatisticsController : Controller
	{
		private static int _statisticsVersion = 0;
		private static Statistics _statistics = new Statistics();

		public StatisticsController()
		{
			Redis.Instance.SetDatabase(ConstantLibrary.Redis.Statistics.DatabaseId);
		}

		[HttpGet]
		public Statistics Get()
		{
			int statisticsVersion = GetStatisticsVersion();
			Console.WriteLine($"Cached statistics version: {_statisticsVersion}");
			Console.WriteLine($"Actual statistics version: {statisticsVersion}");
			if (_statisticsVersion != statisticsVersion)
			{
				_statisticsVersion = statisticsVersion;
				UpdateStatistics();
			}
			return _statistics;
		}

		private static int GetStatisticsVersion()
		{
			return Redis.Instance.Database.KeyExists(ConstantLibrary.Redis.Statistics.Version)
				? Int32.Parse(Redis.Instance.Database.StringGet(ConstantLibrary.Redis.Statistics.Version))
				: 0;
		}

		private void UpdateStatistics()
		{
			_statistics.TotalTextCount = Int32.Parse(Redis.Instance.Database.StringGet(ConstantLibrary.Redis.Statistics.TotalTextCount));
			Console.WriteLine($"'{ConstantLibrary.Redis.Statistics.TotalTextCount}: {_statistics.TotalTextCount}' from redis database({Redis.Instance.Database.Database})");
			_statistics.HighRankCount = Int32.Parse(Redis.Instance.Database.StringGet(ConstantLibrary.Redis.Statistics.HighRankCount));
			Console.WriteLine($"'{ConstantLibrary.Redis.Statistics.HighRankCount}: {_statistics.HighRankCount}' from redis database({Redis.Instance.Database.Database})");
			_statistics.TotalRank = Double.Parse(Redis.Instance.Database.StringGet(ConstantLibrary.Redis.Statistics.TotalRank));
			Console.WriteLine($"'{ConstantLibrary.Redis.Statistics.TotalRank}: {_statistics.TotalRank}' from redis database({Redis.Instance.Database.Database})");
		}
	}
}
