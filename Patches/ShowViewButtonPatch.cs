using System;
using System.Collections.Generic;
using System.Reflection;
using DynamicMaps.Config;
using EFT.InventoryLogic;
using HarmonyLib;
using SPT.Reflection.Patching;
using SPT.Reflection.Utils;

namespace DynamicMaps.Patches
{
	// Token: 0x02000028 RID: 40
	public class ShowViewButtonPatch : ModulePatch
	{
		// Token: 0x0600019D RID: 413 RVA: 0x0000A060 File Offset: 0x00008260
		protected override MethodBase GetTargetMethod()
		{
			Type type = PatchConstants.SingleCustom<Type>(PatchConstants.EftTypes, (Type t) => t.GetProperty("EItemViewType_0") != null);
			ShowViewButtonPatch._itemFieldInfo = AccessTools.Field(type, "item_0");
			return type.GetMethod("IsActive");
		}

		// Token: 0x0600019E RID: 414 RVA: 0x0000A0B0 File Offset: 0x000082B0
		[PatchPostfix]
		public static void PatchPrefix(object __instance, EItemInfoButton button, ref bool __result)
		{
			if (button != EItemInfoButton.ViewMap || Settings.ReplaceMapScreen.Value)
			{
				return;
			}
			Item item = (Item)ShowViewButtonPatch._itemFieldInfo.GetValue(__instance);
			__result = !ShowViewButtonPatch.MapsToIgnore.Contains(item.TemplateId);
		}

		// Token: 0x060001A0 RID: 416 RVA: 0x0000A104 File Offset: 0x00008304
		// Note: this type is marked as 'beforefieldinit'.
		static ShowViewButtonPatch()
		{
			HashSet<string> hashSet = new HashSet<string>();
			hashSet.Add("6738033eb7305d3bdafe9518");
			hashSet.Add("673803448cb3819668d77b1b");
			hashSet.Add("6738034a9713b5f42b4a8b78");
			hashSet.Add("6738034e9d22459ad7cd1b81");
			hashSet.Add("6738035350b24a4ae4a57997");
			ShowViewButtonPatch.MapsToIgnore = hashSet;
		}

		// Token: 0x040000E3 RID: 227
		private static FieldInfo _itemFieldInfo;

		// Token: 0x040000E4 RID: 228
		private static readonly HashSet<string> MapsToIgnore;
	}
}
