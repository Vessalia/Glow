using UnityEngine;

public class CountdownTimerRealtime : Timer
{
	public CountdownTimerRealtime(float value) : base(value) { }

	public override void Tick()
	{
		if (IsRunning && CurrentTime > 0)
			CurrentTime -= Time.unscaledDeltaTime;

		if (IsRunning && CurrentTime <= 0)
			Stop();
	}

	public override bool IsFinished => CurrentTime <= 0;
}
