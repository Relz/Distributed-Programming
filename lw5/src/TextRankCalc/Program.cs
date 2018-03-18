using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace TextRankCalc
{
	class Program
	{
		private static readonly string exchangeName = "backend-api";

		static void Main()
		{
			var factory = new ConnectionFactory() { HostName = "localhost" };
			using(var connection = factory.CreateConnection())
			using(var channel = connection.CreateModel())
			{
				channel.ExchangeDeclare(exchange: exchangeName, type: "fanout");
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
					Console.WriteLine($"{id} to text-rank-tasks queue");
					RabbitMqHelper.Instance.SendMessage(id);
				};
				channel.BasicConsume(
					queue: queueName,
					autoAck: true,
					consumer: consumer
				);

				Console.WriteLine("TextRankCalc has started");
				Console.WriteLine("Press [enter] to exit.");
				Console.ReadLine();
			}
		}
	}
}
