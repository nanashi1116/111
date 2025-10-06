using System;
using DynamicMaps.Data;
using DynamicMaps.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace DynamicMaps.UI.Components
{
	// Token: 0x0200001A RID: 26
	public class MapLayer : MonoBehaviour
	{
		// Token: 0x17000017 RID: 23
		// (get) Token: 0x060000D3 RID: 211 RVA: 0x000075F9 File Offset: 0x000057F9
		// (set) Token: 0x060000D4 RID: 212 RVA: 0x00007601 File Offset: 0x00005801
		public string Name { get; private set; }

		// Token: 0x17000018 RID: 24
		// (get) Token: 0x060000D5 RID: 213 RVA: 0x0000760A File Offset: 0x0000580A
		// (set) Token: 0x060000D6 RID: 214 RVA: 0x00007612 File Offset: 0x00005812
		public Image Image { get; private set; }

		// Token: 0x17000019 RID: 25
		// (get) Token: 0x060000D7 RID: 215 RVA: 0x0000761B File Offset: 0x0000581B
		public RectTransform RectTransform
		{
			get
			{
				return base.gameObject.transform as RectTransform;
			}
		}

		// Token: 0x1700001A RID: 26
		// (get) Token: 0x060000D8 RID: 216 RVA: 0x0000762D File Offset: 0x0000582D
		public int Level
		{
			get
			{
				return this._def.Level;
			}
		}

		// Token: 0x1700001B RID: 27
		// (get) Token: 0x060000D9 RID: 217 RVA: 0x0000763A File Offset: 0x0000583A
		// (set) Token: 0x060000DA RID: 218 RVA: 0x00007642 File Offset: 0x00005842
		public LayerStatus Status { get; private set; }

		// Token: 0x1700001C RID: 28
		// (get) Token: 0x060000DB RID: 219 RVA: 0x0000764B File Offset: 0x0000584B
		// (set) Token: 0x060000DC RID: 220 RVA: 0x00007653 File Offset: 0x00005853
		public bool IsOnDefaultLevel { get; set; }

		// Token: 0x060000DD RID: 221 RVA: 0x0000765C File Offset: 0x0000585C
		public static MapLayer Create(GameObject parent, string name, MapLayerDef def, float degreesRotation)
		{
			GameObject gameObject = new GameObject(name, new Type[]
			{
				typeof(RectTransform),
				typeof(CanvasRenderer)
			});
			gameObject.layer = parent.layer;
			gameObject.transform.SetParent(parent.transform);
			gameObject.ResetRectTransform();
			RectTransform rectTransform = gameObject.GetRectTransform();
			MapLayer mapLayer = gameObject.AddComponent<MapLayer>();
			Vector2 rotatedRectangle = MathUtils.GetRotatedRectangle(def.ImageBounds.Max - def.ImageBounds.Min, degreesRotation);
			rectTransform.sizeDelta = rotatedRectangle;
			Vector2 midpoint = MathUtils.GetMidpoint(def.ImageBounds.Min, def.ImageBounds.Max);
			rectTransform.anchoredPosition = midpoint;
			rectTransform.localRotation = Quaternion.Euler(0f, 0f, degreesRotation);
			mapLayer.Name = name;
			mapLayer._def = def;
			mapLayer.Image = gameObject.AddComponent<Image>();
			mapLayer.Image.raycastTarget = false;
			mapLayer.Image.sprite = TextureUtils.GetOrLoadCachedSprite(def.ImagePath);
			mapLayer.Image.type = Image.Type.Simple;
			return mapLayer;
		}

		// Token: 0x060000DE RID: 222 RVA: 0x0000776C File Offset: 0x0000596C
		public bool IsCoordinateInLayer(Vector3 coordinate)
		{
			foreach (BoundingRectangularSolid boundingRectangularSolid in this._def.GameBounds)
			{
				if (coordinate.x > boundingRectangularSolid.Min.x && coordinate.x < boundingRectangularSolid.Max.x && coordinate.y > boundingRectangularSolid.Min.y && coordinate.y < boundingRectangularSolid.Max.y && coordinate.z > boundingRectangularSolid.Min.z && coordinate.z < boundingRectangularSolid.Max.z)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x060000DF RID: 223 RVA: 0x0000783C File Offset: 0x00005A3C
		public float GetMatchingBoundVolume(Vector3 coordinate)
		{
			foreach (BoundingRectangularSolid boundingRectangularSolid in this._def.GameBounds)
			{
				if (coordinate.x > boundingRectangularSolid.Min.x && coordinate.x < boundingRectangularSolid.Max.x && coordinate.y > boundingRectangularSolid.Min.y && coordinate.y < boundingRectangularSolid.Max.y && coordinate.z > boundingRectangularSolid.Min.z && coordinate.z < boundingRectangularSolid.Max.z)
				{
					return (boundingRectangularSolid.Max.x - boundingRectangularSolid.Min.x) * (boundingRectangularSolid.Max.y - boundingRectangularSolid.Min.y) * (boundingRectangularSolid.Max.z - boundingRectangularSolid.Min.z);
				}
			}
			return float.MinValue;
		}

		// Token: 0x060000E0 RID: 224 RVA: 0x00007960 File Offset: 0x00005B60
		public void OnTopLevelSelected(int newLevel)
		{
			this.Status = LayerStatus.Hidden;
			if (this.Level == newLevel)
			{
				this.Status = LayerStatus.OnTop;
			}
			else if (this.Level < newLevel)
			{
				this.Status = LayerStatus.Underneath;
			}
			bool active = true;
			int num = newLevel - this.Level;
			float num2 = Mathf.Clamp01(Mathf.Pow(MapLayer._fadeMultiplierPerLayer, (float)num));
			float a = 1f;
			if (this.Status == LayerStatus.Hidden)
			{
				active = false;
				if (this.IsOnDefaultLevel)
				{
					a = MapLayer._defaultLevelFallbackAlpha;
					active = true;
				}
			}
			base.gameObject.SetActive(active);
			this.Image.color = new Color(num2, num2, num2, a);
		}

		// Token: 0x04000094 RID: 148
		private static float _fadeMultiplierPerLayer = 0.5f;

		// Token: 0x04000095 RID: 149
		private static float _defaultLevelFallbackAlpha = 0.1f;

		// Token: 0x0400009A RID: 154
		private MapLayerDef _def = new MapLayerDef();
	}
}
