using System;
using System.Collections;
using System.Collections.Generic;
using BepInEx.Configuration;
using Comfort.Common;
using DG.Tweening;
using DynamicMaps.Config;
using DynamicMaps.Data;
using DynamicMaps.DynamicMarkers;
using DynamicMaps.ExternalModSupport;
using DynamicMaps.ExternalModSupport.SamSWATHeliCrash;
using DynamicMaps.Patches;
using DynamicMaps.UI.Components;
using DynamicMaps.UI.Controls;
using DynamicMaps.Utils;
using EFT;
using EFT.UI;
using UnityEngine;
using UnityEngine.UI;

namespace DynamicMaps.UI
{
	// Token: 0x02000011 RID: 17
	public class ModdedMapScreen : MonoBehaviour
	{
		// Token: 0x17000005 RID: 5
		// (get) Token: 0x0600005D RID: 93 RVA: 0x000049DC File Offset: 0x00002BDC
		public RectTransform RectTransform
		{
			get
			{
				return base.gameObject.GetRectTransform();
			}
		}

		// Token: 0x17000006 RID: 6
		// (get) Token: 0x0600005E RID: 94 RVA: 0x000049E9 File Offset: 0x00002BE9
		private RectTransform _parentTransform
		{
			get
			{
				return base.gameObject.transform.parent as RectTransform;
			}
		}

		// Token: 0x17000007 RID: 7
		// (get) Token: 0x0600005F RID: 95 RVA: 0x00004A00 File Offset: 0x00002C00
		private bool _isPeeking
		{
			get
			{
				return this._peekComponent != null && this._peekComponent.IsPeeking;
			}
		}

		// Token: 0x17000008 RID: 8
		// (get) Token: 0x06000060 RID: 96 RVA: 0x00004A1D File Offset: 0x00002C1D
		private bool _showingMiniMap
		{
			get
			{
				return this._peekComponent != null && this._peekComponent.ShowingMiniMap;
			}
		}

		// Token: 0x17000009 RID: 9
		// (get) Token: 0x06000061 RID: 97 RVA: 0x00004A3A File Offset: 0x00002C3A
		// (set) Token: 0x06000062 RID: 98 RVA: 0x00004A42 File Offset: 0x00002C42
		public bool IsShowingMapScreen { get; private set; }

		// Token: 0x06000063 RID: 99 RVA: 0x00004A4B File Offset: 0x00002C4B
		internal static ModdedMapScreen Create(GameObject parent)
		{
			return UIUtils.CreateUIGameObject(parent, "ModdedMapBlock").AddComponent<ModdedMapScreen>();
		}

		// Token: 0x06000064 RID: 100 RVA: 0x00004A60 File Offset: 0x00002C60
		private void Awake()
		{
			GameObject gameObject = UIUtils.CreateUIGameObject(base.gameObject, "Scroll");
			GameObject gameObject2 = UIUtils.CreateUIGameObject(gameObject, "ScrollMask");
			Settings.MiniMapPosition.SettingChanged += delegate(object sender, EventArgs args)
			{
				this.AdjustForMiniMap(false);
			};
			Settings.MiniMapScreenOffsetX.SettingChanged += delegate(object sender, EventArgs args)
			{
				this.AdjustForMiniMap(false);
			};
			Settings.MiniMapScreenOffsetY.SettingChanged += delegate(object sender, EventArgs args)
			{
				this.AdjustForMiniMap(false);
			};
			Settings.MiniMapSizeX.SettingChanged += delegate(object sender, EventArgs args)
			{
				this.AdjustForMiniMap(false);
			};
			Settings.MiniMapSizeY.SettingChanged += delegate(object sender, EventArgs args)
			{
				this.AdjustForMiniMap(false);
			};
			this._mapView = MapView.Create(gameObject2, "MapView");
			gameObject2.AddComponent<Image>().color = new Color(0f, 0f, 0f, 0.5f);
			this._scrollMask = gameObject2.AddComponent<Mask>();
			this._scrollRect = gameObject.AddComponent<ScrollRect>();
			this._scrollRect.scrollSensitivity = 0f;
			this._scrollRect.movementType = ScrollRect.MovementType.Unrestricted;
			this._scrollRect.viewport = this._scrollMask.GetRectTransform();
			this._scrollRect.content = this._mapView.RectTransform;
			GameObject gameObject3 = Singleton<CommonUI>.Instance.transform.Find("Common UI/InventoryScreen/Map Panel/MapBlock/ZoomScroll").gameObject;
			this._levelSelectSlider = LevelSelectSlider.Create(gameObject3, this.RectTransform);
			this._levelSelectSlider.OnLevelSelectedBySlider += this._mapView.SelectTopLevel;
			this._mapView.OnLevelSelected += delegate(int level)
			{
				this._levelSelectSlider.SelectedLevel = level;
			};
			GameObject gameObject4 = Singleton<CommonUI>.Instance.transform.Find("Common UI/InventoryScreen/SkillsAndMasteringPanel/BottomPanel/SkillsPanel/Options/Filter").gameObject;
			this._mapSelectDropdown = MapSelectDropdown.Create(gameObject4, this.RectTransform);
			this._mapSelectDropdown.OnMapSelected += this.ChangeMap;
			this._cursorPositionText = CursorPositionText.Create(base.gameObject, this._mapView.RectTransform, ModdedMapScreen._positionTextFontSize);
			this._cursorPositionText.RectTransform.anchorMin = ModdedMapScreen._textAnchor;
			this._cursorPositionText.RectTransform.anchorMax = ModdedMapScreen._textAnchor;
			this._playerPositionText = PlayerPositionText.Create(base.gameObject, ModdedMapScreen._positionTextFontSize);
			this._playerPositionText.RectTransform.anchorMin = ModdedMapScreen._textAnchor;
			this._playerPositionText.RectTransform.anchorMax = ModdedMapScreen._textAnchor;
			this._playerPositionText.gameObject.SetActive(false);
			this.ReadConfig();
			GameWorldOnDestroyPatch.OnRaidEnd += this.OnRaidEnd;
			this._mapSelectDropdown.LoadMapDefsFromPath("Maps");
			this.PrecacheMapLayerImages();
		}

