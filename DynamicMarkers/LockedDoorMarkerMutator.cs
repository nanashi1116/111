using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DynamicMaps.Data;
using DynamicMaps.UI.Components;
using DynamicMaps.Utils;
using EFT;
using EFT.InventoryLogic;
using UnityEngine;

namespace DynamicMaps.DynamicMarkers
{
	// Token: 0x02000036 RID: 54
	public class LockedDoorMarkerMutator : IDynamicMarkerProvider
	{
		// Token: 0x0600021F RID: 543 RVA: 0x0000BAF4 File Offset: 0x00009CF4
		public void OnShowInRaid(MapView map)
		{
			Player mainPlayer = GameUtils.GetMainPlayer();
			using (IEnumerator<DynamicMaps.UI.Components.MapMarker> enumerator = map.GetMapMarkersByCategory("LockedDoor").GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					DynamicMaps.UI.Components.MapMarker marker = enumerator.Current;
					if (!string.IsNullOrWhiteSpace(marker.AssociatedItemId))
					{
						bool flag = Enumerable.Any<Item>(mainPlayer.Inventory.Equipment.GetAllItems(), (Item i) => i.TemplateId == marker.AssociatedItemId);
						marker.Image.sprite = (flag ? TextureUtils.GetOrLoadCachedSprite(LockedDoorMarkerMutator.doorWithKeyPath) : TextureUtils.GetOrLoadCachedSprite(LockedDoorMarkerMutator.doorWithLockPath));
						marker.Color = (flag ? Color.green : Color.red);
					}
				}
			}
		}

		// Token: 0x06000220 RID: 544 RVA: 0x0000BBD0 File Offset: 0x00009DD0
		public void OnShowOutOfRaid(MapView map)
		{
			Profile playerProfile = GameUtils.GetPlayerProfile();
			using (IEnumerator<DynamicMaps.UI.Components.MapMarker> enumerator = map.GetMapMarkersByCategory("LockedDoor").GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					DynamicMaps.UI.Components.MapMarker marker = enumerator.Current;
					if (!string.IsNullOrWhiteSpace(marker.AssociatedItemId))
					{
						bool flag = Enumerable.Any<Item>(playerProfile.Inventory.Stash.GetAllItems(), (Item i) => i.TemplateId == marker.AssociatedItemId);
						bool flag2 = Enumerable.Any<Item>(playerProfile.Inventory.Equipment.GetAllItems(), (Item i) => i.TemplateId == marker.AssociatedItemId);
						marker.Image.sprite = ((flag || flag2) ? TextureUtils.GetOrLoadCachedSprite(LockedDoorMarkerMutator.doorWithKeyPath) : TextureUtils.GetOrLoadCachedSprite(LockedDoorMarkerMutator.doorWithLockPath));
						marker.Color = (flag2 ? Color.green : (flag ? Color.yellow : Color.red));
					}
				}
			}
		}

		// Token: 0x06000221 RID: 545 RVA: 0x0000BCE0 File Offset: 0x00009EE0
		public void OnMapChanged(MapView map, MapDef mapDef)
		{
			if (GameUtils.IsInRaid())
			{
				this.OnShowInRaid(map);
				return;
			}
			this.OnShowOutOfRaid(map);
		}

		// Token: 0x06000222 RID: 546 RVA: 0x0000BCF8 File Offset: 0x00009EF8
		public void OnDisable(MapView map)
		{
			foreach (DynamicMaps.UI.Components.MapMarker mapMarker in map.GetMapMarkersByCategory("LockedDoor"))
			{
				if (!string.IsNullOrWhiteSpace(mapMarker.AssociatedItemId))
				{
					mapMarker.Image.sprite = TextureUtils.GetOrLoadCachedSprite(LockedDoorMarkerMutator.doorWithLockPath);
					mapMarker.Color = Color.yellow;
				}
			}
		}

		// Token: 0x06000223 RID: 547 RVA: 0x0000BD70 File Offset: 0x00009F70
		public void OnRaidEnd(MapView map)
		{
		}

		// Token: 0x06000224 RID: 548 RVA: 0x0000BD72 File Offset: 0x00009F72
		public void OnHideInRaid(MapView map)
		{
		}

		// Token: 0x06000225 RID: 549 RVA: 0x0000BD74 File Offset: 0x00009F74
		public void OnHideOutOfRaid(MapView map)
		{
		}

		// Token: 0x0400011F RID: 287
		private static string doorWithKeyPath = Path.Combine(Plugin.Path, "Markers/door_with_key.png");

		// Token: 0x04000120 RID: 288
		private static string doorWithLockPath = Path.Combine(Plugin.Path, "Markers/door_with_lock.png");
	}
}
