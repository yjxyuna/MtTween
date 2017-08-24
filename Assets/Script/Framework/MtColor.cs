using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class MaterialExtForMtShaderEffect {
	public static void SetStandardShaderRenderingMode(this Material material, string renderingMode) {
		if (!material.shader.name.Equals("Standard"))
			return;

		switch (renderingMode) {
			case "Opaque":
				material.SetFloat("_Mode", 0.0f);
				material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
				material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
				material.SetInt("_ZWrite", 1);
				material.DisableKeyword("_ALPHATEST_ON");
				material.DisableKeyword("_ALPHABLEND_ON");
				material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
				material.renderQueue = -1;
				break;

			case "Cutout":
				material.SetFloat("_Mode", 1.0f);
				material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
				material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
				material.SetInt("_ZWrite", 1);
				material.EnableKeyword("_ALPHATEST_ON");
				material.DisableKeyword("_ALPHABLEND_ON");
				material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
				material.renderQueue = 2450;
				break;

			case "Fade":
				material.SetFloat("_Mode", 2.0f);
				material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
				material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
				material.SetInt("_ZWrite", 0);
				material.DisableKeyword("_ALPHATEST_ON");
				material.EnableKeyword("_ALPHABLEND_ON");
				material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
				material.renderQueue = 3000;
				break;

			case "Transparent":
				material.SetFloat("_Mode", 3.0f);
				material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
				material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
				material.SetInt("_ZWrite", 0);
				material.DisableKeyword("_ALPHATEST_ON");
				material.DisableKeyword("_ALPHABLEND_ON");
				material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
				material.renderQueue = 3000;
				break;
		}
	}

	public static string GetStandardShaderRenderingMode(this Material material) {
		if (!material.shader.name.Equals("Standard"))
			return null;
		string strMode = null;
		int modeIndex = (int)material.GetFloat("_Mode");
		switch(modeIndex) {
			case 0:
				return "Opaque";
			case 1:
				return "Cutout";
			case 2:
				return "Fade";
			case 3:
				return "Transparent";
		}
		return null;
	}
}

public class MtColorTool {
	public static Color UnityColor(string hexColorValue) {
		try {
			int count = hexColorValue.Length;
			int r = Convert.ToInt32(hexColorValue.Substring(0, 2), 16);
			int g = Convert.ToInt32(hexColorValue.Substring(2, 2), 16);
			int b = Convert.ToInt32(hexColorValue.Substring(4, 2), 16);
			int a = 255;
			if (count == 8) 
				a = Convert.ToInt32(hexColorValue.Substring(6, 2), 16);
			return UnityColor(r, g, b, a);
		}catch (Exception e) {
			Debug.Log(e.Message);
			Debug.LogFormat("<color=yellow>{0}</color>", "十六进制颜色值格式错误,需输入6位或8位(带alpha)16进制值,如:0091EAFF,不加#或0x");
		}
		return Color.white;
	}

	public static Color UnityColor(int r, int g, int b, int a=255) {
		return new Color((float)r/255f, (float)g/255f, (float)b/255f, (float)a/255f);
	}
}

public class MtColor : MtActionInterval {

	protected List<Material> m_ltAllMaterials;

	public MtColor() {
		m_ltAllMaterials = new List<Material>();
	}

	protected void LoadMaterials(Transform trans) {
		Renderer render = trans.GetComponent<Renderer>();
		if (render) {
			List<Material> mats = new List<Material>();
			foreach (Material material in render.materials) {
				if (material.HasProperty("_Color")) {
					m_ltAllMaterials.Add(material);
				}
			}
		}
		foreach (Transform tf in trans) {
			LoadMaterials(tf);
		}
	}

	public void PreLoad(Transform target) {
		LoadMaterials(target);
	}

	#region Override Functions
	public override void StartWithTarget(Transform target) {
		base.StartWithTarget(target);
		if (m_ltAllMaterials.Count == 0)
			LoadMaterials(target);
	}

