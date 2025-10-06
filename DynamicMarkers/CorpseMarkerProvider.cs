using System;
using System.Collections.Generic;
using System.Linq;
using Comfort.Common;
using DynamicMaps.Config;
using DynamicMaps.Data;
using DynamicMaps.Patches;
using DynamicMaps.UI.Components;
using DynamicMaps.Utils;
using EFT;
using UnityEngine;

namespace DynamicMaps.DynamicMarkers
{
	// Token: 0x02000033 RID: 51
	public class CorpseMarkerProvider : IDynamicMarkerProvider
	{
		// Token: 0x1700004A RID: 74
		// (get) Token: 0x060001ED RID: 493 RVA: 0x0000AFC2 File Offset: 0x000091C2
		// (set) Token: 0x060001EE RID: 494 RVA: 0x0000AFCA File Offset: 0x000091CA
		public bool ShowFriendlyCorpses
		{
			get
			{
				return this._showFriendlyCorpses;
			}
			set
			{
				this.HandleSetBoolOption(ref this._showFriendlyCorpses, value);
			}
		}

		// Token: 0x1700004B RID: 75
		// (get) Token: 0x060001EF RID: 495 RVA: 0x0000AFD9 File Offset: 0x000091D9
		// (set) Token: 0x060001F0 RID: 496 RVA: 0x0000AFE1 File Offset: 0x000091E1
		public bool ShowKilledCorpses
		{
			get
			{
				return this._showKilledCorpses;
			}
			set
			{
				this.HandleSetBoolOption(ref this._showKilledCorpses, value);
			}
		}

		// Token: 0x1700004C RID: 76
		// (get) Token: 0x060001F1 RID: 497 RVA: 0x0000AFF0 File Offset: 0x000091F0
		// (set) Token: 0x060001F2 RID: 498 RVA: 0x0000AFF8 File Offset: 0x000091F8
		public bool ShowFriendlyKilledCorpses
		{
			get
			{
				return this._showFriendlyKilledCorpses;
			}
			set
			{
				this.HandleSetBoolOption(ref this._showFriendlyKilledCorpses, value);
			}
		}

		// Token: 0x1700004D RID: 77
		// (get) Token: 0x060001F3 RID: 499 RVA: 0x0000B007 File Offset: 0x00009207
		// (set) Token: 0x060001F4 RID: 500 RVA: 0x0000B00F File Offset: 0x0000920F
		public bool ShowBossCorpses
		{
			get
			{
				return this._showBossCorpses;
			}
			set
			{
				this.HandleSetBoolOption(ref this._showBossCorpses, value);
			}
		}

		// Token: 0x1700004E RID: 78
		// (get) Token: 0x060001F5 RID: 501 RVA: 0x0000B01E File Offset: 0x0000921E
		// (set) Token: 0x060001F6 RID: 502 RVA: 0x0000B026 File Offset: 0x00009226
		public bool ShowOtherCorpses
		{
			get
			{
				return this._showOtherCorpses;
			}
			set
			{
				this.HandleSetBoolOption(ref this._showOtherCorpses, value);
			}
		}

		// Token: 0x060001F7 RID: 503 RVA: 0x0000B035 File Offset: 0x00009235
		public void OnShowInRaid(MapView map)
		{
			this._lastMapView = map;
			this.TryAddMarkers();
			GameWorldUnregisterPlayerPatch.OnUnregisterPlayer += this.OnUnregisterPlayer;
		}

		// Token: 0x060001F8 RID: 504 RVA: 0x0000B055 File Offset: 0x00009255
		public void OnHideInRaid(MapView map)
		{
			GameWorldUnregisterPlayerPatch.OnUnregisterPlayer -= this.OnUnregisterPlayer;
		}

		// Token: 0x060001F9 RID: 505 RVA: 0x0000B068 File Offset: 0x00009268
		public void OnRaidEnd(MapView map)
		{
			GameWorldUnregisterPlayerPatch.OnUnregisterPlayer -= this.OnUnregisterPlayer;
			this._lastMapView = map;
			this.TryRemoveMarkers();
		}

		// Token: 0x060001FA RID: 506 RVA: 0x0000B088 File Offset: 0x00009288
		public void OnMapChanged(MapView map, MapDef mapDef)
		{
			this._lastMapView = map;
			foreach (Player player in Enumerable.ToList<Player>(this._corpseMarkers.Keys))
			{
				this.TryRemoveMarker(player);
				this.TryAddMarker(player);
			}
		}

		// Token: 0x060001FB RID: 507 RVA: 0x0000B0F4 File Offset: 0x000092F4
		public void OnDisable(MapView map)
		{
			GameWorldUnregisterPlayerPatch.OnUnregisterPlayer -= this.OnUnregisterPlayer;
			this.TryRemoveMarkers();
		}

