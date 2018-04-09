namespace ConstantLibrary
{
	public static class Redis
	{
		public const string ConnectionString = "localhost";
		public const int DatabaseCount = 15;

		public static class Prefix
		{
			public const string Text = "text_";
			public const string Count = "count_";
			public const string Rank = "rank_";
		}
	}
}
