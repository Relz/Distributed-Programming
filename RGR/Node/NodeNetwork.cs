using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using NetMQ.Sockets;
using ModelLibrary;

namespace Node
{
	public class NodeNetwork : IEnumerable<KeyValuePair<string, NodeModel>>
	{
		private readonly IDictionary<string, NodeModel> _nodes = new Dictionary<string, NodeModel>();

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

		public void Start(NodeModel me)
		{
			StartMyself(me);
			foreach (var (_, nodeModel) in _nodes)
			{
				Console.Write($"Creating connection tc tcp socket: 127.0.0.1:{nodeModel.Port}");
				try
				{
					nodeModel.Socket = new PairSocket($">tcp://127.0.0.1:{nodeModel.Port}");
				}
				catch (Exception)
				{
					Console.WriteLine(" [FAIL]");
					continue;
				}
				Console.WriteLine(" [OK]");
			}
		}

		private void StartMyself(NodeModel me)
		{
			Console.Write($"Binding to tcp: 127.0.0.1:{me.Port}");
			try
			{
				me.Socket = new PairSocket($"@tcp://127.0.0.1:{me.Port}");
			}
			catch (Exception)
			{
				Console.WriteLine(" [FAIL]");
				return;
			}
			Console.WriteLine(" [OK]");
		}
	}
}