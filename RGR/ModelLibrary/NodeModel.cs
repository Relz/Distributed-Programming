﻿using NetMQ;
using NetMQ.Sockets;

namespace ModelLibrary
{
	public class NodeModel
	{
		public NodeModel()
		{
		}

		public NodeModel(string name, string port)
		{
			Name = name;
			Port = port;
		}
		
		public string Name { get; set; }
		public string Port { get; set; }
		public PairSocket ManagingSocket { get; set; }
		public NetMQSocket Socket { get; set; }
	}
}