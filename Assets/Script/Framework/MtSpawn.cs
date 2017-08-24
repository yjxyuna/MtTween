using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MtSpawn : MtActionInterval {

	protected MtFiniteTimeAction m_one;
	protected MtFiniteTimeAction m_two;

	public static MtSpawn Create(MtFiniteTimeAction action1, params MtFiniteTimeAction[] actions) {
		MtSpawn ret = CreateWithVariableList(action1, actions);
		return ret;
	}

	public static MtSpawn CreateWithVariableList(MtFiniteTimeAction action1, params MtFiniteTimeAction[] actions) {
		MtFiniteTimeAction now = null;
		MtFiniteTimeAction prev = action1;
		bool bOneAction = true;
		int index = 0;
		while(index < actions.Length) {
			now = actions[index];
			index++;
			if (now != null) {
				prev = CreateWithTwoActions(prev, now);
				bOneAction = false;
			}
		}
		// If only one action is added to Sequence, make up a Sequence by adding a simplest finite time action.
		if (bOneAction)
			prev = CreateWithTwoActions(prev, MtExtraAction.Create());
		return prev as MtSpawn;
	}

	public static MtSpawn CreateWithTwoActions(MtFiniteTimeAction actionOne, MtFiniteTimeAction actionTwo) {
		MtSpawn spawn = new MtSpawn();
		if (spawn.InitWithTwoActions(actionOne, actionTwo))
			return spawn;
		return null;
	}

	bool InitWithTwoActions(MtFiniteTimeAction actionOne, MtFiniteTimeAction actionTwo) {
		Debug.Assert(actionOne != null, "actionOne can't be null!");
		Debug.Assert(actionTwo != null, "actionTwo can't be null!");
		if (actionOne == null || actionTwo == null) {
			Debug.Log("MtSpawn.InitWithTwoActions error: action is null!!");
			return false;
		}
		bool ret = false;
		float d1 = actionOne.Duration;
		float d2 = actionTwo.Duration;

		if (base.InitWithDuration(Mathf.Max(d1, d2))) {
			m_one = actionOne;
			m_two = actionTwo;
			if (d1 > d2)
				m_two = MtSequence.CreateWithTwoActions(actionTwo, MtDelayTime.Create(d1 - d2));
			else if (d1 < d2)
				m_one = MtSequence.CreateWithTwoActions(actionOne, MtDelayTime.Create(d2 - d1));
			ret = true;
		}
		return ret;
	}

#region Override Functions
	public override void StartWithTarget(Transform target) {
		if (target == null) {
			Debug.Log("Spawn::startWithTarget error: target is nullptr!");
			return;
		}
		if (m_one == null || m_two == null) {
			Debug.Log("Spawn::startWithTarget error: _one or _two is nullptr!");
			return;
		}
		base.StartWithTarget(target);
		m_one.StartWithTarget(target);
		m_two.StartWithTarget(target);
	}

	public override void Update(float time) {
		if (m_one != null)
			m_one.Update(time);
		if (m_two != null)
			m_two.Update(time);
	}

	public override void Stop() {
		if (m_one != null)
			m_one.Stop();

		if (m_two != null)
			m_two.Stop();
		base.Stop();
	}

	public override MtICloneable Clone() {
		if (m_one != null && m_two != null)
			return MtSpawn.CreateWithTwoActions(m_one.Reverse() as MtFiniteTimeAction, m_two.Reverse() as MtFiniteTimeAction);
		return null;
	}

	public override MtAction Reverse() {
		if (m_one != null && m_two != null)
			return MtSpawn.CreateWithTwoActions(m_one.Reverse() as MtFiniteTimeAction, m_two.Reverse() as MtFiniteTimeAction);
		return null;
	}
#endregion
}
