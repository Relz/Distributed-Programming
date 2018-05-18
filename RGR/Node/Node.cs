using NetMQ.Sockets;

namespace Node
{
	public class Node
	{
		public Node()
		{
		}

		public Node(string name, string port)
		{
			Name = name;
			Port = port;
		}
		
		public string Name { get; set; }
		public string Port { get; set; }
		public PairSocket Socket { get; set; }
	}
}