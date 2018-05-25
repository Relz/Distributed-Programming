using System.Linq;
using NetMQ;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using ModelLibrary;

namespace Node
{
	internal static class Program
	{
		private const string ConfigFileName = "../config.json";
		private const string Success = "Success";
		private const string Failure = "Failure";

		private static NodeModel me = new NodeModel();
		private static NodeNetwork _nodeNetwork;
		private static readonly IDictionary<string, ISet<int>> Services = new Dictionary<string, ISet<int>>();
		private static LogFileLibrary.LogFile _logFile;

		private static void Main(string[] args)
		{
			if (!GetNodeNameFromArguments(args))
			{
				System.Console.WriteLine("Error: Node name not specified");
				return;
			}

			_logFile = new LogFileLibrary.LogFile($"{me.Name}.log");
			_nodeNetwork = new NodeNetwork(_logFile);
			ReadConfig();
			_nodeNetwork.Start(me);
			System.Threading.Tasks.Task.Factory.StartNew(state => ServerActivity(), string.Format($"Server {me.Name}"), System.Threading.Tasks.TaskCreationOptions.LongRunning);
			System.Threading.Tasks.Task.Factory.StartNew(state => NodeActivity(), string.Format($"Node {me.Name}"), System.Threading.Tasks.TaskCreationOptions.LongRunning);

			System.Console.ReadKey();
		}

		private static bool GetNodeNameFromArguments(string[] args)
		{
			if (args.Length < 1)
			{
				return false;
			}
			me.Name = Enumerable.ElementAt(args, 0);
			return true;
		}

		private static void ReadConfig()
		{
			JObject config = JObject.Parse(System.IO.File.ReadAllText(ConfigFileName));
			foreach (var (name, portsToken) in config)
			{
				string managingPort = portsToken.SelectToken("ManagerPort").Value<string>();
				string nodePort = portsToken.SelectToken("NodePort").Value<string>();
				if (name == me.Name)
				{
					me.ManagingPort = managingPort;
					me.NodePort = nodePort;
				}
				else
				{
					_nodeNetwork.Add(new NodeModel(name, managingPort, nodePort));
				}
			}
		}

		private static void ServerActivity()
		{
			while (true)
			{
				string message = me.ManagingSocket.ReceiveFrameString();
				_logFile.AddLine($"Received message from Manager: {message}");
				string[] command = message.Split(' ');
				switch (command[0])
				{
					case "PING":
						me.ManagingSocket.SendFrameEmpty();
						break;
					case "BYE":
						me.ManagingSocket.SendFrameEmpty();
						break;
					case "WHERE":
						Services.TryGetValue(command[1], out var serviceAddresses);
						me.ManagingSocket.SendFrame(serviceAddresses == null ? "" : string.Join(", ", serviceAddresses));
						break;
					case "START":
						_nodeNetwork.SendFrame(message);
						CollectionExtensions.TryAdd(Services, command[1], new HashSet<int>());
						me.ManagingSocket.SendFrame(int.TryParse(command[2], out var port) && Services[command[1]].Add(port) ? Success : Failure);
						break;
					case "STOP":
						_nodeNetwork.SendFrame(message);
						me.ManagingSocket.SendFrame(Services.ContainsKey(command[1]) && Services[command[1]].Remove(int.Parse(command[2])) ? Success : Failure);
						break;
				}
			}
		}

		private static void NodeActivity()
		{
			while (true)
			{
				string message = me.NodeSocket.ReceiveMultipartStrings().ElementAt(1);
				_logFile.AddLine($"Received message from Node: {message}");
				string[] command = message.Split(' ');
				switch (command[0])
				{
					case "START":
						CollectionExtensions.TryAdd(Services, command[1], new HashSet<int>());
						int.TryParse(command[2], out var port);
						Services[command[1]].Add(port);
						break;
					case "STOP":
						if (Services.ContainsKey(command[1]))
						{
							Services[command[1]].Remove(int.Parse(command[2]));
						}
						break;
				}
			}
		}
	}
}
