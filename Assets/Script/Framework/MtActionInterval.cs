using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MtActionInterval : MtFiniteTimeAction {

	public float Elapsed {
		get { return m_elapsed; }
		private set { m_elapsed = value; }
	}

	protected float m_elapsed;
	protected bool m_firstTick;

#region Overrides Functions
	public override void StartWithTarget(Transform target) {
		base.StartWithTarget(target);
		m_elapsed = 0.0f;
		m_firstTick = true;
	}

	public override MtICloneable Clone() {
		Debug.Assert(false, "Subclass should implement this method!");
		return null;
	}

	public override MtAction Reverse() {
		Debug.Assert(false, "Subclass should implement this method!");
		return null;
	}

	public override bool IsDone() {
		return m_elapsed >= m_duration;
	}

	public override void Step(float dt) {
		if (m_firstTick) {
			m_firstTick = false;
			m_elapsed = 0;
		} else {
			m_elapsed += dt;
		}
		float updateDt = Mathf.Max(0, Mathf.Min(1, m_elapsed / m_duration));
		Update(updateDt);
	}

	public bool InitWithDuration(float d) {
		m_duration = d;
		//prevent division by 0
		if (m_duration <= Mathf.Epsilon) {
			m_duration = Mathf.Epsilon;
		}
		m_elapsed = 0;
		m_firstTick = false;
		return true;
	}
#endregion
}
