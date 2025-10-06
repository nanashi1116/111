using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace DynamicMaps.Utils
{
	// Token: 0x0200000F RID: 15
	public static class TextureUtils
	{
		// Token: 0x06000052 RID: 82 RVA: 0x000046B0 File Offset: 0x000028B0
		public static Texture2D LoadTexture2DFromPath(string absolutePath)
		{
			if (!File.Exists(absolutePath))
			{
				return null;
			}
			Texture2D texture2D = new Texture2D(2, 2, TextureFormat.RGBA32, false);
			texture2D.LoadImage(File.ReadAllBytes(absolutePath));
			return texture2D;
		}

		// Token: 0x06000053 RID: 83 RVA: 0x000046D4 File Offset: 0x000028D4
		public static Sprite GetOrLoadCachedSprite(string path)
		{
			if (TextureUtils._spriteCache.ContainsKey(path))
			{
				return TextureUtils._spriteCache[path];
			}
			Texture2D texture2D = TextureUtils.LoadTexture2DFromPath(Path.Combine(Plugin.Path, path));
			TextureUtils._spriteCache[path] = Sprite.Create(texture2D, new Rect(0f, 0f, (float)texture2D.width, (float)texture2D.height), new Vector2((float)(texture2D.width / 2), (float)(texture2D.height / 2)));
			return TextureUtils._spriteCache[path];
		}

		// Token: 0x04000045 RID: 69
		private static Dictionary<string, Sprite> _spriteCache = new Dictionary<string, Sprite>();
	}
}