		// Token: 0x060001FC RID: 508 RVA: 0x0000B110 File Offset: 0x00009310
		private void OnUnregisterPlayer(IPlayer iPlayer)
		{
			if (!(iPlayer is Player))
			{
				return;
			}
			Player player = iPlayer as Player;
			if (player.HasCorpse())
			{
				this.TryAddMarker(player);
			}
		}

		// Token: 0x060001FD RID: 509 RVA: 0x0000B13C File Offset: 0x0000933C
		private void TryRemoveMarkers()
		{
			foreach (Player player in Enumerable.ToList<Player>(this._corpseMarkers.Keys))
			{
				this.TryRemoveMarker(player);
			}
			this._corpseMarkers.Clear();
		}

		// Token: 0x060001FE RID: 510 RVA: 0x0000B1A4 File Offset: 0x000093A4
		private void TryAddMarkers()
		{
			if (!GameUtils.IsInRaid())
			{
				return;
			}
			foreach (Player player in Singleton<GameWorld>.Instance.AllPlayersEverExisted)
			{
				if (player.HasCorpse() && !this._corpseMarkers.ContainsKey(player))
				{
					this.TryAddMarker(player);
				}
			}
		}

		// Token: 0x060001FF RID: 511 RVA: 0x0000B214 File Offset: 0x00009414
		public void RefreshMarkers()
		{
			if (!GameUtils.IsInRaid())
			{
				return;
			}
			foreach (KeyValuePair<Player, MapMarker> keyValuePair in Enumerable.ToArray<KeyValuePair<Player, MapMarker>>(this._corpseMarkers))
			{
				this.TryRemoveMarker(keyValuePair.Key);
			}
			foreach (Player player in Enumerable.Where<Player>(Singleton<GameWorld>.Instance.AllPlayersEverExisted, (Player p) => p.HasCorpse()))
			{
				this.TryAddMarker(player);
			}
		}

		// Token: 0x06000200 RID: 512 RVA: 0x0000B2C4 File Offset: 0x000094C4
		private void TryAddMarker(Player player)
		{
			if (this._lastMapView == null || this._corpseMarkers.ContainsKey(player))
			{
				return;
			}
			int? intelLevel = GameUtils.GetIntelLevel();
			int value = Settings.ShowCorpseIntelLevel.Value;
			int? num = intelLevel;
			if (value > num.GetValueOrDefault() & num != null)
			{
				return;
			}
			string category = "Other Corpse";
			string imagePath = "Markers/skull.png";
			Color color = Settings.KilledOtherColor.Value;
			if (player.IsGroupedWithMainPlayer())
			{
				category = "Friendly Corpse";
				imagePath = "Markers/skull.png";
				color = CorpseMarkerProvider._friendlyCorpseColor;
			}
			else if (player.IsTrackedBoss() && player.DidMainPlayerKill())
			{
				category = "Killed Boss Corpse";
				imagePath = "Markers/skull.png";
				color = Settings.KilledBossColor.Value;
			}
			else if (player.DidMainPlayerKill())
			{
				category = "Killed Corpse";
				imagePath = "Markers/skull.png";
				color = Settings.KilledCorpseColor.Value;
			}
			else if (player.IsTrackedBoss() && player.DidTeammateKill())
			{
				category = "Friendly Killed Boss Corpse";
				imagePath = "Markers/skull.png";
				color = Settings.KilledBossColor.Value;
			}
			else if (player.DidTeammateKill())
			{
				category = "Friendly Killed Corpse";
				imagePath = "Markers/skull.png";
				color = CorpseMarkerProvider._friendlyKilledCorpseColor;
			}
			else if (player.IsTrackedBoss())
			{
				category = "Boss Corpse";
				imagePath = "Markers/skull.png";
				color = Settings.KilledBossColor.Value;
			}
			if (!this.ShouldShowCategory(category))
			{
				return;
			}
			MapMarkerDef markerDef = new MapMarkerDef
			{
				Category = category,
				ImagePath = imagePath,
				Text = player.Profile.GetCorrectedNickname(),
				Color = color,
				Position = MathUtils.ConvertToMapPosition(((IPlayer)player).Position)
			};
			MapMarker value2 = this._lastMapView.AddMapMarker(markerDef);
			this._corpseMarkers[player] = value2;
		}

		// Token: 0x06000201 RID: 513 RVA: 0x0000B45C File Offset: 0x0000965C
		private void RemoveDisabledMarkers()
		{
			foreach (Player player in Enumerable.ToList<Player>(this._corpseMarkers.Keys))
			{
				MapMarker mapMarker = this._corpseMarkers[player];
				if (!this.ShouldShowCategory(mapMarker.Category))
				{
					this.TryRemoveMarker(player);
				}
			}
		}

		// Token: 0x06000202 RID: 514 RVA: 0x0000B4D4 File Offset: 0x000096D4
		private void TryRemoveMarker(Player player)
		{
			if (!this._corpseMarkers.ContainsKey(player))
			{
				return;
			}
			this._corpseMarkers[player].ContainingMapView.RemoveMapMarker(this._corpseMarkers[player]);
			this._corpseMarkers.Remove(player);
		}

