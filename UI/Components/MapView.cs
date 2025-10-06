using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using DynamicMaps.Config;
using DynamicMaps.Data;
using DynamicMaps.Utils;
using EFT;
using UnityEngine;
using UnityEngine.UI;

namespace DynamicMaps.UI.Components
{
	// Token: 0x0200001D RID: 29
	public class MapView : MonoBehaviour
	{
		// Token: 0x14000004 RID: 4
		// (add) Token: 0x06000130 RID: 304 RVA: 0x00008880 File Offset: 0x00006A80
		// (remove) Token: 0x06000131 RID: 305 RVA: 0x000088B8 File Offset: 0x00006AB8
		public event Action<int> OnLevelSelected;

		// Token: 0x17000038 RID: 56
		// (get) Token: 0x06000132 RID: 306 RVA: 0x000088ED File Offset: 0x00006AED
		public RectTransform RectTransform
		{
			get
			{
				return base.gameObject.transform as RectTransform;
			}
		}

		// Token: 0x17000039 RID: 57
		// (get) Token: 0x06000133 RID: 307 RVA: 0x000088FF File Offset: 0x00006AFF
		// (set) Token: 0x06000134 RID: 308 RVA: 0x00008907 File Offset: 0x00006B07
		public MapDef CurrentMapDef { get; private set; }

		// Token: 0x1700003A RID: 58
		// (get) Token: 0x06000135 RID: 309 RVA: 0x00008910 File Offset: 0x00006B10
		// (set) Token: 0x06000136 RID: 310 RVA: 0x00008918 File Offset: 0x00006B18
		public float CoordinateRotation { get; private set; }

		// Token: 0x1700003B RID: 59
		// (get) Token: 0x06000137 RID: 311 RVA: 0x00008921 File Offset: 0x00006B21
		// (set) Token: 0x06000138 RID: 312 RVA: 0x00008929 File Offset: 0x00006B29
		public int SelectedLevel { get; private set; }

		// Token: 0x1700003C RID: 60
		// (get) Token: 0x06000139 RID: 313 RVA: 0x00008932 File Offset: 0x00006B32
		// (set) Token: 0x0600013A RID: 314 RVA: 0x0000893A File Offset: 0x00006B3A
		public GameObject MapMarkerContainer { get; private set; }

		// Token: 0x1700003D RID: 61
		// (get) Token: 0x0600013B RID: 315 RVA: 0x00008943 File Offset: 0x00006B43
		// (set) Token: 0x0600013C RID: 316 RVA: 0x0000894B File Offset: 0x00006B4B
		public GameObject MapLabelsContainer { get; private set; }

		// Token: 0x1700003E RID: 62
		// (get) Token: 0x0600013D RID: 317 RVA: 0x00008954 File Offset: 0x00006B54
		// (set) Token: 0x0600013E RID: 318 RVA: 0x0000895C File Offset: 0x00006B5C
		public GameObject MapLayerContainer { get; private set; }

		// Token: 0x1700003F RID: 63
		// (get) Token: 0x0600013F RID: 319 RVA: 0x00008965 File Offset: 0x00006B65
		// (set) Token: 0x06000140 RID: 320 RVA: 0x0000896D File Offset: 0x00006B6D
		public float ZoomMin { get; private set; }

		// Token: 0x17000040 RID: 64
		// (get) Token: 0x06000141 RID: 321 RVA: 0x00008976 File Offset: 0x00006B76
		// (set) Token: 0x06000142 RID: 322 RVA: 0x0000897E File Offset: 0x00006B7E
		public float ZoomMax { get; private set; }

		// Token: 0x17000041 RID: 65
		// (get) Token: 0x06000143 RID: 323 RVA: 0x00008987 File Offset: 0x00006B87
		// (set) Token: 0x06000144 RID: 324 RVA: 0x0000898F File Offset: 0x00006B8F
		public float ZoomMain { get; set; } = Settings.ZoomMainMap.Value;

		// Token: 0x17000042 RID: 66
		// (get) Token: 0x06000145 RID: 325 RVA: 0x00008998 File Offset: 0x00006B98
		// (set) Token: 0x06000146 RID: 326 RVA: 0x000089A0 File Offset: 0x00006BA0
		public float ZoomMini { get; set; } = Settings.ZoomMiniMap.Value;

