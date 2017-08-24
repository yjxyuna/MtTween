using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Reflection;

public class MtTweenExamples : MonoBehaviour {

	Text m_txtTitle;
	Text m_txtSubTitle;
	Button m_btnLast;
	Button m_btnReset;
	Button m_btnNext;

	Transform m_tfCube;
	int m_currTestIndex;
	ArrayList m_alDemos;
	MtActionDemo m_currentDemo;

	// Use this for initialization
	void Start () {

		m_txtTitle = GameObject.Find("Canvas/txt_title").GetComponent<Text>();
		m_txtSubTitle = GameObject.Find("Canvas/txt_subTitle").GetComponent<Text>();
		m_btnLast = GameObject.Find("Canvas/Buttons/btn_last").GetComponent<Button>();
		m_btnReset = GameObject.Find("Canvas/Buttons/btn_reset").GetComponent<Button>();
		m_btnNext = GameObject.Find("Canvas/Buttons/btn_next").GetComponent<Button>();
		m_tfCube = GameObject.Find("Objects/Cube").transform;

		m_btnLast.onClick.AddListener(LastExample);
		m_btnReset.onClick.AddListener(ResetExample);
		m_btnNext.onClick.AddListener(NextExample);

		InitDemos();
	}

	// Update is called once per frame
	void Update () {

 	}

	void InitDemos() {
		m_alDemos = new ArrayList();
		Assembly assembly = Assembly.GetAssembly(typeof(MtActionDemo));
		Type baseClass = typeof(MtActionDemo);
		Type[] arrType = assembly.GetTypes();  
		foreach (var type in arrType) {  
			if (type.IsSubclassOf(baseClass)) {
				MtActionDemo actionDemo = (MtActionDemo)Activator.CreateInstance(type);
				actionDemo.OnSubTitleChange = OnSubTitleChange;
				actionDemo.CubeTransform = m_tfCube;
				m_alDemos.Add(actionDemo);
			}
		}  
		RunDemo();
	}

	void LastExample() {
		if (m_currTestIndex > 0) {
			m_currTestIndex--;
		} else {
			m_currTestIndex = m_alDemos.Count - 1;
		}
		RunDemo();
	}

	void ResetExample() {
		RunDemo();
	}

	void NextExample() {
		if (m_currTestIndex < m_alDemos.Count - 1) {
			m_currTestIndex++;
		} else {
			m_currTestIndex = 0;
		}
		RunDemo();
	}

	void RunDemo() {
		if (m_currTestIndex < 0)
			return;
		if (m_currentDemo != null)
			m_currentDemo.Exit();
		m_currentDemo = m_alDemos[m_currTestIndex] as MtActionDemo;
		if (m_currentDemo != null)
			m_currentDemo.Enter();
		m_txtTitle.text = string.Format("{0}. {1}", m_currTestIndex+1, m_currentDemo.Title());
		m_txtSubTitle.text = m_currentDemo.SubTitle();
	}

	void OnSubTitleChange(string subTitle) {
		m_txtSubTitle.text = subTitle;
	}
}

public class MtActionDemo {

	public Action<string> OnSubTitleChange { get; set; }
	public Transform CubeTransform {
		get { return m_tfCube; }
		set { m_tfCube = value; }
	}

	protected Transform m_tfCube;
	protected Vector3 m_originalPosition;
	protected Vector3 m_originalRotation;
	protected Vector3 m_originalScale;

	public MtActionDemo() {

	}

	public virtual string Title() {
		return GetType().ToString();
	}

	public virtual string SubTitle() {
		return "";
	}

	public virtual void Enter() {
		Debug.Assert(m_tfCube != null, "Cube transform is null!");
		if (m_tfCube == null)
			return;
		m_originalPosition = m_tfCube.position;
		m_originalRotation = m_tfCube.rotation.eulerAngles;
		m_originalScale = m_tfCube.localScale;
	}

	public virtual void Exit() {
		Debug.Assert(m_tfCube != null, "Cube transform is null!");
		if (m_tfCube == null)
			return;
		m_tfCube.position = m_originalPosition;
		m_tfCube.eulerAngles = m_originalRotation;
		m_tfCube.localScale = m_originalScale;
		m_tfCube.StopAllActions();
	}

	protected void SetSubTitle(string subTitle) {
		if (OnSubTitleChange != null)
			OnSubTitleChange(subTitle);
	}
}

#region MoveDemo
public class MoveDemo : MtActionDemo{

