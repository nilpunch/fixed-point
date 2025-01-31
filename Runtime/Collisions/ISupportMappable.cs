namespace Mathematics.Fixed
{
	public interface ISupportMappable
	{
		FVector3 Centre { get; }

		/// <summary>
		/// Returns furthest point of object in some direction.
		/// </summary>
		FVector3 SupportPoint(FVector3 direction);
	}
}
