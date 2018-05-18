using System.Collections;
using System.Collections.Generic;
using NetMQ.Sockets;

namespace Node
{
	public class NodeNetwork : IEnumerable<KeyValuePair<string, Node>>
	{
		private readonly IDictionary<string, Node> _nodes = new Dictionary<string, Node>();

		public IEnumerator<KeyValuePair<string, Node>> GetEnumerator()
		{
			return _nodes.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public void Add(Node node)
		{
			_nodes.Add(node.Name, node);
		}

		public bool TryGetValue(string name, out Node node)
		{
			return _nodes.TryGetValue(name, out node);
		}

		public void Start()
		{
			foreach (var (_, node) in _nodes)
			{
				node.Socket = new PairSocket($">tcp://127.0.0.1:{node.Port},@tcp://127.0.0.1:{node.Port}");
			}
		}
	}
}