using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using StackExchange.Redis;

namespace Backend
{
	public sealed class RabbitMqHelper
	{
		private static readonly string exchangeName = "backend-api";
		private IModel _channel;

		private RabbitMqHelper()
		{
			ConnectionFactory factory = new ConnectionFactory() { HostName = "localhost" };
			IConnection connection = factory.CreateConnection();
			_channel = connection.CreateModel();
			_channel.ExchangeDeclare(exchange: exchangeName, type: "fanout");
		}

		public static RabbitMqHelper Instance { get; } = new RabbitMqHelper();

		public void SendMessage(string message)
		{
			_channel.BasicPublish(
				exchange: exchangeName,
				routingKey: "",
				basicProperties: null,
				body: Encoding.UTF8.GetBytes(message)
			);
		}
	}
}
