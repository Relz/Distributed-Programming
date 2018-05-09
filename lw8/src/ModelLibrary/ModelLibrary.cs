namespace ModelLibrary
{
	public class Statistics
	{
		public int TotalTextCount {get; set;}
		public int HighRankCount {get; set;}
		public double TotalRank {get; set;}
	}

	public class TextDetails
	{
		public string Id {get; set;}
		public string Status {get; set;}
		public string Rank {get; set;}
	}
}
