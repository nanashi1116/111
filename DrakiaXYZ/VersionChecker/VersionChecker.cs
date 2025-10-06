using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Configuration;
using BepInEx.Logging;
using UnityEngine;

namespace DrakiaXYZ.VersionChecker
{
	// Token: 0x02000005 RID: 5
	[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false)]
	public class VersionChecker : Attribute
	{
		// Token: 0x06000004 RID: 4 RVA: 0x00002070 File Offset: 0x00000270
		public static bool CheckEftVersion(ManualLogSource Logger, PluginInfo Info, ConfigFile Config = null)
		{
			int filePrivatePart = FileVersionInfo.GetVersionInfo(Paths.ExecutablePath).FilePrivatePart;
			int num = 39390;
			if (filePrivatePart != num)
			{
				string text = string.Format("ERROR: This version of DynamicMaps was built for Tarkov {0}, but you are running {1}. Please download the correct plugin version.", num, filePrivatePart);
				Logger.LogError(text);
				Chainloader.DependencyErrors.Add(text);
				if (Config != null)
				{
					string text2 = "";
					string text3 = "TarkovVersion";
					string text4 = "";
					string text5 = text;
					AcceptableValueBase acceptableValueBase = null;
					object[] array = new object[1];
					int num2 = 0;
					VersionChecker.ConfigurationManagerAttributes configurationManagerAttributes = new VersionChecker.ConfigurationManagerAttributes();
					Action<ConfigEntryBase> customDrawer;
					if ((customDrawer = VersionChecker.<>O.<0>__ErrorLabelDrawer) == null)
					{
						customDrawer = (VersionChecker.<>O.<0>__ErrorLabelDrawer = new Action<ConfigEntryBase>(VersionChecker.ErrorLabelDrawer));
					}
					configurationManagerAttributes.CustomDrawer = customDrawer;
					configurationManagerAttributes.ReadOnly = new bool?(true);
					configurationManagerAttributes.HideDefaultButton = new bool?(true);
					configurationManagerAttributes.HideSettingName = new bool?(true);
					configurationManagerAttributes.Category = null;
					array[num2] = configurationManagerAttributes;
					Config.Bind<string>(text2, text3, text4, new ConfigDescription(text5, acceptableValueBase, array));
				}
				return false;
			}
			return true;
		}

		// Token: 0x06000005 RID: 5 RVA: 0x00002140 File Offset: 0x00000340
		private static void ErrorLabelDrawer(ConfigEntryBase entry)
		{
			GUIStyle guistyle = new GUIStyle(GUI.skin.label);
			guistyle.wordWrap = true;
			guistyle.stretchWidth = true;
			GUIStyle guistyle2 = new GUIStyle(GUI.skin.label);
			guistyle2.stretchWidth = true;
			guistyle2.alignment = TextAnchor.MiddleCenter;
			guistyle2.normal.textColor = Color.red;
			guistyle2.fontStyle = FontStyle.Bold;
			GUILayout.BeginVertical(Array.Empty<GUILayoutOption>());
			GUILayout.Label(entry.Description.Description, guistyle, new GUILayoutOption[]
			{
				GUILayout.ExpandWidth(true)
			});
			GUILayout.Label("Plugin has been disabled!", guistyle2, new GUILayoutOption[]
			{
				GUILayout.ExpandWidth(true)
			});
			GUILayout.EndVertical();
		}

		// Token: 0x02000045 RID: 69
		internal sealed class ConfigurationManagerAttributes
		{
			// Token: 0x040001BC RID: 444
			public bool? ShowRangeAsPercent;

			// Token: 0x040001BD RID: 445
			public Action<ConfigEntryBase> CustomDrawer;

			// Token: 0x040001BE RID: 446
			public VersionChecker.ConfigurationManagerAttributes.CustomHotkeyDrawerFunc CustomHotkeyDrawer;

			// Token: 0x040001BF RID: 447
			public bool? Browsable;

			// Token: 0x040001C0 RID: 448
			public string Category;

			// Token: 0x040001C1 RID: 449
			public object DefaultValue;

			// Token: 0x040001C2 RID: 450
			public bool? HideDefaultButton;

			// Token: 0x040001C3 RID: 451
			public bool? HideSettingName;

			// Token: 0x040001C4 RID: 452
			public string Description;

			// Token: 0x040001C5 RID: 453
			public string DispName;

			// Token: 0x040001C6 RID: 454
			public int? Order;

			// Token: 0x040001C7 RID: 455
			public bool? ReadOnly;

			// Token: 0x040001C8 RID: 456
			public bool? IsAdvanced;

			// Token: 0x040001C9 RID: 457
			public Func<object, string> ObjToStr;

			// Token: 0x040001CA RID: 458
			public Func<string, object> StrToObj;

			// Token: 0x02000060 RID: 96
			// (Invoke) Token: 0x0600031C RID: 796
			public delegate void CustomHotkeyDrawerFunc(ConfigEntryBase setting, ref bool isCurrentlyAcceptingInput);
		}

		// Token: 0x02000046 RID: 70
		[CompilerGenerated]
		private static class <>O
		{
			// Token: 0x040001CB RID: 459
			public static Action<ConfigEntryBase> <0>__ErrorLabelDrawer;
		}
	}
}
