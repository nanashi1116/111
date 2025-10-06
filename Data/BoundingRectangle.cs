using System;
using UnityEngine;

namespace DynamicMaps.Data
{
	// Token: 0x0200003D RID: 61
	public class BoundingRectangle
	{
		// Token: 0x17000055 RID: 85
		// (get) Token: 0x06000282 RID: 642 RVA: 0x0000D78F File Offset: 0x0000B98F
		// (set) Token: 0x06000283 RID: 643 RVA: 0x0000D797 File Offset: 0x0000B997
		public Vector2 Min { get; set; }

		// Token: 0x17000056 RID: 86
		// (get) Token: 0x06000284 RID: 644 RVA: 0x0000D7A0 File Offset: 0x0000B9A0
		// (set) Token: 0x06000285 RID: 645 RVA: 0x0000D7A8 File Offset: 0x0000B9A8
		public Vector2 Max { get; set; }
	}
}
