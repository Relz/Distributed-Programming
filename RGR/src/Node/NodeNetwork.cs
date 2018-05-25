using NetMQ;
using System.Collections;
using System.Collections.Generic;
using ModelLibrary;
using System;

namespace Node
{
	public class NodeNetwork : IEnumerable<KeyValuePair<string, NodeModel>>
	{
		private readonly IDictionary<string, NodeModel> _nodes = new Dictionary<string, NodeModel>();
		private readonly LogFileLibrary.LogFile _logFile;

		public NodeNetwork(LogFileLibrary.LogFile logFile)
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
				nodeModel.NodeSocket.SendFrame(message);
			}
		}

		public void Start(NodeModel me)
		{
			StartMyself(me);
			foreach (var (_, nodeModel) in _nodes)
			{
				string logMessage = $"Creating connection tc tcp socket: 127.0.0.1:{nodeModel.NodePort}";
				try
				{
					nodeModel.NodeSocket = new NetMQ.Sockets.PushSocket($">tcp://127.0.0.1:{nodeModel.NodePort}");
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
			string logMessage = $"Binding to tcp: 127.0.0.1:{me.ManagingPort}";
			try
			{
				me.ManagingSocket = new NetMQ.Sockets.PairSocket($"@tcp://127.0.0.1:{me.ManagingPort}");
			}
			catch (Exception)
			{
				logMessage += " [FAIL]";
				_logFile.AddLine(logMessage);
				return;
			}
			logMessage += " [OK]";
			_logFile.AddLine(logMessage);
			logMessage = $"Binding to tcp: 127.0.0.1:{me.NodePort}";
			try
			{
				me.NodeSocket = new NetMQ.Sockets.RouterSocket($"@tcp://127.0.0.1:{me.NodePort}");
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
