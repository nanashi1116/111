using System;
using DynamicMaps.Utils;
using EFT;
using UnityEngine;

namespace DynamicMaps.UI.Components
{
	// Token: 0x0200001E RID: 30
	public class PlayerMapMarker : MapMarker
	{
		// Token: 0x17000045 RID: 69
		// (get) Token: 0x06000169 RID: 361 RVA: 0x00009678 File Offset: 0x00007878
		// (set) Token: 0x0600016A RID: 362 RVA: 0x00009680 File Offset: 0x00007880
		public IPlayer Player { get; private set; }

		// Token: 0x0600016B RID: 363 RVA: 0x0000968C File Offset: 0x0000788C
		public static PlayerMapMarker Create(IPlayer player, GameObject parent, string imagePath, Color color, string category, Vector2 size, float degreesRotation, float scale)
		{
			string text = player.Profile.GetCorrectedNickname() ?? "";
			PlayerMapMarker playerMapMarker = MapMarker.Create<PlayerMapMarker>(parent, text, category, imagePath, color, MathUtils.ConvertToMapPosition(player.Position), size, PlayerMapMarker._pivot, degreesRotation, scale, true, null);
			playerMapMarker.IsDynamic = true;
			playerMapMarker.Player = player;
			return playerMapMarker;
		}

		// Token: 0x0600016C RID: 364 RVA: 0x000096E0 File Offset: 0x000078E0
		public PlayerMapMarker()
		{
			base.ImageAlphaLayerStatus[LayerStatus.Hidden] = 0.25f;
			base.ImageAlphaLayerStatus[LayerStatus.Underneath] = 0.25f;
			base.ImageAlphaLayerStatus[LayerStatus.OnTop] = 1f;
			base.ImageAlphaLayerStatus[LayerStatus.FullReveal] = 1f;
			base.LabelAlphaLayerStatus[LayerStatus.Hidden] = 0f;
			base.LabelAlphaLayerStatus[LayerStatus.Underneath] = 0f;
			base.LabelAlphaLayerStatus[LayerStatus.OnTop] = 0f;
			base.LabelAlphaLayerStatus[LayerStatus.FullReveal] = 1f;
		}

		// Token: 0x0600016D RID: 365 RVA: 0x00009788 File Offset: 0x00007988
		private void LateUpdate()
		{
			IPlayer player = this.Player;
			Object x;
			if (player == null)
			{
				x = null;
			}
			else
			{
				BifacialTransform transform = player.Transform;
				x = ((transform != null) ? transform.Original : null);
			}
			if (x == null)
			{
				return;
			}
			this._callbackTime += Time.deltaTime;
			bool flag = this._callbackTime >= PlayerMapMarker._maxCallbackTime;
			if (flag)
			{
				this._callbackTime = 0f;
			}
			base.MoveAndRotate(MathUtils.ConvertToMapPosition(this.Player.Position), -this.Player.Rotation.x, flag);
		}

		// Token: 0x040000D0 RID: 208
		private static float _maxCallbackTime = 0.5f;

		// Token: 0x040000D1 RID: 209
		private static Vector2 _pivot = new Vector2(0.5f, 0.5f);

		// Token: 0x040000D3 RID: 211
		private float _callbackTime = PlayerMapMarker._maxCallbackTime;
	}
}
