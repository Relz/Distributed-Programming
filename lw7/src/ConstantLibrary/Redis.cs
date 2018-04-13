namespace ConstantLibrary
{
	public static class Redis
	{
		public const string ConnectionString = "localhost";
		public const int RankCalcDatabaseCount = 10;
		public const int StatisticsDatabaseId = 11;

		public static class Prefix
		{
			public const string Text = "text_";
			public const string Count = "count_";
			public const string Rank = "rank_";

			public static class Statistics
			{
				public const string TotalTextCount = "total_text_count";
				public const string HighRankCount = "high_rank_count";
				public const string TotalRank = "total_rank";
			}
		}
	}
}