		// Token: 0x17000043 RID: 67
		// (get) Token: 0x06000147 RID: 327 RVA: 0x000089A9 File Offset: 0x00006BA9
		// (set) Token: 0x06000148 RID: 328 RVA: 0x000089B1 File Offset: 0x00006BB1
		public float ZoomCurrent { get; private set; }

		// Token: 0x17000044 RID: 68
		// (get) Token: 0x06000149 RID: 329 RVA: 0x000089BA File Offset: 0x00006BBA
		// (set) Token: 0x0600014A RID: 330 RVA: 0x000089C2 File Offset: 0x00006BC2
		public Vector2 MainMapPos { get; private set; } = Vector2.zero;

		// Token: 0x0600014B RID: 331 RVA: 0x000089CB File Offset: 0x00006BCB
		public static MapView Create(GameObject parent, string name)
		{
			GameObject gameObject = UIUtils.CreateUIGameObject(parent, name);
			gameObject.AddComponent<Canvas>();
			gameObject.AddComponent<GraphicRaycaster>();
			return gameObject.AddComponent<MapView>();
		}

		// Token: 0x0600014C RID: 332 RVA: 0x000089E8 File Offset: 0x00006BE8
		private void Awake()
		{
			this.MapLayerContainer = UIUtils.CreateUIGameObject(base.gameObject, "Layers");
			this.MapMarkerContainer = UIUtils.CreateUIGameObject(base.gameObject, "Markers");
			this.MapLabelsContainer = UIUtils.CreateUIGameObject(base.gameObject, "Labels");
			this.MapLayerContainer.transform.SetAsFirstSibling();
			this.MapMarkerContainer.transform.SetAsLastSibling();
		}

		// Token: 0x0600014D RID: 333 RVA: 0x00008A57 File Offset: 0x00006C57
		public void AddMapMarker(MapMarker marker)
		{
			if (this._markers.Contains(marker))
			{
				return;
			}
			marker.OnPositionChanged += this.UpdateLayerBound;
			this.UpdateLayerBound(marker);
			marker.ContainingMapView = this;
			this._markers.Add(marker);
		}

		// Token: 0x0600014E RID: 334 RVA: 0x00008A94 File Offset: 0x00006C94
		public MapMarker AddMapMarker(MapMarkerDef markerDef)
		{
			MapMarker mapMarker = MapMarker.Create(this.MapMarkerContainer, markerDef, MapView._markerSize, -this.CoordinateRotation, 1f / this.ZoomCurrent);
			this.AddMapMarker(mapMarker);
			return mapMarker;
		}

		// Token: 0x0600014F RID: 335 RVA: 0x00008AD0 File Offset: 0x00006CD0
		public TransformMapMarker AddTransformMarker(Transform followingTransform, string name, string category, Color color, string imagePath, Vector2 size)
		{
			TransformMapMarker transformMapMarker = TransformMapMarker.Create(followingTransform, this.MapMarkerContainer, imagePath, color, name, category, size, -this.CoordinateRotation, 1f / this.ZoomCurrent);
			this.AddMapMarker(transformMapMarker);
			return transformMapMarker;
		}

		// Token: 0x06000150 RID: 336 RVA: 0x00008B10 File Offset: 0x00006D10
		public PlayerMapMarker AddPlayerMarker(IPlayer player, string category, Color color, string imagePath)
		{
			PlayerMapMarker playerMapMarker = PlayerMapMarker.Create(player, this.MapMarkerContainer, imagePath, color, category, MapView._markerSize, -this.CoordinateRotation, 1f / this.ZoomCurrent);
			this.AddMapMarker(playerMapMarker);
			return playerMapMarker;
		}

		// Token: 0x06000151 RID: 337 RVA: 0x00008B50 File Offset: 0x00006D50
		public IEnumerable<MapMarker> GetMapMarkersByCategory(string category)
		{
			return Enumerable.Where<MapMarker>(this._markers, (MapMarker m) => m.Category == category);
		}

		// Token: 0x06000152 RID: 338 RVA: 0x00008B84 File Offset: 0x00006D84
		public void ChangeMarkerCategoryStatus(string category, bool status)
		{
			foreach (MapMarker mapMarker in this._markers)
			{
				if (!(mapMarker.Category != category))
				{
					mapMarker.gameObject.SetActive(status);
				}
			}
		}

		// Token: 0x06000153 RID: 339 RVA: 0x00008BEC File Offset: 0x00006DEC
		public void ChangeMarkerPartialCategoryStatus(string partial, bool status)
		{
			foreach (MapMarker mapMarker in this._markers)
			{
				if (mapMarker.Category.Contains(partial))
				{
					mapMarker.gameObject.SetActive(status);
				}
			}
		}

