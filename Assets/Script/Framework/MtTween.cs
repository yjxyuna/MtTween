using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static partial class TransformExtForMtTween {
	
	public static void RunAction(this Transform trans, MtAction action) {
		Debug.Assert(action != null, "MtAction must be non-nil");
		MtActionManager.Instance.AddAction(action, trans, false);
	}

	public static void PauseAllActions(this Transform trans) {
		MtActionManager.Instance.PauseTarget(trans);
	}  

	public static void ResumeAllActions(this Transform trans) {
		MtActionManager.Instance.ResumeTarget(trans);
	}

	public static void StopActionByTag(this Transform trans, int tag) {
		Debug.Assert(tag != MtAction.INVALID_TAG, "Invalid tag");
		MtActionManager.Instance.StopActionByTag(trans, tag); 
	}

	public static void StopAction(this Transform trans, MtAction action) {
		Debug.Assert(action != null, "MtAction must be non-nil");
		MtActionManager.Instance.StopAction(action);
	}

	public static void StopAllActions(this Transform trans) {
		MtActionManager.Instance.RemoveAllActionsFromTarget(trans);
	}

	public static int GetNumberOfRunningActions(this Transform trans) {
		return MtActionManager.Instance.GetNumberOfRunningActionsInTarget(trans);
	}

	public static MtAction GetActionByTag(this Transform trans, int tag) {
		Debug.Assert(tag != MtAction.INVALID_TAG, "Invalid tag");
		return MtActionManager.Instance.GetActionByTag(tag, trans);
	}

	public static ArrayList GetRunningActions(this Transform trans) {
		return MtActionManager.Instance.GetRunningActionsInTarget(trans);
	}
}

public class MtTween : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		MtActionManager.Instance.Update(Time.deltaTime);
	}
}
