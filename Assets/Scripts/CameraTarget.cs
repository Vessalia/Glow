using UnityEngine;

namespace Assets.Scripts
{
	public class CameraTarget : MonoBehaviour
	{
		[SerializeField] private Transform _root;
		[SerializeField] private Vector3 _offset;

		public Vector3 WorldPosition => _root.position + _root.rotation * _offset;
	}
}
