using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using BepInEx.Configuration;
using DynamicMaps.ExternalModSupport;
using DynamicMaps.Utils;
using UnityEngine;

namespace DynamicMaps.Config
{
	// Token: 0x02000043 RID: 67
	internal static class Settings
	{
		// Token: 0x060002CB RID: 715 RVA: 0x0000DB0C File Offset: 0x0000BD0C
		public static void Init(ConfigFile config)
		{
			Settings.ConfigEntries.Add(Settings.ReplaceMapScreen = config.Bind<bool>("1. General", "Replace Map Screen", true, new ConfigDescription("If the map should replace the BSG default map screen, requires swapping away from modded map to refresh", null, new object[]
			{
				new ConfigurationManagerAttributes()
			})));
			Settings.ConfigEntries.Add(Settings.CenterOnPlayerHotkey = config.Bind<KeyboardShortcut>("1. General", "Center on Player Hotkey", new KeyboardShortcut(KeyCode.Semicolon, Array.Empty<KeyCode>()), new ConfigDescription("Pressed while the map is open, centers the player", null, new object[]
			{
				new ConfigurationManagerAttributes()
			})));
			Settings.ConfigEntries.Add(Settings.MoveMapUpHotkey = config.Bind<KeyboardShortcut>("1. General", "Move Map Up Hotkey", new KeyboardShortcut(KeyCode.UpArrow, Array.Empty<KeyCode>()), new ConfigDescription("Hotkey to move the map up", null, new object[]
			{
				new ConfigurationManagerAttributes()
			})));
			Settings.ConfigEntries.Add(Settings.MoveMapDownHotkey = config.Bind<KeyboardShortcut>("1. General", "Move Map Down Hotkey", new KeyboardShortcut(KeyCode.DownArrow, Array.Empty<KeyCode>()), new ConfigDescription("Hotkey to move the map down", null, new object[]
			{
				new ConfigurationManagerAttributes()
			})));
			Settings.ConfigEntries.Add(Settings.MoveMapLeftHotkey = config.Bind<KeyboardShortcut>("1. General", "Move Map Left Hotkey", new KeyboardShortcut(KeyCode.LeftArrow, Array.Empty<KeyCode>()), new ConfigDescription("Hotkey to move the map left", null, new object[]
			{
				new ConfigurationManagerAttributes()
			})));
			Settings.ConfigEntries.Add(Settings.MoveMapRightHotkey = config.Bind<KeyboardShortcut>("1. General", "Move Map Right Hotkey", new KeyboardShortcut(KeyCode.RightArrow, Array.Empty<KeyCode>()), new ConfigDescription("Hotkey to move the map right", null, new object[]
			{
				new ConfigurationManagerAttributes()
			})));
			Settings.ConfigEntries.Add(Settings.MapMoveHotkeySpeed = config.Bind<float>("1. General", "Move Map Hotkey Speed", 0.25f, new ConfigDescription("How fast the map should move, units are map percent per second", new AcceptableValueRange<float>(0.05f, 2f), new object[]
			{
				new ConfigurationManagerAttributes()
			})));
			Settings.ConfigEntries.Add(Settings.ChangeMapLevelUpHotkey = config.Bind<KeyboardShortcut>("1. General", "Change Map Level Up Hotkey", new KeyboardShortcut(KeyCode.Period, Array.Empty<KeyCode>()), new ConfigDescription("Hotkey to move the map level up (shift-scroll-up also does this in map screen)", null, new object[]
			{
				new ConfigurationManagerAttributes()
			})));
			Settings.ConfigEntries.Add(Settings.ChangeMapLevelDownHotkey = config.Bind<KeyboardShortcut>("1. General", "Change Map Level Down Hotkey", new KeyboardShortcut(KeyCode.Comma, Array.Empty<KeyCode>()), new ConfigDescription("Hotkey to move the map level down (shift-scroll-down also does this in map screen)", null, new object[]
			{
				new ConfigurationManagerAttributes()
			})));
			Settings.ConfigEntries.Add(Settings.ZoomMapInHotkey = config.Bind<KeyboardShortcut>("1. General", "Zoom Map In Hotkey", new KeyboardShortcut(KeyCode.Equals, Array.Empty<KeyCode>()), new ConfigDescription("Hotkey to zoom the map in (scroll-up also does this in map screen)", null, new object[]
			{
				new ConfigurationManagerAttributes()
			})));
			Settings.ConfigEntries.Add(Settings.ZoomMapOutHotkey = config.Bind<KeyboardShortcut>("1. General", "Zoom Map Out Hotkey", new KeyboardShortcut(KeyCode.Minus, Array.Empty<KeyCode>()), new ConfigDescription("Hotkey to zoom the map out (scroll-down also does this in map screen)", null, new object[]
			{
				new ConfigurationManagerAttributes()
			})));
			Settings.ConfigEntries.Add(Settings.ZoomMapHotkeySpeed = config.Bind<float>("1. General", "Zoom Map Hotkey Speed", 2.5f, new ConfigDescription("How fast the map should zoom by hotkey", new AcceptableValueRange<float>(1f, 10f), new object[]
			{
				new ConfigurationManagerAttributes()
			})));
			Settings.ConfigEntries.Add(Settings.DumpInfoHotkey = config.Bind<KeyboardShortcut>("1. General", "Dump Info Hotkey", new KeyboardShortcut(KeyCode.D, new KeyCode[]
			{
				KeyCode.LeftShift,
				KeyCode.LeftAlt
			}), new ConfigDescription("Pressed while the map is open, dumps json MarkerDefs for extracts, loot, and switches into root of plugin folder", null, new object[]
			{
				new ConfigurationManagerAttributes
				{
					IsAdvanced = new bool?(true)
				}
			})));
			Settings.ConfigEntries.Add(Settings.ShowPlayerMarker = config.Bind<bool>("2. Dynamic Markers", "Show Player Marker", true, new ConfigDescription("If the player marker should be shown in raid", null, new object[]
			{
				new ConfigurationManagerAttributes()
			})));
			Settings.ConfigEntries.Add(Settings.ShowFriendlyPlayerMarkersInRaid = config.Bind<bool>("2. Dynamic Markers", "Show Friendly Player Markers", true, new ConfigDescription("If friendly player markers should be shown in-raid", null, new object[]
			{
				new ConfigurationManagerAttributes()
			})));
			Settings.ConfigEntries.Add(Settings.ShowEnemyPlayerMarkersInRaid = config.Bind<bool>("2. Dynamic Markers", "Show Enemy Player Markers", false, new ConfigDescription("If enemy player markers should be shown in-raid (generally for debug)", null, new object[]
			{
				new ConfigurationManagerAttributes()
			})));
			Settings.ConfigEntries.Add(Settings.ShowScavMarkersInRaid = config.Bind<bool>("2. Dynamic Markers", "Show Scav Markers", false, new ConfigDescription("If enemy scav markers should be shown in-raid (generally for debug)", null, new object[]
			{
				new ConfigurationManagerAttributes()
			})));
			Settings.ConfigEntries.Add(Settings.ShowBossMarkersInRaid = config.Bind<bool>("2. Dynamic Markers", "Show Boss Markers", false, new ConfigDescription("If enemy boss markers should be shown in-raid", null, new object[]
			{
				new ConfigurationManagerAttributes()
			})));
			Settings.ConfigEntries.Add(Settings.ShowLockedDoorStatus = config.Bind<bool>("2. Dynamic Markers", "Show Locked Door Status", true, new ConfigDescription("If locked door markers should be updated with status based on key acquisition", null, new object[]
			{
				new ConfigurationManagerAttributes()
			})));
			Settings.ConfigEntries.Add(Settings.ShowQuestsInRaid = config.Bind<bool>("2. Dynamic Markers", "Show Quests In Raid", true, new ConfigDescription("If quests should be shown in raid", null, new object[]
			{
				new ConfigurationManagerAttributes()
			})));
			Settings.ConfigEntries.Add(Settings.ShowExtractsInRaid = config.Bind<bool>("2. Dynamic Markers", "Show Extracts In Raid", true, new ConfigDescription("If extracts should be shown in raid", null, new object[]
			{
				new ConfigurationManagerAttributes()
			})));
			Settings.ConfigEntries.Add(Settings.ShowExtractStatusInRaid = config.Bind<bool>("2. Dynamic Markers", "Show Extracts Status In Raid", true, new ConfigDescription("If extracts should be colored according to their status in raid", null, new object[]
			{
				new ConfigurationManagerAttributes()
			})));
			Settings.ConfigEntries.Add(Settings.ShowTransitPointsInRaid = config.Bind<bool>("2. Dynamic Markers", "Show Transit Points In Raid", true, new ConfigDescription("If transits should be shown in raid", null, new object[]
			{
				new ConfigurationManagerAttributes()
			})));
			Settings.ConfigEntries.Add(Settings.ShowSecretPointsInRaid = config.Bind<bool>("2. Dynamic Markers", "Show Secret Extracts In Raid", true, new ConfigDescription("If secret extracts should be shown in raid", null, new object[]
			{
				new ConfigurationManagerAttributes()
			})));
			Settings.ConfigEntries.Add(Settings.ShowDroppedBackpackInRaid = config.Bind<bool>("2. Dynamic Markers", "Show Dropped Backpack In Raid", true, new ConfigDescription("If the player's dropped backpacks (not anyone elses) should be shown in raid", null, new object[]
			{
				new ConfigurationManagerAttributes()
			})));
			Settings.ConfigEntries.Add(Settings.ShowWishListItemsInRaid = config.Bind<bool>("2. Dynamic Markers", "Show wish listed items In Raid", true, new ConfigDescription("Shows items that are in your wishlist on the map in raid", null, new object[]
			{
				new ConfigurationManagerAttributes()
			})));
			Settings.ConfigEntries.Add(Settings.ShowBTRInRaid = config.Bind<bool>("2. Dynamic Markers", "Show BTR In Raid", true, new ConfigDescription("If the BTR should be shown in raid", null, new object[]
			{
				new ConfigurationManagerAttributes()
			})));
			Settings.ConfigEntries.Add(Settings.ShowAirdropsInRaid = config.Bind<bool>("2. Dynamic Markers", "Show Airdrops In Raid", true, new ConfigDescription("If airdrops should be shown in raid when they land", null, new object[]
			{
				new ConfigurationManagerAttributes()
			})));
			Settings.ConfigEntries.Add(Settings.ShowHiddenStashesInRaid = config.Bind<bool>("2. Dynamic Markers", "Show Hidden Stashes In Raid", true, new ConfigDescription("If hidden stashes should be shown in raid", null, new object[]
			{
				new ConfigurationManagerAttributes()
			})));
			Settings.ConfigEntries.Add(Settings.ShowFriendlyCorpsesInRaid = config.Bind<bool>("2. Dynamic Markers", "Show Friendly Corpses In Raid", true, new ConfigDescription("If friendly corpses should be shown in raid", null, new object[]
			{
				new ConfigurationManagerAttributes()
			})));
			Settings.ConfigEntries.Add(Settings.ShowKilledCorpsesInRaid = config.Bind<bool>("2. Dynamic Markers", "Show Player-killed Corpses In Raid", true, new ConfigDescription("If corpses killed by the player should be shown in raid, killed bosses will be shown in another color", null, new object[]
			{
				new ConfigurationManagerAttributes()
			})));
			Settings.ConfigEntries.Add(Settings.ShowFriendlyKilledCorpsesInRaid = config.Bind<bool>("2. Dynamic Markers", "Show Friendly-killed Corpses In Raid", true, new ConfigDescription("If corpses killed by friendly players should be shown in raid, killed bosses will be shown in another color", null, new object[]
			{
				new ConfigurationManagerAttributes()
			})));
			Settings.ConfigEntries.Add(Settings.ShowBossCorpsesInRaid = config.Bind<bool>("2. Dynamic Markers", "Show Boss Corpses In Raid", false, new ConfigDescription("If boss corpses (other than ones killed by the player) should be shown in raid", null, new object[]
			{
				new ConfigurationManagerAttributes()
			})));
			Settings.ConfigEntries.Add(Settings.ShowOtherCorpsesInRaid = config.Bind<bool>("2. Dynamic Markers", "Show Other Corpses In Raid", false, new ConfigDescription("If corpses (other than friendly ones or ones killed by the player) should be shown in raid", null, new object[]
			{
				new ConfigurationManagerAttributes()
			})));
			Settings.ConfigEntries.Add(Settings.RequireMapInInventory = config.Bind<bool>("3. Progression", "Require a map in your inventory", false, new ConfigDescription("Requires you to have a map in your inventory in order to view the map in raid.", null, new object[]
			{
				new ConfigurationManagerAttributes()
			})));
			Settings.ConfigEntries.Add(Settings.ShowPmcIntelLevel = config.Bind<int>("3. Progression", "Intel level required to show PMCs", 0, new ConfigDescription("If intel level is at or above this value it will show PMCs", new AcceptableValueRange<int>(0, 3), new object[]
			{
				new ConfigurationManagerAttributes()
			})));
			Settings.ConfigEntries.Add(Settings.ShowBossIntelLevel = config.Bind<int>("3. Progression", "Intel level required to show bosses", 0, new ConfigDescription("If intel level is at or above this value it will show bosses", new AcceptableValueRange<int>(0, 3), new object[]
			{
				new ConfigurationManagerAttributes()
			})));
			Settings.ConfigEntries.Add(Settings.ShowScavIntelLevel = config.Bind<int>("3. Progression", "Intel level required to show scavs", 0, new ConfigDescription("If intel level is at or above this value it will show scavs", new AcceptableValueRange<int>(0, 3), new object[]
			{
				new ConfigurationManagerAttributes()
			})));
			Settings.ConfigEntries.Add(Settings.ShowFriendlyIntelLevel = config.Bind<int>("3. Progression", "Intel level required to show friendly PMCs", 0, new ConfigDescription("If intel level is at or above this value it will show friendly PMCs", new AcceptableValueRange<int>(0, 3), new object[]
			{
				new ConfigurationManagerAttributes()
			})));
			Settings.ConfigEntries.Add(Settings.ShowCorpseIntelLevel = config.Bind<int>("3. Progression", "Intel level required to show corpses", 0, new ConfigDescription("If intel level is at or above this value it will show corpses", new AcceptableValueRange<int>(0, 3), new object[]
			{
				new ConfigurationManagerAttributes()
			})));
			Settings.ConfigEntries.Add(Settings.ShowAirdropIntelLevel = config.Bind<int>("3. Progression", "Intel level required to show airdrops", 0, new ConfigDescription("If intel level is at or above this value it will show airdrops", new AcceptableValueRange<int>(0, 3), new object[]
			{
				new ConfigurationManagerAttributes()
			})));
			Settings.ConfigEntries.Add(Settings.ShowWishListItemsIntelLevel = config.Bind<int>("3. Progression", "Intel level required to show wish listed loot items", 0, new ConfigDescription("If intel level is at or above this value it will show wish listed loot items", new AcceptableValueRange<int>(0, 3), new object[]
			{
				new ConfigurationManagerAttributes()
			})));
			Settings.ConfigEntries.Add(Settings.ShowHiddenStashIntelLevel = config.Bind<int>("3. Progression", "Intel level required to show hidden stashes", 0, new ConfigDescription("If intel level is at or above this value it will show hidden stashes", new AcceptableValueRange<int>(0, 3), new object[]
			{
				new ConfigurationManagerAttributes()
			})));
			Settings.ConfigEntries.Add(Settings.AutoSelectLevel = config.Bind<bool>("4. In-Raid", "Auto Select Level", true, new ConfigDescription("If the level should be automatically selected based on the players position in raid", null, new object[]
			{
				new ConfigurationManagerAttributes()
			})));
			Settings.ConfigEntries.Add(Settings.AutoCenterOnPlayerMarker = config.Bind<bool>("4. In-Raid", "Auto Center On Player Marker", false, new ConfigDescription("If the player marker should be centered when showing the map in raid (Conflicts with 'Remember Map Position')", null, new object[]
			{
				new ConfigurationManagerAttributes()
			})));
			Settings.ConfigEntries.Add(Settings.ResetZoomOnCenter = config.Bind<bool>("4. In-Raid", "Reset Zoom On Center", false, new ConfigDescription("If the zoom level should be reset each time that the map is opened while in raid (Conflicts with 'Remember Map Position')", null, new object[]
			{
				new ConfigurationManagerAttributes()
			})));
			Settings.ConfigEntries.Add(Settings.RetainMapPosition = config.Bind<bool>("4. In-Raid", "Remember Map Position", true, new ConfigDescription("Should we remember the map position (Map position memory is only maintained for the current raid) (Conflicts with 'Auto Center On Player Marker' and 'Reset Zoom On Center')", null, new object[]
			{
				new ConfigurationManagerAttributes()
			})));
			Settings.ConfigEntries.Add(Settings.CenteringZoomResetPoint = config.Bind<float>("4. In-Raid", "Centering On Player Zoom Level", 0.15f, new ConfigDescription("What zoom level should be used as while centering on the player (0 is fully zoomed out, and 1 is fully zoomed in)", new AcceptableValueRange<float>(0f, 1f), new object[]
			{
				new ConfigurationManagerAttributes()
			})));
			Settings.ConfigEntries.Add(Settings.ZoomMainMap = config.Bind<float>("4. In-Raid", "Main map zoom", 0f, new ConfigDescription("What zoom level should be used for the main map. (Tab view/Peek view) (0 is fully zoomed out, and 1 is fully zoomed in)", new AcceptableValueRange<float>(0f, 15f), new object[]
			{
				new ConfigurationManagerAttributes()
			})));
			Settings.ConfigEntries.Add(Settings.PeekShortcut = config.Bind<KeyboardShortcut>("4. In-Raid", "Peek at Map Shortcut", new KeyboardShortcut(KeyCode.M, Array.Empty<KeyCode>()), new ConfigDescription("The keyboard shortcut to peek at the map", null, new object[]
			{
				new ConfigurationManagerAttributes()
			})));
			Settings.ConfigEntries.Add(Settings.HoldForPeek = config.Bind<bool>("4. In-Raid", "Hold for Peek", true, new ConfigDescription("If the shortcut should be held to keep it open. If disabled, button toggles", null, new object[]
			{
				new ConfigurationManagerAttributes()
			})));
			Settings.ConfigEntries.Add(Settings.MapTransitionEnabled = config.Bind<bool>("4. In-Raid", "Peek Transition enabled", true, new ConfigDescription("Enable the map transition animations (When disabled everything will snap)", null, new object[]
			{
				new ConfigurationManagerAttributes()
			})));
			ConfigEntry<bool> autoCenterOnPlayerMarker = Settings.AutoCenterOnPlayerMarker;
			EventHandler eventHandler;
			if ((eventHandler = Settings.<>O.<0>__OnAutoOrCenterEnable) == null)
			{
				eventHandler = (Settings.<>O.<0>__OnAutoOrCenterEnable = new EventHandler(Settings.OnAutoOrCenterEnable));
			}
			autoCenterOnPlayerMarker.SettingChanged += eventHandler;
			ConfigEntry<bool> resetZoomOnCenter = Settings.ResetZoomOnCenter;
			EventHandler eventHandler2;
			if ((eventHandler2 = Settings.<>O.<0>__OnAutoOrCenterEnable) == null)
			{
				eventHandler2 = (Settings.<>O.<0>__OnAutoOrCenterEnable = new EventHandler(Settings.OnAutoOrCenterEnable));
			}
			resetZoomOnCenter.SettingChanged += eventHandler2;
			ConfigEntry<bool> retainMapPosition = Settings.RetainMapPosition;
			EventHandler eventHandler3;
			if ((eventHandler3 = Settings.<>O.<1>__OnPositionRetainEnable) == null)
			{
				eventHandler3 = (Settings.<>O.<1>__OnPositionRetainEnable = new EventHandler(Settings.OnPositionRetainEnable));
			}
			retainMapPosition.SettingChanged += eventHandler3;
			Settings.ConfigEntries.Add(Settings.MiniMapEnabled = config.Bind<bool>("5. Mini-map", "Mini-map enabled", true, new ConfigDescription("Enable the mini-map", null, new object[]
			{
				new ConfigurationManagerAttributes()
			})));
			Settings.ConfigEntries.Add(Settings.MiniMapPosition = config.Bind<EMiniMapPosition>("5. Mini-map", "Mini-map position", EMiniMapPosition.TopRight, new ConfigDescription("What corner is the mini-map displayed in", null, new object[]
			{
				new ConfigurationManagerAttributes()
			})));
			Settings.ConfigEntries.Add(Settings.MiniMapSizeX = config.Bind<float>("5. Mini-map", "Mini-map size horizontal", 275f, new ConfigDescription("Horizontal size of the mini-map", new AcceptableValueRange<float>(0f, 850f), new object[]
			{
				new ConfigurationManagerAttributes()
			})));
			Settings.ConfigEntries.Add(Settings.MiniMapSizeY = config.Bind<float>("5. Mini-map", "Mini-map size vertical", 275f, new ConfigDescription("Vertical size of the mini-map", new AcceptableValueRange<float>(0f, 850f), new object[]
			{
				new ConfigurationManagerAttributes()
			})));
			Settings.ConfigEntries.Add(Settings.MiniMapScreenOffsetX = config.Bind<float>("5. Mini-map", "Mini-map offset horizontal", 0f, new ConfigDescription("Horizontal Offset from the edge (These values update according to screen resolution, REQUIRES RESTART IF YOU CHANGED YOUR RESOLUTION)", new AcceptableValueRange<float>((float)(-(float)Screen.width) / 4f, (float)Screen.width), new object[]
			{
				new ConfigurationManagerAttributes()
			})));
			Settings.ConfigEntries.Add(Settings.MiniMapScreenOffsetY = config.Bind<float>("5. Mini-map", "Mini-map offset vertical", 0f, new ConfigDescription("Vertical offset from the edge (These values update according to screen resolution, REQUIRES RESTART IF YOU CHANGED YOUR RESOLUTION)", new AcceptableValueRange<float>((float)(-(float)Screen.height) / 4f, (float)Screen.height), new object[]
			{
				new ConfigurationManagerAttributes()
			})));
			Settings.ConfigEntries.Add(Settings.MiniMapShowOrHide = config.Bind<KeyboardShortcut>("5. Mini-map", "Show or Hide the mini-map", new KeyboardShortcut(KeyCode.End, Array.Empty<KeyCode>()), new ConfigDescription("Show or hide the mini-map", null, new object[]
			{
				new ConfigurationManagerAttributes()
			})));
			Settings.ConfigEntries.Add(Settings.ZoomMiniMap = config.Bind<float>("5. Mini-map", "Mini map zoom", 5f, new ConfigDescription("What zoom level should be used for the mini map. (0 is fully zoomed out, and 15 is fully zoomed in)", new AcceptableValueRange<float>(0f, 15f), new object[]
			{
				new ConfigurationManagerAttributes()
			})));
			Settings.ConfigEntries.Add(Settings.ZoomInMiniMapHotkey = config.Bind<KeyboardShortcut>("5. Mini-map", "Zoom in key bind", new KeyboardShortcut(KeyCode.Keypad8, Array.Empty<KeyCode>()), new ConfigDescription("Zoom in on mini map key bind", null, new object[]
			{
				new ConfigurationManagerAttributes()
			})));
			Settings.ConfigEntries.Add(Settings.ZoomOutMiniMapHotkey = config.Bind<KeyboardShortcut>("5. Mini-map", "Zoom out key bind", new KeyboardShortcut(KeyCode.Keypad5, Array.Empty<KeyCode>()), new ConfigDescription("Zoom out on mini map key bind", null, new object[]
			{
				new ConfigurationManagerAttributes()
			})));
			Settings.ConfigEntries.Add(Settings.PlayerColor = config.Bind<Color>("6. Marker Colors", "Your player marker color", new Color(0f, 1f, 0f), new ConfigDescription("Color of the marker", null, new object[]
			{
				new ConfigurationManagerAttributes()
			})));
			Settings.ConfigEntries.Add(Settings.PmcBearColor = config.Bind<Color>("6. Marker Colors", "Bear marker color", new Color(1f, 0f, 0f), new ConfigDescription("Color of the marker", null, new object[]
			{
				new ConfigurationManagerAttributes()
			})));
			Settings.ConfigEntries.Add(Settings.PmcUsecColor = config.Bind<Color>("6. Marker Colors", "Usec marker color", new Color(1f, 1f, 0f), new ConfigDescription("Color of the marker", null, new object[]
			{
				new ConfigurationManagerAttributes()
			})));
			Settings.ConfigEntries.Add(Settings.ScavColor = config.Bind<Color>("6. Marker Colors", "Scav marker color", new Color(1f, 0.45f, 0.007f), new ConfigDescription("Color of the marker", null, new object[]
			{
				new ConfigurationManagerAttributes()
			})));
			Settings.ConfigEntries.Add(Settings.BossColor = config.Bind<Color>("6. Marker Colors", "Boss marker color", new Color(1f, 0.45f, 0.007f), new ConfigDescription("Color of the marker", null, new object[]
			{
				new ConfigurationManagerAttributes()
			})));
			Settings.ConfigEntries.Add(Settings.AirdropColor = config.Bind<Color>("6. Marker Colors", "Airdrop marker color", new Color(1f, 0.3f, 0.007f), new ConfigDescription("Color of the marker", null, new object[]
			{
				new ConfigurationManagerAttributes()
			})));
			Settings.ConfigEntries.Add(Settings.BackpackColor = config.Bind<Color>("6. Marker Colors", "Backpack marker color", new Color(0f, 1f, 0f), new ConfigDescription("Color of the marker", null, new object[]
			{
				new ConfigurationManagerAttributes()
			})));
			Settings.ConfigEntries.Add(Settings.LootItemColor = config.Bind<Color>("6. Marker Colors", "Loot marker color", new Color(0.98f, 0.81f, 0.007f), new ConfigDescription("Color of the marker", null, new object[]
			{
				new ConfigurationManagerAttributes()
			})));
			Settings.ConfigEntries.Add(Settings.KilledCorpseColor = config.Bind<Color>("6. Marker Colors", "Killed corpse marker color", new Color(1f, 0f, 0f), new ConfigDescription("Color of the marker", null, new object[]
			{
				new ConfigurationManagerAttributes()
			})));
			Settings.ConfigEntries.Add(Settings.KilledBossColor = config.Bind<Color>("6. Marker Colors", "Killed boss corpse marker color", new Color(1f, 0f, 1f), new ConfigDescription("Color of the marker", null, new object[]
			{
				new ConfigurationManagerAttributes()
			})));
			Settings.ConfigEntries.Add(Settings.KilledOtherColor = config.Bind<Color>("6. Marker Colors", "Killed by other corpse marker color", new Color(1f, 1f, 1f), new ConfigDescription("Color of the marker", null, new object[]
			{
				new ConfigurationManagerAttributes()
			})));
			Settings.ConfigEntries.Add(Settings.BtrColor = config.Bind<Color>("6. Marker Colors", "BTR marker color", new Color(0.21f, 0.39f, 0.16f), new ConfigDescription("Color of the marker", null, new object[]
			{
				new ConfigurationManagerAttributes()
			})));
			Settings.ConfigEntries.Add(Settings.ExtractDefaultColor = config.Bind<Color>("6. Marker Colors", "Extract default marker color", new Color(1f, 0.92f, 0.01f), new ConfigDescription("Color of the marker", null, new object[]
			{
				new ConfigurationManagerAttributes()
			})));
			Settings.ConfigEntries.Add(Settings.ExtractOpenColor = config.Bind<Color>("6. Marker Colors", "Extract open marker color", new Color(0f, 1f, 0f), new ConfigDescription("Color of the marker", null, new object[]
			{
				new ConfigurationManagerAttributes()
			})));
			Settings.ConfigEntries.Add(Settings.ExtractClosedColor = config.Bind<Color>("6. Marker Colors", "Extract closed marker color", new Color(1f, 0f, 0f), new ConfigDescription("Color of the marker", null, new object[]
			{
				new ConfigurationManagerAttributes()
			})));
			Settings.ConfigEntries.Add(Settings.ExtractHasRequirementsColor = config.Bind<Color>("6. Marker Colors", "Extract has requirements marker color", new Color(1f, 0.92f, 0.01f), new ConfigDescription("Color of the marker", null, new object[]
			{
				new ConfigurationManagerAttributes()
			})));
			Settings.ConfigEntries.Add(Settings.TransPointColor = config.Bind<Color>("6. Marker Colors", "Transit point marker color", new Color(1f, 0.62f, 0.2f), new ConfigDescription("Color of the marker", null, new object[]
			{
				new ConfigurationManagerAttributes()
			})));
			Settings.ConfigEntries.Add(Settings.SecretPointColor = config.Bind<Color>("6. Marker Colors", "Secret exfil point marker color", new Color(0.1f, 0.6f, 0.6f), new ConfigDescription("Color of the marker", null, new object[]
			{
				new ConfigurationManagerAttributes()
			})));
			Settings.ConfigEntries.Add(Settings.HiddenStashColor = config.Bind<Color>("6. Marker Colors", "Hidden stash marker color", new Color(1f, 0.92f, 0.01f), new ConfigDescription("Color of the marker", null, new object[]
			{
				new ConfigurationManagerAttributes()
			})));
			Settings.ConfigEntries.Add(Settings.ShowHeliCrashMarker = config.Bind<bool>("7. External Mod Support", "Show Heli Crash Marker", true, new ConfigDescription("If the heli crash site should be marked in raid", null, new object[]
			{
				new ConfigurationManagerAttributes
				{
					Browsable = new bool?(ModDetection.HeliCrashLoaded)
				}
			})));
			Settings.RecalcOrder();
		}

