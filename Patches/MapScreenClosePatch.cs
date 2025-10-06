using System;
using System.Reflection;
using DynamicMaps.Config;
using DynamicMaps.UI;
using DynamicMaps.Utils;
using EFT.UI.Map;
using HarmonyLib;
using SPT.Reflection.Patching;

namespace DynamicMaps.Patches
{
	// Token: 0x0200002A RID: 42
	internal class MapScreenClosePatch : ModulePatch
	{
		// Token: 0x060001A4 RID: 420 RVA: 0x0000A224 File Offset: 0x00008424
		protected override MethodBase GetTargetMethod()
		{
			return AccessTools.Method(typeof(MapScreen), "Close", null, null);
		}

		// Token: 0x060001A5 RID: 421 RVA: 0x0000A23C File Offset: 0x0000843C
		[PatchPrefix]
		public static bool PatchPrefix()
		{
			bool result;
			try
			{
				if (!Settings.ReplaceMapScreen.Value || !GameUtils.ShouldShowMapInRaid())
				{
					result = true;
				}
				else
				{
					ModdedMapScreen map = Plugin.Instance.Map;
					if (map != null)
					{
						map.OnMapScreenClose();
					}
					result = false;
				}
			}
			catch (Exception ex)
			{
				Plugin.Log.LogError("Caught error while trying to close map");
				Plugin.Log.LogError(ex.Message ?? "");
				Plugin.Log.LogError(ex.StackTrace ?? "");
				result = true;
			}
			return result;
		}
	}
}
