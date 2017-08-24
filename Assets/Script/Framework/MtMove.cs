#define MT_ENABLE_STACKABLE_ACTIONS 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region MtMoveBy
public class MtMoveBy : MtActionInterval {

	protected bool m_is3D;
	protected Vector3 m_positionDelta;
	protected Vector3 m_startPosition;
	protected Vector3 m_previousPosition;

	public MtMoveBy() {
		m_is3D = false;
	}

	public static MtMoveBy Create(float duration, Vector2 deltaPosition) {
		return MtMoveBy.Create(duration, new Vector3(deltaPosition.x, deltaPosition.y, 0));
	}

	public static MtMoveBy Create(float duration, Vector3 deltaPosition) {
		MtMoveBy ret = new MtMoveBy();
		if (ret.InitWithDuration(duration, deltaPosition)) {
			return ret;
		}
		return null;
	}

	protected bool InitWithDuration(float duration, Vector2 deltaPosition) {
		return InitWithDuration(duration, new Vector3(deltaPosition.x, deltaPosition.y, 0));
	}

	protected bool InitWithDuration(float duration, Vector3 deltaPosition) {
		if (base.InitWithDuration(duration)) {
			m_positionDelta = deltaPosition;
			m_is3D = true;
			return true;
		}
		return false;
	}

#region Override Functions
	public override MtICloneable Clone() {
		return MtMoveBy.Create(m_duration, m_positionDelta);
	}

	public override MtAction Reverse() {
		return MtMoveBy.Create(m_duration, -m_positionDelta);
	}

	public override void StartWithTarget(Transform target) {
		base.StartWithTarget(target);
		m_previousPosition = m_startPosition = target.position;
	}
		
	public override void Update(float time) {
		/** @def CC_ENABLE_STACKABLE_ACTIONS
		 * If enabled, actions that alter the position property (eg: MoveBy, JumpBy, BezierBy, etc..) will be stacked.
		 * If you run 2 or more 'position' actions at the same time on a node, then end position will be the sum of all the positions.
		 * If disabled, only the last run action will take effect.
		 * Enabled by default. Disable to be compatible with v2.0 and older versions.
		 * @since v2.1
		 */
		if (m_target) {
#if MT_ENABLE_STACKABLE_ACTIONS
			Vector3 currentPos = m_target.position;
			Vector3 diff = currentPos - m_previousPosition;
			m_startPosition = m_startPosition + diff;
			Vector3 newPos =  m_startPosition + (m_positionDelta * time);
			m_target.position = newPos;
			m_previousPosition = newPos;
#else
			m_target.position = m_startPosition + m_positionDelta * t;
#endif // MT_ENABLE_STACKABLE_ACTIONS
		}
	}
#endregion
}
#endregion 


#region MtMoveTo
public class MtMoveTo : MtMoveBy {
	
	protected Vector3 m_endPosition;

	public MtMoveTo() {

	}

	public static MtMoveTo Create(float duration, Vector2 position) {
		return Create(duration, new Vector3(position.x, position.y, 0));
	}

	public static MtMoveTo Create(float duration, Vector3 position) {
		MtMoveTo ret = new MtMoveTo();
		if (ret.InitWithDuration(duration, position)) 
			return ret;
		return null;
	}

	bool InitWithDuration(float duration, Vector2 position) {
		return InitWithDuration(duration, new Vector3(position.x, position.y, 0));
	}

	bool InitWithDuration(float duration, Vector3 position) {
		if (base.InitWithDuration(duration)) {
			m_endPosition = position;
			return true;
		}
		return false;
	}

#region Override Functions
	public override MtICloneable Clone() {
		//no copy constructor
		return MtMoveTo.Create(m_duration, m_endPosition);
	}

	public override void StartWithTarget(Transform target) {
		base.StartWithTarget(target);
		m_positionDelta = m_endPosition - target.transform.position;
	}

	public override MtAction Reverse() {
		Debug.Assert(false, "reverse() not supported in MoveTo");
		return null;
	}
#endregion
}
#endregion 


#region MtMoveLocalBy
public class MtMoveLocalBy : MtMoveBy {

	public MtMoveLocalBy() {

	}

	public static MtMoveLocalBy Create(float duration, Vector3 deltaPosition) {
		MtMoveLocalBy ret = new MtMoveLocalBy();
		if (ret.InitWithDuration(duration, deltaPosition)) {
			return ret;
		}
		return null;
	}

#region Override Functions
	public override void StartWithTarget(Transform target) {
		base.StartWithTarget(target);
		m_previousPosition = m_startPosition = target.localPosition;
	}

