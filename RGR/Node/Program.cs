using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using ModelLibrary;
using NetMQ;

namespace Node
{
	internal static class Program
	{
		private const string ConfigFileName = "../config.json";
		
		private static NodeModel me = new NodeModel();
		private static readonly NodeNetwork NodeNetwork = new NodeNetwork();
		private static readonly IDictionary<string, IList<int>> Services = new Dictionary<string, IList<int>>();

		private static void Main(string[] args)
		{
			if (!GetNodeNameFromArguments(args))
			{
				Console.WriteLine("Error: Node name not specified");
				return;
			}
			
			ReadConfig();
			NodeNetwork.Start(me);
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
				else
				{
					NodeNetwork.Add(new NodeModel(name, port));
				}
			}
		}

		private static void ServerActivity()
		{
			Console.WriteLine("Waiting for message...");
			bool signal = me.Socket.ReceiveSignal();
			WriteMessageLine("Handshake");
			me.Socket.SignalOK();
			while (true)
			{
				string message = me.Socket.ReceiveFrameString();
				WriteMessageLine($"Received message: {message}");
				me.Socket.SendFrame(message);
			}
		}

		private static void ClientActivity(NodeModel node)
		{
		}
	}
}