using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace DynamicMaps.Data
{
	// Token: 0x0200003F RID: 63
	public class MapLayerDef
	{
		// Token: 0x17000059 RID: 89
		// (get) Token: 0x0600028C RID: 652 RVA: 0x0000D7E3 File Offset: 0x0000B9E3
		// (set) Token: 0x0600028D RID: 653 RVA: 0x0000D7EB File Offset: 0x0000B9EB
		[JsonRequired]
		public int Level { get; set; }

		// Token: 0x1700005A RID: 90
		// (get) Token: 0x0600028E RID: 654 RVA: 0x0000D7F4 File Offset: 0x0000B9F4
		// (set) Token: 0x0600028F RID: 655 RVA: 0x0000D7FC File Offset: 0x0000B9FC
		[JsonRequired]
		public string ImagePath { get; set; }

		// Token: 0x1700005B RID: 91
		// (get) Token: 0x06000290 RID: 656 RVA: 0x0000D805 File Offset: 0x0000BA05
		// (set) Token: 0x06000291 RID: 657 RVA: 0x0000D80D File Offset: 0x0000BA0D
		[JsonRequired]
		public BoundingRectangle ImageBounds { get; set; }

		// Token: 0x1700005C RID: 92
		// (get) Token: 0x06000292 RID: 658 RVA: 0x0000D816 File Offset: 0x0000BA16
		// (set) Token: 0x06000293 RID: 659 RVA: 0x0000D81E File Offset: 0x0000BA1E
		[JsonRequired]
		public List<BoundingRectangularSolid> GameBounds { get; set; }
	}
}