	public override void Enter() {
		base.Enter();
		var move = MtMoveBy.Create(1, new Vector3(3, 0, 0));
		m_tfCube.RunAction(move);
	}
}
#endregion

#region RotateDemo
public class RotateDemo : MtActionDemo{

	public override void Enter() {
		base.Enter();
		var rotate = MtRotateBy.Create(2, new Vector3(0, 360, 0));
		m_tfCube.RunAction(rotate);
	}
}
#endregion

#region ScaleDemo
public class ScaleDemo : MtActionDemo{

	public override void Enter() {
		base.Enter();
		var scale = MtScaleTo.Create(1, new Vector3(1.5f, 1.5f, 1.5f));
		m_tfCube.RunAction(scale);
	}
}
#endregion

#region RepeatDemo
public class RepeatDemo : MtActionDemo{

	public override void Enter() {
		base.Enter();
		var scale1 = MtScaleTo.Create(0.5f, new Vector3(1.5f, 1.5f, 1.5f));
		var scale2 = MtScaleTo.Create(0.5f, Vector3.one);
		var sequence = MtSequence.Create(scale1, scale2);
		var rotate = MtRotateBy.Create(1, new Vector3(0, 360, 0));
		var spawn = MtSpawn.Create(sequence, rotate);
		var repeat = MtRepeat.Create(spawn, 3);
		m_tfCube.RunAction(repeat);
	}

	public override string SubTitle() {
		return "MtRepeat只限有限次数的循环,无限循环使用MtRepeatForever";
	}
}
#endregion

#region RepeatForeverDemo
public class RepeatForeverDemo : MtActionDemo{

	public override void Enter() {
		base.Enter();
		var scale1 = MtScaleTo.Create(0.5f, new Vector3(1.5f, 1.5f, 1.5f));
		var scale2 = MtScaleTo.Create(0.5f, Vector3.one);
		var sequence = MtSequence.Create(scale1, scale2);
		var rotate = MtRotateBy.Create(1, new Vector3(0, 360, 0));
		var spawn = MtSpawn.Create(sequence, rotate);
		var repeatForever = MtRepeatForever.Create(spawn);
		m_tfCube.RunAction(repeatForever);
	}

	public override string SubTitle() {
		return "MtRepeatForever不支持MtSequence队列化..";
	}
}
#endregion

#region CallFuncDemo
public class CallFuncDemo : MtActionDemo{

	public override void Enter() {
		base.Enter();
		var func = MtCallFunc.Create((Transform trans) => {
			Debug.Log(trans.name);
		});
		m_tfCube.RunAction(func);
	}

	public override string SubTitle() {
		return "CallFuncDemo可在MtSequence中用作回调";
	}
}
#endregion

#region ValueDemo
public class ValueDemo : MtActionDemo{

	public override void Enter() {
		base.Enter();
		var valueAct = MtActionFloat.Create(4, 0, 1.5f, (float value) => {
			SetSubTitle(string.Format("Value用4秒时间从0变化到1.5,当前值为:{0}", value.ToString("F3")));
		});
		m_tfCube.RunAction(valueAct);
	}

	public override string SubTitle() {
		return "CallFuncDemo可在MtSequence中用作回调";
	}
}
#endregion

#region DelayTimeDemo
public class DelayTimeDemo : MtActionDemo{

	public override void Enter() {
		base.Enter();
		var delayTime = MtDelayTime.Create(2);
		var func = MtCallFunc.Create (()=>{
			SetSubTitle("2秒时间到");
		});
		m_tfCube.RunAction(MtSequence.Create(delayTime, func));
	}

	public override string SubTitle() {
		return "2秒后执行下一个动作!";
	}
}
#endregion

#region BezierDemo
public class BezierDemo : MtActionDemo{

	public override void Enter() {
		base.Enter();
		m_tfCube.position = new Vector3(-5.31f, 1.49f, 0);
		var berzier = MtBezierTo.Create(1, new Vector3(-1.98f, 7f, 0), new Vector3(6.58f, 4f, 0), new Vector3(1.26f, 1.49f, 0));
		m_tfCube.RunAction(berzier);
	}
}
#endregion

#region ColorDemo
public class ColorDemo : MtActionDemo{

	public override void Enter() {
		base.Enter();
		var color = MtColorTo.Create(3, Color.blue);
		m_tfCube.RunAction(color);
	}

	public override void Exit() {
		base.Exit();
		m_tfCube.GetComponent<Renderer>().material.color = Color.white;
	}

