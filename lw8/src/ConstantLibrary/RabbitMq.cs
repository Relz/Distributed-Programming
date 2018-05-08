namespace ConstantLibrary
{
	public static class RabbitMq
	{
		public const string ConnectionString = "localhost";
		public const char Delimiter = '|';

		public static class ProcessingLimiter
		{
			public static class Status
			{
				public const string True = "True";
				public const string False = "False";
			}
		}

		public static class TextSuccessMarker
		{
			public static class Status
			{
				public const string True = "True";
				public const string False = "False";
			}
		}
	}
}
