using System;
using DynamicMaps.Data;
using DynamicMaps.UI.Components;

namespace DynamicMaps.DynamicMarkers
{
	// Token: 0x02000035 RID: 53
	public interface IDynamicMarkerProvider
	{
		// Token: 0x06000218 RID: 536
		void OnShowOutOfRaid(MapView map);

		// Token: 0x06000219 RID: 537
		void OnHideOutOfRaid(MapView map);

		// Token: 0x0600021A RID: 538
		void OnShowInRaid(MapView map);

		// Token: 0x0600021B RID: 539
		void OnHideInRaid(MapView map);

		// Token: 0x0600021C RID: 540
		void OnRaidEnd(MapView map);

		// Token: 0x0600021D RID: 541
		void OnMapChanged(MapView map, MapDef mapDef);

		// Token: 0x0600021E RID: 542
		void OnDisable(MapView map);
	}
}
