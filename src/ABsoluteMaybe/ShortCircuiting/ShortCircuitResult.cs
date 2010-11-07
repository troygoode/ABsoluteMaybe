namespace ABsoluteMaybe.ShortCircuiting
{
	public class ShortCircuitResult
	{
		public ShortCircuitResult(bool shouldShortCircuitRequest, string shortCircuitTo)
		{
			ShouldShortCircuitRequest = shouldShortCircuitRequest;
			ShortCircuitTo = shortCircuitTo;
		}

		public bool ShouldShortCircuitRequest { get; set; }
		public string ShortCircuitTo { get; set; }
	}
}