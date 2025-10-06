using System;
using System.Collections.Generic;
using System.Linq;
using Comfort.Common;
using DynamicMaps.Config;
using DynamicMaps.Data;
using DynamicMaps.Patches;
using DynamicMaps.UI;
using DynamicMaps.UI.Components;
using DynamicMaps.Utils;
using EFT;
using UnityEngine;

namespace DynamicMaps.DynamicMarkers
{
	// Token: 0x02000038 RID: 56
	public class OtherPlayersMarkerProvider : IDynamicMarkerProvider
	{
		// Token: 0x17000050 RID: 80
		// (get) Token: 0x06000234 RID: 564 RVA: 0x0000C0A3 File Offset: 0x0000A2A3
		// (set) Token: 0x06000235 RID: 565 RVA: 0x0000C0AB File Offset: 0x0000A2AB
		public bool ShowFriendlyPlayers
		{
			get
			{
				return this._showFriendlyPlayers;
			}
			set
			{
				Plugin.Log.LogInfo(string.Format("OtherPlayersMarkerProvider: ShowFriendlyPlayers set to {0} (was {1})", value, this._showFriendlyPlayers));
				this.HandleSetBoolOption(ref this._showFriendlyPlayers, value);
			}
		}

		// Token: 0x17000051 RID: 81
		// (get) Token: 0x06000236 RID: 566 RVA: 0x0000C0DF File Offset: 0x0000A2DF
		// (set) Token: 0x06000237 RID: 567 RVA: 0x0000C0E7 File Offset: 0x0000A2E7
		public bool ShowEnemyPlayers
		{
			get
			{
				return this._showEnemyPlayers;
			}
			set
			{
				Plugin.Log.LogInfo(string.Format("OtherPlayersMarkerProvider: ShowEnemyPlayers set to {0} (was {1})", value, this._showEnemyPlayers));
				this.HandleSetBoolOption(ref this._showEnemyPlayers, value);
			}
		}

		// Token: 0x17000052 RID: 82
		// (get) Token: 0x06000238 RID: 568 RVA: 0x0000C11B File Offset: 0x0000A31B
		// (set) Token: 0x06000239 RID: 569 RVA: 0x0000C123 File Offset: 0x0000A323
		public bool ShowScavs
		{
			get
			{
				return this._showScavs;
			}
			set
			{
				this.HandleSetBoolOption(ref this._showScavs, value);
			}
		}

		// Token: 0x17000053 RID: 83
		// (get) Token: 0x0600023A RID: 570 RVA: 0x0000C132 File Offset: 0x0000A332
		// (set) Token: 0x0600023B RID: 571 RVA: 0x0000C13A File Offset: 0x0000A33A
		public bool ShowBosses
		{
			get
			{
				return this._showBosses;
			}
			set
			{
				this.HandleSetBoolOption(ref this._showBosses, value);
			}
		}

