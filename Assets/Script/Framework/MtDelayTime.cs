using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MtDelayTime : MtActionInterval {

	public MtDelayTime() {

	}

	public static MtDelayTime Create(float d) {
		MtDelayTime action = new MtDelayTime();
		if (action.InitWithDuration(d))
			return action;
		return null;
	}
		
#region Override Functions
	public override MtICloneable Clone() {
		// no copy constructor
		return MtDelayTime.Create(m_duration);
	}

	public override MtAction Reverse() {
		return MtDelayTime.Create(m_duration);
	}

	public override void Update(float time) {
		return;
	}
#endregion
}
