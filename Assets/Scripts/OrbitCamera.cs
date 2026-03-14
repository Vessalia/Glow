using Assets.Scripts;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Camera))]
[ExecuteInEditMode]
public class OrbitCamera : MonoBehaviour
{
	[SerializeField]
	private CameraTarget target = default;

	[SerializeField, Range(0f, 20f)]
	private float distance = 5f;

	[SerializeField, Range(0.1f, 20f)]
	private float zoomLerpSpeed = 8f;

	[SerializeField, Min(0f)]
	private float focusRadius = 1f;

	[SerializeField, Range(0f, 1f)]
	private float focusCentering = 0.5f;

	[SerializeField, Range(1f, 360f)]
	private float rotationSpeed = 90f;

	[SerializeField, Range(-89f, 89f)]
	private float minVerticalAngle = -30f, maxVerticalAngle = 60f;

	[SerializeField, Min(0f)]
	private float alignDelay = 5f;

	[SerializeField, Range(0f, 90f)]
	private float alignSmoothRange = 45f;

	private Vector3 focusPoint, previousFocusPoint;

	private float lastManualRotationTime;

	private Vector2 orbitAngles = new Vector2(45f, 0f);

	private float currentDistance;

	private Camera cam;

	private bool isAttacking;

	Vector3 CameraHalfExtents
	{
		get
		{
			Vector3 halfExtents;
			halfExtents.y = cam.nearClipPlane * Mathf.Tan(0.5f * Mathf.Deg2Rad * cam.fieldOfView);
			halfExtents.x = halfExtents.y * cam.aspect;
			halfExtents.z = 0f;
			return halfExtents;
		}
	}

	void Start()
	{
		focusPoint = target.WorldPosition;
		transform.localRotation = Quaternion.Euler(orbitAngles);

		cam = GetComponent<Camera>();
		currentDistance = distance;
	}

	void OnValidate()
	{
		maxVerticalAngle = Mathf.Max(maxVerticalAngle, minVerticalAngle);
	}

	void ConstrainAngles()
	{
		orbitAngles.x = Mathf.Clamp(orbitAngles.x, minVerticalAngle, maxVerticalAngle);

		while (orbitAngles.y < 0f)
			orbitAngles.y += 360f;
		while (orbitAngles.y >= 360f)
			orbitAngles.y -= 360f;
	}

	void LateUpdate()
	{
		CalculateFocusPoint();

		Quaternion lookRotation = Quaternion.Euler(orbitAngles);
		if (ManualRotation() || AutomaticRotation())
		{
			ConstrainAngles();
			lookRotation = Quaternion.Euler(orbitAngles);
		}

		Vector3 lookDirection = lookRotation * Vector3.forward;
		float desiredDistance = distance;

		Vector3 idealLookPosition = focusPoint - lookDirection * distance;

		Vector3 rectOffset = lookDirection * cam.nearClipPlane;
		Vector3 rectPosition = idealLookPosition + rectOffset;
		Vector3 castFrom = target.WorldPosition;
		Vector3 castLine = rectPosition - castFrom;
		float castDistance = castLine.magnitude;
		Vector3 castDirection = castLine / castDistance;

		if (Physics.BoxCast(castFrom, CameraHalfExtents, castDirection, out RaycastHit hit, lookRotation, castDistance, LayerMask.GetMask("Terrain"), QueryTriggerInteraction.Ignore))
		{
			Vector3 clippedRectPosition = castFrom + castDirection * hit.distance;
			Vector3 clippedLookPosition = clippedRectPosition - rectOffset;

			desiredDistance = Vector3.Distance(focusPoint, clippedLookPosition);
		}

		float t = 1f - Mathf.Exp(-zoomLerpSpeed * Time.unscaledDeltaTime);
		currentDistance = Mathf.Lerp(currentDistance, desiredDistance, t);

		Vector3 finalLookPosition = focusPoint - lookDirection * currentDistance;
		transform.SetPositionAndRotation(finalLookPosition, lookRotation);
	}

	bool ManualRotation()
	{
		Vector2 rawInput = InputSystem.actions.FindAction("Look").ReadValue<Vector2>();
		var input = new Vector2(rawInput.y, rawInput.x);

		const float e = 0.001f;
		if (input.x < -e || input.x > e || input.y < -e || input.y > e)
		{
			orbitAngles += rotationSpeed * Time.unscaledDeltaTime * input;
			lastManualRotationTime = Time.unscaledTime;
			return true;
		}
		return false;
	}

	bool AutomaticRotation()
	{
		if (Time.unscaledTime - lastManualRotationTime < alignDelay)
			return false;

		var movement = new Vector2(
			focusPoint.x - previousFocusPoint.x,
			focusPoint.z - previousFocusPoint.z
		);
		float movementDeltaSqr = movement.sqrMagnitude;
		if (movementDeltaSqr < 0.0001f)
			return false;

		float headingAngle = GetAngle(movement / Mathf.Sqrt(movementDeltaSqr));
		float deltaAbs = Mathf.Abs(Mathf.DeltaAngle(orbitAngles.y, headingAngle));
		float rotationChange = rotationSpeed * Mathf.Min(Time.unscaledDeltaTime, movementDeltaSqr);

		if (deltaAbs < alignSmoothRange)
			rotationChange *= deltaAbs / alignSmoothRange;
		else if (180f - deltaAbs < alignSmoothRange)
			rotationChange *= (180f - deltaAbs) / alignSmoothRange;

		orbitAngles.y = Mathf.MoveTowardsAngle(orbitAngles.y, headingAngle, rotationChange);
		return true;
	}

	void CalculateFocusPoint()
	{
		float effectiveFocusRadius = focusRadius * (isAttacking ? 1.5f : 1.0f);
		float effectiveFocusCentering = Mathf.Clamp01(focusCentering * (isAttacking ? 1.5f : 1.0f));

		previousFocusPoint = focusPoint;
		Vector3 targetPoint = target.WorldPosition;
		if (effectiveFocusRadius > 0f)
		{
			float distance = Vector3.Distance(targetPoint, focusPoint);
			float t = 1f;

			if (distance > 0.01f && effectiveFocusCentering > 0f)
				t = Mathf.Pow(1f - effectiveFocusCentering, Time.deltaTime);
			if (distance > effectiveFocusRadius)
				t = Mathf.Min(t, effectiveFocusRadius / distance);

			focusPoint = Vector3.Lerp(targetPoint, focusPoint, t);
		}
		else
		{
			focusPoint = targetPoint;
		}
	}

	static float GetAngle(Vector2 direction)
	{
		float angle = Mathf.Acos(direction.y) * Mathf.Rad2Deg;
		return direction.x < 0f ? 360f - angle : angle;
	}
}
