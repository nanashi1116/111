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
	// Token: 0x02000029 RID: 41
	internal class MapScreenShowPatch : ModulePatch
	{
		// Token: 0x060001A1 RID: 417 RVA: 0x0000A157 File Offset: 0x00008357
		protected override MethodBase GetTargetMethod()
		{
			return AccessTools.Method(typeof(MapScreen), "Show", null, null);
		}

		// Token: 0x060001A2 RID: 418 RVA: 0x0000A170 File Offset: 0x00008370
		[PatchPrefix]
		public static bool PatchPrefix()
		{
			bool result;
			try
			{
				if (!Settings.ReplaceMapScreen.Value || !GameUtils.ShouldShowMapInRaid())
				{
					ModdedMapScreen map = Plugin.Instance.Map;
					if (map != null)
					{
						map.OnMapScreenClose();
					}
					result = true;
				}
				else
				{
					ModdedMapScreen map2 = Plugin.Instance.Map;
					if (map2 != null)
					{
						map2.OnMapScreenShow();
					}
					result = false;
				}
			}
			catch (Exception ex)
			{
				Plugin.Log.LogError("Caught error while trying to show map");
				Plugin.Log.LogError(ex.Message ?? "");
				Plugin.Log.LogError(ex.StackTrace ?? "");
				result = true;
			}
			return result;
		}
	}
}