		// Token: 0x06000065 RID: 101 RVA: 0x00004CF8 File Offset: 0x00002EF8
		private void OnDestroy()
		{
			GameWorldOnDestroyPatch.OnRaidEnd -= this.OnRaidEnd;
			Settings.MiniMapPosition.SettingChanged -= delegate(object sender, EventArgs args)
			{
				this.AdjustForMiniMap(false);
			};
			Settings.MiniMapScreenOffsetX.SettingChanged -= delegate(object sender, EventArgs args)
			{
				this.AdjustForMiniMap(false);
			};
			Settings.MiniMapScreenOffsetY.SettingChanged -= delegate(object sender, EventArgs args)
			{
				this.AdjustForMiniMap(false);
			};
			Settings.MiniMapSizeX.SettingChanged -= delegate(object sender, EventArgs args)
			{
				this.AdjustForMiniMap(false);
			};
			Settings.MiniMapSizeY.SettingChanged -= delegate(object sender, EventArgs args)
			{
				this.AdjustForMiniMap(false);
			};
		}

		// Token: 0x06000066 RID: 102 RVA: 0x00004D84 File Offset: 0x00002F84
		private void Update()
		{
			float axis = Input.GetAxis("Mouse ScrollWheel");
			if (axis != 0f && (!this._mapSelectDropdown.isActiveAndEnabled || !this._mapSelectDropdown.IsDropdownOpen()))
			{
				this.OnScroll(axis);
			}
			if (!this._showingMiniMap)
			{
				if (this._moveMapLevelUpShortcut.BetterIsDown())
				{
					this._levelSelectSlider.ChangeLevelBy(1);
				}
				if (this._moveMapLevelDownShortcut.BetterIsDown())
				{
					this._levelSelectSlider.ChangeLevelBy(-1);
				}
			}
			float num = 0f;
			float num2 = 0f;
			if (!this._showingMiniMap)
			{
				if (this._moveMapUpShortcut.BetterIsPressed())
				{
					num2 += 1f;
				}
				if (this._moveMapDownShortcut.BetterIsPressed())
				{
					num2 -= 1f;
				}
				if (this._moveMapLeftShortcut.BetterIsPressed())
				{
					num -= 1f;
				}
				if (this._moveMapRightShortcut.BetterIsPressed())
				{
					num += 1f;
				}
			}
			if (num != 0f || num2 != 0f)
			{
				this._mapView.ScaledShiftMap(new Vector2(num, num2), this._moveMapSpeed * Time.deltaTime, false);
			}
			if (this._showingMiniMap)
			{
				this.OnZoomMini();
			}
			else
			{
				this.OnZoomMain();
			}
			this.OnCenter();
			if (this._dumpShortcut.BetterIsDown())
			{
				DumpUtils.DumpExtracts();
				DumpUtils.DumpSwitches();
				DumpUtils.DumpLocks();
			}
		}

		// Token: 0x06000067 RID: 103 RVA: 0x00004ED0 File Offset: 0x000030D0
		internal void OnMapScreenShow()
		{
			if (this._peekComponent != null)
			{
				this._peekComponent.WasMiniMapActive = this._showingMiniMap;
				MapPeekComponent peekComponent = this._peekComponent;
				if (peekComponent != null)
				{
					peekComponent.EndPeek();
				}
				MapPeekComponent peekComponent2 = this._peekComponent;
				if (peekComponent2 != null)
				{
					peekComponent2.EndMiniMap();
				}
			}
			this.IsShowingMapScreen = true;
			if (this._rememberMapPosition)
			{
				this._mapView.SetMapPos(this._mapView.MainMapPos, 0f);
			}
			base.transform.parent.Find("MapBlock").gameObject.SetActive(false);
			base.transform.parent.Find("EmptyBlock").gameObject.SetActive(false);
			base.transform.parent.gameObject.SetActive(true);
			this.Show(false);
		}

		// Token: 0x06000068 RID: 104 RVA: 0x00004F9F File Offset: 0x0000319F
		internal void OnMapScreenClose()
		{
			this.Hide();
			this.IsShowingMapScreen = false;
			if (this._peekComponent != null && this._peekComponent.WasMiniMapActive)
			{
				this._peekComponent.BeginMiniMap(true);
			}
		}

		// Token: 0x06000069 RID: 105 RVA: 0x00004FD0 File Offset: 0x000031D0
		internal void Show(bool playAnimation)
		{
			if (!this._initialized)
			{
				this.AdjustSizeAndPosition();
				this._initialized = true;
			}
			this._isShown = true;
			base.gameObject.SetActive(GameUtils.ShouldShowMapInRaid());
			this._mapSelectDropdown.LoadMapDefsFromPath("Maps");
			if (GameUtils.IsInRaid())
			{
				this.OnShowInRaid(playAnimation);
				return;
			}
			this.OnShowOutOfRaid();
		}

		// Token: 0x0600006A RID: 106 RVA: 0x0000502E File Offset: 0x0000322E
		internal void Hide()
		{
			MapSelectDropdown mapSelectDropdown = this._mapSelectDropdown;
			if (mapSelectDropdown != null)
			{
				mapSelectDropdown.TryCloseDropdown();
			}
			if (GameUtils.IsInRaid())
			{
				this.OnHideInRaid();
			}
			else
			{
				this.OnHideOutOfRaid();
			}
			this._isShown = false;
			base.gameObject.SetActive(false);
		}

