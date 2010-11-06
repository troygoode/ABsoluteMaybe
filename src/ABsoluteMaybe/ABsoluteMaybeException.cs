using System;

namespace ABsoluteMaybe
{
	public abstract class ABsoluteMaybeException : Exception
	{
		protected ABsoluteMaybeException(string message) : base(message)
		{
		}
	}
}