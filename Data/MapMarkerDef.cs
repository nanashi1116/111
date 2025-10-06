using System;
using Newtonsoft.Json;
using UnityEngine;

namespace DynamicMaps.Data
{
	// Token: 0x02000040 RID: 64
	public class MapMarkerDef
	{
		// Token: 0x1700005D RID: 93
		// (get) Token: 0x06000295 RID: 661 RVA: 0x0000D82F File Offset: 0x0000BA2F
		// (set) Token: 0x06000296 RID: 662 RVA: 0x0000D837 File Offset: 0x0000BA37
		public string Text { get; set; } = "";

		// Token: 0x1700005E RID: 94
		// (get) Token: 0x06000297 RID: 663 RVA: 0x0000D840 File Offset: 0x0000BA40
		// (set) Token: 0x06000298 RID: 664 RVA: 0x0000D848 File Offset: 0x0000BA48
		[JsonRequired]
		public string ImagePath { get; set; }

		// Token: 0x1700005F RID: 95
		// (get) Token: 0x06000299 RID: 665 RVA: 0x0000D851 File Offset: 0x0000BA51
		// (set) Token: 0x0600029A RID: 666 RVA: 0x0000D859 File Offset: 0x0000BA59
		[JsonRequired]
		public Vector3 Position { get; set; }

		// Token: 0x17000060 RID: 96
		// (get) Token: 0x0600029B RID: 667 RVA: 0x0000D862 File Offset: 0x0000BA62
		// (set) Token: 0x0600029C RID: 668 RVA: 0x0000D86A File Offset: 0x0000BA6A
		public Sprite Sprite { get; set; }

		// Token: 0x17000061 RID: 97
		// (get) Token: 0x0600029D RID: 669 RVA: 0x0000D873 File Offset: 0x0000BA73
		// (set) Token: 0x0600029E RID: 670 RVA: 0x0000D87B File Offset: 0x0000BA7B
		public bool ShowInRaid { get; set; } = true;

		// Token: 0x17000062 RID: 98
		// (get) Token: 0x0600029F RID: 671 RVA: 0x0000D884 File Offset: 0x0000BA84
		// (set) Token: 0x060002A0 RID: 672 RVA: 0x0000D88C File Offset: 0x0000BA8C
		public string Category { get; set; } = "None";

		// Token: 0x17000063 RID: 99
		// (get) Token: 0x060002A1 RID: 673 RVA: 0x0000D895 File Offset: 0x0000BA95
		// (set) Token: 0x060002A2 RID: 674 RVA: 0x0000D89D File Offset: 0x0000BA9D
		public Color Color { get; set; } = Color.white;

		// Token: 0x17000064 RID: 100
		// (get) Token: 0x060002A3 RID: 675 RVA: 0x0000D8A6 File Offset: 0x0000BAA6
		// (set) Token: 0x060002A4 RID: 676 RVA: 0x0000D8AE File Offset: 0x0000BAAE
		public Vector2 Pivot { get; set; } = new Vector2(0.5f, 0.5f);

		// Token: 0x17000065 RID: 101
		// (get) Token: 0x060002A5 RID: 677 RVA: 0x0000D8B7 File Offset: 0x0000BAB7
		// (set) Token: 0x060002A6 RID: 678 RVA: 0x0000D8BF File Offset: 0x0000BABF
		public string AssociatedItemId { get; set; } = "";
	}
}
