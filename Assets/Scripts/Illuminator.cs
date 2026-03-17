using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
	[RequireComponent(typeof(Collider))]
	public class Illuminator : MonoBehaviour, IIlluminator
	{
		private HashSet<IIlluminatable> _illuminated = new();

		public void Illuminate(IIlluminatable other)
		{
			if(_illuminated.Add(other))
				other.OnLit();
		}

		public void DeIlluminate(IIlluminatable other)
		{
			if (_illuminated.Remove(other))
				other.OnUnlit();
		}

		void OnDisable()
		{
			foreach (var illuminated in _illuminated)
				illuminated.OnUnlit();

			_illuminated.Clear();
		}

		private void OnTriggerEnter(Collider collider)
		{
			if (collider.TryGetComponent<IIlluminatable>(out var other))
				Illuminate(other);
		}

		private void OnTriggerExit(Collider collider)
		{
			if (collider.TryGetComponent<IIlluminatable>(out var other))
				DeIlluminate(other);
		}
	}
}
