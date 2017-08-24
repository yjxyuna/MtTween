using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region MtRotateTo
public class MtRotateTo : MtActionInterval {

	protected bool m_is3D;
	protected Vector3 m_dstAngle;
	protected Vector3 m_startAngle;
	protected Vector3 m_diffAngle;

	public MtRotateTo() {
		m_is3D = true;
	}

	public static MtRotateTo Create(float duration, Vector3 dstAngle3D) {
		MtRotateTo rotateTo = new MtRotateTo();
		if (rotateTo.InitWithDuration(duration, dstAngle3D)) 
			return  rotateTo;
		return null;
	}

	protected bool InitWithDuration(float duration, Vector3 dstAngle3D) {
		if (base.InitWithDuration(duration)) {
			m_dstAngle = dstAngle3D;
			m_is3D = true;
			return true;
		}
		return false;
	}

	protected void CalculateAngles(ref float startAngle, ref float diffAngle, float dstAngle) {
		if (startAngle > 0)
			startAngle = MtMath.Fmod(startAngle, 360.0f);
		else
			startAngle = MtMath.Fmod(startAngle, -360.0f);
		diffAngle = dstAngle - startAngle;
		if (diffAngle > 180)
			diffAngle -= 360;
		if (diffAngle < -180)
			diffAngle += 360;
	}

#region Override Functions
	public override void StartWithTarget(Transform target) {
		base.StartWithTarget(target);
		if (m_is3D) {
			m_startAngle = m_target.eulerAngles;
		}
//		else
//		{
//			m_startAngle.x = m_target->getRotationSkewX();
//			m_startAngle.y = m_target->getRotationSkewY();
//		}

		CalculateAngles(ref m_startAngle.x, ref m_diffAngle.x, m_dstAngle.x);
		CalculateAngles(ref m_startAngle.y, ref m_diffAngle.y, m_dstAngle.y);
		CalculateAngles(ref m_startAngle.z, ref m_diffAngle.z, m_dstAngle.z);
	}

	public override void Update(float time) {
		if(m_is3D) {
			m_target.eulerAngles = new Vector3(m_startAngle.x + m_diffAngle.x * time,
				m_startAngle.y + m_diffAngle.y * time,
				m_startAngle.z + m_diffAngle.z * time);
		}
//		else
//		{
//#if CC_USE_PHYSICS
//			if (_startAngle.x == _startAngle.y && _diffAngle.x == _diffAngle.y)
//			{
//			_target->setRotation(_startAngle.x + _diffAngle.x * time);
//			}
//			else
//			{
//			_target->setRotationSkewX(_startAngle.x + _diffAngle.x * time);
//			_target->setRotationSkewY(_startAngle.y + _diffAngle.y * time);
//			}
//#else
//			_target->setRotationSkewX(_startAngle.x + _diffAngle.x * time);
//			_target->setRotationSkewY(_startAngle.y + _diffAngle.y * time);
//#endif // CC_USE_PHYSICS
//		}
	}

	public override MtICloneable Clone() {
		//no copy constructor
		MtRotateTo rotateTo = new MtRotateTo();
		if(m_is3D)
			rotateTo.InitWithDuration(m_duration, m_dstAngle);
//		else
//			rotateTo.InitWithDuration(m_duration, m_dstAngle.x, m_dstAngle.y);
		return rotateTo;
	}

	public override MtAction Reverse() {
		Debug.Assert(false, "MtRotateTo doesn't support the 'reverse' method");
		return null;
	}
#endregion
}
#endregion


#region MtRotateBy
public class MtRotateBy : MtActionInterval {

	protected bool m_is3D;
	protected Vector3 m_deltaAngle;
	protected Vector3 m_startAngle;

	public MtRotateBy() {
		m_is3D = true;
	}

	public static MtRotateBy Create(float duration, Vector3 deltaAngle3D) {
		MtRotateBy rotateBy = new MtRotateBy();
		if (rotateBy.InitWithDuration(duration, deltaAngle3D)) 
			return  rotateBy;
		return null;
	}

	protected bool InitWithDuration(float duration, Vector3 deltaAngle3D) {
		if (base.InitWithDuration(duration)) {
			m_deltaAngle = deltaAngle3D;
			m_is3D = true;
			return true;
		}
		return false;
	}

#region Override Functions
	public override void StartWithTarget(Transform target) {
		base.StartWithTarget(target);
		if (m_is3D) {
			m_startAngle = m_target.rotation.eulerAngles;
		}
//		else
//		{
//			m_startAngle.x = m_target->getRotationSkewX();
//			m_startAngle.y = m_target->getRotationSkewY();
//		}
	}