		// Token: 0x0600006B RID: 107 RVA: 0x0000506C File Offset: 0x0000326C
		private void OnRaidEnd()
		{
			if (!BattleUIScreenShowPatch.IsAttached)
			{
				return;
			}
			foreach (IDynamicMarkerProvider dynamicMarkerProvider in this._dynamicMarkerProviders.Values)
			{
				try
				{
					dynamicMarkerProvider.OnRaidEnd(this._mapView);
				}
				catch (Exception ex)
				{
					Plugin.Log.LogError(string.Format("Dynamic marker provider {0} threw exception in OnRaidEnd", dynamicMarkerProvider));
					Plugin.Log.LogError("  Exception given was: " + ex.Message);
					Plugin.Log.LogError("  " + ex.StackTrace);
				}
			}
			MapPeekComponent peekComponent = this._peekComponent;
			if (peekComponent != null)
			{
				peekComponent.EndPeek();
			}
			MapPeekComponent peekComponent2 = this._peekComponent;
			if (peekComponent2 != null)
			{
				peekComponent2.EndMiniMap();
			}
			Object.Destroy(this._peekComponent.gameObject);
			this._peekComponent = null;
			this._mapView.UnloadMap();
		}

		// Token: 0x0600006C RID: 108 RVA: 0x00005170 File Offset: 0x00003370
		private void AdjustSizeAndPosition()
		{
			Rect rect = Singleton<CommonUI>.Instance.InventoryScreen.GetRectTransform().rect;
			this.RectTransform.sizeDelta = new Vector2(rect.width, rect.height);
			this.RectTransform.anchoredPosition = Vector2.zero;
			this._scrollRect.GetRectTransform().sizeDelta = this.RectTransform.sizeDelta;
			this._scrollMask.GetRectTransform().anchoredPosition = ModdedMapScreen._maskPositionOutOfRaid;
			this._scrollMask.GetRectTransform().sizeDelta = this.RectTransform.sizeDelta + ModdedMapScreen._maskSizeModifierOutOfRaid;
			this._levelSelectSlider.RectTransform.anchoredPosition = ModdedMapScreen._levelSliderPosition;
			this._mapSelectDropdown.RectTransform.sizeDelta = ModdedMapScreen._mapSelectDropdownSize;
			this._mapSelectDropdown.RectTransform.anchoredPosition = ModdedMapScreen._mapSelectDropdownPosition;
			this._cursorPositionText.RectTransform.anchoredPosition = ModdedMapScreen._cursorPositionTextOffset;
			this._playerPositionText.RectTransform.anchoredPosition = ModdedMapScreen._playerPositionTextOffset;
		}

		// Token: 0x0600006D RID: 109 RVA: 0x00005280 File Offset: 0x00003480
		private void AdjustForOutOfRaid()
		{
			this._scrollMask.GetRectTransform().anchoredPosition = ModdedMapScreen._maskPositionOutOfRaid;
			this._scrollMask.GetRectTransform().sizeDelta = this.RectTransform.sizeDelta + ModdedMapScreen._maskSizeModifierOutOfRaid;
			this._cursorPositionText.gameObject.SetActive(true);
			this._levelSelectSlider.gameObject.SetActive(true);
			this._playerPositionText.gameObject.SetActive(false);
		}

		// Token: 0x0600006E RID: 110 RVA: 0x000052FC File Offset: 0x000034FC
		private void AdjustForInRaid(bool playAnimation)
		{
			float num = playAnimation ? 0.35f : 0f;
			this._scrollMask.GetRectTransform().DOSizeDelta(this.RectTransform.sizeDelta + ModdedMapScreen._maskSizeModifierInRaid, this._transitionAnimations ? num : 0f, false);
			this._scrollMask.GetRectTransform().DOAnchorPos(ModdedMapScreen._maskPositionInRaid, this._transitionAnimations ? num : 0f, false);
			this._cursorPositionText.gameObject.SetActive(true);
			this._playerPositionText.gameObject.SetActive(true);
			this._levelSelectSlider.gameObject.SetActive(true);
		}

		// Token: 0x0600006F RID: 111 RVA: 0x000053AC File Offset: 0x000035AC
		private void AdjustForPeek(bool playAnimation)
		{
			float num = playAnimation ? 0.35f : 0f;
			this._scrollMask.GetRectTransform().DOAnchorPos(Vector2.zero, this._transitionAnimations ? num : 0f, false);
			this._scrollMask.GetRectTransform().DOSizeDelta(this.RectTransform.sizeDelta, this._transitionAnimations ? num : 0f, false);
			this._cursorPositionText.gameObject.SetActive(false);
			this._playerPositionText.gameObject.SetActive(false);
			this._levelSelectSlider.gameObject.SetActive(false);
		}

		// Token: 0x06000070 RID: 112 RVA: 0x00005450 File Offset: 0x00003650
		private void AdjustForMiniMap(bool playAnimation)
		{
			float num = playAnimation ? 0.35f : 0f;
			Vector2 endValue = this.ConvertEnumToScreenPos(Settings.MiniMapPosition.Value);
			Vector2 vector = new Vector2(Settings.MiniMapScreenOffsetX.Value, Settings.MiniMapScreenOffsetY.Value);
			vector *= this.ConvertEnumToScenePivot(Settings.MiniMapPosition.Value);
			Vector2 endValue2 = new Vector2(Settings.MiniMapSizeX.Value, Settings.MiniMapSizeY.Value);
			this._scrollMask.GetRectTransform().DOSizeDelta(endValue2, this._transitionAnimations ? num : 0f, false);
			this._scrollMask.GetRectTransform().DOAnchorPos(vector, this._transitionAnimations ? num : 0f, false);
			this._scrollMask.GetRectTransform().DOAnchorMin(endValue, this._transitionAnimations ? num : 0f, false);
			this._scrollMask.GetRectTransform().DOAnchorMax(endValue, this._transitionAnimations ? num : 0f, false);
			this._scrollMask.GetRectTransform().DOPivot(endValue, this._transitionAnimations ? num : 0f);
			this._cursorPositionText.gameObject.SetActive(false);
			this._playerPositionText.gameObject.SetActive(false);
			this._levelSelectSlider.gameObject.SetActive(false);
		}

