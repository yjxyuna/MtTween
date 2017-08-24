#define MT_ENABLE_STACKABLE_ACTIONS 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class MtMath {
	public static float BezierAt(float a, float b, float c, float d, float t) {
		return (Mathf.Pow(1-t, 3) * a + 
			3 * t * (Mathf.Pow(1-t, 2)) * b + 
			3 * Mathf.Pow(t, 2) * (1-t) * c +
			Mathf.Pow(t, 3) * d);
	}
}

#region MtBezierBy
public class MtBezierBy : MtActionInterval {

	protected Vector3 m_startPosition;
	protected Vector3 m_controlPoint_1;
	protected Vector3 m_controlPoint_2;
	protected Vector3 m_endPosition;
	protected Vector3 m_previousPosition;

	public MtBezierBy() {

	}

	public static MtBezierBy Create(float duration, Vector3 controlPoint_1, Vector3 controlPoint_2, Vector3 endPosition) {
		MtBezierBy bezierBy = new MtBezierBy();
		if (bezierBy.InitWithPoint(duration, controlPoint_1, controlPoint_2, endPosition)) 
			return  bezierBy;
		return null;
	}

	protected bool InitWithPoint(float duration, Vector3 controlPoint_1, Vector3 controlPoint_2, Vector3 endPosition) {
		if (base.InitWithDuration(duration)) {
			m_controlPoint_1 = controlPoint_1;
			m_controlPoint_2 = controlPoint_2;
			m_endPosition = endPosition;
			return true;
		}
		return false;
	}

#region Override Functions
	public override void StartWithTarget(Transform target) {
		base.StartWithTarget(target);
		m_previousPosition = m_startPosition = target.position;
	}

	public override void Update(float time) {
		if (m_target != null) {
			float xa = 0;
			float xb = m_controlPoint_1.x;
			float xc = m_controlPoint_2.x;
			float xd = m_endPosition.x;

			float ya = 0;
			float yb = m_controlPoint_1.y;
			float yc = m_controlPoint_2.y;
			float yd = m_endPosition.y;

			float za = 0;
			float zb = m_controlPoint_1.z;
			float zc = m_controlPoint_2.z;
			float zd = m_endPosition.z;

			float x = MtMath.BezierAt(xa, xb, xc, xd, time);
			float y = MtMath.BezierAt(ya, yb, yc, yd, time);
			float z = MtMath.BezierAt(za, zb, zc, zd, time);

#if MT_ENABLE_STACKABLE_ACTIONS
			Vector3 currentPos = m_target.position;
			Vector3 diff = currentPos - m_previousPosition;
			m_startPosition = m_startPosition + diff;

			Vector3 newPos = m_startPosition + new Vector3(x, y, z);
			m_target.position = newPos;
			m_previousPosition = newPos;
#else
			m_target.position = m_startPosition + new Vector2(x, y);
#endif // !MT_ENABLE_STACKABLE_ACTIONS
		}
	}

	public override MtICloneable Clone() {
		return MtBezierBy.Create(m_duration, m_controlPoint_1, m_controlPoint_2, m_endPosition);
	}

	public override MtAction Reverse() {
		Vector2 endPosition = -m_endPosition;
		Vector2 controlPoint_1 = m_controlPoint_2 + (-m_endPosition);
		Vector2 controlPoint_2 = m_controlPoint_1 + (-m_endPosition);
		MtBezierBy action = MtBezierBy.Create(m_duration, controlPoint_1, controlPoint_2, endPosition);
		return action;
	}
#endregion
}
#endregion


#region MtBezierTo
public class MtBezierTo : MtBezierBy {

	public MtBezierTo() {

	}

	public static MtBezierTo Create(float duration, Vector3 controlPoint_1, Vector3 controlPoint_2, Vector3 endPosition) {
		MtBezierTo bezierTo = new MtBezierTo();
		if (bezierTo.InitWithPoint(duration, controlPoint_1, controlPoint_2, endPosition)) 
			return  bezierTo;
		return null;
	}

	protected bool InitWithPoint(float duration, Vector3 controlPoint_1, Vector3 controlPoint_2, Vector3 endPosition) {
		if (base.InitWithDuration(duration)) {
			m_controlPoint_1 = controlPoint_1;
			m_controlPoint_2 = controlPoint_2;
			m_endPosition = endPosition;
			return true;
		}
		return false;
	}

#region Override Functions
	public override void StartWithTarget(Transform target) {
		base.StartWithTarget(target);
		m_controlPoint_1 = m_controlPoint_1 - m_startPosition;
		m_controlPoint_2 = m_controlPoint_2 - m_startPosition;
		m_endPosition = m_endPosition - m_startPosition;
	}

	public override MtICloneable Clone() {
		return MtBezierTo.Create(m_duration, m_controlPoint_1, m_controlPoint_2, m_endPosition);
	}

	public override MtAction Reverse() {
		Debug.Assert(false, "MtBezierTo doesn't support the 'Reverse' method");
		return null;
	}
#endregion
}
#endregion