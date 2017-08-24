using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MtBlink : MtActionInterval {

	protected int m_times;
	protected bool m_originalState;

	public MtBlink() {

	}

	public static MtBlink Create(float duration, int times) {
		MtBlink blink = new MtBlink();
		if (blink.InitWithDuration(duration, times)) 
			return blink;
		return null;
	}

	bool InitWithDuration(float duration, int times) {
		Debug.Assert(times>=0, "times should be >= 0");
		if (times < 0) {
			Debug.Log("MtBlink.InitWithDuration error:times should be >= 0");
			return false;
		}
		if (base.InitWithDuration(duration) && times >= 0) {
			m_times = times;
			return true;
		}
		return false;
	}

#region Override Functions
	public override void Stop() {
		if (m_target != null)
			m_target.gameObject.SetActive(m_originalState);
		base.Stop();
	}

	public override MtICloneable Clone() {
		return MtBlink.Create(m_duration, m_times);
	}

	public override MtAction Reverse() {
		return MtBlink.Create(m_duration, m_times);
	}

	public override void StartWithTarget(Transform target) {
		base.StartWithTarget(target);
		m_originalState = target.gameObject.activeInHierarchy;
	}

	public override void Update(float time) {
		if (m_target != null && !IsDone()) {
			float slice = 1.0f / m_times;
			float m = MtMath.Fmod(time, slice);
			m_target.gameObject.SetActive(m > slice / 2 ? true : false);
		}
	}
#endregion
}
