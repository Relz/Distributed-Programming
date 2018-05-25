using System;
using System.Globalization;
using System.IO;

namespace LogFileLibrary
{
	public class LogFile
	{
		private readonly StreamWriter _streamWriter;

		public LogFile(string fileName)
		{
			_streamWriter = new StreamWriter(fileName);
		}

		public void AddLine(string line)
		{
			// _streamWriter.WriteLine($"{DateTime.Now.ToString(CultureInfo.InvariantCulture)}|{line}");
			// _streamWriter.Flush();
			Console.WriteLine(line);
		}
	}
}
