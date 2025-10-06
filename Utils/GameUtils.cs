using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Comfort.Common;
using DynamicMaps.Config;
using EFT;
using EFT.InventoryLogic;
using EFT.Vehicle;
using HarmonyLib;
using SPT.Reflection.Utils;

namespace DynamicMaps.Utils
{
	// Token: 0x0200000C RID: 12
	public static class GameUtils
	{
		// Token: 0x17000003 RID: 3
		// (get) Token: 0x06000021 RID: 33 RVA: 0x00002A8C File Offset: 0x00000C8C
		public static ISession Session
		{
			get
			{
				return ClientAppUtils.GetMainApp().GetClientBackEndSession();
			}
		}

		// Token: 0x17000004 RID: 4
		// (get) Token: 0x06000022 RID: 34 RVA: 0x00002A98 File Offset: 0x00000C98
		public static Profile PlayerProfile
		{
			get
			{
				return GameUtils._sessionProfileProperty.GetValue(GameUtils.Session) as Profile;
			}
		}

		// Token: 0x06000023 RID: 35 RVA: 0x00002AB0 File Offset: 0x00000CB0
		public static bool IsInRaid()
		{
			AbstractGame instance = Singleton<AbstractGame>.Instance;
			IBotGame instance2 = Singleton<IBotGame>.Instance;
			return (instance != null && instance.InRaid) || (instance2 != null && instance2.Status != GameStatus.Stopped && instance2.Status != GameStatus.Stopping && instance2.Status != GameStatus.SoftStopping);
		}

		// Token: 0x06000024 RID: 36 RVA: 0x00002AFE File Offset: 0x00000CFE
		public static string GetCurrentMapInternalName()
		{
			GameWorld instance = Singleton<GameWorld>.Instance;
			if (instance == null)
			{
				return null;
			}
			Player mainPlayer = instance.MainPlayer;
			if (mainPlayer == null)
			{
				return null;
			}
			return mainPlayer.Location;
		}

		// Token: 0x06000025 RID: 37 RVA: 0x00002B1B File Offset: 0x00000D1B
		public static Player GetMainPlayer()
		{
			GameWorld instance = Singleton<GameWorld>.Instance;
			if (instance == null)
			{
				return null;
			}
			return instance.MainPlayer;
		}

		// Token: 0x06000026 RID: 38 RVA: 0x00002B2D File Offset: 0x00000D2D
		public static Profile GetPlayerProfile()
		{
			return GameUtils.PlayerProfile;
		}

		// Token: 0x06000027 RID: 39 RVA: 0x00002B34 File Offset: 0x00000D34
		public static BTRView GetBTRView()
		{
			GameWorld instance = Singleton<GameWorld>.Instance;
			if (instance == null)
			{
				return null;
			}
			BTRControllerClass btrController = instance.BtrController;
			if (btrController == null)
			{
				return null;
			}
			return btrController.BtrView;
		}

		// Token: 0x06000028 RID: 40 RVA: 0x00002B54 File Offset: 0x00000D54
		public static bool IsScavRaid()
		{
			Player mainPlayer = GameUtils.GetMainPlayer();
			return GameUtils.IsInRaid() && mainPlayer != null && mainPlayer.Side == EPlayerSide.Savage;
		}

		// Token: 0x06000029 RID: 41 RVA: 0x00002B82 File Offset: 0x00000D82
		public static string BSGLocalized(this MongoID id)
		{
			if (string.IsNullOrWhiteSpace(id))
			{
				return "";
			}
			return id.ToString().Localized(null);
		}

		// Token: 0x0600002A RID: 42 RVA: 0x00002BAA File Offset: 0x00000DAA
		public static string BSGLocalized(this string id)
		{
			if (string.IsNullOrWhiteSpace(id))
			{
				return "";
			}
			return id.Localized(null);
		}

		// Token: 0x0600002B RID: 43 RVA: 0x00002BC4 File Offset: 0x00000DC4
		public static bool IsGroupedWithMainPlayer(this IPlayer player)
		{
			string groupId = GameUtils.GetMainPlayer().GroupId;
			return !string.IsNullOrEmpty(groupId) && player.GroupId == groupId;
		}