		// Token: 0x0600023C RID: 572 RVA: 0x0000C14C File Offset: 0x0000A34C
		public void OnShowInRaid(MapView map)
		{
			Plugin.Log.LogInfo("OtherPlayersMarkerProvider: OnShowInRaid called");
			if (map.GetComponentInParent<ModdedMapScreen>() != null)
			{
				Plugin.Log.LogInfo("OtherPlayersMarkerProvider: Found ModdedMapScreen, forcing config refresh");
				ModdedMapScreen.DMServerConfig serverConfig = ModdedMapScreen._serverConfig;
				Plugin.Log.LogInfo(string.Format("OtherPlayersMarkerProvider: Server config null: {0}", serverConfig == null));
				bool flag = serverConfig == null || serverConfig.allowShowFriendlyPlayerMarkersInRaid;
				bool flag2 = serverConfig == null || serverConfig.allowShowEnemyPlayerMarkersInRaid;
				bool flag3 = serverConfig == null || serverConfig.allowShowScavMarkersInRaid;
				bool flag4 = serverConfig == null || serverConfig.allowShowBossMarkersInRaid;
				Plugin.Log.LogInfo(string.Format("OtherPlayersMarkerProvider: Server permissions - Friendly={0}, Enemy={1}, Scavs={2}, Bosses={3}", new object[]
				{
					flag,
					flag2,
					flag3,
					flag4
				}));
				Plugin.Log.LogInfo(string.Format("OtherPlayersMarkerProvider: Client settings - Friendly={0}, Enemy={1}, Scavs={2}, Bosses={3}", new object[]
				{
					Settings.ShowFriendlyPlayerMarkersInRaid.Value,
					Settings.ShowEnemyPlayerMarkersInRaid.Value,
					Settings.ShowScavMarkersInRaid.Value,
					Settings.ShowBossMarkersInRaid.Value
				}));
				this.ShowFriendlyPlayers = (flag && Settings.ShowFriendlyPlayerMarkersInRaid.Value);
				this.ShowEnemyPlayers = (flag2 && Settings.ShowEnemyPlayerMarkersInRaid.Value);
				this.ShowScavs = (flag3 && Settings.ShowScavMarkersInRaid.Value);
				this.ShowBosses = (flag4 && Settings.ShowBossMarkersInRaid.Value);
			}
			this._lastMapView = map;
			Plugin.Log.LogInfo(string.Format("OtherPlayersMarkerProvider: Final settings - ShowFriendly: {0}, ShowEnemy: {1}, ShowScavs: {2}, ShowBosses: {3}", new object[]
			{
				this._showFriendlyPlayers,
				this._showEnemyPlayers,
				this._showScavs,
				this._showBosses
			}));
			this.TryAddMarkers();
			this.RemoveNonActivePlayers();
			Singleton<GameWorld>.Instance.OnPersonAdd += this.TryAddMarker;
			GameWorldUnregisterPlayerPatch.OnUnregisterPlayer += this.OnUnregisterPlayer;
			PlayerOnDeadPatch.OnDead += this.TryRemoveMarker;
			Plugin.Log.LogInfo("OtherPlayersMarkerProvider: OnShowInRaid completed");
		}

		// Token: 0x0600023D RID: 573 RVA: 0x0000C386 File Offset: 0x0000A586
		public void OnHideInRaid(MapView map)
		{
			Singleton<GameWorld>.Instance.OnPersonAdd -= this.TryAddMarker;
			GameWorldUnregisterPlayerPatch.OnUnregisterPlayer -= this.OnUnregisterPlayer;
			PlayerOnDeadPatch.OnDead -= this.TryRemoveMarker;
		}

		// Token: 0x0600023E RID: 574 RVA: 0x0000C3C0 File Offset: 0x0000A5C0
		public void OnRaidEnd(MapView map)
		{
			GameWorld instance = Singleton<GameWorld>.Instance;
			if (instance != null)
			{
				instance.OnPersonAdd -= this.TryAddMarker;
			}
			GameWorldUnregisterPlayerPatch.OnUnregisterPlayer -= this.OnUnregisterPlayer;
			PlayerOnDeadPatch.OnDead -= this.TryRemoveMarker;
			this.TryRemoveMarkers();
		}

		// Token: 0x0600023F RID: 575 RVA: 0x0000C410 File Offset: 0x0000A610
		public void OnMapChanged(MapView map, MapDef mapDef)
		{
			this._lastMapView = map;
			foreach (Player player in Enumerable.ToList<Player>(this._playerMarkers.Keys))
			{
				this.TryRemoveMarker(player);
				this.TryAddMarker(player);
			}
		}

		// Token: 0x06000240 RID: 576 RVA: 0x0000C47C File Offset: 0x0000A67C
		public void OnDisable(MapView map)
		{
			GameWorld instance = Singleton<GameWorld>.Instance;
			if (instance != null)
			{
				instance.OnPersonAdd -= this.TryAddMarker;
			}
			GameWorldUnregisterPlayerPatch.OnUnregisterPlayer -= this.OnUnregisterPlayer;
			PlayerOnDeadPatch.OnDead -= this.TryRemoveMarker;
			this.TryRemoveMarkers();
		}