	public override void Stop() {

	}

	public override MtICloneable Clone() {
		return null;
	}

	public override MtAction Reverse() {
		Debug.Assert(false, "MtShaderEffect not support Reverse, use Restore!");
		return null;
	}

	public override void Update(float time) {

	}

	public virtual void Execute() {

	}
	#endregion
}

#region MtColorTo
public class MtColorTo : MtColor {

	protected Color m_to;
	protected List<Color> m_ltColorFrom;

	public MtColorTo() {
		m_ltColorFrom = new List<Color>();
	}

	public static MtColorTo Create(float duration, Color color) {
		return Create(duration, color.r, color.g, color.b, color.a);
	}

	public static MtColorTo Create(float duration, float red, float green, float blue, float alpha) {
		MtColorTo colorTo = new MtColorTo();
		if (colorTo.InitWithDuration(duration, red, green, blue, alpha)) 
			return  colorTo;
		return null;
	}

	protected bool InitWithDuration(float duration, float red, float green, float blue, float alpha) {
		if (base.InitWithDuration(duration)) {
			m_to = new Color(red, green, blue, alpha);
			return true;
		}
		return false;
	}

#region Override Functions
	public override void StartWithTarget(Transform target) {
		base.StartWithTarget(target);
		m_ltColorFrom.Clear();
		foreach (Material material in m_ltAllMaterials) {
			m_ltColorFrom.Add(material.color); 
		}
	}

	public override void Update(float time) {
		if (m_target != null) {
			for (int i = 0; i < m_ltAllMaterials.Count; i++) {
				Material material = m_ltAllMaterials[i];
				material.color = new Color(
					m_ltColorFrom[i].r + (m_to.r - m_ltColorFrom[i].r) * time,
					m_ltColorFrom[i].g + (m_to.g - m_ltColorFrom[i].g) * time,
					m_ltColorFrom[i].b + (m_to.b - m_ltColorFrom[i].b) * time,
					m_ltColorFrom[i].a + (m_to.a - m_ltColorFrom[i].a) * time);
			}
		}
	}

	public override void Stop() {
		base.Stop();
		m_ltColorFrom.Clear();
	}

	public override MtICloneable Clone() {
		return MtColorTo.Create(m_duration, m_to.r, m_to.g, m_to.b, m_to.a);
	}

	public override MtAction Reverse() {
		Debug.Assert(false, "reverse() not supported in MtColorTo is only temporary!");
		return null;
	}
#endregion 
}
#endregion


#region MtColorBy
public class MtColorBy : MtColor {

	protected float m_deltaR;
	protected float m_deltaG;
	protected float m_deltaB;
	protected float m_deltaA;
	protected List<Color> m_ltColorFrom;

	public MtColorBy() {
		m_ltColorFrom = new List<Color>();
	}

	public static MtColorBy Create(float duration, Color color) {
		return Create(duration, color.r, color.g, color.b, color.a);
	}

	public static MtColorBy Create(float duration, float deltaRed, float deltaGreen, float deltaBlue, float deltaAlpha) {
		MtColorBy colorBy = new MtColorBy();
		if (colorBy.InitWithDuration(duration, deltaRed, deltaGreen, deltaBlue, deltaAlpha)) 
			return  colorBy;
		return null;
	}

	protected bool InitWithDuration(float duration, float deltaRed, float deltaGreen, float deltaBlue, float deltaAlpha) {
		if (base.InitWithDuration(duration)) {
			m_deltaR = deltaRed;
			m_deltaG = deltaGreen;
			m_deltaB = deltaBlue;
			m_deltaA = deltaAlpha;
			return true;
		}
		return false;
	}

#region Override Functions
	public override void StartWithTarget(Transform target) {
		base.StartWithTarget(target);
		m_ltColorFrom.Clear();
		foreach (Material mat in m_ltAllMaterials) {
			m_ltColorFrom.Add(mat.color);
		}
	}