		// Token: 0x060002CC RID: 716 RVA: 0x0000F14C File Offset: 0x0000D34C
		private static void RecalcOrder()
		{
			int num = Settings.ConfigEntries.Count;
			foreach (ConfigEntryBase configEntryBase in Settings.ConfigEntries)
			{
				ConfigurationManagerAttributes configurationManagerAttributes = configEntryBase.Description.Tags[0] as ConfigurationManagerAttributes;
				if (configurationManagerAttributes != null)
				{
					configurationManagerAttributes.Order = new int?(num);
				}
				num--;
			}
		}

		// Token: 0x060002CD RID: 717 RVA: 0x0000F1C8 File Offset: 0x0000D3C8
		private static void OnAutoOrCenterEnable(object sender, EventArgs e)
		{
			if (Settings.AutoCenterOnPlayerMarker.Value || Settings.ResetZoomOnCenter.Value)
			{
				Settings.RetainMapPosition.Value = false;
			}
		}

		// Token: 0x060002CE RID: 718 RVA: 0x0000F1ED File Offset: 0x0000D3ED
		private static void OnPositionRetainEnable(object sender, EventArgs e)
		{
			if (Settings.RetainMapPosition.Value)
			{
				Settings.AutoCenterOnPlayerMarker.Value = false;
				Settings.ResetZoomOnCenter.Value = false;
			}
		}

