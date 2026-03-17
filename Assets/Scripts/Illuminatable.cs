using UnityEngine;

namespace Assets.Scripts
{
	[RequireComponent (typeof (Collider))]
	public class Illuminatable : MonoBehaviour, IIlluminatable
	{
		[SerializeField] private InterfaceReference<IActivateable> _activateable;

		private int _litCounter = 0;
		private bool IsLit => _litCounter > 0;

		public void OnLit()
		{
			if (!IsLit)
				_activateable.Value.Activate();
			_litCounter++;
		}

		public void OnUnlit()
		{
			_litCounter--;
			if (!IsLit)
				_activateable.Value.Deactivate();
		}
	}

	public interface IIlluminatable
	{
		void OnLit();
		void OnUnlit();
	}
}
