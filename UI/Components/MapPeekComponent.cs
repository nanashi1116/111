using System;
using BepInEx.Configuration;
using DynamicMaps.Config;
using DynamicMaps.Utils;
using UnityEngine;

namespace DynamicMaps.UI.Components
{
	// Token: 0x0200001C RID: 28
	internal class MapPeekComponent : MonoBehaviour
	{
		// Token: 0x1700002E RID: 46
		// (get) Token: 0x06000113 RID: 275 RVA: 0x00008525 File Offset: 0x00006725
		// (set) Token: 0x06000114 RID: 276 RVA: 0x0000852D File Offset: 0x0000672D
		public ModdedMapScreen MapScreen { get; set; }

		// Token: 0x1700002F RID: 47
		// (get) Token: 0x06000115 RID: 277 RVA: 0x00008536 File Offset: 0x00006736
		// (set) Token: 0x06000116 RID: 278 RVA: 0x0000853E File Offset: 0x0000673E
		public RectTransform MapScreenTrueParent { get; set; }

		// Token: 0x17000030 RID: 48
		// (get) Token: 0x06000117 RID: 279 RVA: 0x00008547 File Offset: 0x00006747
		// (set) Token: 0x06000118 RID: 280 RVA: 0x0000854F File Offset: 0x0000674F
		public RectTransform RectTransform { get; private set; }

		// Token: 0x17000031 RID: 49
		// (get) Token: 0x06000119 RID: 281 RVA: 0x00008558 File Offset: 0x00006758
		// (set) Token: 0x0600011A RID: 282 RVA: 0x00008560 File Offset: 0x00006760
		public KeyboardShortcut PeekShortcut { get; set; }

		// Token: 0x17000032 RID: 50
		// (get) Token: 0x0600011B RID: 283 RVA: 0x00008569 File Offset: 0x00006769
		// (set) Token: 0x0600011C RID: 284 RVA: 0x00008571 File Offset: 0x00006771
		public KeyboardShortcut HideMinimapShortcut { get; set; }

		// Token: 0x17000033 RID: 51
		// (get) Token: 0x0600011D RID: 285 RVA: 0x0000857A File Offset: 0x0000677A
		// (set) Token: 0x0600011E RID: 286 RVA: 0x00008582 File Offset: 0x00006782
		public bool HoldForPeek { get; set; }

		// Token: 0x17000034 RID: 52
		// (get) Token: 0x0600011F RID: 287 RVA: 0x0000858B File Offset: 0x0000678B
		// (set) Token: 0x06000120 RID: 288 RVA: 0x00008593 File Offset: 0x00006793
		public bool IsPeeking { get; private set; }

		// Token: 0x17000035 RID: 53
		// (get) Token: 0x06000121 RID: 289 RVA: 0x0000859C File Offset: 0x0000679C
		private static bool IsMiniMapEnabled
		{
			get
			{
				return Settings.MiniMapEnabled.Value;
			}
		}

		// Token: 0x17000036 RID: 54
		// (get) Token: 0x06000122 RID: 290 RVA: 0x000085A8 File Offset: 0x000067A8
		// (set) Token: 0x06000123 RID: 291 RVA: 0x000085B0 File Offset: 0x000067B0
		public bool ShowingMiniMap { get; private set; }

		// Token: 0x17000037 RID: 55
		// (get) Token: 0x06000124 RID: 292 RVA: 0x000085B9 File Offset: 0x000067B9
		// (set) Token: 0x06000125 RID: 293 RVA: 0x000085C1 File Offset: 0x000067C1
		public bool WasMiniMapActive { get; set; }

		// Token: 0x06000126 RID: 294 RVA: 0x000085CA File Offset: 0x000067CA
		internal static MapPeekComponent Create(GameObject parent)
		{
			GameObject gameObject = UIUtils.CreateUIGameObject(parent, "MapPeek");
			gameObject.GetRectTransform().sizeDelta = parent.GetRectTransform().sizeDelta;
			return gameObject.AddComponent<MapPeekComponent>();
		}

		// Token: 0x06000127 RID: 295 RVA: 0x000085F2 File Offset: 0x000067F2
		private void Awake()
		{
			this.RectTransform = base.gameObject.GetRectTransform();
		}

