using System;
using System.Collections.Generic;
using System.Linq;
using DynamicMaps.Config;
using DynamicMaps.Data;
using DynamicMaps.UI.Components;
using DynamicMaps.Utils;
using EFT.Interactive;

namespace DynamicMaps.DynamicMarkers
{
	// Token: 0x0200003C RID: 60
	public class TransitMarkerProvider : IDynamicMarkerProvider
	{
		// Token: 0x06000275 RID: 629 RVA: 0x0000D50B File Offset: 0x0000B70B
		public void OnShowInRaid(MapView map)
		{
			if (this._transitMarkers.Count == 0)
			{
				this.AddTransitMarkers(map);
			}
		}

		// Token: 0x06000276 RID: 630 RVA: 0x0000D521 File Offset: 0x0000B721
		public void OnHideInRaid(MapView map)
		{
		}

		// Token: 0x06000277 RID: 631 RVA: 0x0000D523 File Offset: 0x0000B723
		public void OnRaidEnd(MapView map)
		{
			this.TryRemoveMarkers();
		}

		// Token: 0x06000278 RID: 632 RVA: 0x0000D52C File Offset: 0x0000B72C
		public void OnMapChanged(MapView map, MapDef mapDef)
		{
			foreach (TransitPoint transitPoint in Enumerable.ToList<TransitPoint>(this._transitMarkers.Keys))
			{
				this.TryRemoveMarker(transitPoint);
				this.TryAddMarker(map, transitPoint);
			}
		}

		// Token: 0x06000279 RID: 633 RVA: 0x0000D594 File Offset: 0x0000B794
		public void OnDisable(MapView map)
		{
			this.TryRemoveMarkers();
		}

		// Token: 0x0600027A RID: 634 RVA: 0x0000D59C File Offset: 0x0000B79C
		public void RefreshMarkers(MapView map)
		{
			foreach (TransitPoint transitPoint in Enumerable.ToList<TransitPoint>(this._transitMarkers.Keys))
			{
				this.TryRemoveMarker(transitPoint);
				this.TryAddMarker(map, transitPoint);
			}
		}

		// Token: 0x0600027B RID: 635 RVA: 0x0000D604 File Offset: 0x0000B804
		private void AddTransitMarkers(MapView map)
		{
			foreach (TransitPoint point in LocationScene.GetAllObjects<TransitPoint>(false))
			{
				this.TryAddMarker(map, point);
			}
		}

		// Token: 0x0600027C RID: 636 RVA: 0x0000D654 File Offset: 0x0000B854
		private void TryAddMarker(MapView map, TransitPoint point)
		{
			if (this._transitMarkers.ContainsKey(point))
			{
				return;
			}
			MapMarkerDef markerDef = new MapMarkerDef
			{
				Category = "Transit",
				ImagePath = "Markers/transit.png",
				Text = point.parameters.description.BSGLocalized(),
				Position = MathUtils.ConvertToMapPosition(point.transform),
				Color = Settings.TransPointColor.Value
			};
			MapMarker value = map.AddMapMarker(markerDef);
			this._transitMarkers[point] = value;
		}

		// Token: 0x0600027D RID: 637 RVA: 0x0000D6D8 File Offset: 0x0000B8D8
		private void TryRemoveMarkers()
		{
			foreach (TransitPoint transit in Enumerable.ToList<TransitPoint>(this._transitMarkers.Keys))
			{
				this.TryRemoveMarker(transit);
			}
		}

		// Token: 0x0600027E RID: 638 RVA: 0x0000D738 File Offset: 0x0000B938
		private void TryRemoveMarker(TransitPoint transit)
		{
			if (!this._transitMarkers.ContainsKey(transit))
			{
				return;
			}
			this._transitMarkers[transit].ContainingMapView.RemoveMapMarker(this._transitMarkers[transit]);
			this._transitMarkers.Remove(transit);
		}

		// Token: 0x0600027F RID: 639 RVA: 0x0000D778 File Offset: 0x0000B978
		public void OnShowOutOfRaid(MapView map)
		{
		}

		// Token: 0x06000280 RID: 640 RVA: 0x0000D77A File Offset: 0x0000B97A
		public void OnHideOutOfRaid(MapView map)
		{
		}

		// Token: 0x0400013E RID: 318
		private const string TransitCategory = "Transit";

		// Token: 0x0400013F RID: 319
		private const string TransitImagePath = "Markers/transit.png";

		// Token: 0x04000140 RID: 320
		private Dictionary<TransitPoint, MapMarker> _transitMarkers = new Dictionary<TransitPoint, MapMarker>();
	}
}
