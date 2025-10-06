using System;
using DynamicMaps.Config;
using DynamicMaps.Data;
using DynamicMaps.UI.Components;
using DynamicMaps.Utils;
using EFT;
using UnityEngine;

namespace DynamicMaps.DynamicMarkers
{
	// Token: 0x02000039 RID: 57
	public class PlayerMarkerProvider : IDynamicMarkerProvider
	{
		// Token: 0x0600024F RID: 591 RVA: 0x0000CCFD File Offset: 0x0000AEFD
		public void OnShowInRaid(MapView map)
		{
			this.TryAddMarker(map);
		}

		// Token: 0x06000250 RID: 592 RVA: 0x0000CD06 File Offset: 0x0000AF06
		public void OnRaidEnd(MapView map)
		{
			this.TryRemoveMarker();
		}

		// Token: 0x06000251 RID: 593 RVA: 0x0000CD0E File Offset: 0x0000AF0E
		public void OnMapChanged(MapView map, MapDef mapDef)
		{
			this.TryRemoveMarker();
			if (GameUtils.IsInRaid())
			{
				this.TryAddMarker(map);
			}
		}

		// Token: 0x06000252 RID: 594 RVA: 0x0000CD24 File Offset: 0x0000AF24
		public void OnDisable(MapView map)
		{
			this.TryRemoveMarker();
		}

		// Token: 0x06000253 RID: 595 RVA: 0x0000CD2C File Offset: 0x0000AF2C
		private void TryAddMarker(MapView map)
		{
			if (this._playerMarker != null)
			{
				return;
			}
			Player mainPlayer = GameUtils.GetMainPlayer();
			if (mainPlayer == null || mainPlayer.IsDedicatedServer())
			{
				return;
			}
			Color value = Settings.PlayerColor.Value;
			this._playerMarker = map.AddPlayerMarker(mainPlayer, "Main Player", value, "Markers/arrow.png");
		}

		// Token: 0x06000254 RID: 596 RVA: 0x0000CD77 File Offset: 0x0000AF77
		private void TryRemoveMarker()
		{
			if (this._playerMarker == null)
			{
				return;
			}
			this._playerMarker.ContainingMapView.RemoveMapMarker(this._playerMarker);
			this._playerMarker = null;
		}

		// Token: 0x06000255 RID: 597 RVA: 0x0000CD9F File Offset: 0x0000AF9F
		public void OnHideInRaid(MapView map)
		{
		}

		// Token: 0x06000256 RID: 598 RVA: 0x0000CDA1 File Offset: 0x0000AFA1
		public void OnShowOutOfRaid(MapView map)
		{
		}

		// Token: 0x06000257 RID: 599 RVA: 0x0000CDA3 File Offset: 0x0000AFA3
		public void OnHideOutOfRaid(MapView map)
		{
		}

		// Token: 0x04000134 RID: 308
		private const string _playerCategory = "Main Player";

		// Token: 0x04000135 RID: 309
		private const string _playerImagePath = "Markers/arrow.png";

		// Token: 0x04000136 RID: 310
		private PlayerMapMarker _playerMarker;
	}
}
