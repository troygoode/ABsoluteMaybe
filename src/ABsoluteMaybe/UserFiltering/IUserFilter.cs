namespace ABsoluteMaybe.UserFiltering
{
	public interface IUserFilter
	{
		bool FilterOut(string userId);
	}
}