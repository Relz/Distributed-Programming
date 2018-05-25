using NetMQ;
using NetMQ.Sockets;

namespace ModelLibrary
{
	public class NodeModel
	{
		public NodeModel()
		{
		}

		public NodeModel(string name, string managingPort, string port)
		{
			Name = name;
			ManagingPort = managingPort;
			NodePort = port;
		}

		public string Name { get; set; }
		public string ManagingPort { get; set; }
		public string NodePort { get; set; }
		public PairSocket ManagingSocket { get; set; }
		public NetMQSocket NodeSocket { get; set; }
	}
}
