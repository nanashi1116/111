using System;
using Newtonsoft.Json;
using UnityEngine;

namespace DynamicMaps.Data
{
	// Token: 0x02000041 RID: 65
	public class MapLabelDef
	{
		// Token: 0x17000066 RID: 102
		// (get) Token: 0x060002A8 RID: 680 RVA: 0x0000D923 File Offset: 0x0000BB23
		// (set) Token: 0x060002A9 RID: 681 RVA: 0x0000D92B File Offset: 0x0000BB2B
		[JsonRequired]
		public string Text { get; set; }

		// Token: 0x17000067 RID: 103
		// (get) Token: 0x060002AA RID: 682 RVA: 0x0000D934 File Offset: 0x0000BB34
		// (set) Token: 0x060002AB RID: 683 RVA: 0x0000D93C File Offset: 0x0000BB3C
		[JsonRequired]
		public Vector3 Position { get; set; }

		// Token: 0x17000068 RID: 104
		// (get) Token: 0x060002AC RID: 684 RVA: 0x0000D945 File Offset: 0x0000BB45
		// (set) Token: 0x060002AD RID: 685 RVA: 0x0000D94D File Offset: 0x0000BB4D
		public float FontSize { get; set; } = 14f;

		// Token: 0x17000069 RID: 105
		// (get) Token: 0x060002AE RID: 686 RVA: 0x0000D956 File Offset: 0x0000BB56
		// (set) Token: 0x060002AF RID: 687 RVA: 0x0000D95E File Offset: 0x0000BB5E
		public float DegreesRotation { get; set; }

		// Token: 0x1700006A RID: 106
		// (get) Token: 0x060002B0 RID: 688 RVA: 0x0000D967 File Offset: 0x0000BB67
		// (set) Token: 0x060002B1 RID: 689 RVA: 0x0000D96F File Offset: 0x0000BB6F
		public Color Color { get; set; } = Color.white;

		// Token: 0x1700006B RID: 107
		// (get) Token: 0x060002B2 RID: 690 RVA: 0x0000D978 File Offset: 0x0000BB78
		// (set) Token: 0x060002B3 RID: 691 RVA: 0x0000D980 File Offset: 0x0000BB80
		public string Category { get; set; } = "None";
	}
}
