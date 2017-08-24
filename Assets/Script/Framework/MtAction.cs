using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface MtICloneable {
	MtICloneable Clone();
}

public class MtAction : MtICloneable{

	//Default tag used for all the actions.
	public static readonly int INVALID_TAG = -1;

	/** 
     * The "target" is the transform of GameObject
     * The target will be set with the 'startWithTarget' method.
     * When the 'stop' method is called, target will be set to nil.
     */
	public Transform Target {
		get { return m_target; }
		set { m_target = value; }
	}

	/** The action tag. An identifier of the action. */
	public int Tag {
		get { return m_tag; }
		set { m_tag = value; }
	}

	public int Flags {
		get { return m_flags; }
		set { m_flags = value; }
	}

	protected Transform m_target;
	//An identifier of the action.
	protected int m_tag;
	//To categorize action into certain groups
	protected int m_flags;


	public MtAction() {
		m_target = null;
		m_tag = INVALID_TAG;
		m_flags = 0;
	}

	//Returns a clone of action.
	public virtual MtICloneable Clone() {
		Debug.Assert(false, "Subclass should implement this method!");
		return null;
	}

	//Returns a new action that performs the exact reverse of the action. 
	public virtual MtAction Reverse() {
		Debug.Assert(false, "Subclass should implement this method!");
		return null;
	}

	//Return true if the action has finished.
	public virtual bool IsDone() {
		return true;
	}

	//Called before the action start. It will also set the target. 
	public virtual void StartWithTarget(Transform target) {
		m_target = target;
	}

	//Called after the action has finished. It will set the 'target' to null.
	//IMPORTANT: You should never call "MtAction::stop()" manually. Instead, use: "target->stopAction(action);".
	public virtual void Stop() {
//		m_target = null;
	}

	//Called every frame with it's delta time, dt in seconds.
	public virtual void Step(float dt) { }

	/** 
     * Called once per frame. time a value between 0 and 1.
     * For example:
     * - 0 Means that the action just started.
     * - 0.5 Means that the action is in the middle.
     * - 1 Means that the action is over.
     *
     * @param time A value between 0 and 1.
     */
	public virtual void Update(float time) { 
		Debug.Log("[MtAction Update]. override me");
	}
}

public class MtFiniteTimeAction : MtAction {
	
	public float Duration {
		get { return m_duration; }
		set { m_duration = value; }
	}
		
	//Duration in seconds.
	protected float m_duration;

	public MtFiniteTimeAction() {
		m_duration = 0;
	}

#region Overrides Functions
	public override MtICloneable Clone() {
		Debug.Assert(false, "Subclass should implement this method!");
		return null;
	}
		
	public override MtAction Reverse() {
		Debug.Assert(false, "Subclass should implement this method!");
		return null;
	}
#endregion
}
	
//Extra action for making a Sequence or Spawn when only adding one action to it.
public class MtExtraAction : MtFiniteTimeAction {

	public static MtExtraAction Create() {
		MtExtraAction ret = new MtExtraAction();
		return ret;
	}

#region Overrides Functions
	public override MtICloneable Clone() {
		return Create() as MtICloneable;
	}

	public override MtAction Reverse() {
		return Create() as MtAction;
	}

	public override void Step(float dt) {
		
	}

	public override void Update(float time) {

	}
#endregion
}