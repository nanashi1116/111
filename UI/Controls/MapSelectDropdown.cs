using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DynamicMaps.Data;
using DynamicMaps.Utils;
using EFT.UI;
using UnityEngine;

namespace DynamicMaps.UI.Controls
{
	// Token: 0x02000015 RID: 21
	public class MapSelectDropdown : MonoBehaviour
	{
		// Token: 0x14000002 RID: 2
		// (add) Token: 0x060000A9 RID: 169 RVA: 0x00006ECC File Offset: 0x000050CC
		// (remove) Token: 0x060000AA RID: 170 RVA: 0x00006F04 File Offset: 0x00005104
		public event Action<MapDef> OnMapSelected;

		// Token: 0x1700000E RID: 14
		// (get) Token: 0x060000AB RID: 171 RVA: 0x00006F39 File Offset: 0x00005139
		public RectTransform RectTransform
		{
			get
			{
				return base.gameObject.transform as RectTransform;
			}
		}

		// Token: 0x060000AC RID: 172 RVA: 0x00006F4B File Offset: 0x0000514B
		public static MapSelectDropdown Create(GameObject prefab, Transform parent)
		{
			GameObject gameObject = Object.Instantiate<GameObject>(prefab);
			gameObject.name = "MapSelectDropdown";
			RectTransform rectTransform = gameObject.GetRectTransform();
			rectTransform.SetParent(parent);
			rectTransform.localScale = Vector3.one;
			return gameObject.AddComponent<MapSelectDropdown>();
		}

		// Token: 0x060000AD RID: 173 RVA: 0x00006F7A File Offset: 0x0000517A
		private void Awake()
		{
			this._dropdown = base.gameObject.GetComponentInChildren<DropDownBox>();
			this._dropdown.SetLabelText("Select a Map");
		}

		// Token: 0x060000AE RID: 174 RVA: 0x00006F9D File Offset: 0x0000519D
		private void OnSelectDropdownMap(int index)
		{
			Action<MapDef> onMapSelected = this.OnMapSelected;
			if (onMapSelected == null)
			{
				return;
			}
			onMapSelected(this._selectableMapDefs[index]);
		}

		// Token: 0x060000AF RID: 175 RVA: 0x00006FBC File Offset: 0x000051BC
		private void ChangeAvailableMapDefs(IEnumerable<MapDef> mapDefs)
		{
			if (this._selectableMapDefs != null && Enumerable.SequenceEqual<MapDef>(mapDefs, this._selectableMapDefs))
			{
				return;
			}
			this._selectableMapDefs = Enumerable.ToList<MapDef>(mapDefs);
			this._dropdown.Show(Enumerable.Select<MapDef, string>(this._selectableMapDefs, (MapDef def) => def.DisplayName), null);
			this._dropdown.OnValueChanged.Subscribe(new Action<int>(this.OnSelectDropdownMap));
			base.gameObject.SetActive(this._selectableMapDefs.Count > 1);
		}

		// Token: 0x060000B0 RID: 176 RVA: 0x00007058 File Offset: 0x00005258
		public void LoadMapDefsFromPath(string relPath)
		{
			foreach (string text in Enumerable.Where<string>(Directory.EnumerateFiles(Path.Combine(Plugin.Path, relPath), "*.*", 1), (string p) => MapSelectDropdown._acceptableExtensions.Contains(Path.GetExtension(p).TrimStart(new char[]
			{
				'.'
			}).ToLowerInvariant())))
			{
				DateTime lastWriteTimeUtc = File.GetLastWriteTimeUtc(text);
				if (!this._writeTimes.ContainsKey(text) || !(this._writeTimes[text] == lastWriteTimeUtc))
				{
					this._writeTimes[text] = lastWriteTimeUtc;
					Plugin.Log.LogInfo("Trying to load MapDef from path: " + text);
					MapDef mapDef = MapDef.LoadFromPath(text);
					if (mapDef != null)
					{
						Plugin.Log.LogInfo("Loaded MapDef with display name: " + mapDef.DisplayName);
						this._mapDefs[text] = mapDef;
					}
				}
			}
			this.ChangeAvailableMapDefs(this.FilteredMapDefs());
		}

		// Token: 0x060000B1 RID: 177 RVA: 0x00007160 File Offset: 0x00005360
		private IEnumerable<MapDef> FilteredMapDefs()
		{
			return Enumerable.OrderBy<MapDef, string>(Enumerable.Where<MapDef>(this._mapDefs.Values, (MapDef m) => string.IsNullOrEmpty(this._nameFilter) || m.MapInternalNames.Contains(this._nameFilter)), (MapDef m) => m.DisplayName);
		}

		// Token: 0x060000B2 RID: 178 RVA: 0x000071AD File Offset: 0x000053AD
		public IEnumerable<MapDef> GetMapDefs()
		{
			return this._mapDefs.Values;
		}

		// Token: 0x060000B3 RID: 179 RVA: 0x000071BA File Offset: 0x000053BA
		public void LoadFirstAvailableMap()
		{
			if (this._selectableMapDefs.Count == 0)
			{
				return;
			}
			this.OnSelectDropdownMap(0);
		}

		// Token: 0x060000B4 RID: 180 RVA: 0x000071D4 File Offset: 0x000053D4
		public void FilterByInternalMapName(string internalMapName)
		{
			this._nameFilter = internalMapName;
			if (Enumerable.FirstOrDefault<MapDef>(this.FilteredMapDefs()) == null)
			{
				this._nameFilter = "";
				Plugin.Log.LogWarning("Cannot filter by " + internalMapName + ", no MapDefs match that map");
			}
			this.ChangeAvailableMapDefs(this.FilteredMapDefs());
		}

		// Token: 0x060000B5 RID: 181 RVA: 0x00007226 File Offset: 0x00005426
		public void ClearFilter()
		{
			this._nameFilter = null;
			this.ChangeAvailableMapDefs(this.FilteredMapDefs());
		}

		// Token: 0x060000B6 RID: 182 RVA: 0x0000723C File Offset: 0x0000543C
		public void OnLoadMap(MapDef mapDef)
		{
			this._dropdown.UpdateValue(this._selectableMapDefs.IndexOf(mapDef), true, null, null);
		}

		// Token: 0x060000B7 RID: 183 RVA: 0x00007273 File Offset: 0x00005473
		public bool IsDropdownOpen()
		{
			return this._dropdown.CurrentState;
		}

		// Token: 0x060000B8 RID: 184 RVA: 0x00007280 File Offset: 0x00005480
		public void TryCloseDropdown()
		{
			this._dropdown.Close();
		}

		// Token: 0x060000BA RID: 186 RVA: 0x000072AB File Offset: 0x000054AB
		// Note: this type is marked as 'beforefieldinit'.
		static MapSelectDropdown()
		{
			HashSet<string> hashSet = new HashSet<string>();
			hashSet.Add("json");
			hashSet.Add("jsonc");
			MapSelectDropdown._acceptableExtensions = hashSet;
		}

		// Token: 0x04000081 RID: 129
		private static HashSet<string> _acceptableExtensions;

		// Token: 0x04000083 RID: 131
		private DropDownBox _dropdown;

		// Token: 0x04000084 RID: 132
		private Dictionary<string, DateTime> _writeTimes = new Dictionary<string, DateTime>();

		// Token: 0x04000085 RID: 133
		private Dictionary<string, MapDef> _mapDefs = new Dictionary<string, MapDef>();

		// Token: 0x04000086 RID: 134
		private List<MapDef> _selectableMapDefs;

		// Token: 0x04000087 RID: 135
		private string _nameFilter;
	}
}
