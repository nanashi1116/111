using System;
using BepInEx.Configuration;

// Token: 0x02000004 RID: 4
internal sealed class ConfigurationManagerAttributes
{
	// Token: 0x04000002 RID: 2
	public bool? ShowRangeAsPercent;

	// Token: 0x04000003 RID: 3
	public Action<ConfigEntryBase> CustomDrawer;

	// Token: 0x04000004 RID: 4
	public ConfigurationManagerAttributes.CustomHotkeyDrawerFunc CustomHotkeyDrawer;

	// Token: 0x04000005 RID: 5
	public bool? Browsable;

	// Token: 0x04000006 RID: 6
	public string Category;

	// Token: 0x04000007 RID: 7
	public object DefaultValue;

	// Token: 0x04000008 RID: 8
	public bool? HideDefaultButton;

	// Token: 0x04000009 RID: 9
	public bool? HideSettingName;

	// Token: 0x0400000A RID: 10
	public string Description;

	// Token: 0x0400000B RID: 11
	public string DispName;

	// Token: 0x0400000C RID: 12
	public int? Order;

	// Token: 0x0400000D RID: 13
	public bool? ReadOnly;

	// Token: 0x0400000E RID: 14
	public bool? IsAdvanced;

	// Token: 0x0400000F RID: 15
	public Func<object, string> ObjToStr;

	// Token: 0x04000010 RID: 16
	public Func<string, object> StrToObj;

	// Token: 0x02000044 RID: 68
	// (Invoke) Token: 0x060002D1 RID: 721
	public delegate void CustomHotkeyDrawerFunc(ConfigEntryBase setting, ref bool isCurrentlyAcceptingInput);
}
