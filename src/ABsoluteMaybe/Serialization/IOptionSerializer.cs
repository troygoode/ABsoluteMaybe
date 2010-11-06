namespace ABsoluteMaybe.Serialization
{
	public interface IOptionSerializer
	{
		string Serialize<T>(T option);
	}
}