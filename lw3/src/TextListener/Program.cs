using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace TextListener
{
	class Program
	{
		static void Main()
		{
			var factory = new ConnectionFactory() { HostName = "localhost" };
			using(var connection = factory.CreateConnection())
			using(var channel = connection.CreateModel())
			{
				channel.QueueDeclare(
					queue: "backend-api",
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
					Console.WriteLine($"{message}: {RedisHelper.Instance.Get(message)}");
				};
				channel.BasicConsume(
					queue: "backend-api",
					autoAck: true,
					consumer: consumer
				);

				Console.WriteLine(" Press [enter] to exit.");
				Console.ReadLine();
			}
		}
	}
}
