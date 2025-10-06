using System;
using System.Collections.Generic;
using System.Linq;
using DynamicMaps.Data;
using DynamicMaps.Utils;
using EFT.UI;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace DynamicMaps.UI.Controls
{
	// Token: 0x02000014 RID: 20
	public class LevelSelectSlider : MonoBehaviour
	{
		// Token: 0x14000001 RID: 1
		// (add) Token: 0x06000099 RID: 153 RVA: 0x00006990 File Offset: 0x00004B90
		// (remove) Token: 0x0600009A RID: 154 RVA: 0x000069C8 File Offset: 0x00004BC8
		public event Action<int> OnLevelSelectedBySlider;

		// Token: 0x1700000C RID: 12
		// (get) Token: 0x0600009B RID: 155 RVA: 0x000069FD File Offset: 0x00004BFD
		public RectTransform RectTransform
		{
			get
			{
				return base.gameObject.transform as RectTransform;
			}
		}

		// Token: 0x1700000D RID: 13
		// (get) Token: 0x0600009C RID: 156 RVA: 0x00006A0F File Offset: 0x00004C0F
		// (set) Token: 0x0600009D RID: 157 RVA: 0x00006A18 File Offset: 0x00004C18
		public int SelectedLevel
		{
			get
			{
				return this._selectedLevel;
			}
			set
			{
				if (this._selectedLevel == value && !this._levels.Contains(value))
				{
					return;
				}
				if (this._levels.Count == 1)
				{
					this._scrollbar.value = 0.5f;
				}
				else
				{
					this._scrollbar.value = (float)this._levels.IndexOf(value) / ((float)this._levels.Count - 1f);
				}
				this._text.text = string.Format("Level {0}", value);
				this._selectedLevel = value;
			}
		}

		// Token: 0x0600009E RID: 158 RVA: 0x00006AAC File Offset: 0x00004CAC
		public static LevelSelectSlider Create(GameObject prefab, Transform parent)
		{
			GameObject gameObject = Object.Instantiate<GameObject>(prefab);
			gameObject.name = "LevelSelectScrollbar";
			gameObject.transform.SetParent(parent);
			gameObject.transform.localScale = Vector3.one;
			Vector2 anchoredPosition = gameObject.GetRectTransform().anchoredPosition;
			Object.Destroy(gameObject.GetComponent<MapZoomer>());
			return gameObject.AddComponent<LevelSelectSlider>();
		}

		// Token: 0x0600009F RID: 159 RVA: 0x00006B04 File Offset: 0x00004D04
		private void Awake()
		{
			GameObject gameObject = UIUtils.CreateUIGameObject(base.gameObject.transform.Find("Scrollbar/Sliding Area/Handle").gameObject, "SlidingLayerText");
			this._text = gameObject.AddComponent<TextMeshProUGUI>();
			this._text.fontSize = LevelSelectSlider._levelTextSize;
			this._text.alignment = TextAlignmentOptions.Left;
			this._text.GetRectTransform().offsetMin = Vector2.zero;
			this._text.GetRectTransform().offsetMax = Vector2.zero;
			this._text.GetRectTransform().anchoredPosition = LevelSelectSlider._levelTextOffset;
			this._hasSetOutline = UIUtils.TrySetTMPOutline(this._text);
			GameObject gameObject2 = base.gameObject.transform.Find("Scrollbar").gameObject;
			this._scrollbar = gameObject2.GetComponent<Scrollbar>();
			this._scrollbar.direction = Scrollbar.Direction.BottomToTop;
			this._scrollbar.onValueChanged.AddListener(new UnityAction<float>(this.OnScrollbarChanged));
			this._scrollbar.onValueChanged.SetPersistentListenerState(0, UnityEventCallState.Off);
			Button component = base.gameObject.transform.Find("Plus").gameObject.GetComponent<Button>();
			component.onClick.AddListener(delegate()
			{
				this.ChangeLevelBy(1);
			});
			component.onClick.SetPersistentListenerState(0, UnityEventCallState.Off);
			Button component2 = base.gameObject.transform.Find("Minus").gameObject.GetComponent<Button>();
			component2.onClick.AddListener(delegate()
			{
				this.ChangeLevelBy(-1);
			});
			component2.onClick.SetPersistentListenerState(0, UnityEventCallState.Off);
			base.gameObject.SetActive(false);
		}

		// Token: 0x060000A0 RID: 160 RVA: 0x00006CA3 File Offset: 0x00004EA3
		private void OnEnable()
		{
			if (this._hasSetOutline || this._text == null)
			{
				return;
			}
			this._hasSetOutline = UIUtils.TrySetTMPOutline(this._text);
			this._text.text = this._text.text;
		}

		// Token: 0x060000A1 RID: 161 RVA: 0x00006CE3 File Offset: 0x00004EE3
		private void OnDestroy()
		{
			this.OnLevelSelectedBySlider = null;
		}

		// Token: 0x060000A2 RID: 162 RVA: 0x00006CEC File Offset: 0x00004EEC
		private void OnScrollbarChanged(float newValue)
		{
			if (this._levels.Count == 1)
			{
				this._scrollbar.value = 0.5f;
				return;
			}
			int index = Mathf.RoundToInt(newValue * (float)(this._levels.Count - 1));
			int num = this._levels[index];
			if (this._selectedLevel != -2147483648 && this._selectedLevel != num)
			{
				Action<int> onLevelSelectedBySlider = this.OnLevelSelectedBySlider;
				if (onLevelSelectedBySlider == null)
				{
					return;
				}
				onLevelSelectedBySlider(num);
			}
		}

		// Token: 0x060000A3 RID: 163 RVA: 0x00006D64 File Offset: 0x00004F64
		public void ChangeLevelBy(int delta)
		{
			int num = this._levels.IndexOf(this._selectedLevel) + delta;
			if (num < 0 || num >= this._levels.Count)
			{
				return;
			}
			Action<int> onLevelSelectedBySlider = this.OnLevelSelectedBySlider;
			if (onLevelSelectedBySlider == null)
			{
				return;
			}
			onLevelSelectedBySlider(this._levels[num]);
		}

		// Token: 0x060000A4 RID: 164 RVA: 0x00006DB4 File Offset: 0x00004FB4
		public void OnLoadMap(MapDef mapDef, int initialLevel)
		{
			this._levels.Clear();
			this._selectedLevel = int.MinValue;
			HashSet<int> hashSet = new HashSet<int>();
			foreach (MapLayerDef mapLayerDef in mapDef.Layers.Values)
			{
				hashSet.Add(mapLayerDef.Level);
			}
			this._levels = Enumerable.ToList<int>(hashSet);
			this._levels.Sort();
			this._scrollbar.numberOfSteps = Enumerable.Count<int>(this._levels);
			this.SelectedLevel = initialLevel;
			if (Enumerable.Count<int>(this._levels) == 1)
			{
				base.gameObject.SetActive(false);
			}
		}

		// Token: 0x04000079 RID: 121
		private static float _levelTextSize = 15f;

		// Token: 0x0400007A RID: 122
		private static Vector2 _levelTextOffset = new Vector2(10f, 0f);

		// Token: 0x0400007C RID: 124
		private int _selectedLevel = int.MinValue;

		// Token: 0x0400007D RID: 125
		private TextMeshProUGUI _text;

		// Token: 0x0400007E RID: 126
		private Scrollbar _scrollbar;

		// Token: 0x0400007F RID: 127
		private List<int> _levels = new List<int>();

		// Token: 0x04000080 RID: 128
		private bool _hasSetOutline;
	}
}
