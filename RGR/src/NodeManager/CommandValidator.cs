using System;
using System.Collections.Generic;
using System.Linq;

namespace NodeManager
{
	public class CommandValidator
	{
		private readonly IDictionary<string, int> _commandArgumentCount = new Dictionary<string, int>();
		private readonly IDictionary<string, string> _commandHelp = new Dictionary<string, string>();
		
		public void Add(string name, int commandArgumentCount, string help)
		{
			_commandArgumentCount.Add(name, commandArgumentCount);
			_commandHelp.Add(name, help);
		}

		public bool IsValid(string commandString)
		{
			string[] commandArguments = commandString.Split(' ');
			if (commandArguments.Length == 0)
			{
				return false;
			}
			string commandName = commandArguments.ElementAt(0);
			if (!_commandArgumentCount.ContainsKey(commandName) || commandArguments.Length - 1 != _commandArgumentCount[commandName])
			{
				return false;
			}

			return true;
		}

		public void WriteHelpForCommand(string commandString)
		{
			string[] commandArguments = commandString.Split(' ');
			if (commandArguments.Length == 0)
			{
				Console.WriteLine($"It's not seems to be a command: \"{commandString}\"");
				return;
			}
			string commandName = commandArguments.ElementAt(0);
			if (!_commandHelp.ContainsKey(commandName))
			{
				Console.WriteLine($"Unknown command: \"{commandString}\"");
				return;
			}
			Console.WriteLine(_commandHelp[commandName]);
		}
	}
}