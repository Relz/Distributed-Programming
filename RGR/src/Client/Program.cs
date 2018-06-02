using System;
using System.Text;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Collections.Generic;
using ModelLibrary;
using System.Linq;
using NetMQ;
using NetMQ.Sockets;

namespace Client
{
	static class Program
	{
		private const string ConfigFileName = "../config.json";

		private static readonly List<NodeModel> Nodes = new List<NodeModel>();
		private static IDictionary<string, ISet<int>> _services = new Dictionary<string, ISet<int>>();

		private static void Main()
		{
			ReadConfig();

			NodeModel nodeModel = Nodes.ElementAt(0);

			if (nodeModel.ManagingSocket == null)
			{
				Console.Write($"Creating connection to tcp socket: 127.0.0.1:{nodeModel.ManagingPort}");
				try
				{
					nodeModel.ManagingSocket = new PairSocket($">tcp://127.0.0.1:{nodeModel.ManagingPort}");
				}
				catch (Exception e)
				{
					Console.WriteLine(" [FAIL]");
					Console.WriteLine(e.Message);
					return;
				}

				Console.WriteLine(" [OK]");
			}

			Console.Write("Handshake");

			if (!DoesHandshakeSucceeded(nodeModel))
			{
				Console.WriteLine(" [FAIL]");
				return;
			}

			Console.WriteLine(" [OK]");

			WriteServices(nodeModel);

			while (true)
			{
				Console.Write("Enter a <port> <arguments>");
				string commandString = Console.ReadLine();

				if (commandString == "EXIT")
				{
					break;
				}

				string[] command = commandString.Split(" ");

				IList<KeyValuePair<string, string>> arguments = ParseArguments(command[1]);

				string jsonData = "";
				foreach (KeyValuePair<string, string> argument in arguments)
				{
					jsonData += $"\"{argument.Key}\": \"{argument.Value}\",";
				}

				jsonData = jsonData.Remove(jsonData.Length - 1);
				jsonData = "{" + jsonData + "}";

				string url = $"http://127.0.0.1:{command[0]}/api/values";
				StringContent stringContent = new StringContent(
																 jsonData,
																 Encoding.UTF8,
																 "application/json");
				using (HttpClient httpClient = new HttpClient())
				{
					httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/plain"));

					using (HttpResponseMessage response = httpClient.PostAsync(url, stringContent).Result)
					using (HttpContent content = response.Content)
					{
						Console.WriteLine(content.ReadAsStringAsync().Result);
					}
				}
			}
		}

		private static void ReadConfig()
		{
			JObject config = JObject.Parse(System.IO.File.ReadAllText(ConfigFileName));

			foreach (var (name, portsToken) in config)
			{
				string managingPort = portsToken.SelectToken("ManagerPort").Value<string>();
				string nodePort = portsToken.SelectToken("NodePort").Value<string>();
				Nodes.Add(new NodeModel(name, managingPort, nodePort));
			}
		}

		private static bool DoesHandshakeSucceeded(NodeModel nodeModel)
		{
			return nodeModel.ManagingSocket.TrySendFrame(TimeSpan.FromSeconds(3), message: "PING")
					&& nodeModel.ManagingSocket.TryReceiveFrameString(TimeSpan.FromSeconds(3), out _);
		}

		private static void WriteServices(NodeModel nodeModel)
		{
			nodeModel.ManagingSocket.SendFrame("WHERE_ALL");
			_services = JsonConvert.DeserializeObject<IDictionary<string, ISet<int>>>(nodeModel.ManagingSocket.ReceiveFrameString());

			Console.WriteLine("Available services:");

			foreach (KeyValuePair<string, ISet<int>> service in _services)
			{
				string serviceName = service.Key;
				ISet<int> servicePorts = service.Value;
				Console.WriteLine($"{serviceName}: {string.Join(", ", servicePorts)}");
			}
		}

		private static IList<KeyValuePair<string, string>> ParseArguments(string argumentsString)
		{
			IList<KeyValuePair<string, string>> arguments = new List<KeyValuePair<string, string>>();

			string[] argumentsStrings = argumentsString.Split(";");
			foreach (string argumentString in argumentsStrings)
			{
				string[] argumentStrings = argumentString.Split("=");
				arguments.Add(new KeyValuePair<string, string>(argumentStrings[0], argumentStrings[1]));
			}

			return arguments;
		}
	}
}
