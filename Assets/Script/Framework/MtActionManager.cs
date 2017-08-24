using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MtActionElement
{
	public ArrayList     actions;
	public Transform     target;
	public int           actionIndex;
	public MtAction      currentAction;
	public bool 		 paused;
} 

public class MtActionManager {

	static MtActionManager m_instance;
	static object m_lock = new object();
	public static MtActionManager Instance {
		get {
			if (m_instance == null) {
				lock (m_lock) {
					if (m_instance == null)
						m_instance = new MtActionManager();
				}
			}
			return m_instance;
		}
	}

	LinkedList<MtActionElement> m_targets;
	LinkedListNode<MtActionElement> m_currentTarget;
	Dictionary<Transform, LinkedListNode<MtActionElement>> m_dicActionElement;

	GameObject m_goMtTween;

	public MtActionManager() {
		m_dicActionElement = new Dictionary<Transform, LinkedListNode<MtActionElement>>();
		m_targets = new LinkedList<MtActionElement>();
		CreateMonoBehaviourNode();
	}

	public void AddAction(MtAction action, Transform target, bool paused) {
		Debug.Assert(action != null, "action can't be null!");
		Debug.Assert(target != null, "target can't be null!");
		if(action == null || target == null)
			return;

		MtActionElement element = null;
		if (!m_dicActionElement.ContainsKey(target)) {
			element = new MtActionElement();
			element.paused = paused;
			element.target = target;
			element.actions = new ArrayList();
			LinkedListNode<MtActionElement> listNode = new LinkedListNode<MtActionElement>(element);
			m_dicActionElement.Add(target, listNode);
			m_targets.AddLast(listNode);
		} else {
			element = m_dicActionElement[target].Value;
		}
//		Debug.Assert(!element.actions.Contains(action), "Action already be added!");
		if (!element.actions.Contains(action)) {
			element.actions.Add(action);
			action.StartWithTarget(target);
		}
	}

	public void RemoveAction(MtAction action) {
		if (action == null)
			return;
		Transform tf = action.Target;
		if (m_dicActionElement.ContainsKey(tf)) {
			LinkedListNode<MtActionElement> element = m_dicActionElement[tf];
			int index = element.Value.actions.IndexOf(action);
			RemoveActionAtIndex(index, element);
		}
	}

	public void RemoveActionAtIndex(int index, LinkedListNode<MtActionElement> element) {
		MtAction action = element.Value.actions[index] as MtAction;
		action.Stop();
		element.Value.actions.RemoveAt(index);
		if (element.Value.actionIndex >= index)
			element.Value.actionIndex--;
		if (element.Value.actions.Count == 0) {
			m_dicActionElement.Remove(action.Target);
			LinkedListNode<MtActionElement> nodeTemp = element.Next;
			m_targets.Remove(element);
			m_currentTarget = nodeTemp;
		}
	}

	public void RemoveActionByTag(int tag, Transform target) {
		Debug.Assert(tag != MtAction.INVALID_TAG, "Invalid tag value!");
		Debug.Assert(target != null, "target can't be null!");
		if (target == null || !m_dicActionElement.ContainsKey(target))
			return;

		LinkedListNode<MtActionElement> element = m_dicActionElement[target];
		if (element != null) {
			int count = element.Value.actions.Count;
			for (int i = 0; i < count; i++) {
				MtAction action = element.Value.actions[i] as MtAction;
				action.Stop();
				if (action.Tag == tag && action.Target.Equals(target)) {
					RemoveActionAtIndex(i, element);
					break;
				}
			}
		}
	}

	public void RemoveAllActionsFromTarget(Transform target) {
		if (target == null || !m_dicActionElement.ContainsKey(target))
			return;

		LinkedListNode<MtActionElement> element = null;
		if (m_dicActionElement.ContainsKey (target)) {
			element = m_dicActionElement[target];
		}

		if (element != null) {
			foreach (var item in element.Value.actions) {
				MtFiniteTimeAction action = item as MtFiniteTimeAction;
				if (action != null)
					action.Stop();
			}
			element.Value.actions.Clear();
		}
		m_dicActionElement.Remove(target);
	}

