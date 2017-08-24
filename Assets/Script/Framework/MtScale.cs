using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region MtScaleTo
public class MtScaleTo : MtActionInterval {

	protected float m_scaleX;
	protected float m_scaleY;
	protected float m_scaleZ;
	protected float m_startScaleX;
	protected float m_startScaleY;
	protected float m_startScaleZ;
	protected float m_endScaleX;
	protected float m_endScaleY;
	protected float m_endScaleZ;
	protected float m_deltaX;
	protected float m_deltaY;
	protected float m_deltaZ;

	public MtScaleTo() {

	}

	public static MtScaleTo Create(float duration, Vector3 s) {
		MtScaleTo scaleTo = new MtScaleTo();
		if (scaleTo.InitWithDuration(duration, s.x, s.y, s.z)) 
			return  scaleTo;
		return null;
	}

	public static MtScaleTo Create(float duration, float s) {
		MtScaleTo scaleTo = new MtScaleTo();
		if (scaleTo.InitWithDuration(duration, s)) 
			return  scaleTo;
		return null;
	}

	protected bool InitWithDuration(float duration, float s) {
		if (base.InitWithDuration(duration)) {
			m_endScaleX = s;
			m_endScaleY = s;
			m_endScaleZ = s;
			return true;
		}
		return false;
	}

	protected bool InitWithDuration(float duration, float sx, float sy, float sz) {
		if (base.InitWithDuration(duration)) {
			m_endScaleX = sx;
			m_endScaleY = sy;
			m_endScaleZ = sz;
			return true;
		}
		return false;
	}

#region Override Functions
	public override void StartWithTarget(Transform target) {
		base.StartWithTarget(target);
		m_startScaleX = target.localScale.x;
		m_startScaleY = target.localScale.y;
		m_startScaleZ = target.localScale.z;
		m_deltaX = m_endScaleX - m_startScaleX;
		m_deltaY = m_endScaleY - m_startScaleY;
		m_deltaZ = m_endScaleZ - m_startScaleZ;
	}

	public override MtICloneable Clone() {
		return MtScaleTo.Create(m_duration, new Vector3(m_endScaleX, m_endScaleY, m_endScaleZ));
	}

	public override MtAction Reverse() {
		Debug.Assert(false, "Reverse() not supported in ScaleTo");
		return null;
	}

	public override void Update(float time) {
		if (m_target != null) {
			m_target.localScale = new Vector3(m_startScaleX + m_deltaX * time, m_startScaleY + m_deltaY * time, m_startScaleZ + m_deltaZ * time);
		}
	}
#endregion
}
#endregion 


#region MtScaleBy
public class MtScaleBy : MtScaleTo {

	public MtScaleBy() {

	}

	public static MtScaleBy Create(float duration, Vector3 s) {
		MtScaleBy scaleBy = new MtScaleBy();
		if (scaleBy.InitWithDuration(duration, s.x, s.y, s.z)) 
			return  scaleBy;
		return null;
	}

#region Override Functions
	public override void StartWithTarget(Transform target) {
		base.StartWithTarget(target);
		m_deltaX = m_startScaleX * m_endScaleX - m_startScaleX;
		m_deltaY = m_startScaleY * m_endScaleY - m_startScaleY;
		m_deltaZ = m_startScaleZ * m_endScaleZ - m_startScaleZ;
	}

	public override MtICloneable Clone() {
		return MtScaleBy.Create(m_duration, new Vector3(m_endScaleX, m_endScaleY, m_endScaleZ));
	}

	public override MtAction Reverse() {
		return MtScaleBy.Create(m_duration, new Vector3(1/m_endScaleX, 1/m_endScaleY, 1/m_endScaleZ));
	}
#endregion
}
#endregion

#region MtScaleXTo
public class MtScaleXTo : MtScaleTo {

	public MtScaleXTo() {

	}

	public static MtScaleXTo Create(float duration, float s) {
		MtScaleXTo scaleXTo = new MtScaleXTo();
		if (scaleXTo.InitScaleWithDuration(duration, s)) 
			return scaleXTo;
		return null;
	}

	protected bool InitScaleWithDuration(float duration, float s) {
		if (base.InitWithDuration(duration)) {
			m_endScaleX = s;
			return true;
		}
		return false;
	}

#region Override Functions
	public override MtICloneable Clone() {
		return MtScaleXTo.Create(m_duration, m_endScaleX);
	}

	public override MtAction Reverse() {
		Debug.Assert(false, "Reverse() not supported in ScaleXTo");
		return null;
	}

	public override void Update(float time) {
		if (m_target != null) {
			m_target.localScale = new Vector3(m_startScaleX + m_deltaX * time, m_startScaleY, m_startScaleZ);
		}
	}
#endregion
}
#endregion 

#region MtScaleYTo
public class MtScaleYTo : MtScaleTo {

	public static MtScaleYTo Create(float duration, float s) {
		MtScaleYTo scaleYTo = new MtScaleYTo();
		if (scaleYTo.InitScaleWithDuration(duration, s)) 
			return scaleYTo;
		return null;
	}

	protected bool InitScaleWithDuration(float duration, float s) {
		if (base.InitWithDuration(duration)) {
			m_endScaleY = s;
			return true;
		}
		return false;
	}

#region Override Functions
	public override MtICloneable Clone() {
		return MtScaleYTo.Create(m_duration, m_endScaleY);
	}

	public override MtAction Reverse() {
		Debug.Assert(false, "Reverse() not supported in ScaleYTo");
		return null;
	}

	public override void Update(float time) {
		if (m_target != null) {
			m_target.localScale = new Vector3(m_startScaleX, m_startScaleY + m_deltaY * time, m_startScaleZ);
		}
	}
#endregion
}
#endregion 

#region MtScaleZTo
public class MtScaleZTo : MtScaleTo {

	public static MtScaleZTo Create(float duration, float s) {
		MtScaleZTo scaleZTo = new MtScaleZTo();
		if (scaleZTo.InitScaleWithDuration(duration, s)) 
			return scaleZTo;
		return null;
	}

	protected bool InitScaleWithDuration(float duration, float s) {
		if (base.InitWithDuration(duration)) {
			m_endScaleZ = s;
			return true;
		}
		return false;
	}

#region Override Functions
	public override MtICloneable Clone() {
		return MtScaleZTo.Create(m_duration, m_endScaleZ);
	}

	public override MtAction Reverse() {
		Debug.Assert(false, "Reverse() not supported in ScaleZTo");
		return null;
	}

	public override void Update(float time) {
		if (m_target != null) {
			m_target.localScale = new Vector3(m_startScaleX, m_startScaleY, m_startScaleZ + m_deltaZ * time);
		}
	}
#endregion
}
#endregion 