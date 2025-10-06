using System;
using System.Collections.Generic;
using DynamicMaps.Data;
using DynamicMaps.DynamicMarkers;
using DynamicMaps.UI.Components;
using DynamicMaps.Utils;
using EFT;

namespace DynamicMaps
{
	// Token: 0x02000006 RID: 6
	public class QuestMarkerProvider : IDynamicMarkerProvider
	{
		// Token: 0x06000007 RID: 7 RVA: 0x000021F1 File Offset: 0x000003F1
		public void OnShowInRaid(MapView map)
		{
			if (GameUtils.IsScavRaid())
			{
				return;
			}
			this.AddQuestObjectiveMarkers(map);
		}

		// Token: 0x06000008 RID: 8 RVA: 0x00002202 File Offset: 0x00000402
		public void OnHideInRaid(MapView map)
		{
			this.TryRemoveMarkers();
		}

		// Token: 0x06000009 RID: 9 RVA: 0x0000220A File Offset: 0x0000040A
		public void OnMapChanged(MapView map, MapDef mapDef)
		{
			if (!GameUtils.IsInRaid())
			{
				return;
			}
			this.TryRemoveMarkers();
			this.AddQuestObjectiveMarkers(map);
		}

		// Token: 0x0600000A RID: 10 RVA: 0x00002221 File Offset: 0x00000421
		public void OnRaidEnd(MapView map)
		{
			QuestUtils.DiscardQuestData();
			this.TryRemoveMarkers();
		}

		// Token: 0x0600000B RID: 11 RVA: 0x0000222E File Offset: 0x0000042E
		public void OnDisable(MapView map)
		{
			this.TryRemoveMarkers();
		}

		// Token: 0x0600000C RID: 12 RVA: 0x00002238 File Offset: 0x00000438
		private void AddQuestObjectiveMarkers(MapView map)
		{
			try
			{
				QuestUtils.TryCaptureQuestData();
				Player mainPlayer = GameUtils.GetMainPlayer();
				if (mainPlayer == null)
				{
					Plugin.Log.LogWarning("Main player is null, cannot add quest markers");
				}
				else
				{
					IEnumerable<MapMarkerDef> markerDefsForPlayer = QuestUtils.GetMarkerDefsForPlayer(mainPlayer);
					if (markerDefsForPlayer == null)
					{
						Plugin.Log.LogWarning("Quest marker definitions are null");
					}
					else
					{
						foreach (MapMarkerDef mapMarkerDef in markerDefsForPlayer)
						{
							if (mapMarkerDef != null)
							{
								MapMarker mapMarker = map.AddMapMarker(mapMarkerDef);
								if (mapMarker != null)
								{
									this._questMarkers.Add(mapMarker);
								}
							}
						}
					}
				}
			}
			catch (Exception arg)
			{
				Plugin.Log.LogError(string.Format("Exception in AddQuestObjectiveMarkers: {0}", arg));
			}
		}

		// Token: 0x0600000D RID: 13 RVA: 0x00002304 File Offset: 0x00000504
		private void TryRemoveMarkers()
		{
			foreach (MapMarker mapMarker in this._questMarkers)
			{
				mapMarker.ContainingMapView.RemoveMapMarker(mapMarker);
			}
			this._questMarkers.Clear();
		}

		// Token: 0x0600000E RID: 14 RVA: 0x00002368 File Offset: 0x00000568
		public void OnShowOutOfRaid(MapView map)
		{
		}

		// Token: 0x0600000F RID: 15 RVA: 0x0000236A File Offset: 0x0000056A
		public void OnHideOutOfRaid(MapView map)
		{
		}

		// Token: 0x04000011 RID: 17
		private List<MapMarker> _questMarkers = new List<MapMarker>();
	}
}