		// Token: 0x04000162 RID: 354
		private static readonly List<ConfigEntryBase> ConfigEntries = new List<ConfigEntryBase>();

		// Token: 0x04000163 RID: 355
		private const string GeneralTitle = "1. General";

		// Token: 0x04000164 RID: 356
		public static ConfigEntry<bool> ReplaceMapScreen;

		// Token: 0x04000165 RID: 357
		public static ConfigEntry<KeyboardShortcut> CenterOnPlayerHotkey;

		// Token: 0x04000166 RID: 358
		public static ConfigEntry<KeyboardShortcut> DumpInfoHotkey;

		// Token: 0x04000167 RID: 359
		public static ConfigEntry<KeyboardShortcut> MoveMapUpHotkey;

		// Token: 0x04000168 RID: 360
		public static ConfigEntry<KeyboardShortcut> MoveMapDownHotkey;

		// Token: 0x04000169 RID: 361
		public static ConfigEntry<KeyboardShortcut> MoveMapLeftHotkey;

		// Token: 0x0400016A RID: 362
		public static ConfigEntry<KeyboardShortcut> MoveMapRightHotkey;

		// Token: 0x0400016B RID: 363
		public static ConfigEntry<float> MapMoveHotkeySpeed;

		// Token: 0x0400016C RID: 364
		public static ConfigEntry<KeyboardShortcut> ChangeMapLevelUpHotkey;

