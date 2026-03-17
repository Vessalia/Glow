namespace Assets.Scripts
{
	public interface IIlluminator
	{
		void Illuminate(IIlluminatable other);
		void DeIlluminate(IIlluminatable other);
	}
}
