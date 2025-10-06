using System;
using DynamicMaps.Utils;
using TMPro;
using UnityEngine;

namespace DynamicMaps.UI.Controls
{
	// Token: 0x02000012 RID: 18
	public abstract class AbstractTextControl : MonoBehaviour
	{
		// Token: 0x1700000A RID: 10
		// (get) Token: 0x06000090 RID: 144 RVA: 0x00006831 File Offset: 0x00004A31
		// (set) Token: 0x06000091 RID: 145 RVA: 0x00006839 File Offset: 0x00004A39
		public TextMeshProUGUI Text { get; protected set; }

		// Token: 0x1700000B RID: 11
		// (get) Token: 0x06000092 RID: 146 RVA: 0x00006842 File Offset: 0x00004A42
		public RectTransform RectTransform
		{
			get
			{
				return base.gameObject.transform as RectTransform;
			}
		}

		// Token: 0x06000093 RID: 147 RVA: 0x00006854 File Offset: 0x00004A54
		public static T Create<T>(GameObject parent, string name, float fontSize) where T : AbstractTextControl
		{
			GameObject gameObject = UIUtils.CreateUIGameObject(parent, name);
			T t = gameObject.AddComponent<T>();
			t.Text = gameObject.AddComponent<TextMeshProUGUI>();
			t.Text.autoSizeTextContainer = true;
			t.Text.fontSize = fontSize;
			t.Text.alignment = TextAlignmentOptions.Left;
			t._hasSetOutline = UIUtils.TrySetTMPOutline(t.Text);
			return t;
		}

		// Token: 0x06000094 RID: 148 RVA: 0x000068D4 File Offset: 0x00004AD4
		private void OnEnable()
		{
			if (this._hasSetOutline || this.Text == null)
			{
				return;
			}
			this._hasSetOutline = UIUtils.TrySetTMPOutline(this.Text);
			this.Text.text = this.Text.text;
		}

		// Token: 0x04000077 RID: 119
		private bool _hasSetOutline;
	}
}