		// Token: 0x0400016D RID: 365
		public static ConfigEntry<KeyboardShortcut> ChangeMapLevelDownHotkey;

		// Token: 0x0400016E RID: 366
		public static ConfigEntry<KeyboardShortcut> ZoomMapInHotkey;

		// Token: 0x0400016F RID: 367
		public static ConfigEntry<KeyboardShortcut> ZoomMapOutHotkey;

		// Token: 0x04000170 RID: 368
		public static ConfigEntry<float> ZoomMapHotkeySpeed;

		// Token: 0x04000171 RID: 369
		private const string DynamicMarkerTitle = "2. Dynamic Markers";

		// Token: 0x04000172 RID: 370
		public static ConfigEntry<bool> ShowPlayerMarker;

		// Token: 0x04000173 RID: 371
		public static ConfigEntry<bool> ShowFriendlyPlayerMarkersInRaid;

		// Token: 0x04000174 RID: 372
		public static ConfigEntry<bool> ShowEnemyPlayerMarkersInRaid;

		// Token: 0x04000175 RID: 373
		public static ConfigEntry<bool> ShowScavMarkersInRaid;

		// Token: 0x04000176 RID: 374
		public static ConfigEntry<bool> ShowBossMarkersInRaid;

		// Token: 0x04000177 RID: 375
		public static ConfigEntry<bool> ShowLockedDoorStatus;