		// Token: 0x06000071 RID: 113 RVA: 0x000055AC File Offset: 0x000037AC
		private Vector2 ConvertEnumToScreenPos(EMiniMapPosition pos)
		{
			switch (pos)
			{
			case EMiniMapPosition.TopRight:
				return new Vector2(1f, 1f);
			case EMiniMapPosition.BottomRight:
				return new Vector2(1f, 0f);
			case EMiniMapPosition.TopLeft:
				return new Vector2(0f, 1f);
			case EMiniMapPosition.BottomLeft:
				return new Vector2(0f, 0f);
			default:
				return Vector2.zero;
			}
		}

		// Token: 0x06000072 RID: 114 RVA: 0x00005618 File Offset: 0x00003818
		private Vector2 ConvertEnumToScenePivot(EMiniMapPosition pos)
		{
			switch (pos)
			{
			case EMiniMapPosition.TopRight:
				return new Vector2(-1f, -1f);
			case EMiniMapPosition.BottomRight:
				return new Vector2(-1f, 1f);
			case EMiniMapPosition.TopLeft:
				return new Vector2(1f, -1f);
			case EMiniMapPosition.BottomLeft:
				return new Vector2(1f, 1f);
			default:
				return Vector2.zero;
			}
		}

		// Token: 0x06000073 RID: 115 RVA: 0x00005684 File Offset: 0x00003884
		private void OnShowInRaid(bool playAnimation)
		{
			if (this._showingMiniMap)
			{
				this.AdjustForMiniMap(playAnimation);
			}
			else if (this._isPeeking)
			{
				this.AdjustForPeek(playAnimation);
			}
			else
			{
				this.AdjustForInRaid(playAnimation);
			}
			Plugin.Log.LogInfo("OnShowInRaid: Refreshing config before showing markers");
			this.ReadConfig();
			string currentMapInternalName = GameUtils.GetCurrentMapInternalName();
			this._mapSelectDropdown.FilterByInternalMapName(currentMapInternalName);
			this._mapSelectDropdown.LoadFirstAvailableMap();
			foreach (IDynamicMarkerProvider dynamicMarkerProvider in this._dynamicMarkerProviders.Values)
			{
				try
				{
					dynamicMarkerProvider.OnShowInRaid(this._mapView);
				}
				catch (Exception ex)
				{
					Plugin.Log.LogError(string.Format("Dynamic marker provider {0} threw exception in OnShowInRaid", dynamicMarkerProvider));
					Plugin.Log.LogError("  Exception given was: " + ex.Message);
					Plugin.Log.LogError("  " + ex.StackTrace);
				}
			}
			Player mainPlayer = GameUtils.GetMainPlayer();
			if (mainPlayer == null)
			{
				return;
			}
			Vector3 vector = MathUtils.ConvertToMapPosition(((IPlayer)mainPlayer).Position);
			if (this._autoSelectLevel)
			{
				this._mapView.SelectLevelByCoords(vector);
			}
			if (this._rememberMapPosition && !this._showingMiniMap && this._mapView.MainMapPos != Vector2.zero)
			{
				this._mapView.SetMapPos(this._mapView.MainMapPos, this._transitionAnimations ? 0.35f : 0f);
				return;
			}
			if (this._autoCenterOnPlayerMarker && !this._showingMiniMap)
			{
				if (this._resetZoomOnCenter)
				{
					this._mapView.SetMapZoom(this.GetInRaidStartingZoom(), 0f, true, false);
				}
				this._mapView.ShiftMapToPlayer(vector, 0f, false);
			}
		}

		// Token: 0x06000074 RID: 116 RVA: 0x00005860 File Offset: 0x00003A60
		private void OnHideInRaid()
		{
			foreach (IDynamicMarkerProvider dynamicMarkerProvider in this._dynamicMarkerProviders.Values)
			{
				try
				{
					dynamicMarkerProvider.OnHideInRaid(this._mapView);
				}
				catch (Exception ex)
				{
					Plugin.Log.LogError(string.Format("Dynamic marker provider {0} threw exception in OnHideInRaid", dynamicMarkerProvider));
					Plugin.Log.LogError("  Exception given was: " + ex.Message);
					Plugin.Log.LogError("  " + ex.StackTrace);
				}
			}
		}

		// Token: 0x06000075 RID: 117 RVA: 0x00005918 File Offset: 0x00003B18
		private void OnShowOutOfRaid()
		{
			this.AdjustForOutOfRaid();
			this._mapSelectDropdown.ClearFilter();
			if (this._mapView.CurrentMapDef == null)
			{
				this._mapSelectDropdown.LoadFirstAvailableMap();
			}
			foreach (IDynamicMarkerProvider dynamicMarkerProvider in this._dynamicMarkerProviders.Values)
			{
				try
				{
					dynamicMarkerProvider.OnShowOutOfRaid(this._mapView);
				}
				catch (Exception ex)
				{
					Plugin.Log.LogError(string.Format("Dynamic marker provider {0} threw exception in OnShowOutOfRaid", dynamicMarkerProvider));
					Plugin.Log.LogError("  Exception given was: " + ex.Message);
					Plugin.Log.LogError("  " + ex.StackTrace);
				}
			}
		}

		// Token: 0x06000076 RID: 118 RVA: 0x000059F8 File Offset: 0x00003BF8
		private void OnHideOutOfRaid()
		{
			foreach (IDynamicMarkerProvider dynamicMarkerProvider in this._dynamicMarkerProviders.Values)
			{
				try
				{
					dynamicMarkerProvider.OnHideOutOfRaid(this._mapView);
				}
				catch (Exception ex)
				{
					Plugin.Log.LogError(string.Format("Dynamic marker provider {0} threw exception in OnHideOutOfRaid", dynamicMarkerProvider));
					Plugin.Log.LogError("  Exception given was: " + ex.Message);
					Plugin.Log.LogError("  " + ex.StackTrace);
				}
			}
		}

