
namespace Assets.Scripts
{
	public interface IInteractable
	{
		string Description { get;}
		void OnInteract();
	}

	public interface IActivateable
	{
		void Activate();
		void Deactivate();
	}
}
