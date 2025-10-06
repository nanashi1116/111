using System;
using System.Reflection;
using EFT;
using HarmonyLib;
using SPT.Reflection.Patching;

namespace DynamicMaps.Patches
{
	// Token: 0x02000025 RID: 37
	internal class GameWorldUnregisterPlayerPatch : ModulePatch
	{
		// Token: 0x14000007 RID: 7
		// (add) Token: 0x0600018E RID: 398 RVA: 0x00009DEC File Offset: 0x00007FEC
		// (remove) Token: 0x0600018F RID: 399 RVA: 0x00009E20 File Offset: 0x00008020
		internal static event Action<IPlayer> OnUnregisterPlayer;

		// Token: 0x06000190 RID: 400 RVA: 0x00009E53 File Offset: 0x00008053
		protected override MethodBase GetTargetMethod()
		{
			return AccessTools.Method(typeof(GameWorld), "UnregisterPlayer", null, null);
		}

		// Token: 0x06000191 RID: 401 RVA: 0x00009E6B File Offset: 0x0000806B
		[PatchPostfix]
		public static void PatchPostfix(IPlayer iPlayer)
		{
			Action<IPlayer> onUnregisterPlayer = GameWorldUnregisterPlayerPatch.OnUnregisterPlayer;
			if (onUnregisterPlayer == null)
			{
				return;
			}
			onUnregisterPlayer(iPlayer);
		}
	}
}
