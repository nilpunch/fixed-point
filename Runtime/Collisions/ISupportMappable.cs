namespace Mathematics.Fixed
{
	public interface ISupportMappable
	{
		FVector3 Center { get; }

		/// <summary>
		/// Returns furthest point of object in some direction.
		/// </summary>
		FVector3 SupportPoint(FVector3 direction);
	}
}
