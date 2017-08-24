using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


#region MtActionFloat
/**
 * @class MtActionFloat
 * @brief Action used to animate any value in range [from,to] over specified time interval
 */
public class MtActionFloat : MtActionInterval {

	protected float m_from;
	protected float m_to;
	protected float m_delta;
	protected Action<float> m_onValueChanged;

	public MtActionFloat() {
		
	}

	public static MtActionFloat Create(float duration, float from, float to, Action<float> onValueChanged) {
		MtActionFloat actValue = new MtActionFloat();
		if (actValue.InitWithDuration(duration, from, to, onValueChanged)) 
			return  actValue;
		return null;
	}

	protected bool InitWithDuration(float duration, float from, float to, Action<float> onValueChanged) {
		if (base.InitWithDuration(duration)) {
			m_from = from;
			m_to = to;
			m_onValueChanged = onValueChanged;
			return true;
		}
		return false;
	}

#region Override Functions
	public override void StartWithTarget(Transform target) {
		base.StartWithTarget(target);
		m_delta = m_to - m_from;
	}

	public override void Update(float delta) {
		float value = m_to - m_delta * (1 - delta);
		if (m_onValueChanged != null)
			m_onValueChanged(value);
	}

	public override MtICloneable Clone() {
		return MtActionFloat.Create(m_duration, m_from, m_to, m_onValueChanged);
	}

	public override MtAction Reverse() {
		return MtActionFloat.Create(m_duration, m_to, m_from, m_onValueChanged);
	}
#endregion 
}
#endregion


#region MtActionInt
public class MtActionInt : MtActionInterval {

	protected int m_from;
	protected int m_to;
	protected int m_delta;
	protected Action<int> m_onValueChanged;
	protected int m_currentValue;

	public MtActionInt() {
		
	}

	public static MtActionInt Create(float duration, int from, int to, Action<int> onValueChanged) {
		MtActionInt actValue = new MtActionInt();
		if (actValue.InitWithDuration(duration, from, to, onValueChanged)) 
			return  actValue;
		return null;
	}

	protected bool InitWithDuration(float duration, int from, int to, Action<int> onValueChanged) {
		if (base.InitWithDuration(duration)) {
			m_from = from;
			m_to = to;
			m_onValueChanged = onValueChanged;
			m_currentValue = from;
			return true;
		}
		return false;
	}

#region Override Functions
	public override void StartWithTarget(Transform target) {
		base.StartWithTarget(target);
		m_delta = m_to - m_from;
	}

	public override void Update(float delta) {
		int value = m_to - (int)(m_delta * (1 - delta));
		if (m_onValueChanged != null && m_currentValue != value)
			m_onValueChanged(value);
		m_currentValue = value;
	}

	public override MtICloneable Clone() {
		return MtActionInt.Create(m_duration, m_from, m_to, m_onValueChanged);
	}

	public override MtAction Reverse() {
		return MtActionInt.Create(m_duration, m_to, m_from, m_onValueChanged);
	}
#endregion 
}
#endregion