		// Token: 0x06000128 RID: 296 RVA: 0x00008605 File Offset: 0x00006805
		private void Update()
		{
			if (!GameUtils.ShouldShowMapInRaid())
			{
				if (this.ShowingMiniMap)
				{
					this.EndMiniMap();
				}
				if (this.IsPeeking)
				{
					this.EndPeek();
				}
				return;
			}
			this.HandleMinimapState();
			this.HandlePeekState();
		}

		// Token: 0x06000129 RID: 297 RVA: 0x00008638 File Offset: 0x00006838
		private void HandleMinimapState()
		{
			if (!MapPeekComponent.IsMiniMapEnabled)
			{
				if (this.ShowingMiniMap)
				{
					this.EndMiniMap();
					this.WasMiniMapActive = false;
				}
				return;
			}
			if (this.HideMinimapShortcut.BetterIsDown())
			{
				this.IsMiniMapHidden = !this.IsMiniMapHidden;
				if (!this.IsMiniMapHidden && !this.ShowingMiniMap)
				{
					this.BeginMiniMap(false);
					return;
				}
				this.EndMiniMap();
				return;
			}
			else
			{
				if (this.IsMiniMapHidden)
				{
					return;
				}
				if (!this.IsPeeking && !this.MapScreen.IsShowingMapScreen)
				{
					this.BeginMiniMap(true);
					return;
				}
				this.EndMiniMap();
				return;
			}
		}

		// Token: 0x0600012A RID: 298 RVA: 0x000086CC File Offset: 0x000068CC
		private void HandlePeekState()
		{
			if (!this.HoldForPeek || this.PeekShortcut.BetterIsPressed() == this.IsPeeking)
			{
				if (!this.HoldForPeek && this.PeekShortcut.BetterIsDown())
				{
					if (!this.IsPeeking)
					{
						this.WasMiniMapActive = this.ShowingMiniMap;
						this.EndMiniMap();
						this.BeginPeek(this.WasMiniMapActive);
						return;
					}
					this.EndPeek();
				}
				return;
			}
			if (this.PeekShortcut.BetterIsPressed())
			{
				this.WasMiniMapActive = this.ShowingMiniMap;
				this.EndMiniMap();
				this.BeginPeek(this.WasMiniMapActive && !this.IsMiniMapHidden);
				return;
			}
			this.EndPeek();
		}

		// Token: 0x0600012B RID: 299 RVA: 0x00008777 File Offset: 0x00006977
		private void BeginPeek(bool playAnimation = true)
		{
			if (this.IsPeeking)
			{
				return;
			}
			base.transform.SetAsLastSibling();
			this.IsPeeking = true;
			this.MapScreen.transform.SetParent(this.RectTransform);
			this.MapScreen.Show(playAnimation);
		}

		// Token: 0x0600012C RID: 300 RVA: 0x000087B8 File Offset: 0x000069B8
		internal void EndPeek()
		{
			if (!this.IsPeeking)
			{
				return;
			}
			this.IsPeeking = false;
			this.MapScreen.Hide();
			this.MapScreen.transform.SetParent(this.MapScreenTrueParent);
			if (this.WasMiniMapActive)
			{
				this.BeginMiniMap(true);
			}
		}

		// Token: 0x0600012D RID: 301 RVA: 0x00008805 File Offset: 0x00006A05
		internal void BeginMiniMap(bool playAnimation = true)
		{
			if (this.ShowingMiniMap)
			{
				return;
			}
			this.ShowingMiniMap = true;
			base.transform.SetAsLastSibling();
			this.MapScreen.transform.SetParent(this.RectTransform);
			this.MapScreen.Show(playAnimation);
		}

		// Token: 0x0600012E RID: 302 RVA: 0x00008844 File Offset: 0x00006A44
		internal void EndMiniMap()
		{
			if (!this.ShowingMiniMap)
			{
				return;
			}
			this.ShowingMiniMap = false;
			this.MapScreen.Hide();
			this.MapScreen.transform.SetParent(this.MapScreenTrueParent);
		}

		// Token: 0x040000BB RID: 187
		private bool IsMiniMapHidden;
	}
}
