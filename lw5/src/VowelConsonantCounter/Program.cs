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
				string text = Redis.Instance.Database.StringGet(textId);
				VowelConsonant vowelConsonant = CalculateVowelConsonant(text);
				string countId = Guid.NewGuid().ToString();
				Console.WriteLine($"'{countId}: {textId}|{vowelConsonant.vowelCount}|{vowelConsonant.consonantCount}' to redis");
				Redis.Instance.Database.StringSet(countId, $"{textId}|{vowelConsonant.vowelCount}|{vowelConsonant.consonantCount}");
				Console.WriteLine($"{countId} to vowel-cons-counter queue");
				rabbitMq.PublishToExchange("vowel-cons-counter", countId);
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
