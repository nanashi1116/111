using System;
using System.Collections.Generic;
using DynamicMaps.Data;
using DynamicMaps.Utils;
using TMPro;
using UnityEngine;

namespace DynamicMaps.UI.Components
{
	// Token: 0x02000017 RID: 23
	public class MapLabel : MonoBehaviour, ILayerBound
	{
		// Token: 0x1700000F RID: 15
		// (get) Token: 0x060000BF RID: 191 RVA: 0x00007367 File Offset: 0x00005567
		// (set) Token: 0x060000C0 RID: 192 RVA: 0x0000736F File Offset: 0x0000556F
		public string Text { get; protected set; }

		// Token: 0x17000010 RID: 16
		// (get) Token: 0x060000C1 RID: 193 RVA: 0x00007378 File Offset: 0x00005578
		// (set) Token: 0x060000C2 RID: 194 RVA: 0x00007380 File Offset: 0x00005580
		public string Category { get; protected set; }

		// Token: 0x17000011 RID: 17
		// (get) Token: 0x060000C3 RID: 195 RVA: 0x00007389 File Offset: 0x00005589
		// (set) Token: 0x060000C4 RID: 196 RVA: 0x00007391 File Offset: 0x00005591
		public TextMeshProUGUI Label { get; protected set; }

		// Token: 0x17000012 RID: 18
		// (get) Token: 0x060000C5 RID: 197 RVA: 0x0000739A File Offset: 0x0000559A
		public RectTransform RectTransform
		{
			get
			{
				return base.gameObject.transform as RectTransform;
			}
		}

		// Token: 0x17000013 RID: 19
		// (get) Token: 0x060000C6 RID: 198 RVA: 0x000073AC File Offset: 0x000055AC
		// (set) Token: 0x060000C7 RID: 199 RVA: 0x000073B4 File Offset: 0x000055B4
		public Vector3 Position
		{
			get
			{
				return this._position;
			}
			set
			{
				base.gameObject.GetRectTransform().anchoredPosition = value;
				this._position = value;
			}
		}

		// Token: 0x17000014 RID: 20
		// (get) Token: 0x060000C8 RID: 200 RVA: 0x000073D3 File Offset: 0x000055D3
		// (set) Token: 0x060000C9 RID: 201 RVA: 0x000073DB File Offset: 0x000055DB
		public Color Color
		{
			get
			{
				return this._color;
			}
			set
			{
				this._color = value;
				this.Label.color = value;
			}
		}

		// Token: 0x17000015 RID: 21
		// (get) Token: 0x060000CA RID: 202 RVA: 0x000073F0 File Offset: 0x000055F0
		// (set) Token: 0x060000CB RID: 203 RVA: 0x000073F8 File Offset: 0x000055F8
		public Dictionary<LayerStatus, float> LabelAlphaLayerStatus { get; set; } = new Dictionary<LayerStatus, float>
		{
			{
				LayerStatus.Hidden,
				0f
			},
			{
				LayerStatus.Underneath,
				0f
			},
			{
				LayerStatus.OnTop,
				1f
			},
			{
				LayerStatus.FullReveal,
				1f
			}
		};

		// Token: 0x060000CC RID: 204 RVA: 0x00007404 File Offset: 0x00005604
		public static MapLabel Create(GameObject parent, MapLabelDef def, float degreesRotation, float scale)
		{
			GameObject gameObject = UIUtils.CreateUIGameObject(parent, "MapLabel " + def.Text);
			RectTransform rectTransform = gameObject.GetRectTransform();
			rectTransform.anchoredPosition = def.Position;
			rectTransform.localScale = scale * Vector2.one;
			rectTransform.localRotation = Quaternion.Euler(0f, 0f, degreesRotation - def.DegreesRotation);
			MapLabel mapLabel = gameObject.AddComponent<MapLabel>();
			mapLabel.Text = def.Text;
			mapLabel.Category = def.Category;
			mapLabel.Position = def.Position;
			mapLabel.Label = gameObject.AddComponent<TextMeshProUGUI>();
			mapLabel.Color = def.Color;
			mapLabel.Label.autoSizeTextContainer = true;
			mapLabel.Label.fontSize = def.FontSize;
			mapLabel.Label.alignment = TextAlignmentOptions.Center;
			mapLabel.Label.text = mapLabel.Text;
			mapLabel._hasSetOutline = UIUtils.TrySetTMPOutline(mapLabel.Label);
			return mapLabel;
		}

		// Token: 0x060000CD RID: 205 RVA: 0x00007504 File Offset: 0x00005704
		private void OnEnable()
		{
			if (this._hasSetOutline || this.Label == null)
			{
				return;
			}
			this._hasSetOutline = UIUtils.TrySetTMPOutline(this.Label);
			this.Label.text = this.Label.text;
		}

		// Token: 0x060000CE RID: 206 RVA: 0x00007544 File Offset: 0x00005744
		public void HandleNewLayerStatus(LayerStatus status)
		{
			this.Label.color = new Color(this.Label.color.r, this.Label.color.g, this.Label.color.b, this.LabelAlphaLayerStatus[status]);
		}

		// Token: 0x0400008B RID: 139
		private Vector3 _position;

		// Token: 0x0400008C RID: 140
		private Color _color = Color.white;

		// Token: 0x0400008E RID: 142
		private bool _hasSetOutline;
	}
}
