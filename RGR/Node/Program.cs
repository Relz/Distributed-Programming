using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetMQ;
using NetMQ.Sockets;
using Newtonsoft.Json.Linq;

namespace Node
{
	internal static class Program
	{
		private const string ConfigFileName = "config.json";
		
		private static Node me = new Node();
		private static readonly NodeNetwork NodeNetwork = new NodeNetwork();
		private static readonly IDictionary<string, IList<int>> Services = new Dictionary<string, IList<int>>();

		private static void Main(string[] args)
		{
			if (!GetNodeNameFromArguments(args))
			{
				Console.WriteLine("Error: Node name not specified");
				return;
			}
			
			NodeNetwork.Add(me);

			ReadConfig();
			NodeNetwork.Start();
			Task.Factory.StartNew(state => ServerActivity(), string.Format($"Server {me.Name}"), TaskCreationOptions.LongRunning);
			
			foreach (var (_, node) in NodeNetwork)
			{
				Task.Factory.StartNew(state => ClientActivity(node), string.Format($"Client {node.Name}"), TaskCreationOptions.LongRunning);
			}
			
			Console.ReadKey();
		}

		private static void WriteMessage(string message)
		{
			Console.Write($"Node {me.Name}({me.Port}): {message}");
		}

		private static void WriteMessageLine(string message)
		{
			WriteMessage(message);
			Console.WriteLine();
		}

		private static bool GetNodeNameFromArguments(string[] args)
		{
			if (args.Length < 1)
			{
				return false;
			}
			me.Name = args.ElementAt(0);
			return true;
		}

		private static void ReadConfig()
		{
			JObject config = JObject.Parse(File.ReadAllText(ConfigFileName));
			foreach (var (name, portToken) in config)
			{
				var port = portToken.Value<string>();
				if (name == me.Name)
				{
					me.Port = port;
				}
				{
					NodeNetwork.Add(new Node(name, port));
				}
			}
		}

		private static void ServerActivity()
		{
			while (true)
			{
				string message = me.Socket.ReceiveFrameString();
				WriteMessageLine($"Received: {message}");
			}
		}

		private static void ClientActivity(Node node)
		{
		}
	}
}