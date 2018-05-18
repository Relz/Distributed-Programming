using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ModelLibrary;
using NetMQ;
using NetMQ.Sockets;
using Newtonsoft.Json.Linq;

namespace NodeManager
{
	internal static class Program
	{
		private const string ConfigFileName = "../config.json";
		
		private static readonly List<NodeModel> Nodes = new List<NodeModel>();
		
		private static void Main()
		{
			ReadConfig();
			WriteNodes();
			int nodeSerialNumber = AskNodeSerialNumberToConnect();
			NodeModel nodeModel = Nodes.ElementAt(nodeSerialNumber - 1);
			Console.Write($"Creating connection tc tcp socket: 127.0.0.1:{nodeModel.Port}");
			try
			{
				nodeModel.Socket = new PairSocket($"@tcp://127.0.0.1:{nodeModel.Port}");
			}
			catch (Exception)
			{
				Console.WriteLine(" [FAIL]");
				return;
			}
			Console.WriteLine(" [OK]");

			Console.Write("Handshake");
			if (!DoesHandshakeSucceeded(nodeModel))
			{
				Console.WriteLine(" [FAIL]");
				return;
			}
			Console.WriteLine(" [OK]");

			CommandValidator commandValidator = new CommandValidator();
			InitializeCommandValidator(commandValidator);
			
			CommunicateWithNode(nodeModel, commandValidator);
		}

		private static void ReadConfig()
		{
			JObject config = JObject.Parse(File.ReadAllText(ConfigFileName));
			foreach (var (name, portToken) in config)
			{
				var port = portToken.Value<string>();
				Nodes.Add(new NodeModel(name, port));
			}
		}

		private static void WriteNodes()
		{
			Console.WriteLine("Available nodes:");
			for (var i = 0; i < Nodes.Count; ++i)
			{
				NodeModel nodeModel = Nodes.ElementAt(i);
				Console.WriteLine($"{(i + 1).ToString()}. {nodeModel.Name}({nodeModel.Port})");
			}
		}

		private static int AskNodeSerialNumberToConnect()
		{
			int serialNumber;
			do
			{
				Console.Write("Type node serial number to connect: ");
			} while (!int.TryParse(Console.ReadLine(), out serialNumber) || serialNumber < 1 || serialNumber >= Nodes.Count);

			return serialNumber;
		}

		private static bool DoesHandshakeSucceeded(NodeModel nodeModel)
		{
			nodeModel.Socket.SignalOK();
			return nodeModel.Socket.ReceiveSignal();
		}

		private static void InitializeCommandValidator(CommandValidator commandValidator)
		{
			commandValidator.Add("WHERE", 1, "Command signature: WHERE <service_name>");
			commandValidator.Add("START", 2, "Command signature: START <service_name> <port>");
			commandValidator.Add("STOP", 2, "Command signature: STOP <service_name> <port>");
			commandValidator.Add("BYE", 0, "Command signature: BYE");
		}

		private static void CommunicateWithNode(NodeModel nodeModel, CommandValidator commandValidator)
		{
			string commandString = "";
			while (commandString != "BYE")
			{
				Console.Write("> ");
				commandString = Console.ReadLine();
				if (!commandValidator.IsValid(commandString))
				{
					commandValidator.WriteHelpForCommand(commandString);
					continue;
				}
				nodeModel.Socket.SendFrame(commandString);
				Console.WriteLine(nodeModel.Socket.ReceiveFrameString());
			}
		}
	}
}