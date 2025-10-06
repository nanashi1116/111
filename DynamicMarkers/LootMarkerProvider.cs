using System;
using System.Collections.Generic;
using System.Linq;
using Comfort.Common;
using DynamicMaps.Config;
using DynamicMaps.Data;
using DynamicMaps.UI.Components;
using DynamicMaps.Utils;
using EFT;
using EFT.Interactive;
using EFT.UI;
using EFT.UI.DragAndDrop;
using UnityEngine;

namespace DynamicMaps.DynamicMarkers
{
	// Token: 0x02000037 RID: 55
	public class LootMarkerProvider : IDynamicMarkerProvider
	{
		// Token: 0x06000228 RID: 552 RVA: 0x0000BDA8 File Offset: 0x00009FA8
		public void OnShowInRaid(MapView map)
		{
			this._lastMapView = map;
			foreach (IKillableLootItem killableLootItem in Singleton<GameWorld>.Instance.LootList)
			{
				LootItem lootItem = killableLootItem as LootItem;
				if (lootItem != null && Enumerable.Contains<MongoID>(GameUtils.GetWishListItems(), new MongoID(lootItem.TemplateId)))
				{
					this.TryAddMarker(lootItem);
				}
			}
		}

		// Token: 0x06000229 RID: 553 RVA: 0x0000BE28 File Offset: 0x0000A028
		public void OnHideInRaid(MapView map)
		{
		}

		// Token: 0x0600022A RID: 554 RVA: 0x0000BE2A File Offset: 0x0000A02A
		public void OnRaidEnd(MapView map)
		{
			this.TryRemoveMarkers();
		}

		// Token: 0x0600022B RID: 555 RVA: 0x0000BE34 File Offset: 0x0000A034
		public void OnMapChanged(MapView map, MapDef mapDef)
		{
			this._lastMapView = map;
			foreach (LootItem item in Enumerable.ToList<LootItem>(this._lootMarkers.Keys))
			{
				this.TryRemoveMarker(item);
				this.TryAddMarker(item);
			}
		}

		// Token: 0x0600022C RID: 556 RVA: 0x0000BEA0 File Offset: 0x0000A0A0
		public void OnDisable(MapView map)
		{
			this.OnRaidEnd(map);
		}

		// Token: 0x0600022D RID: 557 RVA: 0x0000BEAC File Offset: 0x0000A0AC
		public void RefreshMarkers()
		{
			if (!GameUtils.IsInRaid())
			{
				return;
			}
			foreach (LootItem item in Enumerable.ToList<LootItem>(this._lootMarkers.Keys))
			{
				this.TryRemoveMarker(item);
				this.TryAddMarker(item);
			}
		}

		// Token: 0x0600022E RID: 558 RVA: 0x0000BF18 File Offset: 0x0000A118
		private void TryAddMarker(LootItem item)
		{
			if (this._lootMarkers.ContainsKey(item))
			{
				return;
			}
			int value = Settings.ShowWishListItemsIntelLevel.Value;
			int? intelLevel = GameUtils.GetIntelLevel();
			if (value > intelLevel.GetValueOrDefault() & intelLevel != null)
			{
				return;
			}
			StaticIcons staticIcons = EFTHardSettings.Instance.StaticIcons;
			EItemType itemType = ItemViewFactory.GetItemType(item.Item.GetType());
			Sprite valueOrDefault = staticIcons.ItemTypeSprites.GetValueOrDefault(itemType);
			MapMarkerDef markerDef = new MapMarkerDef
			{
				Category = "Loot",
				Color = Settings.LootItemColor.Value,
				Sprite = valueOrDefault,
				Position = MathUtils.ConvertToMapPosition(item.transform),
				Text = item.Item.TemplateId.LocalizedName()
			};
			MapMarker value2 = this._lastMapView.AddMapMarker(markerDef);
			this._lootMarkers[item] = value2;
		}

		// Token: 0x0600022F RID: 559 RVA: 0x0000BFEC File Offset: 0x0000A1EC
		private void TryRemoveMarkers()
		{
			foreach (LootItem item in Enumerable.ToList<LootItem>(this._lootMarkers.Keys))
			{
				this.TryRemoveMarker(item);
			}
		}

		// Token: 0x06000230 RID: 560 RVA: 0x0000C04C File Offset: 0x0000A24C
		private void TryRemoveMarker(LootItem item)
		{
			if (!this._lootMarkers.ContainsKey(item))
			{
				return;
			}
			this._lootMarkers[item].ContainingMapView.RemoveMapMarker(this._lootMarkers[item]);
			this._lootMarkers.Remove(item);
		}

		// Token: 0x06000231 RID: 561 RVA: 0x0000C08C File Offset: 0x0000A28C
		public void OnShowOutOfRaid(MapView map)
		{
		}

		// Token: 0x06000232 RID: 562 RVA: 0x0000C08E File Offset: 0x0000A28E
		public void OnHideOutOfRaid(MapView map)
		{
		}

		// Token: 0x04000121 RID: 289
		private MapView _lastMapView;

		// Token: 0x04000122 RID: 290
		private Dictionary<LootItem, MapMarker> _lootMarkers = new Dictionary<LootItem, MapMarker>();
	}
}