		// Token: 0x06000241 RID: 577 RVA: 0x0000C4CC File Offset: 0x0000A6CC
		private void TryRemoveMarkers()
		{
			foreach (Player player in Enumerable.ToList<Player>(this._playerMarkers.Keys))
			{
				this.TryRemoveMarker(player);
			}
			this._playerMarkers.Clear();
		}

		// Token: 0x06000242 RID: 578 RVA: 0x0000C534 File Offset: 0x0000A734
		private void TryAddMarkers()
		{
			if (!GameUtils.IsInRaid())
			{
				Plugin.Log.LogInfo("TryAddMarkers: Not in raid, skipping");
				return;
			}
			GameWorld instance = Singleton<GameWorld>.Instance;
			if (instance == null)
			{
				Plugin.Log.LogError("TryAddMarkers: GameWorld is null");
				return;
			}
			List<Player> allAlivePlayersList = instance.AllAlivePlayersList;
			Plugin.Log.LogInfo(string.Format("TryAddMarkers: Found {0} alive players", allAlivePlayersList.Count));
			foreach (Player player in allAlivePlayersList)
			{
				if (player.IsYourPlayer)
				{
					Plugin.Log.LogDebug("TryAddMarkers: Skipping main player: " + player.Profile.Nickname);
				}
				else if (this._playerMarkers.ContainsKey(player))
				{
					Plugin.Log.LogDebug("TryAddMarkers: Player " + player.Profile.Nickname + " already has marker");
				}
				else
				{
					Plugin.Log.LogInfo("TryAddMarkers: Attempting to add marker for player: " + player.Profile.Nickname);
					this.TryAddMarker(player);
				}
			}
			Plugin.Log.LogInfo(string.Format("TryAddMarkers: Total markers after adding: {0}", this._playerMarkers.Count));
		}

		// Token: 0x06000243 RID: 579 RVA: 0x0000C684 File Offset: 0x0000A884
		private void OnUnregisterPlayer(IPlayer iPlayer)
		{
			Player player = iPlayer as Player;
			if (player == null)
			{
				return;
			}
			this.TryRemoveMarker(player);
		}

		// Token: 0x06000244 RID: 580 RVA: 0x0000C6A4 File Offset: 0x0000A8A4
		private void RemoveNonActivePlayers()
		{
			HashSet<Player> hashSet = new HashSet<Player>(Singleton<GameWorld>.Instance.AllAlivePlayersList);
			foreach (Player player in Enumerable.ToList<Player>(this._playerMarkers.Keys))
			{
				if (player.HasCorpse() || !hashSet.Contains(player))
				{
					this.TryRemoveMarker(player);
				}
			}
		}

		// Token: 0x06000245 RID: 581 RVA: 0x0000C724 File Offset: 0x0000A924
		public void RefreshMarkers()
		{
			if (!GameUtils.IsInRaid())
			{
				return;
			}
			foreach (KeyValuePair<Player, PlayerMapMarker> keyValuePair in Enumerable.ToArray<KeyValuePair<Player, PlayerMapMarker>>(this._playerMarkers))
			{
				if (!keyValuePair.Key.IsYourPlayer)
				{
					this.TryRemoveMarker(keyValuePair.Key);
					this.TryAddMarker(keyValuePair.Key);
				}
			}
		}