		// Token: 0x06000077 RID: 119 RVA: 0x00005AB0 File Offset: 0x00003CB0
		private void OnScroll(float scrollAmount)
		{
			if (this._isPeeking || this._showingMiniMap)
			{
				return;
			}
			if (!Input.GetKey(KeyCode.LeftShift))
			{
				Vector2 rectPoint;
				RectTransformUtility.ScreenPointToLocalPointInRectangle(this._mapView.RectTransform, Input.mousePosition, null, out rectPoint);
				float zoomDelta = scrollAmount * this._mapView.ZoomCurrent * ModdedMapScreen._scrollZoomScaler;
				this._mapView.IncrementalZoomInto(zoomDelta, rectPoint, ModdedMapScreen._zoomScrollTweenTime);
				return;
			}
			if (scrollAmount > 0f)
			{
				this._levelSelectSlider.ChangeLevelBy(1);
				return;
			}
			this._levelSelectSlider.ChangeLevelBy(-1);
		}

		// Token: 0x06000078 RID: 120 RVA: 0x00005B40 File Offset: 0x00003D40
		private void OnZoomMain()
		{
			float num = 0f;
			if (this._zoomMainMapOutShortcut.BetterIsPressed())
			{
				num -= 1f;
			}
			if (this._zoomMainMapInShortcut.BetterIsPressed())
			{
				num += 1f;
			}
			if (num != 0f)
			{
				Vector2 rectPoint = this._mapView.RectTransform.anchoredPosition / this._mapView.ZoomMain;
				num = this._mapView.ZoomMain * num * (this._zoomMapHotkeySpeed * Time.deltaTime);
				this._mapView.IncrementalZoomInto(num, rectPoint, 0f);
				return;
			}
			this._mapView.SetMapZoom(this._mapView.ZoomMain, 0f, true, false);
		}

		// Token: 0x06000079 RID: 121 RVA: 0x00005BF4 File Offset: 0x00003DF4
		private void OnZoomMini()
		{
			float num = 0f;
			if (this._zoomMiniMapOutShortcut.BetterIsPressed())
			{
				num -= 1f;
			}
			if (this._zoomMiniMapInShortcut.BetterIsPressed())
			{
				num += 1f;
			}
			if (num != 0f)
			{
				Vector3 v = MathUtils.ConvertToMapPosition(((IPlayer)GameUtils.GetMainPlayer()).Position);
				num = this._mapView.ZoomMini * num * (this._zoomMapHotkeySpeed * Time.deltaTime);
				this._mapView.IncrementalZoomIntoMiniMap(num, v, 0f);
				return;
			}
			this._mapView.SetMapZoom(this._mapView.ZoomMini, 0f, false, true);
		}

		// Token: 0x0600007A RID: 122 RVA: 0x00005C9C File Offset: 0x00003E9C
		private void OnCenter()
		{
			if (this._centerPlayerShortcut.BetterIsDown() || this._showingMiniMap)
			{
				Player mainPlayer = GameUtils.GetMainPlayer();
				if (mainPlayer != null)
				{
					Vector3 vector = MathUtils.ConvertToMapPosition(((IPlayer)mainPlayer).Position);
					this._mapView.ShiftMapToCoordinate(vector, this._showingMiniMap ? 0f : ModdedMapScreen._positionTweenTime, this._showingMiniMap);
					this._mapView.SelectLevelByCoords(vector);
				}
			}
		}

