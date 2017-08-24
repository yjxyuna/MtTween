using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/** @class Speed
 * @brief Changes the speed of an action, making it take longer (speed>1)
 * or shorter (speed<1) time.
 * Useful to simulate 'slow motion' or 'fast forward' effect.
 * @warning This action can't be Sequenceable because it is not an IntervalAction.
 */
public class MtSpeed : MtAction {

	public float Speed {
		get { return m_speed; }
		set { m_speed = value; }
	}

	public MtActionInterval InnerAction {
		get { return m_innerAction; }
		set { m_innerAction = value; }
	}

	protected float m_speed;
	protected MtActionInterval m_innerAction;

	public MtSpeed() {

	}

	public static MtSpeed Create(MtActionInterval action, float speed) {
		MtSpeed _speed = new MtSpeed();
		if (_speed.InitWithAction(action, speed)) 
			return  _speed;
		return null;
	}

	protected bool InitWithAction(MtActionInterval action, float speed) {
		Debug.Assert(action != null, "action must not be NULL");
		if (action == null) {
			Debug.Log("MtSpeed::InitWithAction error: action is null!");
			return false;
		}
		m_speed = speed;
		m_innerAction = action;
		return true;
	}

#region Override Functions
	public override void StartWithTarget(Transform target) {
		if (target != null && m_innerAction != null) {
			base.StartWithTarget(target);
			m_innerAction.StartWithTarget(target);
		} else {
			Debug.LogFormat("MtSpeed::StartWithTarget error: target{0} or m_innerAction{1} is null!", target, m_innerAction);
		}
	}

	public override void Step(float dt) {
		m_innerAction.Step(dt * m_speed);
	}

	public override void Stop() {
		if (m_innerAction != null)
			m_innerAction.Stop();
		base.Stop();
	}

	public override bool IsDone() {
		return m_innerAction.IsDone();
	}

	public override MtICloneable Clone() {
		if (m_innerAction != null)
			return MtSpeed.Create(m_innerAction.Clone() as MtActionInterval, m_speed);
		return null;
	}

	public override MtAction Reverse() {
		if (m_innerAction != null)
			return MtSpeed.Create(m_innerAction.Reverse() as MtActionInterval, m_speed);
		return null;
	}
	#endregion 
}
