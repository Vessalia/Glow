using UnityEngine;

public class EnemyController : MonoBehaviour
{
	[SerializeField] private float _chaseRange = 12f;
	[SerializeField] private float _attackRange = 1.8f;
	[SerializeField] private Transform _target;

	[SerializeField]
	private CharacterMotor _motor;

	private void Start()
	{
		if (_target == null)
		{
			var player = GameObject.FindGameObjectWithTag("Player");
			_target = player?.transform;
		}
	}

	private void FixedUpdate()
	{
		var intent = new CharacterIntent();
		if (_target == null)
		{
			_motor.ApplyIntent(intent);
			return;
		}

		float dist = Vector3.Distance(transform.position, _target.position);


		if (dist <= _attackRange)
		{
			intent.AttackPressed = true;
		}
		else if (dist <= _chaseRange)
		{
			Vector3 toTarget = (_target.position - transform.position).normalized;
			intent.MoveDirection = new Vector2(toTarget.x, toTarget.z);
		}

		_motor.ApplyIntent(intent);
	}
}