		// Token: 0x0600007B RID: 123 RVA: 0x00005D0C File Offset: 0x00003F0C
		internal void ReadConfig()
		{
			this._centerPlayerShortcut = Settings.CenterOnPlayerHotkey.Value;
			this._dumpShortcut = Settings.DumpInfoHotkey.Value;
			this._moveMapUpShortcut = Settings.MoveMapUpHotkey.Value;
			this._moveMapDownShortcut = Settings.MoveMapDownHotkey.Value;
			this._moveMapLeftShortcut = Settings.MoveMapLeftHotkey.Value;
			this._moveMapRightShortcut = Settings.MoveMapRightHotkey.Value;
			this._moveMapSpeed = Settings.MapMoveHotkeySpeed.Value;
			this._moveMapLevelUpShortcut = Settings.ChangeMapLevelUpHotkey.Value;
			this._moveMapLevelDownShortcut = Settings.ChangeMapLevelDownHotkey.Value;
			this._zoomMainMapInShortcut = Settings.ZoomMapInHotkey.Value;
			this._zoomMainMapOutShortcut = Settings.ZoomMapOutHotkey.Value;
			this._zoomMiniMapInShortcut = Settings.ZoomInMiniMapHotkey.Value;
			this._zoomMiniMapOutShortcut = Settings.ZoomOutMiniMapHotkey.Value;
			this._zoomMapHotkeySpeed = Settings.ZoomMapHotkeySpeed.Value;
			this._autoCenterOnPlayerMarker = Settings.AutoCenterOnPlayerMarker.Value;
			this._resetZoomOnCenter = Settings.ResetZoomOnCenter.Value;
			this._rememberMapPosition = Settings.RetainMapPosition.Value;
			this._autoSelectLevel = Settings.AutoSelectLevel.Value;
			this._centeringZoomResetPoint = Settings.CenteringZoomResetPoint.Value;
			this._transitionAnimations = Settings.MapTransitionEnabled.Value;
			if (this._mapView != null)
			{
				this._mapView.ZoomMain = Settings.ZoomMainMap.Value;
				this._mapView.ZoomMini = Settings.ZoomMiniMap.Value;
			}
			if (this._peekComponent != null)
			{
				this._peekComponent.PeekShortcut = Settings.PeekShortcut.Value;
				this._peekComponent.HoldForPeek = Settings.HoldForPeek.Value;
				this._peekComponent.HideMinimapShortcut = Settings.MiniMapShowOrHide.Value;
			}
			this.AddRemoveMarkerProvider<PlayerMarkerProvider>(Settings.ShowPlayerMarker.Value);
			this.AddRemoveMarkerProvider<QuestMarkerProvider>(Settings.ShowQuestsInRaid.Value);
			this.AddRemoveMarkerProvider<LockedDoorMarkerMutator>(Settings.ShowLockedDoorStatus.Value);
			this.AddRemoveMarkerProvider<BackpackMarkerProvider>(Settings.ShowDroppedBackpackInRaid.Value);
			this.AddRemoveMarkerProvider<BTRMarkerProvider>(Settings.ShowBTRInRaid.Value);
			this.AddRemoveMarkerProvider<AirdropMarkerProvider>(Settings.ShowAirdropsInRaid.Value);
			this.AddRemoveMarkerProvider<LootMarkerProvider>(Settings.ShowWishListItemsInRaid.Value);
			this.AddRemoveMarkerProvider<HiddenStashMarkerProvider>(Settings.ShowHiddenStashesInRaid.Value);
			this.AddRemoveMarkerProvider<TransitMarkerProvider>(Settings.ShowTransitPointsInRaid.Value);
			this.AddRemoveMarkerProvider<SecretMarkerProvider>(Settings.ShowSecretPointsInRaid.Value);
			if (Settings.ShowAirdropsInRaid.Value)
			{
				this.GetMarkerProvider<AirdropMarkerProvider>().RefreshMarkers();
			}
			if (Settings.ShowWishListItemsInRaid.Value)
			{
				this.GetMarkerProvider<LootMarkerProvider>().RefreshMarkers();
			}
			if (Settings.ShowHiddenStashesInRaid.Value)
			{
				this.GetMarkerProvider<HiddenStashMarkerProvider>().RefreshMarkers();
			}
			if (Settings.ShowTransitPointsInRaid.Value)
			{
				this.GetMarkerProvider<TransitMarkerProvider>().RefreshMarkers(this._mapView);
			}
			this.AddRemoveMarkerProvider<SecretMarkerProvider>(Settings.ShowSecretPointsInRaid.Value);
			if (Settings.ShowSecretPointsInRaid.Value)
			{
				this.GetMarkerProvider<SecretMarkerProvider>().ShowExtractStatusInRaid = Settings.ShowExtractStatusInRaid.Value;
			}
			this.AddRemoveMarkerProvider<ExtractMarkerProvider>(Settings.ShowExtractsInRaid.Value);
			if (Settings.ShowExtractsInRaid.Value)
			{
				this.GetMarkerProvider<ExtractMarkerProvider>().ShowExtractStatusInRaid = Settings.ShowExtractStatusInRaid.Value;
			}
			bool flag = Settings.ShowFriendlyPlayerMarkersInRaid.Value || Settings.ShowEnemyPlayerMarkersInRaid.Value || Settings.ShowBossMarkersInRaid.Value || Settings.ShowScavMarkersInRaid.Value;
			Plugin.Log.LogInfo(string.Format("ReadConfig: needOtherPlayerMarkers={0}, based on settings: Friendly={1}, Enemy={2}, Scavs={3}, Bosses={4}", new object[]
			{
				flag,
				Settings.ShowFriendlyPlayerMarkersInRaid.Value,
				Settings.ShowEnemyPlayerMarkersInRaid.Value,
				Settings.ShowScavMarkersInRaid.Value,
				Settings.ShowBossMarkersInRaid.Value
			}));
			this.AddRemoveMarkerProvider<OtherPlayersMarkerProvider>(flag);
			if (flag)
			{
				Plugin.Log.LogInfo(string.Format("ReadConfig: Setting up OtherPlayersMarkerProvider, _serverConfig is null: {0}", ModdedMapScreen._serverConfig == null));
				OtherPlayersMarkerProvider markerProvider = this.GetMarkerProvider<OtherPlayersMarkerProvider>();
				ModdedMapScreen.DMServerConfig serverConfig = ModdedMapScreen._serverConfig;
				bool flag2 = serverConfig == null || serverConfig.allowShowFriendlyPlayerMarkersInRaid;
				ModdedMapScreen.DMServerConfig serverConfig2 = ModdedMapScreen._serverConfig;
				bool flag3 = serverConfig2 == null || serverConfig2.allowShowEnemyPlayerMarkersInRaid;
				ModdedMapScreen.DMServerConfig serverConfig3 = ModdedMapScreen._serverConfig;
				bool flag4 = serverConfig3 == null || serverConfig3.allowShowScavMarkersInRaid;
				ModdedMapScreen.DMServerConfig serverConfig4 = ModdedMapScreen._serverConfig;
				bool flag5 = serverConfig4 == null || serverConfig4.allowShowBossMarkersInRaid;
				Plugin.Log.LogInfo(string.Format("OtherPlayersMarkerProvider config - Server permissions: Friendly={0}, Enemy={1}, Scavs={2}, Bosses={3}", new object[]
				{
					flag2,
					flag3,
					flag4,
					flag5
				}));
				Plugin.Log.LogInfo(string.Format("OtherPlayersMarkerProvider config - Client settings: Friendly={0}, Enemy={1}, Scavs={2}, Bosses={3}", new object[]
				{
					Settings.ShowFriendlyPlayerMarkersInRaid.Value,
					Settings.ShowEnemyPlayerMarkersInRaid.Value,
					Settings.ShowScavMarkersInRaid.Value,
					Settings.ShowBossMarkersInRaid.Value
				}));
				markerProvider.ShowFriendlyPlayers = (flag2 && Settings.ShowFriendlyPlayerMarkersInRaid.Value);
				markerProvider.ShowEnemyPlayers = (flag3 && Settings.ShowEnemyPlayerMarkersInRaid.Value);
				markerProvider.ShowScavs = (flag4 && Settings.ShowScavMarkersInRaid.Value);
				markerProvider.ShowBosses = (flag5 && Settings.ShowBossMarkersInRaid.Value);
				Plugin.Log.LogInfo(string.Format("OtherPlayersMarkerProvider final settings: ShowFriendly={0}, ShowEnemy={1}, ShowScavs={2}, ShowBosses={3}", new object[]
				{
					markerProvider.ShowFriendlyPlayers,
					markerProvider.ShowEnemyPlayers,
					markerProvider.ShowScavs,
					markerProvider.ShowBosses
				}));
				markerProvider.RefreshMarkers();
			}
			bool flag6 = Settings.ShowFriendlyCorpsesInRaid.Value || Settings.ShowKilledCorpsesInRaid.Value || Settings.ShowFriendlyKilledCorpsesInRaid.Value || Settings.ShowBossCorpsesInRaid.Value || Settings.ShowOtherCorpsesInRaid.Value;
			this.AddRemoveMarkerProvider<CorpseMarkerProvider>(flag6);
			if (flag6)
			{
				CorpseMarkerProvider markerProvider2 = this.GetMarkerProvider<CorpseMarkerProvider>();
				markerProvider2.ShowFriendlyCorpses = Settings.ShowFriendlyCorpsesInRaid.Value;
				markerProvider2.ShowKilledCorpses = Settings.ShowKilledCorpsesInRaid.Value;
				markerProvider2.ShowFriendlyKilledCorpses = Settings.ShowFriendlyKilledCorpsesInRaid.Value;
				markerProvider2.ShowBossCorpses = Settings.ShowBossCorpsesInRaid.Value;
				markerProvider2.ShowOtherCorpses = Settings.ShowOtherCorpsesInRaid.Value;
				markerProvider2.RefreshMarkers();
			}
			if (ModDetection.HeliCrashLoaded)
			{
				this.AddRemoveMarkerProvider<HeliCrashMarkerProvider>(Settings.ShowHeliCrashMarker.Value);
			}
		}