		// Token: 0x06000154 RID: 340 RVA: 0x00008C54 File Offset: 0x00006E54
		public void RemoveMapMarker(MapMarker marker)
		{
			if (!this._markers.Contains(marker))
			{
				return;
			}
			this._markers.Remove(marker);
			marker.OnPositionChanged -= this.UpdateLayerBound;
			DOTween.Kill(marker.transform, false);
			marker.gameObject.SetActive(false);
			Object.Destroy(marker.gameObject);
		}

		// Token: 0x06000155 RID: 341 RVA: 0x00008CB4 File Offset: 0x00006EB4
		public void AddMapLabel(MapLabelDef labelDef)
		{
			MapLabel mapLabel = MapLabel.Create(this.MapLabelsContainer, labelDef, -this.CoordinateRotation, 1f / this.ZoomCurrent);
			this.UpdateLayerBound(mapLabel);
			this._labels.Add(mapLabel);
		}

		// Token: 0x06000156 RID: 342 RVA: 0x00008CF4 File Offset: 0x00006EF4
		public void RemoveMapLabel(MapLabel label)
		{
			if (!this._labels.Contains(label))
			{
				return;
			}
			this._labels.Remove(label);
			DOTween.Kill(label.transform, false);
			label.gameObject.SetActive(false);
			Object.Destroy(label.gameObject);
		}

		// Token: 0x06000157 RID: 343 RVA: 0x00008D44 File Offset: 0x00006F44
		public void LoadMap(MapDef mapDef)
		{
			if (mapDef == null || this.CurrentMapDef == mapDef)
			{
				return;
			}
			if (this.CurrentMapDef != null)
			{
				this.UnloadMap();
			}
			this.CurrentMapDef = mapDef;
			this.CoordinateRotation = mapDef.CoordinateRotation;
			Vector2 rotatedRectangle = MathUtils.GetRotatedRectangle(mapDef.Bounds.Max - mapDef.Bounds.Min, this.CoordinateRotation);
			this.RectTransform.sizeDelta = rotatedRectangle;
			this.RectTransform.localRotation = Quaternion.Euler(0f, 0f, this.CoordinateRotation);
			this.SetMinMaxZoom(base.transform.parent as RectTransform);
			foreach (KeyValuePair<string, MapLayerDef> keyValuePair in Enumerable.OrderBy<KeyValuePair<string, MapLayerDef>, int>(mapDef.Layers, (KeyValuePair<string, MapLayerDef> pair) => pair.Value.Level))
			{
				string key = keyValuePair.Key;
				MapLayerDef value = keyValuePair.Value;
				MapLayer mapLayer = MapLayer.Create(this.MapLayerContainer, key, value, -this.CoordinateRotation);
				mapLayer.IsOnDefaultLevel = (value.Level == mapDef.DefaultLevel);
				this._layers.Add(mapLayer);
			}
			this.SelectTopLevel(mapDef.DefaultLevel);
			foreach (MapMarkerDef markerDef in mapDef.StaticMarkers)
			{
				this.AddMapMarker(markerDef);
			}
			foreach (MapLabelDef labelDef in mapDef.Labels)
			{
				this.AddMapLabel(labelDef);
			}
		}

		// Token: 0x06000158 RID: 344 RVA: 0x00008F28 File Offset: 0x00007128
		public void UnloadMap()
		{
			if (this.CurrentMapDef == null)
			{
				return;
			}
			List<MapMarker> list = Enumerable.ToList<MapMarker>(this._markers);
			foreach (MapMarker marker in list)
			{
				this.RemoveMapMarker(marker);
			}
			list.Clear();
			this._markers.Clear();
			List<MapLabel> list2 = Enumerable.ToList<MapLabel>(this._labels);
			foreach (MapLabel label in list2)
			{
				this.RemoveMapLabel(label);
			}
			list2.Clear();
			this._labels.Clear();
			foreach (MapLayer mapLayer in this._layers)
			{
				Object.Destroy(mapLayer.gameObject);
			}
			this._layers.Clear();
			this._immediateMapAnchor = Vector2.zero;
			this.CurrentMapDef = null;
		}

