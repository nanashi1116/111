using System;
using System.Collections.Generic;
using System.Linq;
using DynamicMaps.Config;
using DynamicMaps.Data;
using DynamicMaps.Patches;
using DynamicMaps.UI.Components;
using DynamicMaps.Utils;
using EFT.SynchronizableObjects;
using UnityEngine;

namespace DynamicMaps.DynamicMarkers
{
	// Token: 0x02000030 RID: 48
	public class AirdropMarkerProvider : IDynamicMarkerProvider
	{
		// Token: 0x060001C5 RID: 453 RVA: 0x0000A7AC File Offset: 0x000089AC
		public void OnShowInRaid(MapView map)
		{
			this._lastMapView = map;
			foreach (AirdropSynchronizableObject airdrop in AirdropBoxOnBoxLandPatch.Airdrops)
			{
				this.TryAddMarker(airdrop);
			}
			AirdropBoxOnBoxLandPatch.OnAirdropLanded += this.TryAddMarker;
		}

		// Token: 0x060001C6 RID: 454 RVA: 0x0000A818 File Offset: 0x00008A18
		public void OnHideInRaid(MapView map)
		{
			AirdropBoxOnBoxLandPatch.OnAirdropLanded -= this.TryAddMarker;
		}

		// Token: 0x060001C7 RID: 455 RVA: 0x0000A82B File Offset: 0x00008A2B
		public void OnRaidEnd(MapView map)
		{
			this.TryRemoveMarkers();
		}

		// Token: 0x060001C8 RID: 456 RVA: 0x0000A834 File Offset: 0x00008A34
		public void OnMapChanged(MapView map, MapDef mapDef)
		{
			this._lastMapView = map;
			foreach (AirdropSynchronizableObject airdrop in Enumerable.ToList<AirdropSynchronizableObject>(this._airdropMarkers.Keys))
			{
				this.TryRemoveMarker(airdrop);
				this.TryAddMarker(airdrop);
			}
		}

		// Token: 0x060001C9 RID: 457 RVA: 0x0000A8A0 File Offset: 0x00008AA0
		public void OnDisable(MapView map)
		{
			this.OnRaidEnd(map);
		}

		// Token: 0x060001CA RID: 458 RVA: 0x0000A8AC File Offset: 0x00008AAC
		public void RefreshMarkers()
		{
			if (!GameUtils.IsInRaid())
			{
				return;
			}
			foreach (KeyValuePair<AirdropSynchronizableObject, MapMarker> keyValuePair in Enumerable.ToArray<KeyValuePair<AirdropSynchronizableObject, MapMarker>>(this._airdropMarkers))
			{
				this.TryRemoveMarker(keyValuePair.Key);
			}
			foreach (AirdropSynchronizableObject airdrop in AirdropBoxOnBoxLandPatch.Airdrops)
			{
				this.TryAddMarker(airdrop);
			}
		}

		// Token: 0x060001CB RID: 459 RVA: 0x0000A938 File Offset: 0x00008B38
		private void TryAddMarker(AirdropSynchronizableObject airdrop)
		{
			if (this._airdropMarkers.ContainsKey(airdrop))
			{
				return;
			}
			int? intelLevel = GameUtils.GetIntelLevel();
			int value = Settings.ShowAirdropIntelLevel.Value;
			int? num = intelLevel;
			if (value > num.GetValueOrDefault() & num != null)
			{
				return;
			}
			MapMarkerDef markerDef = new MapMarkerDef
			{
				Category = "Airdrop",
				Color = Settings.AirdropColor.Value,
				ImagePath = "Markers/airdrop.png",
				Position = MathUtils.ConvertToMapPosition(airdrop.transform),
				Pivot = AirdropMarkerProvider._airdropPivot,
				Text = "Airdrop"
			};
			MapMarker value2 = this._lastMapView.AddMapMarker(markerDef);
			this._airdropMarkers[airdrop] = value2;
		}

		// Token: 0x060001CC RID: 460 RVA: 0x0000A9E8 File Offset: 0x00008BE8
		private void TryRemoveMarkers()
		{
			foreach (AirdropSynchronizableObject airdrop in Enumerable.ToList<AirdropSynchronizableObject>(this._airdropMarkers.Keys))
			{
				this.TryRemoveMarker(airdrop);
			}
		}

		// Token: 0x060001CD RID: 461 RVA: 0x0000AA48 File Offset: 0x00008C48
		private void TryRemoveMarker(AirdropSynchronizableObject airdrop)
		{
			if (!this._airdropMarkers.ContainsKey(airdrop))
			{
				return;
			}
			this._airdropMarkers[airdrop].ContainingMapView.RemoveMapMarker(this._airdropMarkers[airdrop]);
			this._airdropMarkers.Remove(airdrop);
		}

		// Token: 0x060001CE RID: 462 RVA: 0x0000AA88 File Offset: 0x00008C88
		public void OnShowOutOfRaid(MapView map)
		{
		}

		// Token: 0x060001CF RID: 463 RVA: 0x0000AA8A File Offset: 0x00008C8A
		public void OnHideOutOfRaid(MapView map)
		{
		}

		// Token: 0x040000F3 RID: 243
		private MapView _lastMapView;

		// Token: 0x040000F4 RID: 244
		private Dictionary<AirdropSynchronizableObject, MapMarker> _airdropMarkers = new Dictionary<AirdropSynchronizableObject, MapMarker>();

		// Token: 0x040000F5 RID: 245
		private const string _airdropName = "Airdrop";

		// Token: 0x040000F6 RID: 246
		private const string _airdropCategory = "Airdrop";

		// Token: 0x040000F7 RID: 247
		private const string _airdropImagePath = "Markers/airdrop.png";

		// Token: 0x040000F8 RID: 248
		private static Vector2 _airdropPivot = new Vector2(0.5f, 0.25f);
	}
}