		// Token: 0x04000178 RID: 376
		public static ConfigEntry<bool> ShowQuestsInRaid;

		// Token: 0x04000179 RID: 377
		public static ConfigEntry<bool> ShowExtractsInRaid;

		// Token: 0x0400017A RID: 378
		public static ConfigEntry<bool> ShowExtractStatusInRaid;

		// Token: 0x0400017B RID: 379
		public static ConfigEntry<bool> ShowTransitPointsInRaid;

		// Token: 0x0400017C RID: 380
		public static ConfigEntry<bool> ShowSecretPointsInRaid;

		// Token: 0x0400017D RID: 381
		public static ConfigEntry<bool> ShowDroppedBackpackInRaid;

		// Token: 0x0400017E RID: 382
		public static ConfigEntry<bool> ShowWishListItemsInRaid;

		// Token: 0x0400017F RID: 383
		public static ConfigEntry<bool> ShowBTRInRaid;

		// Token: 0x04000180 RID: 384
		public static ConfigEntry<bool> ShowAirdropsInRaid;

		// Token: 0x04000181 RID: 385
		public static ConfigEntry<bool> ShowHiddenStashesInRaid;

		// Token: 0x04000182 RID: 386
		public static ConfigEntry<bool> ShowFriendlyCorpsesInRaid;