		// Token: 0x0600007C RID: 124 RVA: 0x00006364 File Offset: 0x00004564
		internal void TryAddPeekComponent(EftBattleUIScreen battleUI)
		{
			if (this._peekComponent != null)
			{
				return;
			}
			Plugin.Log.LogInfo("Trying to attach peek component to BattleUI");
			this._peekComponent = MapPeekComponent.Create(battleUI.gameObject);
			this._peekComponent.MapScreen = this;
			this._peekComponent.MapScreenTrueParent = this._parentTransform;
			this.ReadConfig();
		}

		// Token: 0x0600007D RID: 125 RVA: 0x000063C0 File Offset: 0x000045C0
		public void AddRemoveMarkerProvider<T>(bool status) where T : IDynamicMarkerProvider, new()
		{
			if (status && !this._dynamicMarkerProviders.ContainsKey(typeof(T)))
			{
				this._dynamicMarkerProviders[typeof(T)] = Activator.CreateInstance<T>();
				if (this._isShown && GameUtils.IsInRaid())
				{
					this._dynamicMarkerProviders[typeof(T)].OnShowInRaid(this._mapView);
					return;
				}
				if (this._isShown && !GameUtils.IsInRaid())
				{
					this._dynamicMarkerProviders[typeof(T)].OnShowOutOfRaid(this._mapView);
					return;
				}
			}
			else if (!status && this._dynamicMarkerProviders.ContainsKey(typeof(T)))
			{
				this._dynamicMarkerProviders[typeof(T)].OnDisable(this._mapView);
				this._dynamicMarkerProviders.Remove(typeof(T));
			}
		}

		// Token: 0x0600007E RID: 126 RVA: 0x000064BC File Offset: 0x000046BC
		private T GetMarkerProvider<T>() where T : IDynamicMarkerProvider
		{
			if (!this._dynamicMarkerProviders.ContainsKey(typeof(T)))
			{
				return default(T);
			}
			return (T)((object)this._dynamicMarkerProviders[typeof(T)]);
		}

		// Token: 0x0600007F RID: 127 RVA: 0x00006504 File Offset: 0x00004704
		private float GetInRaidStartingZoom()
		{
			return this._mapView.ZoomMin + this._centeringZoomResetPoint * (this._mapView.ZoomMax - this._mapView.ZoomMin);
		}

		// Token: 0x06000080 RID: 128 RVA: 0x00006530 File Offset: 0x00004730
		private void ChangeMap(MapDef mapDef)
		{
			if (mapDef == null || this._mapView.CurrentMapDef == mapDef)
			{
				return;
			}
			Plugin.Log.LogInfo("MapScreen: Loading map " + mapDef.DisplayName);
			if (GameUtils.IsInRaid())
			{
				this.AdjustSizeAndPosition();
			}
			this._mapView.LoadMap(mapDef);
			this._mapSelectDropdown.OnLoadMap(mapDef);
			this._levelSelectSlider.OnLoadMap(mapDef, this._mapView.SelectedLevel);
			foreach (IDynamicMarkerProvider dynamicMarkerProvider in this._dynamicMarkerProviders.Values)
			{
				try
				{
					dynamicMarkerProvider.OnMapChanged(this._mapView, mapDef);
				}
				catch (Exception ex)
				{
					Plugin.Log.LogError(string.Format("Dynamic marker provider {0} threw exception in ChangeMap", dynamicMarkerProvider));
					Plugin.Log.LogError("  Exception given was: " + ex.Message);
					Plugin.Log.LogError("  " + ex.StackTrace);
				}
			}
		}

		// Token: 0x06000081 RID: 129 RVA: 0x00006650 File Offset: 0x00004850
		private void PrecacheMapLayerImages()
		{
			Singleton<CommonUI>.Instance.StartCoroutine(ModdedMapScreen.PrecacheCoroutine(this._mapSelectDropdown.GetMapDefs()));
		}