		// Token: 0x06000159 RID: 345 RVA: 0x0000905C File Offset: 0x0000725C
		public void SelectTopLevel(int level)
		{
			foreach (MapLayer mapLayer in this._layers)
			{
				mapLayer.OnTopLevelSelected(level);
			}
			this.SelectedLevel = level;
			this.UpdateLayerStatus();
			Action<int> onLevelSelected = this.OnLevelSelected;
			if (onLevelSelected == null)
			{
				return;
			}
			onLevelSelected(level);
		}

		// Token: 0x0600015A RID: 346 RVA: 0x000090CC File Offset: 0x000072CC
		public void SelectLevelByCoords(Vector3 coords)
		{
			MapLayer mapLayer = this.FindMatchingLayerByCoordinate(coords);
			if (mapLayer == null)
			{
				return;
			}
			this.SelectTopLevel(mapLayer.Level);
		}

		// Token: 0x0600015B RID: 347 RVA: 0x000090F8 File Offset: 0x000072F8
		public void SetMinMaxZoom(RectTransform parentTransform)
		{
			Vector2 sizeDelta = this.RectTransform.sizeDelta;
			this.ZoomMin = Mathf.Min(parentTransform.sizeDelta.x / sizeDelta.x, parentTransform.sizeDelta.y / sizeDelta.y) / MapView._zoomMinScaler;
			this.ZoomMax = MapView._zoomMaxScaler * this.ZoomMin;
			this.SetMapZoom(this.ZoomMin, 0f, true, false);
			this.RectTransform.anchoredPosition = Vector2.zero;
			Vector2 midpoint = MathUtils.GetMidpoint(this.CurrentMapDef.Bounds.Min, this.CurrentMapDef.Bounds.Max);
			this.ShiftMapToCoordinate(midpoint, 0f, false);
		}

		// Token: 0x0600015C RID: 348 RVA: 0x000091B0 File Offset: 0x000073B0
		public void SetMapZoom(float zoomNew, float tweenTime, bool updateMainZoom = true, bool updateMiniZoom = false)
		{
			zoomNew = Mathf.Clamp(zoomNew, this.ZoomMin, this.ZoomMax);
			if (zoomNew == this.ZoomCurrent)
			{
				return;
			}
			if (updateMainZoom)
			{
				this.ZoomMain = zoomNew;
				Settings.ZoomMainMap.Value = zoomNew;
			}
			if (updateMiniZoom)
			{
				this.ZoomMini = zoomNew;
				Settings.ZoomMiniMap.Value = zoomNew;
			}
			this.ZoomCurrent = zoomNew;
			ShortcutExtensions.DOScale(this.RectTransform, this.ZoomCurrent * Vector3.one, updateMainZoom ? 0f : tweenTime);
			foreach (MonoBehaviour component in Enumerable.Concat<MonoBehaviour>(Enumerable.Cast<MonoBehaviour>(this._markers), this._labels))
			{
				ShortcutExtensions.DOScale(component.GetRectTransform(), 1f / this.ZoomCurrent * Vector3.one, tweenTime);
			}
		}

		// Token: 0x0600015D RID: 349 RVA: 0x000092A0 File Offset: 0x000074A0
		public void IncrementalZoomInto(float zoomDelta, Vector2 rectPoint, float zoomTweenTime)
		{
			float num = Mathf.Clamp(this.ZoomMain + zoomDelta, this.ZoomMin, this.ZoomMax);
			float d = num - this.ZoomMain;
			Vector2 rotatedVector = MathUtils.GetRotatedVector2(rectPoint, this.CoordinateRotation);
			this.ShiftMap(-rotatedVector * d, zoomTweenTime, false);
			this.SetMapZoom(num, zoomTweenTime, true, false);
		}

		// Token: 0x0600015E RID: 350 RVA: 0x000092FC File Offset: 0x000074FC
		public void IncrementalZoomIntoMiniMap(float zoomDelta, Vector2 rectPoint, float zoomTweenTime)
		{
			float num = Mathf.Clamp(this.ZoomMini + zoomDelta, this.ZoomMin, this.ZoomMax);
			float d = num - this.ZoomMini;
			Vector2 rotatedVector = MathUtils.GetRotatedVector2(rectPoint, this.CoordinateRotation);
			this.ShiftMap(-rotatedVector * d, zoomTweenTime, true);
			this.SetMapZoom(num, zoomTweenTime, false, true);
		}