	public override string SubTitle() {
		return "用3秒时间变成蓝色";
	}
}
#endregion

#region FadeDemo
public class FadeDemo : MtActionDemo{

	public override void Enter() {
		base.Enter();
		var opacity = MtFadeTo.Create(3, 0.2f);
		m_tfCube.RunAction(opacity);
	}

	public override void Exit() {
		base.Exit();
		m_tfCube.GetComponent<Renderer>().material.color = Color.white;
	}

	public override string SubTitle() {
		return "用3秒时间透明度降到30%";
	}
}
#endregion

#region BlinkDemo
public class BlinkDemo : MtActionDemo{

	public override void Enter() {
		base.Enter();
		var blink = MtBlink.Create(2, 5);
		m_tfCube.RunAction(blink);
	}

	public override string SubTitle() {
		return "2秒闪烁5次..";
	}
}
#endregion

#region SequenceDemo
public class SequenceDemo : MtActionDemo{

	public override void Enter() {
		base.Enter();
		var move1 = MtMoveBy.Create(0.5f, new Vector3(3, 0, 0));
		var move2 = MtMoveBy.Create(0.5f, new Vector3(0, 3, 0));
		var func = MtCallFunc.Create((Transform trans) => {
			SetSubTitle(string.Format("名字为{0}的GameObject走完一周...", trans.name));
		});
		var sequence1 = MtSequence.Create(move1, move2);
		var sequence2 = MtSequence.Create(sequence1, sequence1.Reverse() as MtSequence, func);
		m_tfCube.RunAction(sequence2);
	}

	public override string SubTitle() {
		return string.Format("{0}开始Move", m_tfCube.name);
	}
}
#endregion

#region SpawnDemo
public class SpawnDemo : MtActionDemo{

	public override void Enter() {
		base.Enter();
		var scale1 = MtScaleTo.Create(0.5f, new Vector3(1.5f, 1.5f, 1.5f));
		var scale2 = MtScaleTo.Create(0.5f, Vector3.one);
		var sequence = MtSequence.Create(scale1, scale2);
		var rotate = MtRotateBy.Create(1, new Vector3(0, 360, 0));
		var spawn = MtSpawn.Create(sequence, rotate);
		m_tfCube.RunAction(spawn);
	}

	public override string SubTitle() {
		return "旋转同时进行缩放";
	}
}
#endregion

#region SpeedControlDemo
public class SpeedControlDemo : MtActionDemo{

	int m_count;

	public override void Enter() {
		base.Enter();

		MtSpeed speedCtrl = null;
		var rotate = MtRotateBy.Create(1f, new Vector3(0, 360, 0));
		var func = MtCallFunc.Create(()=>{
			if (m_count > 0)
				speedCtrl.Speed = 4f;
			if (m_count > 6)
				speedCtrl.Speed = 0.25f;
			if (m_count > 7) {
				speedCtrl.Speed = 1;
				m_count = -1;
			}
			m_count++;
		});
		var sequence = MtSequence.Create(rotate, func);
		var repeatForever = MtRepeatForever.Create(sequence);
		speedCtrl = MtSpeed.Create(repeatForever, 1);
		m_tfCube.RunAction(speedCtrl);
	}

	public override void Exit() {
		base.Exit();
		m_count = 0;
	}

	public override string SubTitle() {
		return "正常速度->4倍速度->1/4倍速度..";
	}
}
#endregion

#region RotateAround
public class RotateAroundDemo : MtActionDemo{

	public override void Enter() {
		base.Enter();

		var rotateAround = MtRotateAround.Create(2, new Vector3(0, 0, 2), Vector3.up, 360);
		m_tfCube.RunAction(rotateAround);
	}

	public override string SubTitle() {
		return "围绕世界空间下某点和某轴向为参数,旋转指定度数";
	}
}
#endregion

#region EaseExpoInOut
public class EaseExpoInOutDemo : MtActionDemo{

	public override void Enter() {
		base.Enter();

		var move = MtMoveBy.Create(1, new Vector3(3, 0, 0));
		var easeIn = MtEaseExponentialIn.Create(move);
		var moveBack = MtMoveBy.Create(1, new Vector3(-3, 0, 0));
		var easeOut = MtEaseExponentialOut.Create(moveBack);
		var sequence = MtSequence.Create(easeIn, easeOut);
		m_tfCube.RunAction(sequence);
	}

	public override string SubTitle() {
		return "指数曲线动画,先EaseIn后EaseOut";
	}
}
#endregion