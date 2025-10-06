using System;
using UnityEngine;

namespace DynamicMaps.UI.Controls
{
	// Token: 0x02000013 RID: 19
	public class CursorPositionText : AbstractTextControl
	{
		// Token: 0x06000096 RID: 150 RVA: 0x0000691C File Offset: 0x00004B1C
		public static CursorPositionText Create(GameObject parent, RectTransform mapViewTransform, float fontSize)
		{
			CursorPositionText cursorPositionText = AbstractTextControl.Create<CursorPositionText>(parent, "CursorPositionText", fontSize);
			cursorPositionText._mapViewTransform = mapViewTransform;
			return cursorPositionText;
		}

		// Token: 0x06000097 RID: 151 RVA: 0x00006934 File Offset: 0x00004B34
		private void Update()
		{
			Vector2 vector;
			RectTransformUtility.ScreenPointToLocalPointInRectangle(this._mapViewTransform, Input.mousePosition, null, out vector);
			base.Text.text = string.Format("Cursor: {0:F} {1:F}", vector.x, vector.y);
		}

		// Token: 0x04000078 RID: 120
		private RectTransform _mapViewTransform;
	}
}
