using System;
using DynamicMaps.Config;
using DynamicMaps.Data;
using DynamicMaps.UI.Components;
using DynamicMaps.Utils;
using EFT.Vehicle;
using UnityEngine;

namespace DynamicMaps.DynamicMarkers
{
	// Token: 0x02000032 RID: 50
	public class BTRMarkerProvider : IDynamicMarkerProvider
	{
		// Token: 0x060001E2 RID: 482 RVA: 0x0000AECD File Offset: 0x000090CD
		public void OnShowInRaid(MapView map)
		{
			if (this._btrMarker != null)
			{
				return;
			}
			this.TryAddMarker(map);
		}

		// Token: 0x060001E3 RID: 483 RVA: 0x0000AEDF File Offset: 0x000090DF
		public void OnMapChanged(MapView map, MapDef mapDef)
		{
			this.TryRemoveMarker();
			this.TryAddMarker(map);
		}

		// Token: 0x060001E4 RID: 484 RVA: 0x0000AEEE File Offset: 0x000090EE
		public void OnRaidEnd(MapView map)
		{
			this.TryRemoveMarker();
		}

		// Token: 0x060001E5 RID: 485 RVA: 0x0000AEF6 File Offset: 0x000090F6
		public void OnDisable(MapView map)
		{
			this.TryRemoveMarker();
		}

		// Token: 0x060001E6 RID: 486 RVA: 0x0000AF00 File Offset: 0x00009100
		private void TryAddMarker(MapView map)
		{
			if (this._btrMarker != null)
			{
				return;
			}
			BTRView btrview = GameUtils.GetBTRView();
			if (btrview == null)
			{
				return;
			}
			Color value = Settings.BtrColor.Value;
			this._btrMarker = map.AddTransformMarker(btrview.transform, BTRMarkerProvider._btrName, BTRMarkerProvider._btrCategory, value, BTRMarkerProvider._btrIconPath, BTRMarkerProvider._btrSize);
		}

		// Token: 0x060001E7 RID: 487 RVA: 0x0000AF58 File Offset: 0x00009158
		private void TryRemoveMarker()
		{
			if (this._btrMarker == null)
			{
				return;
			}
			this._btrMarker.ContainingMapView.RemoveMapMarker(this._btrMarker);
			this._btrMarker = null;
		}

		// Token: 0x060001E8 RID: 488 RVA: 0x0000AF80 File Offset: 0x00009180
		public void OnHideInRaid(MapView map)
		{
		}

		// Token: 0x060001E9 RID: 489 RVA: 0x0000AF82 File Offset: 0x00009182
		public void OnShowOutOfRaid(MapView map)
		{
		}

		// Token: 0x060001EA RID: 490 RVA: 0x0000AF84 File Offset: 0x00009184
		public void OnHideOutOfRaid(MapView map)
		{
		}

		// Token: 0x040000FE RID: 254
		private static string _btrIconPath = "Markers/btr.png";

		// Token: 0x040000FF RID: 255
		private static Vector2 _btrSize = new Vector2(45f, 45f);

		// Token: 0x04000100 RID: 256
		private static string _btrName = "BTR";

		// Token: 0x04000101 RID: 257
		private static string _btrCategory = "BTR";

		// Token: 0x04000102 RID: 258
		private MapMarker _btrMarker;
	}
}
