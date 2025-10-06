using System;
using System.Collections.Generic;
using BepInEx.Configuration;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DynamicMaps.Utils
{
	// Token: 0x02000010 RID: 16
	public static class UIUtils
	{
		// Token: 0x06000055 RID: 85 RVA: 0x00004768 File Offset: 0x00002968
		public static void ResetRectTransform(this GameObject gameObject)
		{
			RectTransform rectTransform = gameObject.transform as RectTransform;
			rectTransform.localPosition = Vector3.zero;
			rectTransform.localRotation = Quaternion.identity;
			rectTransform.localScale = Vector3.one;
			rectTransform.anchoredPosition = Vector2.zero;
			rectTransform.anchoredPosition3D = Vector3.zero;
			rectTransform.offsetMin = Vector2.zero;
			rectTransform.offsetMax = Vector2.zero;
			rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
			rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
			rectTransform.pivot = new Vector2(0.5f, 0.5f);
		}

		// Token: 0x06000056 RID: 86 RVA: 0x0000480B File Offset: 0x00002A0B
		public static RectTransform GetRectTransform(this GameObject gameObject)
		{
			return gameObject.transform as RectTransform;
		}

		// Token: 0x06000057 RID: 87 RVA: 0x00004818 File Offset: 0x00002A18
		public static RectTransform GetRectTransform(this Component component)
		{
			return component.gameObject.transform as RectTransform;
		}

		// Token: 0x06000058 RID: 88 RVA: 0x0000482C File Offset: 0x00002A2C
		public static Tween TweenColor(this Image image, Color to, float duration)
		{
			return DOTween.To(() => image.color, delegate(Color c)
			{
				image.color = c;
			}, to, duration);
		}

		// Token: 0x06000059 RID: 89 RVA: 0x00004865 File Offset: 0x00002A65
		public static GameObject CreateUIGameObject(GameObject parent, string name)
		{
			GameObject gameObject = new GameObject(name, new Type[]
			{
				typeof(RectTransform)
			});
			gameObject.layer = parent.layer;
			gameObject.transform.SetParent(parent.transform);
			gameObject.ResetRectTransform();
			return gameObject;
		}

		// Token: 0x0600005A RID: 90 RVA: 0x000048A4 File Offset: 0x00002AA4
		public static bool TrySetTMPOutline(TextMeshProUGUI text)
		{
			if (text == null)
			{
				Plugin.Log.LogWarning("TrySetTMPOutline: text cannot be null");
				return false;
			}
			try
			{
				text.outlineColor = new Color32(0, 0, 0, byte.MaxValue);
				text.outlineWidth = 0.15f;
				text.fontStyle = FontStyles.Bold;
				text.ForceMeshUpdate(true, true);
				return true;
			}
			catch
			{
			}
			return false;
		}

		// Token: 0x0600005B RID: 91 RVA: 0x00004914 File Offset: 0x00002B14
		public static bool BetterIsPressed(this KeyboardShortcut key)
		{
			if (!Input.GetKey(key.MainKey))
			{
				return false;
			}
			using (IEnumerator<KeyCode> enumerator = key.Modifiers.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (!Input.GetKey(enumerator.Current))
					{
						return false;
					}
				}
			}
			return true;
		}

		// Token: 0x0600005C RID: 92 RVA: 0x00004978 File Offset: 0x00002B78
		public static bool BetterIsDown(this KeyboardShortcut key)
		{
			if (!Input.GetKeyDown(key.MainKey))
			{
				return false;
			}
			using (IEnumerator<KeyCode> enumerator = key.Modifiers.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (!Input.GetKey(enumerator.Current))
					{
						return false;
					}
				}
			}
			return true;
		}
	}
}
