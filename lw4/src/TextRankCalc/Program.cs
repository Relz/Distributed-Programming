using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace TextRankCalc
{
	class Program
	{
		private static string vowelLetters = "aeiouAEIOU";
		
		static void Main()
		{
			var factory = new ConnectionFactory() { HostName = "localhost" };
			using(var connection = factory.CreateConnection())
			using(var channel = connection.CreateModel())
			{
				channel.QueueDeclare(
					queue: "booking-api",
					durable: false,
					exclusive: false,
					autoDelete: false,
					arguments: null
				);

				EventingBasicConsumer consumer = new EventingBasicConsumer(channel);
				consumer.Received += (model, ea) =>
				{
					byte[] body = ea.Body;
					string message = Encoding.UTF8.GetString(body);
					string text = RedisHelper.Instance.Get(message);
					RedisHelper.Instance.Set(text, CalculateRank(text));
				};
				channel.BasicConsume(
					queue: "booking-api",
					autoAck: true,
					consumer: consumer
				);

				Console.WriteLine(" Press [enter] to exit.");
				Console.ReadLine();
			}
		}

		private static string CalculateRank(string text)
		{
			uint vowelLetterCount = 0;
			uint consonantLetterCount = 0;
			foreach (char letter in text)
			{
				if (vowelLetters.IndexOf(letter) >= 0)
				{
					++vowelLetterCount;
				}
				else
				{
					++consonantLetterCount;
				}
			}
			return consonantLetterCount == 0 ? "Infinity" : ((double)vowelLetterCount / consonantLetterCount).ToString();
		}
	}
}
