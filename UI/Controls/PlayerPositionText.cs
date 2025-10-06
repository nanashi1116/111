using System;
using DynamicMaps.Utils;
using EFT;
using UnityEngine;

namespace DynamicMaps.UI.Controls
{
	// Token: 0x02000016 RID: 22
	public class PlayerPositionText : AbstractTextControl
	{
		// Token: 0x060000BC RID: 188 RVA: 0x000072F1 File Offset: 0x000054F1
		public static PlayerPositionText Create(GameObject parent, float fontSize)
		{
			return AbstractTextControl.Create<PlayerPositionText>(parent, "PlayerPositionText", fontSize);
		}

		// Token: 0x060000BD RID: 189 RVA: 0x00007300 File Offset: 0x00005500
		private void Update()
		{
			Player mainPlayer = GameUtils.GetMainPlayer();
			if (mainPlayer == null)
			{
				return;
			}
			Vector3 vector = MathUtils.ConvertToMapPosition(((IPlayer)mainPlayer).Position);
			base.Text.text = string.Format("Player: {0:F} {1:F} {2:F}", vector.x, vector.y, vector.z);
		}
	}
}
