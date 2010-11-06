namespace ABsoluteMaybe.Serialization
{
	public class ToStringOptionSerializer : IOptionSerializer
	{
		#region IOptionSerializer Members

		public string Serialize<T>(T option)
		{
			return option.ToString();
		}

		#endregion
	}
}