		// Token: 0x0600015F RID: 351 RVA: 0x00009358 File Offset: 0x00007558
		public void ShiftMap(Vector2 shift, float tweenTime, bool isMini)
		{
			if (shift == Vector2.zero)
			{
				return;
			}
			if (!DOTween.IsTweening(this.RectTransform, true) || tweenTime == 0f)
			{
				this._immediateMapAnchor = this.RectTransform.anchoredPosition;
			}
			this._immediateMapAnchor += shift;
			if (!isMini)
			{
				this.MainMapPos = this._immediateMapAnchor;
			}
			this.RectTransform.DOAnchorPos(this._immediateMapAnchor, tweenTime, false);
		}

		// Token: 0x06000160 RID: 352 RVA: 0x000093CF File Offset: 0x000075CF
		public void SetMapPos(Vector2 pos, float tweenTime)
		{
			this.MainMapPos = pos;
			this.RectTransform.DOAnchorPos(pos, tweenTime, false);
		}

		// Token: 0x06000161 RID: 353 RVA: 0x000093E8 File Offset: 0x000075E8
		public void ShiftMapToCoordinate(Vector2 coord, float tweenTime, bool isMini)
		{
			Vector2 rotatedVector = MathUtils.GetRotatedVector2(coord, this.CoordinateRotation);
			Vector2 b = this.RectTransform.anchoredPosition / this.ZoomCurrent;
			this.ShiftMap((-rotatedVector - b) * this.ZoomCurrent, tweenTime, isMini);
		}

		// Token: 0x06000162 RID: 354 RVA: 0x00009438 File Offset: 0x00007638
		public void ShiftMapToPlayer(Vector2 coord, float tweenTime, bool isMini)
		{
			Vector2 rotatedVector = MathUtils.GetRotatedVector2(coord, this.CoordinateRotation);
			Vector2 b = this.RectTransform.anchoredPosition / this.ZoomMain;
			this.ShiftMap((-rotatedVector - b) * this.ZoomMain, tweenTime, isMini);
		}

		// Token: 0x06000163 RID: 355 RVA: 0x00009488 File Offset: 0x00007688
		public void ScaledShiftMap(Vector2 shiftIncrements, float incrementScale, bool isMini)
		{
			float d = Mathf.Min(this.CurrentMapDef.Bounds.Max.x - this.CurrentMapDef.Bounds.Min.x, this.CurrentMapDef.Bounds.Max.y - this.CurrentMapDef.Bounds.Min.y) * this.ZoomCurrent * incrementScale;
			this.ShiftMap(shiftIncrements * d, 0f, isMini);
		}

		// Token: 0x06000164 RID: 356 RVA: 0x00009510 File Offset: 0x00007710
		private MapLayer FindMatchingLayerByCoordinate(Vector3 coordinate)
		{
			return Enumerable.FirstOrDefault<MapLayer>(Enumerable.OrderBy<MapLayer, float>(Enumerable.Where<MapLayer>(this._layers, (MapLayer l) => l.IsCoordinateInLayer(coordinate)), (MapLayer l) => l.GetMatchingBoundVolume(coordinate)));
		}

		// Token: 0x06000165 RID: 357 RVA: 0x00009558 File Offset: 0x00007758
		private void UpdateLayerBound(ILayerBound bound)
		{
			MapLayer mapLayer = this.FindMatchingLayerByCoordinate(bound.Position);
			if (mapLayer == null)
			{
				return;
			}
			bound.HandleNewLayerStatus(mapLayer.Status);
		}

		// Token: 0x06000166 RID: 358 RVA: 0x00009584 File Offset: 0x00007784
		private void UpdateLayerStatus()
		{
			foreach (ILayerBound bound in Enumerable.Concat<ILayerBound>(Enumerable.Cast<ILayerBound>(this._markers), this._labels))
			{
				this.UpdateLayerBound(bound);
			}
		}

		// Token: 0x040000BC RID: 188
		private static Vector2 _markerSize = new Vector2(30f, 30f);

		// Token: 0x040000BD RID: 189
		private static float _zoomMaxScaler = 10f;

		// Token: 0x040000BE RID: 190
		private static float _zoomMinScaler = 1.1f;

		// Token: 0x040000CC RID: 204
		private Vector2 _immediateMapAnchor = Vector2.zero;

		// Token: 0x040000CD RID: 205
		private List<MapMarker> _markers = new List<MapMarker>();

		// Token: 0x040000CE RID: 206
		private List<MapLayer> _layers = new List<MapLayer>();

		// Token: 0x040000CF RID: 207
		private List<MapLabel> _labels = new List<MapLabel>();
	}
}