		// Token: 0x06000246 RID: 582 RVA: 0x0000C784 File Offset: 0x0000A984
		private void TryAddMarker(IPlayer iPlayer)
		{
			Player player = iPlayer as Player;
			if (player == null || player.IsDedicatedServer())
			{
				Plugin.Log.LogDebug("TryAddMarker: Skipping - player is null or dedicated server");
				return;
			}
			if (this._lastMapView == null || player.IsBTRShooter() || this._playerMarkers.ContainsKey(player))
			{
				Plugin.Log.LogDebug(string.Format("TryAddMarker: Skipping {0} - MapView null: {1}, BTR shooter: {2}, Already has marker: {3}", new object[]
				{
					player.Profile.Nickname,
					this._lastMapView == null,
					player.IsBTRShooter(),
					this._playerMarkers.ContainsKey(player)
				}));
				return;
			}
			Plugin.Log.LogInfo("TryAddMarker: Processing player " + player.Profile.Nickname);
			string text = string.Empty;
			string imagePath = string.Empty;
			Color color = Color.clear;
			int? intelLevel = GameUtils.GetIntelLevel();
			Plugin.Log.LogInfo(string.Format("TryAddMarker: Current intel level: {0}", intelLevel));
			if (player.IsGroupedWithMainPlayer())
			{
				int value = Settings.ShowFriendlyIntelLevel.Value;
				int? num = intelLevel;
				if (value <= num.GetValueOrDefault() & num != null)
				{
					text = "Friendly Player";
					imagePath = "Markers/arrow.png";
					color = OtherPlayersMarkerProvider._friendlyPlayerColor;
					Plugin.Log.LogInfo("TryAddMarker: " + player.Profile.Nickname + " identified as FRIENDLY player");
					goto IL_312;
				}
			}
			if (player.IsTrackedBoss())
			{
				int value2 = Settings.ShowBossIntelLevel.Value;
				int? num = intelLevel;
				if (value2 <= num.GetValueOrDefault() & num != null)
				{
					text = "Boss";
					imagePath = "Markers/star.png";
					color = Settings.BossColor.Value;
					Plugin.Log.LogInfo("TryAddMarker: " + player.Profile.Nickname + " identified as BOSS");
					goto IL_312;
				}
			}
			if (player.IsPMC())
			{
				int value3 = Settings.ShowPmcIntelLevel.Value;
				int? num = intelLevel;
				if (value3 <= num.GetValueOrDefault() & num != null)
				{
					text = "Enemy Player";
					imagePath = "Markers/arrow.png";
					color = ((player.Side == EPlayerSide.Bear) ? Settings.PmcBearColor.Value : Settings.PmcUsecColor.Value);
					Plugin.Log.LogInfo(string.Format("TryAddMarker: {0} identified as ENEMY PMC ({1})", player.Profile.Nickname, player.Side));
					goto IL_312;
				}
			}
			if (player.IsScav())
			{
				int value4 = Settings.ShowScavIntelLevel.Value;
				int? num = intelLevel;
				if (value4 <= num.GetValueOrDefault() & num != null)
				{
					text = "Scav";
					imagePath = "Markers/arrow.png";
					color = Settings.ScavColor.Value;
					Plugin.Log.LogInfo("TryAddMarker: " + player.Profile.Nickname + " identified as SCAV");
					goto IL_312;
				}
			}
			Plugin.Log.LogInfo(string.Format("TryAddMarker: {0} does not match any category or intel level too low. IsPMC: {1}, IsScav: {2}, IsTrackedBoss: {3}, IsGrouped: {4}", new object[]
			{
				player.Profile.Nickname,
				player.IsPMC(),
				player.IsScav(),
				player.IsTrackedBoss(),
				player.IsGroupedWithMainPlayer()
			}));
			IL_312:
			if (!this.ShouldShowCategory(text))
			{
				Plugin.Log.LogInfo(string.Format("TryAddMarker: Category '{0}' is disabled for {1}. ShowFriendly: {2}, ShowEnemy: {3}, ShowScavs: {4}, ShowBosses: {5}", new object[]
				{
					text,
					player.Profile.Nickname,
					this._showFriendlyPlayers,
					this._showEnemyPlayers,
					this._showScavs,
					this._showBosses
				}));
				return;
			}
			Plugin.Log.LogInfo(string.Concat(new string[]
			{
				"TryAddMarker: Adding marker for ",
				player.Profile.Nickname,
				" with category '",
				text,
				"'"
			}));
			PlayerMapMarker value5 = this._lastMapView.AddPlayerMarker(player, text, color, imagePath);
			this._playerMarkers[player] = value5;
			Plugin.Log.LogInfo("TryAddMarker: Successfully added marker for " + player.Profile.Nickname);
		}

