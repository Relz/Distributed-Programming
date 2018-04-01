using System;
using RabbitMqLibrary;
using RedisLibrary;

namespace VowelConsonantRater
{
	class Program
	{
		private const string queueName = "vowel-cons-rater";
		private const string exchangeName = "vowel-cons-counter";

		static void Main()
		{
			var rabbitMq = new RabbitMq();
			rabbitMq.QueueDeclare(queueName);
			rabbitMq.ExchangeDeclare(exchangeName, ExchangeType.Direct);
			rabbitMq.BindQueueToExchange(exchangeName);
			rabbitMq.ConsumeQueue(countId =>
			{
				string[] countData = Redis.Instance.Database.StringGet(countId).ToString().Split('|');
				string text = Redis.Instance.Database.StringGet(countData[0]);
				int vowelCount;
				int consonantCount;
				if (Int32.TryParse(countData[1], out vowelCount) && Int32.TryParse(countData[2], out consonantCount))
				{
					string rate = CalculateRate(vowelCount, consonantCount);
					Console.WriteLine($"'{text}: {rate}' to redis");
					Redis.Instance.Database.StringSet(text, rate);
				}
			});

			Console.WriteLine("VowelConsonantRater has started");
			Console.WriteLine("Press [enter] to exit.");
			Console.ReadLine();
		}

		private static string CalculateRate(int vowelCount, int consonantCount)
		{
			return consonantCount == 0 ? "Infinity" : ((double)vowelCount / consonantCount).ToString();
		}
	}
}
