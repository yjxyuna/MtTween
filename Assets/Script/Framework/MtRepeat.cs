using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public partial class MtMath {
	public static float Fmod(float x, float y) {
		int n = (int)(x / y);
		return x - n * y;
	}
}

#region MtRepeat
/** @class Repeat
 * @brief Repeats an action a number of times.
 * To repeat an action forever use the RepeatForever action.
 */
public class MtRepeat : MtActionInterval {

	int m_times;
	int m_total;
	float m_nextDt;
	bool m_actionInstant;
	MtFiniteTimeAction m_innerAction;

	public MtRepeat() {

	}

	public static MtRepeat Create(MtFiniteTimeAction action, int times) {
		MtRepeat repeat = new MtRepeat();
		if (repeat.InitWithAction(action, times))
			return repeat;
		return null;
	}

	public bool InitWithAction(MtFiniteTimeAction action, int times) {
		float d = action.Duration * times;
		if (action != null && base.InitWithDuration(d)) {
			m_times = times;
			m_innerAction = action;

			m_actionInstant = action.GetType().IsSubclassOf(typeof(MtActionInstant));
			//an instant action needs to be executed one time less in the update method since it uses startWithTarget to execute the action
			// minggo: instant action doesn't execute action in Repeat::startWithTarget(), so comment it.
			//        if (_actionInstant) 
			//        {
			//            _times -=1;
			//        }
			m_total = 0;
			return true;
		}

		return false;
	}

#region Override Functions
	public override void StartWithTarget(Transform target) {
		m_total = 0;
		m_nextDt = m_innerAction.Duration / m_duration;
		base.StartWithTarget(target);
		m_innerAction.StartWithTarget(target);
	}

	public override void Update(float dt) {
		if (dt >= m_nextDt) {
			while (dt >= m_nextDt && m_total < m_times) {
				m_innerAction.Update(1.0f);
				m_total++;

				m_innerAction.Stop();
				m_innerAction.StartWithTarget(m_target);
				m_nextDt = m_innerAction.Duration / m_duration * (m_total + 1);
			}

			if (Mathf.Abs(dt - 1.0f) < Mathf.Epsilon && m_total < m_times) {
				m_innerAction.Update(1.0f);
				m_total++;
			}

			// don't set an instant action back or update it, it has no use because it has no duration
			if (!m_actionInstant) {
				if (m_total == m_times)	{
					// minggo: inner action update is invoked above, don't have to invoke it here
					//                if (!(sendUpdateEventToScript(1, _innerAction)))
					//                    _innerAction->update(1);
					m_innerAction.Stop();
				} else {
					// issue #390 prevent jerk, use right update
					m_innerAction.Update(dt - (m_nextDt - m_innerAction.Duration / m_duration));
				}
			}
		} else {
			m_innerAction.Update(dt * m_times - Mathf.FloorToInt(dt * m_times));
		}
	}

	public override void Stop() {
		m_innerAction.Stop();
		base.Stop();
	}

	public override bool IsDone() {
		return m_total == m_times;
	}

	public override MtICloneable Clone() {
		// no copy constructor
		return MtRepeat.Create(m_innerAction.Clone() as MtFiniteTimeAction, m_times);
	}

	public override MtAction Reverse() {
		return MtRepeat.Create(m_innerAction.Reverse() as MtFiniteTimeAction, m_times);
	}
#endregion
}
#endregion 


#region MtRepeatForever
/** @class RepeatForever
 * @brief Repeats an action for ever.
 To repeat the an action for a limited number of times use the Repeat action.
 * @warning This action can't be Sequenceable because it is not an IntervalAction.
 */
public class MtRepeatForever : MtActionInterval {
	
	public MtActionInterval InnerAction {
		get { return m_innerAction; }
		set { m_innerAction = value; }
	}
	protected MtActionInterval m_innerAction;

	public MtRepeatForever() {

	}

	public static MtRepeatForever Create(MtActionInterval action) {
		MtRepeatForever ret = new MtRepeatForever();
		if (ret.InitWithAction(action))
			return ret;
		return null;
	}

	public bool InitWithAction(MtActionInterval action) {
		Debug.Assert(action != null, "action can't be null!");
		if (action == null) {
			Debug.Log("MtRepeatForever.InitWithAction error:action is null!");
			return false;
		}
		m_innerAction = action;
		return true;
	}

#region Override Functions
	public override void StartWithTarget(Transform target) {
		base.StartWithTarget(target);
		m_innerAction.StartWithTarget(target);
	}

	public override void Step(float dt) {
		m_innerAction.Step(dt);
		if (m_innerAction.IsDone()) {
			float diff = m_innerAction.Elapsed - m_innerAction.Duration;
			if (diff > m_innerAction.Duration)
				diff = MtMath.Fmod(diff, m_innerAction.Duration);
			m_innerAction.StartWithTarget(m_target);
			// to prevent jerk. issue #390, 1247
			m_innerAction.Step(0.0f);
			m_innerAction.Step(diff);
		}
	}
		
	public override bool IsDone() {
		return false;
	}

	public override MtICloneable Clone() {
		// no copy constructor
		return MtRepeatForever.Create(m_innerAction.Clone() as MtActionInterval);
	}

	public override MtAction Reverse() {
		return MtRepeatForever.Create(m_innerAction.Reverse() as MtActionInterval);
	}
#endregion
}
#endregion 