		// Token: 0x06000082 RID: 130 RVA: 0x0000666D File Offset: 0x0000486D
		private static IEnumerator PrecacheCoroutine(IEnumerable<MapDef> mapDefs)
		{
			ModdedMapScreen.<PrecacheCoroutine>d__91 <PrecacheCoroutine>d__ = new ModdedMapScreen.<PrecacheCoroutine>d__91(0);
			<PrecacheCoroutine>d__.mapDefs = mapDefs;
			return <PrecacheCoroutine>d__;
		}

		// Token: 0x04000046 RID: 70
		private const string _mapRelPath = "Maps";

		// Token: 0x04000047 RID: 71
		private bool _initialized;

		// Token: 0x04000048 RID: 72
		private static float _positionTweenTime = 0.25f;

		// Token: 0x04000049 RID: 73
		private static float _scrollZoomScaler = 1.75f;

		// Token: 0x0400004A RID: 74
		private static float _zoomScrollTweenTime = 0.25f;

		// Token: 0x0400004B RID: 75
		private static Vector2 _levelSliderPosition = new Vector2(15f, 750f);

		// Token: 0x0400004C RID: 76
		private static Vector2 _mapSelectDropdownPosition = new Vector2(-780f, -50f);

		// Token: 0x0400004D RID: 77
		private static Vector2 _mapSelectDropdownSize = new Vector2(360f, 31f);

		// Token: 0x0400004E RID: 78
		private static Vector2 _maskSizeModifierInRaid = new Vector2(0f, -42f);

		// Token: 0x0400004F RID: 79
		private static Vector2 _maskPositionInRaid = new Vector2(0f, -20f);

		// Token: 0x04000050 RID: 80
		private static Vector2 _maskSizeModifierOutOfRaid = new Vector2(0f, -70f);

		// Token: 0x04000051 RID: 81
		private static Vector2 _maskPositionOutOfRaid = new Vector2(0f, -5f);

		// Token: 0x04000052 RID: 82
		private static Vector2 _textAnchor = new Vector2(0f, 1f);

		// Token: 0x04000053 RID: 83
		private static Vector2 _cursorPositionTextOffset = new Vector2(15f, -52f);

		// Token: 0x04000054 RID: 84
		private static Vector2 _playerPositionTextOffset = new Vector2(15f, -68f);

		// Token: 0x04000055 RID: 85
		private static float _positionTextFontSize = 15f;

		// Token: 0x04000056 RID: 86
		private bool _isShown;

		// Token: 0x04000057 RID: 87
		private ScrollRect _scrollRect;

		// Token: 0x04000058 RID: 88
		private Mask _scrollMask;

		// Token: 0x04000059 RID: 89
		private MapView _mapView;

		// Token: 0x0400005A RID: 90
		private LevelSelectSlider _levelSelectSlider;

		// Token: 0x0400005B RID: 91
		private MapSelectDropdown _mapSelectDropdown;

		// Token: 0x0400005C RID: 92
		private CursorPositionText _cursorPositionText;

		// Token: 0x0400005D RID: 93
		private PlayerPositionText _playerPositionText;

		// Token: 0x0400005E RID: 94
		private MapPeekComponent _peekComponent;

		// Token: 0x04000060 RID: 96
		private Dictionary<Type, IDynamicMarkerProvider> _dynamicMarkerProviders = new Dictionary<Type, IDynamicMarkerProvider>();

		// Token: 0x04000061 RID: 97
		private bool _autoCenterOnPlayerMarker = true;

		// Token: 0x04000062 RID: 98
		private bool _autoSelectLevel = true;

		// Token: 0x04000063 RID: 99
		private bool _resetZoomOnCenter;

		// Token: 0x04000064 RID: 100
		private bool _rememberMapPosition = true;

		// Token: 0x04000065 RID: 101
		private bool _transitionAnimations = true;

		// Token: 0x04000066 RID: 102
		private float _centeringZoomResetPoint;

		// Token: 0x04000067 RID: 103
		private KeyboardShortcut _centerPlayerShortcut;

		// Token: 0x04000068 RID: 104
		private KeyboardShortcut _dumpShortcut;

		// Token: 0x04000069 RID: 105
		private KeyboardShortcut _moveMapUpShortcut;

		// Token: 0x0400006A RID: 106
		private KeyboardShortcut _moveMapDownShortcut;

		// Token: 0x0400006B RID: 107
		private KeyboardShortcut _moveMapLeftShortcut;

		// Token: 0x0400006C RID: 108
		private KeyboardShortcut _moveMapRightShortcut;

		// Token: 0x0400006D RID: 109
		private float _moveMapSpeed = 0.25f;

		// Token: 0x0400006E RID: 110
		private KeyboardShortcut _moveMapLevelUpShortcut;

		// Token: 0x0400006F RID: 111
		private KeyboardShortcut _moveMapLevelDownShortcut;

		// Token: 0x04000070 RID: 112
		private KeyboardShortcut _zoomMainMapInShortcut;

		// Token: 0x04000071 RID: 113
		private KeyboardShortcut _zoomMainMapOutShortcut;

		// Token: 0x04000072 RID: 114
		private KeyboardShortcut _zoomMiniMapInShortcut;

		// Token: 0x04000073 RID: 115
		private KeyboardShortcut _zoomMiniMapOutShortcut;

		// Token: 0x04000074 RID: 116
		public static ModdedMapScreen.DMServerConfig _serverConfig;

		// Token: 0x04000075 RID: 117
		private float _zoomMapHotkeySpeed = 2.5f;

		// Token: 0x0200004F RID: 79
		public class DMServerConfig
		{
			// Token: 0x040001DF RID: 479
			public bool allowShowFriendlyPlayerMarkersInRaid;

			// Token: 0x040001E0 RID: 480
			public bool allowShowEnemyPlayerMarkersInRaid;

			// Token: 0x040001E1 RID: 481
			public bool allowShowBossMarkersInRaid;

			// Token: 0x040001E2 RID: 482
			public bool allowShowScavMarkersInRaid;
		}
	}
}