		// Token: 0x0600002C RID: 44 RVA: 0x00002BF2 File Offset: 0x00000DF2
		public static bool IsTrackedBoss(this IPlayer player)
		{
			return player.Profile.Side == EPlayerSide.Savage && GameUtils._trackedBosses.Contains(player.Profile.Info.Settings.Role);
		}

		// Token: 0x0600002D RID: 45 RVA: 0x00002C23 File Offset: 0x00000E23
		public static bool IsPMC(this IPlayer player)
		{
			return player.Profile.Side == EPlayerSide.Bear || player.Profile.Side == EPlayerSide.Usec;
		}

		// Token: 0x0600002E RID: 46 RVA: 0x00002C43 File Offset: 0x00000E43
		public static bool IsScav(this IPlayer player)
		{
			return player.Profile.Side == EPlayerSide.Savage;
		}

		// Token: 0x0600002F RID: 47 RVA: 0x00002C54 File Offset: 0x00000E54
		public static bool DidMainPlayerKill(this IPlayer player)
		{
			IPlayer player2 = GameUtils._playerLastAggressorField.GetValue(player) as IPlayer;
			if (player2 == null)
			{
				return false;
			}
			Player mainPlayer = GameUtils.GetMainPlayer();
			return player2.ProfileId == mainPlayer.ProfileId;
		}

		// Token: 0x06000030 RID: 48 RVA: 0x00002C94 File Offset: 0x00000E94
		public static bool DidTeammateKill(this IPlayer player)
		{
			IPlayer player2 = GameUtils._playerLastAggressorField.GetValue(player) as IPlayer;
			if (player2 == null || string.IsNullOrEmpty(player2.GroupId))
			{
				return false;
			}
			Player mainPlayer = GameUtils.GetMainPlayer();
			return player2.ProfileId != mainPlayer.ProfileId && player2.GroupId == GameUtils.GetMainPlayer().GroupId;
		}

		// Token: 0x06000031 RID: 49 RVA: 0x00002CF7 File Offset: 0x00000EF7
		public static bool IsBTRShooter(this IPlayer player)
		{
			return player.Profile.Side == EPlayerSide.Savage && player.Profile.Info.Settings.Role == WildSpawnType.shooterBTR;
		}

		// Token: 0x06000032 RID: 50 RVA: 0x00002D22 File Offset: 0x00000F22
		public static bool HasCorpse(this Player player)
		{
			return GameUtils._playerCorpseField.GetValue(player) != null;
		}

		// Token: 0x06000033 RID: 51 RVA: 0x00002D34 File Offset: 0x00000F34
		public static bool IsDedicatedServer(this IPlayer player)
		{
			string text = "^headless_[a-fA-F0-9]{24}$";
			return Regex.IsMatch(player.Profile.GetCorrectedNickname(), text);
		}

		// Token: 0x06000034 RID: 52 RVA: 0x00002D58 File Offset: 0x00000F58
		public static int? GetIntelLevel()
		{
			GClass2204 gclass = Enumerable.SingleOrDefault<GClass2204>(GameUtils.PlayerProfile.Hideout.Areas, (GClass2204 a) => a.AreaType == EAreaType.IntelligenceCenter);
			if (gclass == null)
			{
				return null;
			}
			return new int?(gclass.Level);
		}

		// Token: 0x06000035 RID: 53 RVA: 0x00002DB0 File Offset: 0x00000FB0
		public static MongoID[] GetWishListItems()
		{
			return Enumerable.ToArray<MongoID>(GameUtils.PlayerProfile.WishlistManager.GetWishlist().Keys);
		}

