using System;
using System.Collections.Generic;
using DynamicMaps.Data;
using DynamicMaps.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DynamicMaps.UI.Components
{
	// Token: 0x0200001B RID: 27
	public class MapMarker : MonoBehaviour, ILayerBound, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
	{
		// Token: 0x1700001D RID: 29
		// (get) Token: 0x060000E3 RID: 227 RVA: 0x00007A1C File Offset: 0x00005C1C
		// (set) Token: 0x060000E4 RID: 228 RVA: 0x00007A23 File Offset: 0x00005C23
		public static Dictionary<string, Dictionary<LayerStatus, float>> CategoryImageAlphaLayerStatus { get; protected set; } = new Dictionary<string, Dictionary<LayerStatus, float>>
		{
			{
				"Extract",
				new Dictionary<LayerStatus, float>
				{
					{
						LayerStatus.Hidden,
						0.5f
					},
					{
						LayerStatus.Underneath,
						0.75f
					},
					{
						LayerStatus.OnTop,
						1f
					},
					{
						LayerStatus.FullReveal,
						1f
					}
				}
			},
			{
				"Secret",
				new Dictionary<LayerStatus, float>
				{
					{
						LayerStatus.Hidden,
						0.5f
					},
					{
						LayerStatus.Underneath,
						0.75f
					},
					{
						LayerStatus.OnTop,
						1f
					},
					{
						LayerStatus.FullReveal,
						1f
					}
				}
			},
			{
				"Transit",
				new Dictionary<LayerStatus, float>
				{
					{
						LayerStatus.Hidden,
						0.5f
					},
					{
						LayerStatus.Underneath,
						0.75f
					},
					{
						LayerStatus.OnTop,
						1f
					},
					{
						LayerStatus.FullReveal,
						1f
					}
				}
			},
			{
				"Quest",
				new Dictionary<LayerStatus, float>
				{
					{
						LayerStatus.Hidden,
						0.5f
					},
					{
						LayerStatus.Underneath,
						0.75f
					},
					{
						LayerStatus.OnTop,
						1f
					},
					{
						LayerStatus.FullReveal,
						1f
					}
				}
			},
			{
				"Airdrop",
				new Dictionary<LayerStatus, float>
				{
					{
						LayerStatus.Hidden,
						0.5f
					},
					{
						LayerStatus.Underneath,
						0.75f
					},
					{
						LayerStatus.OnTop,
						1f
					},
					{
						LayerStatus.FullReveal,
						1f
					}
				}
			}
		};

		// Token: 0x1700001E RID: 30
		// (get) Token: 0x060000E5 RID: 229 RVA: 0x00007A2B File Offset: 0x00005C2B
		// (set) Token: 0x060000E6 RID: 230 RVA: 0x00007A32 File Offset: 0x00005C32
		public static Dictionary<string, Dictionary<LayerStatus, float>> CategoryLabelAlphaLayerStatus { get; protected set; } = new Dictionary<string, Dictionary<LayerStatus, float>>
		{
			{
				"Extract",
				new Dictionary<LayerStatus, float>
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
				}
			},
			{
				"Secret",
				new Dictionary<LayerStatus, float>
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
				}
			},
			{
				"Transit",
				new Dictionary<LayerStatus, float>
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
				}
			},
			{
				"Quest",
				new Dictionary<LayerStatus, float>
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
				}
			}
		};

		// Token: 0x14000003 RID: 3
		// (add) Token: 0x060000E7 RID: 231 RVA: 0x00007A3C File Offset: 0x00005C3C
		// (remove) Token: 0x060000E8 RID: 232 RVA: 0x00007A74 File Offset: 0x00005C74
		public event Action<ILayerBound> OnPositionChanged;

		// Token: 0x1700001F RID: 31
		// (get) Token: 0x060000E9 RID: 233 RVA: 0x00007AA9 File Offset: 0x00005CA9
		// (set) Token: 0x060000EA RID: 234 RVA: 0x00007AB1 File Offset: 0x00005CB1
		public string Text { get; protected set; }

		// Token: 0x17000020 RID: 32
		// (get) Token: 0x060000EB RID: 235 RVA: 0x00007ABA File Offset: 0x00005CBA
		// (set) Token: 0x060000EC RID: 236 RVA: 0x00007AC2 File Offset: 0x00005CC2
		public string Category { get; protected set; }

		// Token: 0x17000021 RID: 33
		// (get) Token: 0x060000ED RID: 237 RVA: 0x00007ACB File Offset: 0x00005CCB
		// (set) Token: 0x060000EE RID: 238 RVA: 0x00007AD3 File Offset: 0x00005CD3
		public MapView ContainingMapView { get; set; }

		// Token: 0x17000022 RID: 34
		// (get) Token: 0x060000EF RID: 239 RVA: 0x00007ADC File Offset: 0x00005CDC
		// (set) Token: 0x060000F0 RID: 240 RVA: 0x00007AE4 File Offset: 0x00005CE4
		public Image Image { get; protected set; }

		// Token: 0x17000023 RID: 35
		// (get) Token: 0x060000F1 RID: 241 RVA: 0x00007AED File Offset: 0x00005CED
		// (set) Token: 0x060000F2 RID: 242 RVA: 0x00007AF5 File Offset: 0x00005CF5
		public TextMeshProUGUI Label { get; protected set; }

		// Token: 0x17000024 RID: 36
		// (get) Token: 0x060000F3 RID: 243 RVA: 0x00007AFE File Offset: 0x00005CFE
		public RectTransform RectTransform
		{
			get
			{
				return base.gameObject.transform as RectTransform;
			}
		}

		// Token: 0x17000025 RID: 37
		// (get) Token: 0x060000F4 RID: 244 RVA: 0x00007B10 File Offset: 0x00005D10
		// (set) Token: 0x060000F5 RID: 245 RVA: 0x00007B18 File Offset: 0x00005D18
		public string AssociatedItemId { get; protected set; } = "";

		// Token: 0x17000026 RID: 38
		// (get) Token: 0x060000F6 RID: 246 RVA: 0x00007B21 File Offset: 0x00005D21
		// (set) Token: 0x060000F7 RID: 247 RVA: 0x00007B29 File Offset: 0x00005D29
		public bool IsDynamic { get; protected set; }

		// Token: 0x17000027 RID: 39
		// (get) Token: 0x060000F8 RID: 248 RVA: 0x00007B32 File Offset: 0x00005D32
		// (set) Token: 0x060000F9 RID: 249 RVA: 0x00007B3A File Offset: 0x00005D3A
		public bool ShowInRaid { get; protected set; } = true;

		// Token: 0x17000028 RID: 40
		// (get) Token: 0x060000FA RID: 250 RVA: 0x00007B43 File Offset: 0x00005D43
		// (set) Token: 0x060000FB RID: 251 RVA: 0x00007B4B File Offset: 0x00005D4B
		public Vector3 Position
		{
			get
			{
				return this._position;
			}
			set
			{
				this.Move(value, true);
			}
		}

		// Token: 0x17000029 RID: 41
		// (get) Token: 0x060000FC RID: 252 RVA: 0x00007B55 File Offset: 0x00005D55
		// (set) Token: 0x060000FD RID: 253 RVA: 0x00007B5D File Offset: 0x00005D5D
		public float Rotation
		{
			get
			{
				return this._rotation;
			}
			set
			{
				this.SetRotation(value);
			}
		}

		// Token: 0x1700002A RID: 42
		// (get) Token: 0x060000FE RID: 254 RVA: 0x00007B66 File Offset: 0x00005D66
		// (set) Token: 0x060000FF RID: 255 RVA: 0x00007B6E File Offset: 0x00005D6E
		public Color Color
		{
			get
			{
				return this._color;
			}
			set
			{
				this._color = value;
				this.Image.color = value;
				this.Label.color = value;
			}
		}

		// Token: 0x1700002B RID: 43
		// (get) Token: 0x06000100 RID: 256 RVA: 0x00007B8F File Offset: 0x00005D8F
		// (set) Token: 0x06000101 RID: 257 RVA: 0x00007B98 File Offset: 0x00005D98
		public Vector2 Size
		{
			get
			{
				return this._size;
			}
			set
			{
				this._size = value;
				this.RectTransform.sizeDelta = this._size;
				this.Image.GetRectTransform().sizeDelta = this._size;
				this.Label.GetRectTransform().sizeDelta = this._size * MapMarker._labelSizeMultiplier;
			}
		}

		// Token: 0x1700002C RID: 44
		// (get) Token: 0x06000102 RID: 258 RVA: 0x00007BF3 File Offset: 0x00005DF3
		// (set) Token: 0x06000103 RID: 259 RVA: 0x00007BFB File Offset: 0x00005DFB
		public Dictionary<LayerStatus, float> ImageAlphaLayerStatus { get; protected set; } = new Dictionary<LayerStatus, float>
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

		// Token: 0x1700002D RID: 45
		// (get) Token: 0x06000104 RID: 260 RVA: 0x00007C04 File Offset: 0x00005E04
		// (set) Token: 0x06000105 RID: 261 RVA: 0x00007C0C File Offset: 0x00005E0C
		public Dictionary<LayerStatus, float> LabelAlphaLayerStatus { get; protected set; } = new Dictionary<LayerStatus, float>
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
				0f
			},
			{
				LayerStatus.FullReveal,
				1f
			}
		};

		// Token: 0x06000106 RID: 262 RVA: 0x00007C18 File Offset: 0x00005E18
		public static MapMarker Create(GameObject parent, MapMarkerDef def, Vector2 size, float degreesRotation, float scale)
		{
			MapMarker mapMarker = MapMarker.Create<MapMarker>(parent, def.Text, def.Category, def.ImagePath, def.Color, def.Position, size, def.Pivot, degreesRotation, scale, def.ShowInRaid, def.Sprite);
			mapMarker.AssociatedItemId = def.AssociatedItemId;
			return mapMarker;
		}

		// Token: 0x06000107 RID: 263 RVA: 0x00007C6C File Offset: 0x00005E6C
		public static T Create<T>(GameObject parent, string text, string category, string imageRelativePath, Color color, Vector3 position, Vector2 size, Vector2 pivot, float degreesRotation, float scale, bool showInRaid = true, Sprite sprite = null) where T : MapMarker
		{
			GameObject gameObject = UIUtils.CreateUIGameObject(parent, "MapMarker " + text);
			Image image = gameObject.AddComponent<Image>();
			image.color = Color.clear;
			image.raycastTarget = true;
			RectTransform rectTransform = gameObject.GetRectTransform();
			rectTransform.anchoredPosition = position;
			rectTransform.sizeDelta = size;
			rectTransform.localScale = scale * Vector2.one;
			rectTransform.localRotation = Quaternion.Euler(0f, 0f, degreesRotation);
			rectTransform.pivot = pivot;
			T t = gameObject.AddComponent<T>();
			t.Text = text;
			t.Category = category;
			t.Position = position;
			t._initialRotation = degreesRotation;
			t.ShowInRaid = showInRaid;
			GameObject gameObject2 = UIUtils.CreateUIGameObject(gameObject, "image");
			gameObject2.AddComponent<CanvasRenderer>();
			gameObject2.GetRectTransform().sizeDelta = size;
			gameObject2.GetRectTransform().pivot = new Vector2(0.5f, 0.5f);
			t.Image = gameObject2.AddComponent<Image>();
			t.Image.raycastTarget = false;
			t.Image.sprite = ((sprite == null) ? TextureUtils.GetOrLoadCachedSprite(imageRelativePath) : sprite);
			t.Image.type = Image.Type.Simple;
			GameObject gameObject3 = UIUtils.CreateUIGameObject(gameObject, "label");
			gameObject3.AddComponent<CanvasRenderer>();
			gameObject3.GetRectTransform().anchorMin = new Vector2(0.5f, 0f);
			gameObject3.GetRectTransform().anchorMax = new Vector2(0.5f, 0f);
			gameObject3.GetRectTransform().pivot = new Vector2(0.5f, 1f);
			gameObject3.GetRectTransform().sizeDelta = size * MapMarker._labelSizeMultiplier;
			t.Label = gameObject3.AddComponent<TextMeshProUGUI>();
			t.Label.alignment = TextAlignmentOptions.Top;
			t.Label.enableWordWrapping = true;
			t.Label.enableAutoSizing = true;
			t.Label.fontSizeMin = MapMarker._markerMinFontSize;
			t.Label.fontSizeMax = MapMarker._markerMaxFontSize;
			t.Label.text = t.Text;
			t._hasSetOutline = UIUtils.TrySetTMPOutline(t.Label);
			t.Color = color;
			t._size = size;
			t.Label.gameObject.SetActive(false);
			return t;
		}

		// Token: 0x06000108 RID: 264 RVA: 0x00007F12 File Offset: 0x00006112
		protected virtual void OnEnable()
		{
			this.TrySetOutlineAndResize();
		}

		// Token: 0x06000109 RID: 265 RVA: 0x00007F1A File Offset: 0x0000611A
		protected virtual void OnDestroy()
		{
			this.OnPositionChanged = null;
		}

		// Token: 0x0600010A RID: 266 RVA: 0x00007F23 File Offset: 0x00006123
		public void Move(Vector3 newPosition, bool callback = true)
		{
			this.RectTransform.anchoredPosition = newPosition;
			this._position = newPosition;
			if (callback)
			{
				Action<ILayerBound> onPositionChanged = this.OnPositionChanged;
				if (onPositionChanged == null)
				{
					return;
				}
				onPositionChanged(this);
			}
		}

		// Token: 0x0600010B RID: 267 RVA: 0x00007F51 File Offset: 0x00006151
		public void SetRotation(float degreesRotation)
		{
			this._rotation = degreesRotation;
			this.Image.gameObject.GetRectTransform().localRotation = Quaternion.Euler(0f, 0f, degreesRotation - this._initialRotation);
		}

		// Token: 0x0600010C RID: 268 RVA: 0x00007F86 File Offset: 0x00006186
		public void MoveAndRotate(Vector3 newPosition, float rotation, bool callback = true)
		{
			this.Move(newPosition, callback);
			this.SetRotation(rotation);
		}

		// Token: 0x0600010D RID: 269 RVA: 0x00007F98 File Offset: 0x00006198
		public void HandleNewLayerStatus(LayerStatus status)
		{
			if (!this.ShowInRaid && GameUtils.IsInRaid())
			{
				base.gameObject.SetActive(false);
				return;
			}
			if (this._isInFullReveal)
			{
				status = LayerStatus.FullReveal;
			}
			float num = this.ImageAlphaLayerStatus[status];
			float num2 = this.LabelAlphaLayerStatus[status];
			if (MapMarker.CategoryImageAlphaLayerStatus.ContainsKey(this.Category))
			{
				num = MapMarker.CategoryImageAlphaLayerStatus[this.Category][status];
			}
			if (MapMarker.CategoryLabelAlphaLayerStatus.ContainsKey(this.Category))
			{
				num2 = MapMarker.CategoryLabelAlphaLayerStatus[this.Category][status];
			}
			this.Image.color = new Color(this.Image.color.r, this.Image.color.g, this.Image.color.b, num);
			this.Label.color = new Color(this.Label.color.r, this.Label.color.g, this.Label.color.b, num2);
			this.Image.gameObject.SetActive(num > 0f);
			this.Label.gameObject.SetActive(num2 > 0f);
			base.gameObject.SetActive(num2 > 0f || num > 0f);
		}

		// Token: 0x0600010E RID: 270 RVA: 0x00008108 File Offset: 0x00006308
		public void OnPointerEnter(PointerEventData eventData)
		{
			this.TrySetOutlineAndResize();
			this._isInFullReveal = true;
			base.transform.SetAsLastSibling();
			this.HandleNewLayerStatus(LayerStatus.FullReveal);
		}

		// Token: 0x0600010F RID: 271 RVA: 0x00008129 File Offset: 0x00006329
		public void OnPointerExit(PointerEventData eventData)
		{
			this._isInFullReveal = false;
			Action<ILayerBound> onPositionChanged = this.OnPositionChanged;
			if (onPositionChanged == null)
			{
				return;
			}
			onPositionChanged(this);
		}

		// Token: 0x06000110 RID: 272 RVA: 0x00008144 File Offset: 0x00006344
		private void TrySetOutlineAndResize()
		{
			if (this._hasSetOutline || this.Label == null)
			{
				return;
			}
			this.Label.enableAutoSizing = true;
			this.Label.enableWordWrapping = true;
			this.Label.fontSizeMin = MapMarker._markerMinFontSize;
			this.Label.fontSizeMax = MapMarker._markerMaxFontSize;
			this.Label.alignment = TextAlignmentOptions.Top;
			this.Label.text = (this.Label.text ?? "");
			this._hasSetOutline = UIUtils.TrySetTMPOutline(this.Label);
		}

		// Token: 0x0400009D RID: 157
		private static Vector2 _labelSizeMultiplier = new Vector2(2.5f, 2f);

		// Token: 0x0400009E RID: 158
		private static float _markerMinFontSize = 9f;

		// Token: 0x0400009F RID: 159
		private static float _markerMaxFontSize = 13f;

		// Token: 0x040000A9 RID: 169
		private Vector3 _position;

		// Token: 0x040000AA RID: 170
		private float _rotation;

		// Token: 0x040000AB RID: 171
		private Color _color = Color.white;

		// Token: 0x040000AC RID: 172
		private Vector2 _size = new Vector2(30f, 30f);

		// Token: 0x040000AF RID: 175
		private float _initialRotation;

		// Token: 0x040000B0 RID: 176
		private bool _hasSetOutline;

		// Token: 0x040000B1 RID: 177
		private bool _isInFullReveal;
	}
}