	public override void Update(float time) {
		/** @def CC_ENABLE_STACKABLE_ACTIONS
		 * If enabled, actions that alter the position property (eg: MoveBy, JumpBy, BezierBy, etc..) will be stacked.
		 * If you run 2 or more 'position' actions at the same time on a node, then end position will be the sum of all the positions.
		 * If disabled, only the last run action will take effect.
		 * Enabled by default. Disable to be compatible with v2.0 and older versions.
		 */
		if (m_target) {
#if MT_ENABLE_STACKABLE_ACTIONS
			Vector3 currentPos = m_target.localPosition;
			Vector3 diff = currentPos - m_previousPosition;
			m_startPosition = m_startPosition + diff;
			Vector3 newPos =  m_startPosition + (m_positionDelta * time);
			m_target.localPosition = newPos;
			m_previousPosition = newPos;
#else
			m_target.localPosition = m_startPosition + m_positionDelta * t;
#endif // MT_ENABLE_STACKABLE_ACTIONS
		}
	}
#endregion
}
#endregion


#region MtMoveLocalTo
public class MtMoveLocalTo : MtMoveLocalBy{

	protected Vector3 m_endPosition;

	public MtMoveLocalTo() {

	}

	public static MtMoveLocalTo Create(float duration, Vector3 position) {
		MtMoveLocalTo ret = new MtMoveLocalTo();
		if (ret.InitWithDuration(duration, position)) {
			return ret;
		}
		return null;
	}

	protected bool InitWithDuration(float duration, Vector3 position) {
		if (InitWithDuration(duration)) {
			m_endPosition = position;
			m_is3D = true;
			return true;
		}
		return false;
	}

#region Override Functions
	public override void StartWithTarget(Transform target) {
		base.StartWithTarget(target);
		m_positionDelta = m_endPosition - target.transform.localPosition;
	}
#endregion
}
#endregion


#region MtMoveForwardBy
public class MtMoveForwardBy : MtActionInterval {

	protected Vector3 m_startPosition;
	protected Vector3 m_previousPosition;
	protected Vector3 m_direction;
	protected float m_deltaDistance;

	public MtMoveForwardBy() {
		
	}

	public static MtMoveForwardBy Create(float duration, float deltaDistance) {
		MtMoveForwardBy ret = new MtMoveForwardBy();
		if (ret.InitWithDuration(duration, deltaDistance)) {
			return ret;
		}
		return null;
	}

	protected bool InitWithDuration(float duration, float deltaDistance) {
		if (base.InitWithDuration(duration)) {
			m_deltaDistance = deltaDistance;
			return true;
		}
		return false;
	}

	protected virtual void SetDirection() {
		m_direction = m_target.forward;
	}

#region Override Functions
	public override void StartWithTarget(Transform target) {
		base.StartWithTarget(target);
		m_previousPosition = m_startPosition = target.localPosition;
		SetDirection();
	}

	public override void Update(float time) {
		if (m_target) {
#if MT_ENABLE_STACKABLE_ACTIONS
			Vector3 currentPos = m_target.localPosition;
			Vector3 diff = currentPos - m_previousPosition;
			m_startPosition = m_startPosition + diff;
			Vector3 newPos =  m_startPosition + m_direction * (m_deltaDistance * time);
			m_target.localPosition = newPos;
			m_previousPosition = newPos;
#else
			m_target.localPosition = m_startPosition + m_positionDelta * t;
#endif // MT_ENABLE_STACKABLE_ACTIONS
		}
	}
	#endregion
}
#endregion

#region MtMoveRightBy
public class MtMoveRightBy : MtMoveForwardBy {

	public static MtMoveRightBy Create(float duration, float deltaDistance) {
		MtMoveRightBy ret = new MtMoveRightBy();
		if (ret.InitWithDuration(duration, deltaDistance)) {
			return ret;
		}
		return null;
	}

#region Override Functions
	protected override void SetDirection() {
		m_direction = m_target.right;
	}
#endregion
}
#endregion

#region MtMoveUpBy
public class MtMoveUpBy : MtMoveForwardBy {

	public static MtMoveUpBy Create(float duration, float deltaDistance) {
		MtMoveUpBy ret = new MtMoveUpBy();
		if (ret.InitWithDuration(duration, deltaDistance)) {
			return ret;
		}
		return null;
	}

#region Override Functions
	protected override void SetDirection() {
		m_direction = m_target.up;
	}
#endregion
}
#endregion
