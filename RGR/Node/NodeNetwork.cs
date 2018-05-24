using System;
using System.Collections;
using System.Collections.Generic;
using LogFileLibrary;
using ModelLibrary;
using NetMQ;
using NetMQ.Sockets;

namespace Node
{
	public class NodeNetwork : IEnumerable<KeyValuePair<string, NodeModel>>
	{
		private readonly IDictionary<string, NodeModel> _nodes = new Dictionary<string, NodeModel>();
		private readonly LogFile _logFile;

		public NodeNetwork(LogFile logFile)
		{
			_logFile = logFile;
		}

		public IEnumerator<KeyValuePair<string, NodeModel>> GetEnumerator()
		{
			return _nodes.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public void Add(NodeModel nodeModel)
		{
			_nodes.Add(nodeModel.Name, nodeModel);
		}

		public void SendFrame(string message)
		{
			foreach (var (_, nodeModel) in _nodes)
			{
				nodeModel.Socket.SendFrame(message);
			}
		}

		public void Start(NodeModel me)
		{
			StartMyself(me);
			foreach (var (_, nodeModel) in _nodes)
			{
				string logMessage = $"Creating connection tc tcp socket: 127.0.0.1:{nodeModel.Port}";
				try
				{
					nodeModel.Socket = new PushSocket($">tcp://127.0.0.1:{nodeModel.Port}");
				}
				catch (Exception)
				{
					logMessage += " [FAIL]";
					_logFile.AddLine(logMessage);
					continue;
				}

				logMessage += " [OK]";
				_logFile.AddLine(logMessage);
			}
		}

		private void StartMyself(NodeModel me)
		{
			string logMessage = $"Binding to tcp: 127.0.0.1:{me.Port}1";
			try
			{
				me.ManagingSocket = new PairSocket($"@tcp://127.0.0.1:{me.Port}1");
			}
			catch (Exception)
			{
				logMessage += " [FAIL]";
				_logFile.AddLine(logMessage);
				return;
			}
			logMessage += " [OK]";
			_logFile.AddLine(logMessage);
			logMessage = $"Binding to tcp: 127.0.0.1:{me.Port}";
			try
			{
				me.Socket = new RouterSocket($"@tcp://127.0.0.1:{me.Port}");
			}
			catch (Exception)
			{
				logMessage += " [FAIL]";
				_logFile.AddLine(logMessage);
				return;
			}
			logMessage += " [OK]";
			_logFile.AddLine(logMessage);
		}
	}
}