	public override void Update(float time) {
		if (m_target != null) {
			for (int i = 0; i < m_ltAllMaterials.Count; i++) {
				Material material = m_ltAllMaterials[i];
				material.color = new Color(
					m_ltColorFrom[i].r + m_deltaR * time,
					m_ltColorFrom[i].g + m_deltaG * time,
					m_ltColorFrom[i].b + m_deltaB * time,
					m_ltColorFrom[i].a + m_deltaA * time);
			}
		}
	}

	public override void Stop() {
		base.Stop();
		m_ltColorFrom.Clear();
	}

	public override MtICloneable Clone() {
		return MtColorBy.Create(m_duration, m_deltaR, m_deltaG, m_deltaB, m_deltaA);
	}

	public override MtAction Reverse() {
		return MtColorBy.Create(m_duration, -m_deltaR, -m_deltaG, -m_deltaB, -m_deltaA);
	}
#endregion
}
#endregion


#region MtColorTo
public class MtFadeTo : MtColorTo {

	protected float m_opacityTo;

	public MtFadeTo() {
		
	}
		
	public static MtFadeTo Create(float duration, float opacityTo) {
		MtFadeTo fadeTo = new MtFadeTo();
		if (fadeTo.InitWithDuration(duration, opacityTo)) {
			return fadeTo;
		}
		return null;
	}

	protected bool InitWithDuration(float duration,float opacityTo) {
		if (base .InitWithDuration(duration)) {
			m_opacityTo = opacityTo;
			return true;
		}
		return false;
	}

#region Override Functions
	public override void StartWithTarget(Transform target) {
		base.StartWithTarget(target);
		foreach (Material material in m_ltAllMaterials) {
			material.SetStandardShaderRenderingMode("Fade");
		}
	}

	public override void Update(float time) {
		if (m_target != null) {
			for (int i = 0; i < m_ltAllMaterials.Count; i++) {
				Material material = m_ltAllMaterials[i];
				material.color = new Color(
					m_ltColorFrom[i].r,
					m_ltColorFrom[i].g,
					m_ltColorFrom[i].b,
					m_ltColorFrom[i].a + (m_opacityTo - m_ltColorFrom[i].a) * time);
			}
		}
	}

	public override MtICloneable Clone() {
		return MtFadeTo.Create(m_duration, m_opacityTo);
	}

	public override MtAction Reverse() {
		Debug.Assert(false, "reverse() not supported in MtFadeTo is only temporary!");
		return null;
	}
#endregion 
}
#endregion


#region MtColorBy
public class MtFadeBy : MtColorBy {

	public MtFadeBy() {
		
	}

	public static MtFadeBy Create(float duration, float deltaAlpha) {
		MtFadeBy fadeBy = new MtFadeBy();
		if (fadeBy.InitWithDuration(duration, deltaAlpha)) 
			return  fadeBy;
		return null;
	}

	protected bool InitWithDuration(float duration, float deltaAlpha) {
		if (base.InitWithDuration(duration)) {
			m_deltaA = deltaAlpha;
			return true;
		}
		return false;
	}

#region Override Functions
	public override void Update(float time) {
		if (m_target != null) {
			for (int i = 0; i < m_ltAllMaterials.Count; i++) {
				Material material = m_ltAllMaterials[i];
				material.color = new Color(
					m_ltColorFrom[i].r,
					m_ltColorFrom[i].g,
					m_ltColorFrom[i].b,
					m_ltColorFrom[i].a + m_deltaA * time);
			}
		}
	}
		
	public override MtICloneable Clone() {
		return MtFadeBy.Create(m_duration, m_deltaA);
	}

	public override MtAction Reverse() {
		return MtFadeBy.Create(m_duration, -m_deltaA);
	}
#endregion
}
#endregion