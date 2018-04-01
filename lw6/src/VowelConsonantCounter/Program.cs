using System;
using RabbitMqLibrary;
using RedisLibrary;

namespace VowelConsonantCounter
{
	struct VowelConsonant
	{
		public int vowelCount;
		public int consonantCount;
	}

	class Program
	{
		private const string queueName = "vowel-cons-counter";
		private const string exchangeName = "text-rank-tasks";
		private const string vowelLetters = "aeiouyAEIOUY";

		static void Main()
		{
			var rabbitMq = new RabbitMq();
			rabbitMq.QueueDeclare(queueName);
			rabbitMq.ExchangeDeclare(exchangeName, ExchangeType.Direct);
			rabbitMq.BindQueueToExchange(exchangeName);
			rabbitMq.ConsumeQueue(textId =>
			{
				Redis.Instance.SetDatabase(Redis.Instance.CalculateDatabase(textId));
				string text = Redis.Instance.Database.StringGet($"{ConstantLibrary.Redis.Prefix.Text}{textId}");
				Console.WriteLine($"'{ConstantLibrary.Redis.Prefix.Text}{textId}: {text}' from redis database({Redis.Instance.Database.Database})");
				VowelConsonant vowelConsonant = CalculateVowelConsonant(text);
				Redis.Instance.Database.StringSet($"{ConstantLibrary.Redis.Prefix.Count}{textId}", $"{vowelConsonant.vowelCount}|{vowelConsonant.consonantCount}");
				Console.WriteLine($"'{ConstantLibrary.Redis.Prefix.Count}{textId}: {vowelConsonant.vowelCount}|{vowelConsonant.consonantCount}' to redis database({Redis.Instance.Database.Database})");
				Console.WriteLine($"{textId} to vowel-cons-counter queue");
				rabbitMq.PublishToExchange("vowel-cons-counter", textId);
				Console.WriteLine("----------");
			});

			Console.WriteLine("VowelConsonantCounter has started");
			Console.WriteLine("Press [enter] to exit.");
			Console.ReadLine();
		}

		private static VowelConsonant CalculateVowelConsonant(string text)
		{
			VowelConsonant result = new VowelConsonant();
			foreach (char letter in text)
			{
				if (vowelLetters.IndexOf(letter) >= 0)
				{
					++result.vowelCount;
				}
				else
				{
					++result.consonantCount;
				}
			}
			return result;
		}
	}
}
