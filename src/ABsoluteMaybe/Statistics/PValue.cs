using System;

namespace ABsoluteMaybe.Statistics
{
	public class PValue
	{
		private static readonly double[,] ZScores = {{0.10, 1.29}, {0.05, 1.65}, {0.01, 2.33}, {0.001, 3.08}};
		private readonly double _zScore;

		public PValue(double zScore)
		{
			_zScore = zScore;
		}

		public double Execute()
		{
			var z = Math.Abs(_zScore);

			for (var a = 0; a < ZScores.Length/2; a++)
				if (z > ZScores[a, 1])
					return ZScores[a, 0];

			return 1;
		}
	}
}