		// Token: 0x06000247 RID: 583 RVA: 0x0000CB90 File Offset: 0x0000AD90
		private void RemoveDisabledMarkers()
		{
			foreach (Player player in Enumerable.ToList<Player>(this._playerMarkers.Keys))
			{
				PlayerMapMarker playerMapMarker = this._playerMarkers[player];
				if (!this.ShouldShowCategory(playerMapMarker.Category))
				{
					this.TryRemoveMarker(player);
				}
			}
		}

		// Token: 0x06000248 RID: 584 RVA: 0x0000CC08 File Offset: 0x0000AE08
		private void TryRemoveMarker(Player player)
		{
			if (!this._playerMarkers.ContainsKey(player))
			{
				return;
			}
			this._playerMarkers[player].ContainingMapView.RemoveMapMarker(this._playerMarkers[player]);
			this._playerMarkers.Remove(player);
		}

		// Token: 0x06000249 RID: 585 RVA: 0x0000CC48 File Offset: 0x0000AE48
		private bool ShouldShowCategory(string category)
		{
			if (category == "Friendly Player")
			{
				return this._showFriendlyPlayers;
			}
			if (category == "Enemy Player")
			{
				return this._showEnemyPlayers;
			}
			if (!(category == "Boss"))
			{
				return category == "Scav" && this._showScavs;
			}
			return this._showBosses;
		}

		// Token: 0x0600024A RID: 586 RVA: 0x0000CCA8 File Offset: 0x0000AEA8
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

		// Token: 0x0600024B RID: 587 RVA: 0x0000CCC4 File Offset: 0x0000AEC4
		public void OnShowOutOfRaid(MapView map)
		{
		}

		// Token: 0x0600024C RID: 588 RVA: 0x0000CCC6 File Offset: 0x0000AEC6
		public void OnHideOutOfRaid(MapView map)
		{
		}

		// Token: 0x04000123 RID: 291
		private const string _arrowImagePath = "Markers/arrow.png";

		// Token: 0x04000124 RID: 292
		private const string _starImagePath = "Markers/star.png";

		// Token: 0x04000125 RID: 293
		private const string _friendlyPlayerCategory = "Friendly Player";

		// Token: 0x04000126 RID: 294
		private const string _friendlyPlayerImagePath = "Markers/arrow.png";

		// Token: 0x04000127 RID: 295
		private static Color _friendlyPlayerColor = Color.Lerp(Color.blue, Color.white, 0.5f);

		// Token: 0x04000128 RID: 296
		private const string _enemyPlayerCategory = "Enemy Player";

		// Token: 0x04000129 RID: 297
		private const string _enemyPlayerImagePath = "Markers/arrow.png";

		// Token: 0x0400012A RID: 298
		private const string _scavCategory = "Scav";

		// Token: 0x0400012B RID: 299
		private const string _scavImagePath = "Markers/arrow.png";

		// Token: 0x0400012C RID: 300
		private const string _bossCategory = "Boss";

		// Token: 0x0400012D RID: 301
		private const string _bossImagePath = "Markers/star.png";

		// Token: 0x0400012E RID: 302
		private bool _showFriendlyPlayers = true;

		// Token: 0x0400012F RID: 303
		private bool _showEnemyPlayers;

		// Token: 0x04000130 RID: 304
		private bool _showScavs;

		// Token: 0x04000131 RID: 305
		private bool _showBosses;

		// Token: 0x04000132 RID: 306
		private MapView _lastMapView;

		// Token: 0x04000133 RID: 307
		private Dictionary<Player, PlayerMapMarker> _playerMarkers = new Dictionary<Player, PlayerMapMarker>();
	}
}