		// Token: 0x06000203 RID: 515 RVA: 0x0000B514 File Offset: 0x00009714
		private bool ShouldShowCategory(string category)
		{
			if (category != null)
			{
				int length = category.Length;
				switch (length)
				{
				case 11:
					if (!(category == "Boss Corpse"))
					{
						return false;
					}
					return this._showBossCorpses;
				case 12:
					if (!(category == "Other Corpse"))
					{
						return false;
					}
					return this._showOtherCorpses;
				case 13:
					if (!(category == "Killed Corpse"))
					{
						return false;
					}
					break;
				case 14:
				case 16:
				case 17:
					return false;
				case 15:
					if (!(category == "Friendly Corpse"))
					{
						return false;
					}
					return this._showFriendlyCorpses;
				case 18:
					if (!(category == "Killed Boss Corpse"))
					{
						return false;
					}
					break;
				default:
					if (length != 22)
					{
						if (length != 27)
						{
							return false;
						}
						if (!(category == "Friendly Killed Boss Corpse"))
						{
							return false;
						}
					}
					else if (!(category == "Friendly Killed Corpse"))
					{
						return false;
					}
					return this._showFriendlyKilledCorpses;
				}
				return this._showKilledCorpses;
			}
			return false;
		}

		// Token: 0x06000204 RID: 516 RVA: 0x0000B5F3 File Offset: 0x000097F3
		private void HandleSetBoolOption(ref bool boolOption, bool value)
		{
			if (value == boolOption)
			{
				return;
			}
			boolOption = value;
			if (boolOption)
			{
				this.TryAddMarkers();
				return;
			}
			this.RemoveDisabledMarkers();
		}

		// Token: 0x06000205 RID: 517 RVA: 0x0000B60F File Offset: 0x0000980F
		public void OnShowOutOfRaid(MapView map)
		{
		}

		// Token: 0x06000206 RID: 518 RVA: 0x0000B611 File Offset: 0x00009811
		public void OnHideOutOfRaid(MapView map)
		{
		}

		// Token: 0x04000103 RID: 259
		private const string _skullImagePath = "Markers/skull.png";

		// Token: 0x04000104 RID: 260
		private const string _friendlyCorpseCategory = "Friendly Corpse";

		// Token: 0x04000105 RID: 261
		private const string _friendlyCorpseImagePath = "Markers/skull.png";

		// Token: 0x04000106 RID: 262
		private static Color _friendlyCorpseColor = Color.Lerp(Color.blue, Color.white, 0.5f);

		// Token: 0x04000107 RID: 263
		private const string _killedCorpseCategory = "Killed Corpse";

		// Token: 0x04000108 RID: 264
		private const string _killedCorpseImagePath = "Markers/skull.png";

		// Token: 0x04000109 RID: 265
		private const string _killedBossCorpseCategory = "Killed Boss Corpse";

		// Token: 0x0400010A RID: 266
		private const string _killedBossCorpseImagePath = "Markers/skull.png";

		// Token: 0x0400010B RID: 267
		private const string _bossCorpseCategory = "Boss Corpse";

		// Token: 0x0400010C RID: 268
		private const string _bossCorpseImagePath = "Markers/skull.png";

		// Token: 0x0400010D RID: 269
		private const string _friendlyKilledCorpseCategory = "Friendly Killed Corpse";

		// Token: 0x0400010E RID: 270
		private const string _friendlyKilledCorpseImagePath = "Markers/skull.png";

		// Token: 0x0400010F RID: 271
		private static Color _friendlyKilledCorpseColor = Color.Lerp(Color.Lerp(Color.blue, Color.white, 0.5f), Color.red, 0.5f);

		// Token: 0x04000110 RID: 272
		private const string _friendlyKilledBossCorpseCategory = "Friendly Killed Boss Corpse";

		// Token: 0x04000111 RID: 273
		private const string _friendlyKilledBossCorpseImagePath = "Markers/skull.png";

		// Token: 0x04000112 RID: 274
		private const string _otherCorpseCategory = "Other Corpse";

		// Token: 0x04000113 RID: 275
		private const string _otherCorpseImagePath = "Markers/skull.png";

		// Token: 0x04000114 RID: 276
		private bool _showFriendlyCorpses = true;

		// Token: 0x04000115 RID: 277
		private bool _showKilledCorpses = true;

		// Token: 0x04000116 RID: 278
		private bool _showFriendlyKilledCorpses = true;

		// Token: 0x04000117 RID: 279
		private bool _showBossCorpses;

		// Token: 0x04000118 RID: 280
		private bool _showOtherCorpses;

		// Token: 0x04000119 RID: 281
		private MapView _lastMapView;

		// Token: 0x0400011A RID: 282
		private Dictionary<Player, MapMarker> _corpseMarkers = new Dictionary<Player, MapMarker>();
	}
}
