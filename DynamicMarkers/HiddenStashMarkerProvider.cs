using System;
using System.Collections.Generic;
using System.Linq;
using DynamicMaps.Config;
using DynamicMaps.Data;
using DynamicMaps.Patches;
using DynamicMaps.UI.Components;
using DynamicMaps.Utils;
using EFT.Interactive;

namespace DynamicMaps.DynamicMarkers
{
	// Token: 0x0200003B RID: 59
	public class HiddenStashMarkerProvider : IDynamicMarkerProvider
	{
		// Token: 0x06000269 RID: 617 RVA: 0x0000D264 File Offset: 0x0000B464
		public void OnShowInRaid(MapView map)
		{
			this._lastMapView = map;
			foreach (LootableContainer stash in GameStartedPatch.HiddenStashes)
			{
				this.TryAddMarker(stash);
			}
		}

		// Token: 0x0600026A RID: 618 RVA: 0x0000D2C0 File Offset: 0x0000B4C0
		public void OnHideInRaid(MapView map)
		{
		}

		// Token: 0x0600026B RID: 619 RVA: 0x0000D2C2 File Offset: 0x0000B4C2
		public void OnRaidEnd(MapView map)
		{
			this.TryRemoveMarkers();
		}

		// Token: 0x0600026C RID: 620 RVA: 0x0000D2CC File Offset: 0x0000B4CC
		public void OnMapChanged(MapView map, MapDef mapDef)
		{
			this._lastMapView = map;
			foreach (LootableContainer stash in Enumerable.ToList<LootableContainer>(this._stashMarkers.Keys))
			{
				this.TryRemoveMarker(stash);
				this.TryAddMarker(stash);
			}
		}

		// Token: 0x0600026D RID: 621 RVA: 0x0000D338 File Offset: 0x0000B538
		public void OnDisable(MapView map)
		{
			this.OnRaidEnd(map);
		}

		// Token: 0x0600026E RID: 622 RVA: 0x0000D344 File Offset: 0x0000B544
		public void RefreshMarkers()
		{
			if (!GameUtils.IsInRaid())
			{
				return;
			}
			foreach (LootableContainer stash in Enumerable.ToList<LootableContainer>(this._stashMarkers.Keys))
			{
				this.TryRemoveMarker(stash);
				this.TryAddMarker(stash);
			}
		}

		// Token: 0x0600026F RID: 623 RVA: 0x0000D3B0 File Offset: 0x0000B5B0
		private void TryAddMarker(LootableContainer stash)
		{
			if (this._stashMarkers.ContainsKey(stash))
			{
				return;
			}
			int value = Settings.ShowHiddenStashIntelLevel.Value;
			int? intelLevel = GameUtils.GetIntelLevel();
			if (value > intelLevel.GetValueOrDefault() & intelLevel != null)
			{
				return;
			}
			MapMarkerDef markerDef = new MapMarkerDef
			{
				Category = "Hidden Stash",
				Color = Settings.HiddenStashColor.Value,
				ImagePath = "Markers/barrel.png",
				Position = MathUtils.ConvertToMapPosition(stash.transform),
				Text = "Hidden Stash"
			};
			MapMarker value2 = this._lastMapView.AddMapMarker(markerDef);
			this._stashMarkers[stash] = value2;
		}

		// Token: 0x06000270 RID: 624 RVA: 0x0000D454 File Offset: 0x0000B654
		private void TryRemoveMarkers()
		{
			foreach (LootableContainer stash in Enumerable.ToList<LootableContainer>(this._stashMarkers.Keys))
			{
				this.TryRemoveMarker(stash);
			}
		}

		// Token: 0x06000271 RID: 625 RVA: 0x0000D4B4 File Offset: 0x0000B6B4
		private void TryRemoveMarker(LootableContainer stash)
		{
			if (!this._stashMarkers.ContainsKey(stash))
			{
				return;
			}
			this._stashMarkers[stash].ContainingMapView.RemoveMapMarker(this._stashMarkers[stash]);
			this._stashMarkers.Remove(stash);
		}

		// Token: 0x06000272 RID: 626 RVA: 0x0000D4F4 File Offset: 0x0000B6F4
		public void OnShowOutOfRaid(MapView map)
		{
		}

		// Token: 0x06000273 RID: 627 RVA: 0x0000D4F6 File Offset: 0x0000B6F6
		public void OnHideOutOfRaid(MapView map)
		{
		}

		// Token: 0x0400013B RID: 315
		private MapView _lastMapView;

		// Token: 0x0400013C RID: 316
		private Dictionary<LootableContainer, MapMarker> _stashMarkers = new Dictionary<LootableContainer, MapMarker>();

		// Token: 0x0400013D RID: 317
		private const string _hiddenCacheImagePath = "Markers/barrel.png";
	}
}