	public override void Update(float time) {
		// FIXME: shall I add % 360
		if (m_target) {
			if(m_is3D) {
				Vector3 eulerAngles;
				eulerAngles.x = m_startAngle.x + m_deltaAngle.x * time;
				eulerAngles.y = m_startAngle.y + m_deltaAngle.y * time;
				eulerAngles.z = m_startAngle.z + m_deltaAngle.z * time;
				m_target.eulerAngles = eulerAngles;
			}
//			else
//			{
//#if CC_USE_PHYSICS
//				if (_startAngle.x == _startAngle.y && _deltaAngle.x == _deltaAngle.y)
//				{
//				_target->setRotation(_startAngle.x + _deltaAngle.x * time);
//				}
//				else
//				{
//				_target->setRotationSkewX(_startAngle.x + _deltaAngle.x * time);
//				_target->setRotationSkewY(_startAngle.y + _deltaAngle.y * time);
//				}
//#else
//				_target->setRotationSkewX(_startAngle.x + _deltaAngle.x * time);
//				_target->setRotationSkewY(_startAngle.y + _deltaAngle.y * time);
//#endif // CC_USE_PHYSICS
//			}
		}
	}

	public override MtICloneable Clone() {
		//no copy constructor
		MtRotateBy rotateBy = new MtRotateBy();
		if(m_is3D)
			rotateBy.InitWithDuration(m_duration, m_deltaAngle);
//		else
//			rotateTo.InitWithDuration(m_duration, m_dstAngle.x, m_dstAngle.y);
		return rotateBy;
	}

	public override MtAction Reverse() {
		if(m_is3D) {
			Vector3 v;
			v.x = - m_deltaAngle.x;
			v.y = - m_deltaAngle.y;
			v.z = - m_deltaAngle.z;
			return MtRotateBy.Create(m_duration, v);
		} else {
//			return MtRotateBy.Create(m_duration, -m_deltaAngle.x, -m_deltaAngle.y);
		}
		return null;
	}
#endregion
}
#endregion

#region MtRotateLocalTo
public class MtRotateLocalTo : MtRotateTo {

	public MtRotateLocalTo() {

	}

	public static MtRotateLocalTo Create(float duration, Vector3 dstAngle3D) {
		return MtRotateTo.Create(duration, dstAngle3D) as MtRotateLocalTo;
	}

#region Override Functions
	public override void StartWithTarget(Transform target) {
		base.StartWithTarget(target);
		if (m_is3D) {
			m_startAngle = m_target.localEulerAngles;
		}
		CalculateAngles(ref m_startAngle.x, ref m_diffAngle.x, m_dstAngle.x);
		CalculateAngles(ref m_startAngle.y, ref m_diffAngle.y, m_dstAngle.y);
		CalculateAngles(ref m_startAngle.z, ref m_diffAngle.z, m_dstAngle.z);
	}

	public override void Update(float time) {
		if(m_is3D) {
			m_target.localRotation = Quaternion.Euler(new Vector3(m_startAngle.x + m_diffAngle.x * time,
				m_startAngle.y + m_diffAngle.y * time,
				m_startAngle.z + m_diffAngle.z * time));
		}
	}

	public override MtICloneable Clone() {
		return base.Clone() as MtRotateLocalTo;
	}
#endregion
}
#endregion



public class MtRotateAround : MtActionInterval {

	protected bool m_is3D;
	protected Vector3 m_point;
	protected Vector3 m_axis;
	protected float m_deltaAngle;
	protected float m_currentAngle;

	public MtRotateAround() {
		m_is3D = true;
	}

	public static MtRotateAround Create(float duration, Vector3 point, Vector3 axis, float angle) {
		MtRotateAround rotateAround = new MtRotateAround();
		if (rotateAround.InitWithDuration(duration, point, axis, angle)) 
			return  rotateAround;
		return null;
	}

	protected bool InitWithDuration(float duration, Vector3 point, Vector3 axis, float angle) {
		if (base.InitWithDuration(duration)) {
			m_point = point;
			m_axis = axis;
			m_deltaAngle = angle;
			m_is3D = true;
			return true;
		}
		return false;
	}

	#region Override Functions
	public override void StartWithTarget(Transform target) {
		base.StartWithTarget(target);
		if (m_is3D) {
			m_currentAngle = 0;
		}
	}

	public override void Update(float time) {
		if (m_target) {
			if(m_is3D) {
				float angle = m_deltaAngle * time - m_currentAngle;
				m_target.RotateAround(m_point, m_axis, angle);  
				m_currentAngle = m_deltaAngle * time;
			}
		}
	}

	public override MtICloneable Clone() {
		//no copy constructor
		MtRotateAround rotateAround = new MtRotateAround();
		if (m_is3D)
			rotateAround.InitWithDuration(m_duration, m_point, m_axis, m_deltaAngle);
		return rotateAround;
	}

	public override MtAction Reverse() {
		if(m_is3D) {
			return MtRotateAround.Create(m_duration, m_point, m_axis, -m_deltaAngle);
		} else {
			//			return MtRotateBy.Create(m_duration, -m_deltaAngle.x, -m_deltaAngle.y);
		}
		return null;
	}
	#endregion
}