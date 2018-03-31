using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RabbitMqLibrary
{
	public static class ExchangeType
	{
		public const string Direct = RabbitMQ.Client.ExchangeType.Direct;
		public const string Fanout = RabbitMQ.Client.ExchangeType.Fanout;
	}

	public class RabbitMq
	{
		private static readonly ConnectionFactory ConnectionFactory =
			new ConnectionFactory { HostName = ConstantLibrary.RabbitMq.ConnectionString };

		private readonly IModel _channel = ConnectionFactory.CreateConnection().CreateModel();

		public string QueueName { get; private set; } = null;

		public void QueueDeclare(string queueName = "")
		{
			QueueName = _channel.QueueDeclare(
				queue: queueName,
				exclusive: false,
				autoDelete: true,
				arguments: null).QueueName;

		}

		public void ConsumeQueue(Action<string> onReceive)
		{
			var consumer = new EventingBasicConsumer(_channel);

			consumer.Received += (model, ea) =>
			{
				byte[] body = ea.Body;
				string message = Encoding.UTF8.GetString(body);
				onReceive(message);
			};

			_channel.BasicConsume(
				queue: QueueName,
				autoAck: true,
				consumer: consumer
			);
		}

		public void ExchangeDeclare(string exchangeName, string exchangeType)
		{
			_channel.ExchangeDeclare(
				exchange: exchangeName,
				type: exchangeType,
				autoDelete: true);
		}

		public void PublishToExchange(string exchangeName, string message)
		{
			_channel.BasicPublish(
				exchange: exchangeName,
				routingKey: "",
				basicProperties: null,
				body: Encoding.UTF8.GetBytes(message));
		}

		public void BindQueueToExchange(string exchangeName)
		{
			_channel.QueueBind(
				queue: QueueName,
				exchange: exchangeName,
				routingKey: "");
		}
	}
}
