using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace VowelConsonantRater
{
	struct VowelConsonant
	{
		public int vowelCount;
		public int consonantCount;
	}

	class Program
	{
		private static readonly string exchangeName = "vowel-cons-counter";

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
					string[] data = Encoding.UTF8.GetString(body).Split('|');
					int vowelCount;
					int consonantCount;
					if (Int32.TryParse(data[1], out vowelCount) && Int32.TryParse(data[2], out consonantCount)) {
						RedisHelper.Instance.Set(data[0], CalculateRate(vowelCount, consonantCount));
					}
				};
				channel.BasicConsume(
					queue: queueName,
					autoAck: true,
					consumer: consumer
				);

				Console.WriteLine(" Press [enter] to exit.");
				Console.ReadLine();
			}
		}

		private static string CalculateRate(int vowelCount, int consonantCount)
		{
			return consonantCount == 0 ? "Infinity" : ((double)vowelCount / consonantCount).ToString();
		}
	}
}
