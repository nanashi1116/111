using System;
using UnityEngine;

namespace DynamicMaps.UI.Components
{
	// Token: 0x02000019 RID: 25
	public interface ILayerBound
	{
		// Token: 0x17000016 RID: 22
		// (get) Token: 0x060000D0 RID: 208
		// (set) Token: 0x060000D1 RID: 209
		Vector3 Position { get; set; }

		// Token: 0x060000D2 RID: 210
		void HandleNewLayerStatus(LayerStatus status);
	}
}