	public MtAction GetActionByTag(int tag, Transform target) {
		Debug.Assert(tag != MtAction.INVALID_TAG, "Invalid tag value!");
		Debug.Assert(target != null, "target can't be null!");
		if (target == null || !m_dicActionElement.ContainsKey(target))
			return null;

		LinkedListNode<MtActionElement> element = m_dicActionElement[target];
		if (element != null) {
			int count = element.Value.actions.Count;
			for (int i = 0; i < count; i++) {
				MtAction action = element.Value.actions[i] as MtAction;
				if (action.Tag == tag && action.Target.Equals(target)) {
					return action;
				}
			}
		}
		return null;
	}

	public void PauseTarget(Transform target) {
		if (!m_dicActionElement.ContainsKey(target))
			return;
		LinkedListNode<MtActionElement> element = m_dicActionElement[target];
		if (element != null) {
			element.Value.paused = true;
		}
	}

	public void ResumeTarget(Transform target) {
		if (!m_dicActionElement.ContainsKey(target))
			return;
		LinkedListNode<MtActionElement> element = m_dicActionElement[target];
		if (element != null) {
			element.Value.paused = false;
		}
	}

	public ArrayList GetRunningActionsInTarget(Transform target) {
		if (!m_dicActionElement.ContainsKey(target))
			return null;
		LinkedListNode<MtActionElement> element = m_dicActionElement[target];
		if (element != null) {
			return element.Value.actions != null ? element.Value.actions : null;
		}
		return null;
	}

	public int GetNumberOfRunningActionsInTarget(Transform target) {
		if (!m_dicActionElement.ContainsKey(target))
			return 0;
		LinkedListNode<MtActionElement> element = m_dicActionElement[target];
		if (element != null) {
			return element.Value.actions != null ? element.Value.actions.Count : 0;
		}
		return 0;
	}

	public void StopAction(MtAction action) {
		RemoveAction(action);
	}

	public void StopActionByTag(Transform target, int tag) {
		RemoveActionByTag(tag, target);
	}
		
	/** Main loop of ActionManager.
     * @param dt    In seconds.
     */
	public void Update(float deltaTime) {

		for (m_currentTarget = m_targets.First; m_currentTarget != null;) {
			if (!m_currentTarget.Value.paused) {
				// The 'actions' ArrayList may change while inside this loop.
				for (m_currentTarget.Value.actionIndex = 0; m_currentTarget.Value.actionIndex < m_currentTarget.Value.actions.Count; m_currentTarget.Value.actionIndex++) {
					m_currentTarget.Value.currentAction = m_currentTarget.Value.actions[m_currentTarget.Value.actionIndex] as MtAction;
					if (m_currentTarget.Value.currentAction == null)
						continue;

					m_currentTarget.Value.currentAction.Step(deltaTime);

					if (m_currentTarget.Value.currentAction.IsDone()) {
						m_currentTarget.Value.currentAction.Stop();
						MtAction action = m_currentTarget.Value.currentAction;
						//Make currentAction null to prevent removeAction from salvaging it.
						m_currentTarget.Value.currentAction = null;
						RemoveAction(action);
					}
					if (m_currentTarget != null)
						m_currentTarget.Value.currentAction = null;
					else
						break;
				}
			}
			if (m_currentTarget != null)
				m_currentTarget = m_currentTarget.Next;
		}
		m_currentTarget = null;
	}

#region Private Functions
	private void CreateMonoBehaviourNode() {
		if (m_goMtTween == null) {
			m_goMtTween = new GameObject();
			m_goMtTween.name = "~MtTween";
			m_goMtTween.isStatic = true;
			m_goMtTween.AddComponent<MtTween>();
#if !UNITY_EDITOR
//			m_goMtTween.hideFlags = HideFlags.HideAndDontSave;
#endif
//			Object.DontDestroyOnLoad(m_goMtTween);
		}
	}
			
#endregion
}
