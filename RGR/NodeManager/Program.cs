using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
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
			while (true)
			{
				Console.Write("Type node serial number to connect: ");
				string command = Console.ReadLine();
				if (command == "EXIT")
				{
					break;
				}
				if (!int.TryParse(command, out var nodeSerialNumber) || nodeSerialNumber < 1 || nodeSerialNumber > Nodes.Count)
				{
					continue;
				}

				NodeModel nodeModel = Nodes.ElementAt(nodeSerialNumber - 1);
				if (nodeModel.ManagingSocket == null)
				{
					Console.Write($"Creating connection to tcp socket: 127.0.0.1:{nodeModel.Port}1");
					try
					{
						nodeModel.ManagingSocket = new PairSocket($">tcp://127.0.0.1:{nodeModel.Port}1");
					}
					catch (Exception e)
					{
						Console.WriteLine(" [FAIL]");
						Console.WriteLine(e.Message);
						continue;
					}
					Console.WriteLine(" [OK]");
				}

				Console.Write("Handshake");
				if (!DoesHandshakeSucceeded(nodeModel))
				{
					Console.WriteLine(" [FAIL]");
					continue;
				}

				Console.WriteLine(" [OK]");

				CommandValidator commandValidator = new CommandValidator();
				InitializeCommandValidator(commandValidator);

				CommunicateWithNode(nodeModel, commandValidator);
			}
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

		private static bool DoesHandshakeSucceeded(NodeModel nodeModel)
		{
			return nodeModel.ManagingSocket.TrySendFrame(TimeSpan.FromSeconds(3), "PING") && nodeModel.ManagingSocket.TryReceiveFrameString(TimeSpan.FromSeconds(3), out _);
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

				nodeModel.ManagingSocket.SendFrame(commandString);
				Console.WriteLine(nodeModel.ManagingSocket.ReceiveFrameString());
			}
		}
	}
}