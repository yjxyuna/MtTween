using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/** @class ActionInstant
* @brief Instant actions are immediate actions. They don't have a duration like the IntervalAction actions.
**/
public class MtActionInstant : MtFiniteTimeAction {

#region Override Functions
	public override MtICloneable Clone() {
		Debug.Assert(false, "Subclass should implement this method!");
		return null;
	}

	public override MtAction Reverse() {
		Debug.Assert(false, "Subclass should implement this method!");
		return null;
	}

	public override bool IsDone() {
		return true;
	}

	public override void Step(float dt) {
		Update(1);
	}

	public override void Update(float time) {

	}
#endregion
}

public delegate void MtCallback(params object[] arrParams);

public class MtCallFunc : MtActionInstant {

//	protected MtCallback m_function;
	protected Action m_function;
	protected Action<Transform> m_functionWithParam;

	public static MtCallFunc Create(Action func) {
		MtCallFunc ret = new MtCallFunc();
		if (ret.InitWithFunction(func))
			return ret;
		return null;
	}

	public static MtCallFunc Create(Action<Transform> func) {
		MtCallFunc ret = new MtCallFunc();
		if (ret.InitWithFunction(func))
			return ret;
		return null;
	}

	protected bool InitWithFunction(Action func) {
		m_function = func;
		return true;
	}

	protected bool InitWithFunction(Action<Transform> func) {
		m_functionWithParam = func;
		return true;
	}

#region Override Functions
	public override MtICloneable Clone() {
		MtCallFunc func = new MtCallFunc();
		if(m_function != null) {
			func.InitWithFunction(m_function);
		}
		if(m_functionWithParam != null) {
			func.InitWithFunction(m_functionWithParam);
		}
		return func;
	}

	public override MtAction Reverse() {
		return Clone() as MtAction;
	}

	public override void Update(float time) {
		Execute();
	}

	public virtual void Execute() {
		if(m_function != null) {
			m_function();
		}
		if(m_functionWithParam != null) {
			m_functionWithParam(m_target);
		}
	}
#endregion
}