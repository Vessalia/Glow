using System;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts
{
	public class Door : MonoBehaviour, IActivateable
	{
		[SerializeField] private bool _startsClosed;
		[SerializeField] private Transform openPosition;
		[SerializeField] private Transform closedPosition;

		private Vector3 _open;
		private Vector3 _closed;

		Coroutine _activeRoutine;

		void Awake()
		{
			_open = openPosition.transform.position;
			_closed = closedPosition.transform.position;
		}

		void OnValidate()
		{
			if (openPosition != null)
				_open = openPosition.transform.position;
			if (closedPosition != null)
				_closed = closedPosition.transform.position;
		}

		public void Open()
		{
			if (_activeRoutine != null)
				StopCoroutine(_activeRoutine);

			_activeRoutine = StartCoroutine(MoveDoor(_open));
		}

		public void Close()
		{
			if (_activeRoutine != null)
				StopCoroutine(_activeRoutine);

			_activeRoutine = StartCoroutine(MoveDoor(_closed));
		}

		IEnumerator MoveDoor(Vector3 end)
		{
			const float time = 3;
			float timer = 0;

			Vector3 start = transform.position;
			while (timer < time)
			{
				float t = timer / time;
				this.transform.position = Vector3.Lerp(start, end, t);
				yield return null;
				timer += Time.deltaTime;
			}
			this.transform.position = end;
		}

		public void Activate() => Open();
		public void Deactivate() => Close();
	}
}
