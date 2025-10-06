using System;
using BepInEx.Bootstrap;

namespace DynamicMaps.ExternalModSupport
{
	// Token: 0x0200002E RID: 46
	public static class ModDetection
	{
		// Token: 0x17000049 RID: 73
		// (get) Token: 0x060001B7 RID: 439 RVA: 0x0000A618 File Offset: 0x00008818
		// (set) Token: 0x060001B8 RID: 440 RVA: 0x0000A61F File Offset: 0x0000881F
		public static bool HeliCrashLoaded { get; private set; }

		// Token: 0x060001B9 RID: 441 RVA: 0x0000A627 File Offset: 0x00008827
		public static void CheckforMods()
		{
			if (Chainloader.PluginInfos.ContainsKey("com.SamSWAT.HeliCrash.ArysReloaded"))
			{
				ModDetection.HeliCrashLoaded = true;
			}
		}
	}
}
