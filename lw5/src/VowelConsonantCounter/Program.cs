using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace VowelConsonantCounter
{
	struct VowelConsonant
	{
		public int vowelCount;
		public int consonantCount;
	}

	class Program
	{
		private static readonly string exchangeName = "text-rank-tasks";
		private static readonly string vowelLetters = "aeiouyAEIOUY";

		static void Main()
		{
			var factory = new ConnectionFactory() { HostName = "localhost" };
			using(var connection = factory.CreateConnection())
			using(var channel = connection.CreateModel())
			{
				channel.ExchangeDeclare(exchange: exchangeName, type: "direct");
				string queueName = channel.QueueDeclare().QueueName;
				channel.QueueBind(
					queue: queueName,
					exchange: exchangeName,
					routingKey: ""
				);

				EventingBasicConsumer consumer = new EventingBasicConsumer(channel);
				consumer.Received += (model, ea) =>
				{
					byte[] body = ea.Body;
					string id = Encoding.UTF8.GetString(body);
					string text = RedisHelper.Instance.Get(id);
					VowelConsonant vowelConsonant = CalculateVowelConsonant(text);
					Console.WriteLine($"{id}|{vowelConsonant.vowelCount}|{vowelConsonant.consonantCount} to vowel-cons-counter queue");
					RabbitMqHelper.Instance.SendMessage($"{id}|{vowelConsonant.vowelCount}|{vowelConsonant.consonantCount}");
				};
				channel.BasicConsume(
					queue: queueName,
					autoAck: true,
					consumer: consumer
				);

				Console.WriteLine("VowelConsonantCounter has started");
				Console.WriteLine("Press [enter] to exit.");
				Console.ReadLine();
			}
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
