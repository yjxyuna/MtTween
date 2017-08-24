using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MtSequence : MtActionInterval {

	MtFiniteTimeAction[] m_actions;
	float m_split;
	int m_last;

	public MtSequence() {
		m_actions = new MtFiniteTimeAction[2];
	}

	public static MtSequence Create(MtFiniteTimeAction action1, params MtFiniteTimeAction[] actions) {
		MtSequence ret = CreateWithVariableList(action1, actions);
		return ret;
	}

	public static MtSequence Create(List<MtFiniteTimeAction> listActions) {
		Debug.Assert(false, "...");
		return null;
	}

	public static MtSequence CreateWithVariableList(MtFiniteTimeAction action1, params MtFiniteTimeAction[] actions) {
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
		return prev as MtSequence;
	}

	public static MtSequence CreateWithTwoActions(MtFiniteTimeAction actionOne, MtFiniteTimeAction actionTwo) {
		MtSequence sequence = new MtSequence();
		if (sequence.InitWithTwoActions(actionOne, actionTwo))
			return sequence;
		return null;
	}

	bool InitWithTwoActions(MtFiniteTimeAction actionOne, MtFiniteTimeAction actionTwo) {
		Debug.Assert(actionOne != null, "actionOne can't be null!");
		Debug.Assert(actionTwo != null, "actionTwo can't be null!");
		if (actionOne == null || actionTwo == null) {
			Debug.Log("MtSequence.InitWithTwoActions error: action is null!!");
			return false;
		}
		float d = actionOne.Duration + actionTwo.Duration;
		base.InitWithDuration(d);
		m_actions[0] = actionOne;
		m_actions[1] = actionTwo;
		return true;
	}

#region Override Functions
	public override void StartWithTarget(Transform target) {
		if (target == null) {
			Debug.Log("Sequence::startWithTarget error: target is nullptr!");
			return;
		}
		if (m_actions[0] == null || m_actions[1] == null) {
			Debug.Log("Sequence::startWithTarget error: m_actions[0] or m_actions[1] is null!");
			return;
		}
		if (m_duration > Mathf.Epsilon)
			m_split = m_actions[0].Duration / m_duration;

		base.StartWithTarget(target);
		m_last = -1;
	}

	public override void Stop() {
		if(m_last != - 1 && m_actions[m_last] != null)
			m_actions[m_last].Stop();
		base.Stop();
	}

	public override void Update(float time) {
		int found = 0;
		float new_t = 0.0f;
		if(time < m_split) {
			//action[0]
			found = 0;
			if(m_split != 0)
				new_t = time / m_split;
			else
				new_t = 1;
		} else {
			//action[1]
			found = 1;
			if (m_split == 1)
				new_t = 1;
			else
				new_t = (time-m_split) / (1-m_split);
		}

		if (found == 1) {
			if(m_last == -1) {
				// action[0] was skipped, execute it.
				m_actions[0].StartWithTarget(m_target);
				m_actions[0].Update(1.0f);
				m_actions[0].Stop();
			} else if(m_last == 0) {
				// switching to action 1. stop action 0.
				m_actions[0].Update(1.0f);
				m_actions[0].Stop();
			}
		} else if(found==0 && m_last==1) {
			// Reverse mode ?
			// FIXME: Bug. this case doesn't contemplate when _last==-1, found=0 and in "reverse mode"
			// since it will require a hack to know if an action is on reverse mode or not.
			// "step" should be overridden, and the "reverseMode" value propagated to inner Sequences.
			m_actions[1].Update(1.0f);
			m_actions[1].Stop();
		}
		// Last action found and it is done.
		if(found == m_last && m_actions[found].IsDone())
			return;

		// Last action found and it is done
		if(found != m_last) {
			m_actions[found].StartWithTarget(m_target);
		}

		m_actions[found].Update(new_t);
		m_last = found;
	}

	public override MtICloneable Clone() {
		// no copy constructor
		if (m_actions[0] != null && m_actions[1] != null)
			return MtSequence.Create(m_actions[0].Clone() as MtFiniteTimeAction, m_actions[1].Clone() as MtFiniteTimeAction, null);
		else 
			return null;
	}

	public override MtAction Reverse() {
		if (m_actions[0] != null && m_actions[1] != null)
			return MtSequence.Create(m_actions[0].Reverse() as MtFiniteTimeAction, m_actions[1].Reverse() as MtFiniteTimeAction, null);
		else 
			return null;
	}
#endregion
}
