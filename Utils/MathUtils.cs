using System;
using UnityEngine;

namespace DynamicMaps.Utils
{
	// Token: 0x0200000D RID: 13
	public static class MathUtils
	{
		// Token: 0x06000038 RID: 56 RVA: 0x0000304C File Offset: 0x0000124C
		public static Vector2 GetRotatedVector2(Vector2 vector, float degreeRotation)
		{
			float x = vector.x;
			float y = vector.y;
			float num = Mathf.Sin(degreeRotation * 0.017453292f);
			float num2 = Mathf.Cos(degreeRotation * 0.017453292f);
			return new Vector2(x * num2 - y * num, x * num + y * num2);
		}

		// Token: 0x06000039 RID: 57 RVA: 0x00003094 File Offset: 0x00001294
		public static Vector2 GetRotatedRectangle(Vector2 rectangle, float degreeRotation)
		{
			float x = rectangle.x;
			float y = rectangle.y;
			float num = degreeRotation * 0.017453292f;
			float f = 1.5707964f - num;
			float num2 = Mathf.Abs(y * Mathf.Sin(num));
			float num3 = Mathf.Abs(y * Mathf.Sin(f));
			float num4 = Mathf.Abs(x * Mathf.Sin(num));
			float num5 = Mathf.Abs(x * Mathf.Sin(f));
			return new Vector2(num2 + num5, num3 + num4);
		}

		// Token: 0x0600003A RID: 58 RVA: 0x00003102 File Offset: 0x00001302
		public static Vector2 GetMidpoint(Vector2 minBound, Vector2 maxBound)
		{
			return (minBound + maxBound) / 2f;
		}

		// Token: 0x0600003B RID: 59 RVA: 0x00003115 File Offset: 0x00001315
		public static Vector3 ConvertToMapPosition(Vector3 unityPosition)
		{
			return new Vector3(unityPosition.x, unityPosition.z, unityPosition.y);
		}

		// Token: 0x0600003C RID: 60 RVA: 0x0000312E File Offset: 0x0000132E
		public static Vector3 ConvertToMapPosition(Transform transform)
		{
			return MathUtils.ConvertToMapPosition(transform.position);
		}

		// Token: 0x0600003D RID: 61 RVA: 0x0000313C File Offset: 0x0000133C
		public static float ConvertToMapRotation(Transform transform, MathUtils.RotationAxis axis = MathUtils.RotationAxis.Y)
		{
			switch (axis)
			{
			case MathUtils.RotationAxis.X:
				return -transform.rotation.eulerAngles.x;
			case MathUtils.RotationAxis.Y:
				return -transform.rotation.eulerAngles.y;
			case MathUtils.RotationAxis.Z:
				return -transform.rotation.eulerAngles.z;
			default:
				return 0f;
			}
		}

		// Token: 0x0600003E RID: 62 RVA: 0x000031A1 File Offset: 0x000013A1
		public static bool ApproxEquals(float first, float second)
		{
			return Mathf.Abs(first - second) < float.Epsilon;
		}

		// Token: 0x02000049 RID: 73
		public enum RotationAxis
		{
			// Token: 0x040001D1 RID: 465
			X,
			// Token: 0x040001D2 RID: 466
			Y,
			// Token: 0x040001D3 RID: 467
			Z
		}
	}
}
