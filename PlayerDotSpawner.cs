using System;
using DynamicMaps.Data;
using DynamicMaps.UI.Components;
using DynamicMaps.Utils;
using EFT;
using UnityEngine;

namespace DynamicMaps
{
	// Token: 0x02000008 RID: 8
	public class PlayerDotSpawner : MonoBehaviour
	{
		// Token: 0x17000002 RID: 2
		// (get) Token: 0x06000018 RID: 24 RVA: 0x0000251B File Offset: 0x0000071B
		// (set) Token: 0x06000019 RID: 25 RVA: 0x00002523 File Offset: 0x00000723
		public MapView MapView { get; set; }

		// Token: 0x0600001A RID: 26 RVA: 0x0000252C File Offset: 0x0000072C
		private void Update()
		{
			if (this.MapView == null)
			{
				return;
			}
			if (!Input.GetKey(KeyCode.M) || !Input.GetKey(KeyCode.LeftShift))
			{
				return;
			}
			this._timeAccumulator += Time.deltaTime;
			if (this._timeAccumulator <= PlayerDotSpawner._spawnTime && !Input.GetKeyDown(KeyCode.M))
			{
				return;
			}
			MapMarkerDef markerDef = new MapMarkerDef
			{
				ImagePath = "Markers/dot.png",
				Position = MathUtils.ConvertToMapPosition(((IPlayer)GameUtils.GetMainPlayer()).Position)
			};
			this.MapView.AddMapMarker(markerDef);
			this._timeAccumulator = 0f;
		}

		// Token: 0x04000016 RID: 22
		private static float _spawnTime = 0.25f;

		// Token: 0x04000017 RID: 23
		private float _timeAccumulator;
	}
}
