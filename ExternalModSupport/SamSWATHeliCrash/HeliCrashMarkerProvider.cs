using System;
using System.Collections.Generic;
using Comfort.Common;
using DynamicMaps.Data;
using DynamicMaps.DynamicMarkers;
using DynamicMaps.UI.Components;
using DynamicMaps.Utils;
using EFT;
using EFT.InventoryLogic;
using UnityEngine;

namespace DynamicMaps.ExternalModSupport.SamSWATHeliCrash
{
	// Token: 0x0200002F RID: 47
	public class HeliCrashMarkerProvider : IDynamicMarkerProvider
	{
		// Token: 0x060001BA RID: 442 RVA: 0x0000A640 File Offset: 0x00008840
		public void OnShowInRaid(MapView map)
		{
			this._lastMapView = map;
			if (this._HeliCrashMarkers.Count > 0)
			{
				return;
			}
			this.TryAddMarker();
		}

		// Token: 0x060001BB RID: 443 RVA: 0x0000A65E File Offset: 0x0000885E
		public void OnHideInRaid(MapView map)
		{
		}

		// Token: 0x060001BC RID: 444 RVA: 0x0000A660 File Offset: 0x00008860
		public void OnRaidEnd(MapView map)
		{
			this.TryRemoveMarker();
		}

		// Token: 0x060001BD RID: 445 RVA: 0x0000A668 File Offset: 0x00008868
		public void OnMapChanged(MapView map, MapDef mapDef)
		{
		}

		// Token: 0x060001BE RID: 446 RVA: 0x0000A66A File Offset: 0x0000886A
		public void OnDisable(MapView map)
		{
			this.OnRaidEnd(map);
		}

		// Token: 0x060001BF RID: 447 RVA: 0x0000A674 File Offset: 0x00008874
		private void TryAddMarker()
		{
			ValueTuple<Item, GameWorld.GStruct162> value = Singleton<GameWorld>.Instance.FindItemWithWorldData("6223349b3136504a544d1608").Value;
			Item item = value.Item1;
			GameWorld.GStruct162 item2 = value.Item2;
			if (item == null)
			{
				return;
			}
			MapMarkerDef markerDef = new MapMarkerDef
			{
				Category = "Airdrop",
				Color = HeliCrashMarkerProvider._markerColor,
				ImagePath = "Markers/helicopter.png",
				Position = MathUtils.ConvertToMapPosition(item2.Transform),
				Pivot = HeliCrashMarkerProvider._markerPivot,
				Text = "Crashed Helicopter"
			};
			DynamicMaps.UI.Components.MapMarker item3 = this._lastMapView.AddMapMarker(markerDef);
			this._HeliCrashMarkers.Add(item3);
		}

		// Token: 0x060001C0 RID: 448 RVA: 0x0000A710 File Offset: 0x00008910
		private void TryRemoveMarker()
		{
			if (this._HeliCrashMarkers.Count == 0)
			{
				return;
			}
			this._HeliCrashMarkers[0].ContainingMapView.RemoveMapMarker(this._HeliCrashMarkers[0]);
			this._HeliCrashMarkers.Remove(this._HeliCrashMarkers[0]);
		}

		// Token: 0x060001C1 RID: 449 RVA: 0x0000A765 File Offset: 0x00008965
		public void OnShowOutOfRaid(MapView map)
		{
		}

		// Token: 0x060001C2 RID: 450 RVA: 0x0000A767 File Offset: 0x00008967
		public void OnHideOutOfRaid(MapView map)
		{
		}

		// Token: 0x040000EC RID: 236
		private MapView _lastMapView;

		// Token: 0x040000ED RID: 237
		private List<DynamicMaps.UI.Components.MapMarker> _HeliCrashMarkers = new List<DynamicMaps.UI.Components.MapMarker>();

		// Token: 0x040000EE RID: 238
		private const string _markerName = "Crashed Helicopter";

		// Token: 0x040000EF RID: 239
		private const string _markerCategory = "Airdrop";

		// Token: 0x040000F0 RID: 240
		private const string _markerImagePath = "Markers/helicopter.png";

		// Token: 0x040000F1 RID: 241
		private static Vector2 _markerPivot = new Vector2(0.5f, 0.25f);

		// Token: 0x040000F2 RID: 242
		private static Color _markerColor = Color.Lerp(Color.red, Color.white, 0.333f);
	}
}