		// Token: 0x04000183 RID: 387
		public static ConfigEntry<bool> ShowKilledCorpsesInRaid;

		// Token: 0x04000184 RID: 388
		public static ConfigEntry<bool> ShowFriendlyKilledCorpsesInRaid;

		// Token: 0x04000185 RID: 389
		public static ConfigEntry<bool> ShowBossCorpsesInRaid;

		// Token: 0x04000186 RID: 390
		public static ConfigEntry<bool> ShowOtherCorpsesInRaid;

		// Token: 0x04000187 RID: 391
		private const string ProgressionTitle = "3. Progression";

		// Token: 0x04000188 RID: 392
		public static ConfigEntry<bool> RequireMapInInventory;

		// Token: 0x04000189 RID: 393
		public static ConfigEntry<int> ShowPmcIntelLevel;

		// Token: 0x0400018A RID: 394
		public static ConfigEntry<int> ShowBossIntelLevel;

		// Token: 0x0400018B RID: 395
		public static ConfigEntry<int> ShowScavIntelLevel;

		// Token: 0x0400018C RID: 396
		public static ConfigEntry<int> ShowFriendlyIntelLevel;

		// Token: 0x0400018D RID: 397
		public static ConfigEntry<int> ShowCorpseIntelLevel;

		// Token: 0x0400018E RID: 398
		public static ConfigEntry<int> ShowAirdropIntelLevel;