		// Token: 0x06000036 RID: 54 RVA: 0x00002DCC File Offset: 0x00000FCC
		public static bool ShouldShowMapInRaid()
		{
			if (!Settings.RequireMapInInventory.Value || !GameUtils.IsInRaid())
			{
				return true;
			}
			Player mainPlayer = GameUtils.GetMainPlayer();
			string location = mainPlayer.Location;
			string id;
			if (!GameUtils.MapLookUp.TryGetValue(location, out id))
			{
				Plugin.Log.LogWarning("Could not find map id for location " + location + " is this a new location?");
				return true;
			}
			return Enumerable.Any<Item>(mainPlayer.Inventory.Equipment.GetAllItems(), (Item m) => m.TemplateId == id);
		}

		// Token: 0x06000037 RID: 55 RVA: 0x00002E54 File Offset: 0x00001054
		// Note: this type is marked as 'beforefieldinit'.
		static GameUtils()
		{
			HashSet<WildSpawnType> hashSet = new HashSet<WildSpawnType>();
			hashSet.Add(WildSpawnType.bossBoar);
			hashSet.Add(WildSpawnType.bossBully);
			hashSet.Add(WildSpawnType.bossGluhar);
			hashSet.Add(WildSpawnType.bossKilla);
			hashSet.Add(WildSpawnType.bossKnight);
			hashSet.Add(WildSpawnType.followerBigPipe);
			hashSet.Add(WildSpawnType.followerBirdEye);
			hashSet.Add(WildSpawnType.bossKolontay);
			hashSet.Add(WildSpawnType.bossKojaniy);
			hashSet.Add(WildSpawnType.bossSanitar);
			hashSet.Add(WildSpawnType.bossTagilla);
			hashSet.Add(WildSpawnType.bossPartisan);
			hashSet.Add(WildSpawnType.bossZryachiy);
			hashSet.Add(WildSpawnType.gifter);
			hashSet.Add(WildSpawnType.arenaFighterEvent);
			hashSet.Add(WildSpawnType.sectantPriest);
			hashSet.Add((WildSpawnType)199);
			hashSet.Add((WildSpawnType)801);
			GameUtils._trackedBosses = hashSet;
			GameUtils.MapLookUp = new Dictionary<string, string>
			{
				{
					"factory4_day",
					"574eb85c245977648157eec3"
				},
				{
					"factory4_night",
					"574eb85c245977648157eec3"
				},
				{
					"Woods",
					"5900b89686f7744e704a8747"
				},
				{
					"bigmap",
					"5798a2832459774b53341029"
				},
				{
					"Interchange",
					"5be4038986f774527d3fae60"
				},
				{
					"RezervBase",
					"6738034a9713b5f42b4a8b78"
				},
				{
					"Shoreline",
					"5a8036fb86f77407252ddc02"
				},
				{
					"laboratory",
					"6738034e9d22459ad7cd1b81"
				},
				{
					"Lighthouse",
					"6738035350b24a4ae4a57997"
				},
				{
					"TarkovStreets",
					"673803448cb3819668d77b1b"
				},
				{
					"Sandbox",
					"6738033eb7305d3bdafe9518"
				},
				{
					"Sandbox_high",
					"6738033eb7305d3bdafe9518"
				}
			};
		}

		// Token: 0x0400002E RID: 46
		private static FieldInfo _playerCorpseField = AccessTools.Field(typeof(Player), "Corpse");

		// Token: 0x0400002F RID: 47
		private static FieldInfo _playerLastAggressorField = AccessTools.Field(typeof(Player), "LastAggressor");

		// Token: 0x04000030 RID: 48
		private static Type _profileInterface = Enumerable.First<Type>(typeof(ISession).GetInterfaces(), delegate(Type i)
		{
			PropertyInfo[] properties = i.GetProperties();
			if (properties.Length == 2)
			{
				return Enumerable.Any<PropertyInfo>(properties, (PropertyInfo p) => p.Name == "Profile");
			}
			return false;
		});

		// Token: 0x04000031 RID: 49
		private static PropertyInfo _sessionProfileProperty = AccessTools.Property(GameUtils._profileInterface, "Profile");

		// Token: 0x04000032 RID: 50
		private static HashSet<WildSpawnType> _trackedBosses;

		// Token: 0x04000033 RID: 51
		private static readonly Dictionary<string, string> MapLookUp;
	}
}
