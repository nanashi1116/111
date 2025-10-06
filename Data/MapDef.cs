using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace DynamicMaps.Data
{
	// Token: 0x02000042 RID: 66
	public class MapDef
	{
		// Token: 0x1700006C RID: 108
		// (get) Token: 0x060002B5 RID: 693 RVA: 0x0000D9B2 File Offset: 0x0000BBB2
		// (set) Token: 0x060002B6 RID: 694 RVA: 0x0000D9BA File Offset: 0x0000BBBA
		[JsonRequired]
		public string DisplayName { get; set; }

		// Token: 0x1700006D RID: 109
		// (get) Token: 0x060002B7 RID: 695 RVA: 0x0000D9C3 File Offset: 0x0000BBC3
		// (set) Token: 0x060002B8 RID: 696 RVA: 0x0000D9CB File Offset: 0x0000BBCB
		[JsonRequired]
		public BoundingRectangle Bounds { get; set; }

		// Token: 0x1700006E RID: 110
		// (get) Token: 0x060002B9 RID: 697 RVA: 0x0000D9D4 File Offset: 0x0000BBD4
		// (set) Token: 0x060002BA RID: 698 RVA: 0x0000D9DC File Offset: 0x0000BBDC
		[JsonRequired]
		public Dictionary<string, MapLayerDef> Layers { get; set; } = new Dictionary<string, MapLayerDef>();

		// Token: 0x1700006F RID: 111
		// (get) Token: 0x060002BB RID: 699 RVA: 0x0000D9E5 File Offset: 0x0000BBE5
		// (set) Token: 0x060002BC RID: 700 RVA: 0x0000D9ED File Offset: 0x0000BBED
		public List<string> MapInternalNames { get; set; } = new List<string>();

		// Token: 0x17000070 RID: 112
		// (get) Token: 0x060002BD RID: 701 RVA: 0x0000D9F6 File Offset: 0x0000BBF6
		// (set) Token: 0x060002BE RID: 702 RVA: 0x0000D9FE File Offset: 0x0000BBFE
		public float CoordinateRotation { get; set; }

		// Token: 0x17000071 RID: 113
		// (get) Token: 0x060002BF RID: 703 RVA: 0x0000DA07 File Offset: 0x0000BC07
		// (set) Token: 0x060002C0 RID: 704 RVA: 0x0000DA0F File Offset: 0x0000BC0F
		public List<MapLabelDef> Labels { get; set; } = new List<MapLabelDef>();

		// Token: 0x17000072 RID: 114
		// (get) Token: 0x060002C1 RID: 705 RVA: 0x0000DA18 File Offset: 0x0000BC18
		// (set) Token: 0x060002C2 RID: 706 RVA: 0x0000DA20 File Offset: 0x0000BC20
		public List<MapMarkerDef> StaticMarkers { get; set; } = new List<MapMarkerDef>();

		// Token: 0x17000073 RID: 115
		// (get) Token: 0x060002C3 RID: 707 RVA: 0x0000DA29 File Offset: 0x0000BC29
		// (set) Token: 0x060002C4 RID: 708 RVA: 0x0000DA31 File Offset: 0x0000BC31
		public int DefaultLevel { get; set; }

		// Token: 0x17000074 RID: 116
		// (get) Token: 0x060002C5 RID: 709 RVA: 0x0000DA3A File Offset: 0x0000BC3A
		// (set) Token: 0x060002C6 RID: 710 RVA: 0x0000DA42 File Offset: 0x0000BC42
		public string Author { get; set; }

		// Token: 0x17000075 RID: 117
		// (get) Token: 0x060002C7 RID: 711 RVA: 0x0000DA4B File Offset: 0x0000BC4B
		// (set) Token: 0x060002C8 RID: 712 RVA: 0x0000DA53 File Offset: 0x0000BC53
		public string AuthorLink { get; set; }

		// Token: 0x060002C9 RID: 713 RVA: 0x0000DA5C File Offset: 0x0000BC5C
		public static MapDef LoadFromPath(string absolutePath)
		{
			try
			{
				return JsonConvert.DeserializeObject<MapDef>(File.ReadAllText(absolutePath));
			}
			catch (Exception ex)
			{
				Plugin.Log.LogError("Loading MapMappingDef failed from json at path: " + absolutePath);
				Plugin.Log.LogError("Exception given was: " + ex.Message);
				Plugin.Log.LogError(ex.StackTrace ?? "");
			}
			return null;
		}
	}
}
