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
using EFT.Interactive;
using EFT.InventoryLogic;
using EFT.UI.DragAndDrop;
using UnityEngine;

namespace DynamicMaps.DynamicMarkers
{
	// Token: 0x02000031 RID: 49
	public class BackpackMarkerProvider : IDynamicMarkerProvider
	{
		// Token: 0x060001D2 RID: 466 RVA: 0x0000AAB5 File Offset: 0x00008CB5
		public void OnShowInRaid(MapView map)
		{
			this._lastMapView = map;
			this.RemoveStaleMarkers();
			this.AddThrownBackpacks();
			GameWorldRegisterLootItemPatch.OnRegisterLoot += this.OnRegisterLoot;
			GameWorldDestroyLootPatch.OnDestroyLoot += this.OnDestroyLoot;
		}

		// Token: 0x060001D3 RID: 467 RVA: 0x0000AAEC File Offset: 0x00008CEC
		public void OnHideInRaid(MapView map)
		{
			GameWorldRegisterLootItemPatch.OnRegisterLoot -= this.OnRegisterLoot;
			GameWorldDestroyLootPatch.OnDestroyLoot -= this.OnDestroyLoot;
		}

		// Token: 0x060001D4 RID: 468 RVA: 0x0000AB10 File Offset: 0x00008D10
		public void OnMapChanged(MapView map, MapDef mapDef)
		{
			this._lastMapView = map;
			GameWorld instance = Singleton<GameWorld>.Instance;
			foreach (int num in Enumerable.ToList<int>(this._backpackMarkers.Keys))
			{
				this.TryRemoveMarker(num);
				if (instance.LootItems.ContainsKey(num))
				{
					this.TryAddMarker(map, instance.LootItems.GetByKey(num));
				}
			}
		}

		// Token: 0x060001D5 RID: 469 RVA: 0x0000AB9C File Offset: 0x00008D9C
		public void OnRaidEnd(MapView map)
		{
			GameWorldRegisterLootItemPatch.OnRegisterLoot -= this.OnRegisterLoot;
			GameWorldDestroyLootPatch.OnDestroyLoot -= this.OnDestroyLoot;
			this.TryRemoveMarkers();
		}

		// Token: 0x060001D6 RID: 470 RVA: 0x0000ABC6 File Offset: 0x00008DC6
		public void OnDisable(MapView map)
		{
			GameWorldRegisterLootItemPatch.OnRegisterLoot -= this.OnRegisterLoot;
			GameWorldDestroyLootPatch.OnDestroyLoot -= this.OnDestroyLoot;
			this.TryRemoveMarkers();
		}

		// Token: 0x060001D7 RID: 471 RVA: 0x0000ABF0 File Offset: 0x00008DF0
		private void AddThrownBackpacks()
		{
			GameWorld instance = Singleton<GameWorld>.Instance;
			foreach (KeyValuePair<int, Item> keyValuePair in PlayerInventoryThrowItemPatch.ThrownItems)
			{
				int key = keyValuePair.Key;
				EItemType itemType = ItemViewFactory.GetItemType(keyValuePair.Value.GetType());
				if (!this._backpackMarkers.ContainsKey(key) && itemType == EItemType.Backpack && instance.LootItems.ContainsKey(key))
				{
					this.TryAddMarker(this._lastMapView, instance.LootItems.GetByKey(key));
				}
			}
		}

		// Token: 0x060001D8 RID: 472 RVA: 0x0000AC98 File Offset: 0x00008E98
		private void OnRegisterLoot(LootItem lootItem)
		{
			if (lootItem == null || lootItem.Item == null)
			{
				return;
			}
			int netId = lootItem.GetNetId();
			EItemType itemType = ItemViewFactory.GetItemType(lootItem.Item.GetType());
			if (this._backpackMarkers.ContainsKey(netId) || itemType != EItemType.Backpack || !PlayerInventoryThrowItemPatch.ThrownItems.ContainsKey(netId))
			{
				return;
			}
			this.TryAddMarker(this._lastMapView, lootItem);
		}

		// Token: 0x060001D9 RID: 473 RVA: 0x0000ACF6 File Offset: 0x00008EF6
		private void OnDestroyLoot(LootItem lootItem)
		{
			if (lootItem == null || lootItem.Item == null)
			{
				return;
			}
			this.TryRemoveMarker(lootItem.GetNetId());
		}

		// Token: 0x060001DA RID: 474 RVA: 0x0000AD10 File Offset: 0x00008F10
		private void RemoveStaleMarkers()
		{
			GameWorld instance = Singleton<GameWorld>.Instance;
			foreach (int num in Enumerable.ToList<int>(this._backpackMarkers.Keys))
			{
				if (!instance.LootItems.ContainsKey(num))
				{
					this.TryRemoveMarker(num);
				}
			}
		}

		// Token: 0x060001DB RID: 475 RVA: 0x0000AD84 File Offset: 0x00008F84
		private void TryAddMarker(MapView map, LootItem lootItem)
		{
			if (lootItem == null || lootItem.Item == null)
			{
				return;
			}
			int netId = lootItem.GetNetId();
			if (this._backpackMarkers.ContainsKey(netId))
			{
				return;
			}
			Color value = Settings.BackpackColor.Value;
			TransformMapMarker value2 = map.AddTransformMarker(lootItem.TrackableTransform, lootItem.Item.ShortName.BSGLocalized(), "Backpack", value, "Markers/backpack.png", BackpackMarkerProvider._backpackSize);
			this._backpackMarkers[netId] = value2;
		}

		// Token: 0x060001DC RID: 476 RVA: 0x0000ADF8 File Offset: 0x00008FF8
		private void TryRemoveMarker(int itemNetId)
		{
			if (!this._backpackMarkers.ContainsKey(itemNetId))
			{
				return;
			}
			this._backpackMarkers[itemNetId].ContainingMapView.RemoveMapMarker(this._backpackMarkers[itemNetId]);
			this._backpackMarkers.Remove(itemNetId);
		}

		// Token: 0x060001DD RID: 477 RVA: 0x0000AE38 File Offset: 0x00009038
		private void TryRemoveMarkers()
		{
			foreach (int itemNetId in Enumerable.ToList<int>(this._backpackMarkers.Keys))
			{
				this.TryRemoveMarker(itemNetId);
			}
			this._backpackMarkers.Clear();
		}

		// Token: 0x060001DE RID: 478 RVA: 0x0000AEA0 File Offset: 0x000090A0
		public void OnHideOutOfRaid(MapView map)
		{
		}

		// Token: 0x060001DF RID: 479 RVA: 0x0000AEA2 File Offset: 0x000090A2
		public void OnShowOutOfRaid(MapView map)
		{
		}

		// Token: 0x040000F9 RID: 249
		private const string _backpackCategory = "Backpack";

		// Token: 0x040000FA RID: 250
		private const string _backpackImagePath = "Markers/backpack.png";

		// Token: 0x040000FB RID: 251
		private static Vector2 _backpackSize = new Vector2(30f, 30f);

		// Token: 0x040000FC RID: 252
		private MapView _lastMapView;

		// Token: 0x040000FD RID: 253
		private Dictionary<int, DynamicMaps.UI.Components.MapMarker> _backpackMarkers = new Dictionary<int, DynamicMaps.UI.Components.MapMarker>();
	}
}