		// Token: 0x0400018F RID: 399
		public static ConfigEntry<int> ShowWishListItemsIntelLevel;

		// Token: 0x04000190 RID: 400
		public static ConfigEntry<int> ShowHiddenStashIntelLevel;

		// Token: 0x04000191 RID: 401
		private const string InRaidTitle = "4. In-Raid";

		// Token: 0x04000192 RID: 402
		public static ConfigEntry<bool> ResetZoomOnCenter;

		// Token: 0x04000193 RID: 403
		public static ConfigEntry<float> CenteringZoomResetPoint;

		// Token: 0x04000194 RID: 404
		public static ConfigEntry<float> ZoomMainMap;

		// Token: 0x04000195 RID: 405
		public static ConfigEntry<bool> AutoCenterOnPlayerMarker;

		// Token: 0x04000196 RID: 406
		public static ConfigEntry<bool> RetainMapPosition;

		// Token: 0x04000197 RID: 407
		public static ConfigEntry<bool> AutoSelectLevel;

		// Token: 0x04000198 RID: 408
		public static ConfigEntry<KeyboardShortcut> PeekShortcut;

		// Token: 0x04000199 RID: 409
		public static ConfigEntry<bool> HoldForPeek;

		// Token: 0x0400019A RID: 410
		private const string MiniMapTitle = "5. Mini-map";

