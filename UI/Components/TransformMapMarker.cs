using System;
using DynamicMaps.Utils;
using UnityEngine;

namespace DynamicMaps.UI.Components
{
	// Token: 0x0200001F RID: 31
	public class TransformMapMarker : MapMarker
	{
		// Token: 0x17000046 RID: 70
		// (get) Token: 0x0600016F RID: 367 RVA: 0x00009835 File Offset: 0x00007A35
		// (set) Token: 0x06000170 RID: 368 RVA: 0x0000983D File Offset: 0x00007A3D
		public Transform FollowingTransform { get; private set; }

		// Token: 0x17000047 RID: 71
		// (get) Token: 0x06000171 RID: 369 RVA: 0x00009846 File Offset: 0x00007A46
		// (set) Token: 0x06000172 RID: 370 RVA: 0x0000984E File Offset: 0x00007A4E
		public MathUtils.RotationAxis RotationAxis { get; set; } = MathUtils.RotationAxis.Y;

		// Token: 0x06000173 RID: 371 RVA: 0x00009858 File Offset: 0x00007A58
		public static TransformMapMarker Create(Transform followingTransform, GameObject parent, string imagePath, Color color, string name, string category, Vector2 size, float degreesRotation, float scale)
		{
			TransformMapMarker transformMapMarker = MapMarker.Create<TransformMapMarker>(parent, name, category, imagePath, color, MathUtils.ConvertToMapPosition(followingTransform), size, TransformMapMarker._pivot, degreesRotation, scale, true, null);
			transformMapMarker.IsDynamic = true;
			transformMapMarker.FollowingTransform = followingTransform;
			return transformMapMarker;
		}

		// Token: 0x06000174 RID: 372 RVA: 0x00009894 File Offset: 0x00007A94
		public TransformMapMarker()
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

		// Token: 0x06000175 RID: 373 RVA: 0x00009944 File Offset: 0x00007B44
		private void LateUpdate()
		{
			if (this.FollowingTransform == null)
			{
				return;
			}
			if (!this.FollowingTransform.gameObject.activeInHierarchy)
			{
				if (!this._warnedAttachedIsDisabled)
				{
					Plugin.Log.LogWarning("FollowingTransform for TransformMapMarker has been disabled without removing the map marker");
					base.Color = Color.red;
					this._warnedAttachedIsDisabled = true;
				}
				return;
			}
			this._warnedAttachedIsDisabled = false;
			Vector3 vector = MathUtils.ConvertToMapPosition(this.FollowingTransform);
			float num = MathUtils.ConvertToMapRotation(this.FollowingTransform, this.RotationAxis);
			if (MathUtils.ApproxEquals(base.Position.x, vector.x) && MathUtils.ApproxEquals(base.Position.y, vector.y) && MathUtils.ApproxEquals(base.Position.z, vector.z) && MathUtils.ApproxEquals(base.Rotation, num))
			{
				return;
			}
			this._callbackTime += Time.deltaTime;
			bool flag = this._callbackTime >= TransformMapMarker._maxCallbackTime;
			if (flag)
			{
				this._callbackTime = 0f;
			}
			base.MoveAndRotate(vector, num, flag);
		}

		// Token: 0x040000D4 RID: 212
		private static float _maxCallbackTime = 0.5f;

		// Token: 0x040000D5 RID: 213
		private static Vector2 _pivot = new Vector2(0.5f, 0.5f);

		// Token: 0x040000D8 RID: 216
		private float _callbackTime = TransformMapMarker._maxCallbackTime;

		// Token: 0x040000D9 RID: 217
		private bool _warnedAttachedIsDisabled;
	}
}