		// Token: 0x0400019B RID: 411
		public static ConfigEntry<bool> MiniMapEnabled;

		// Token: 0x0400019C RID: 412
		public static ConfigEntry<EMiniMapPosition> MiniMapPosition;

		// Token: 0x0400019D RID: 413
		public static ConfigEntry<float> MiniMapSizeX;

		// Token: 0x0400019E RID: 414
		public static ConfigEntry<float> MiniMapSizeY;

		// Token: 0x0400019F RID: 415
		public static ConfigEntry<float> MiniMapScreenOffsetX;

		// Token: 0x040001A0 RID: 416
		public static ConfigEntry<float> MiniMapScreenOffsetY;

		// Token: 0x040001A1 RID: 417
		public static ConfigEntry<bool> MapTransitionEnabled;

		// Token: 0x040001A2 RID: 418
		public static ConfigEntry<KeyboardShortcut> MiniMapShowOrHide;

		// Token: 0x040001A3 RID: 419
		public static ConfigEntry<float> ZoomMiniMap;

		// Token: 0x040001A4 RID: 420
		public static ConfigEntry<KeyboardShortcut> ZoomInMiniMapHotkey;

		// Token: 0x040001A5 RID: 421
		public static ConfigEntry<KeyboardShortcut> ZoomOutMiniMapHotkey;

		// Token: 0x040001A6 RID: 422
		private const string MarkerColors = "6. Marker Colors";

		// Token: 0x040001A7 RID: 423
		public static ConfigEntry<Color> PlayerColor;

		// Token: 0x040001A8 RID: 424
		public static ConfigEntry<Color> PmcBearColor;

		// Token: 0x040001A9 RID: 425
		public static ConfigEntry<Color> PmcUsecColor;

		// Token: 0x040001AA RID: 426
		public static ConfigEntry<Color> ScavColor;

		// Token: 0x040001AB RID: 427
		public static ConfigEntry<Color> BossColor;

		// Token: 0x040001AC RID: 428
		public static ConfigEntry<Color> AirdropColor;

		// Token: 0x040001AD RID: 429
		public static ConfigEntry<Color> BackpackColor;

		// Token: 0x040001AE RID: 430
		public static ConfigEntry<Color> LootItemColor;

		// Token: 0x040001AF RID: 431
		public static ConfigEntry<Color> KilledCorpseColor;

		// Token: 0x040001B0 RID: 432
		public static ConfigEntry<Color> KilledBossColor;

		// Token: 0x040001B1 RID: 433
		public static ConfigEntry<Color> KilledOtherColor;

		// Token: 0x040001B2 RID: 434
		public static ConfigEntry<Color> BtrColor;

		// Token: 0x040001B3 RID: 435
		public static ConfigEntry<Color> ExtractDefaultColor;

		// Token: 0x040001B4 RID: 436
		public static ConfigEntry<Color> ExtractOpenColor;

		// Token: 0x040001B5 RID: 437
		public static ConfigEntry<Color> ExtractClosedColor;

		// Token: 0x040001B6 RID: 438
		public static ConfigEntry<Color> ExtractHasRequirementsColor;

		// Token: 0x040001B7 RID: 439
		public static ConfigEntry<Color> TransPointColor;

		// Token: 0x040001B8 RID: 440
		public static ConfigEntry<Color> SecretPointColor;

		// Token: 0x040001B9 RID: 441
		public static ConfigEntry<Color> HiddenStashColor;

		// Token: 0x040001BA RID: 442
		private const string ExternModSupport = "7. External Mod Support";

		// Token: 0x040001BB RID: 443
		public static ConfigEntry<bool> ShowHeliCrashMarker;

		// Token: 0x0200005F RID: 95
		[CompilerGenerated]
		private static class <>O
		{
			// Token: 0x04000201 RID: 513
			public static EventHandler <0>__OnAutoOrCenterEnable;

			// Token: 0x04000202 RID: 514
			public static EventHandler <1>__OnPositionRetainEnable;
		}
	}
}
