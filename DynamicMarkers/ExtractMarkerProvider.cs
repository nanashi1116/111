using System;
using System.Collections.Generic;
using System.Linq;
using Comfort.Common;
using DynamicMaps.Config;
using DynamicMaps.Data;
using DynamicMaps.UI.Components;
using DynamicMaps.Utils;
using EFT;
using EFT.Interactive;

namespace DynamicMaps.DynamicMarkers
{
	// Token: 0x02000034 RID: 52
	public class ExtractMarkerProvider : IDynamicMarkerProvider
	{
		// Token: 0x1700004F RID: 79
		// (get) Token: 0x06000209 RID: 521 RVA: 0x0000B68A File Offset: 0x0000988A
		// (set) Token: 0x0600020A RID: 522 RVA: 0x0000B694 File Offset: 0x00009894
		public bool ShowExtractStatusInRaid
		{
			get
			{
				return this._showExtractStatusInRaid;
			}
			set
			{
				if (this._showExtractStatusInRaid == value)
				{
					return;
				}
				this._showExtractStatusInRaid = value;
				foreach (ExfiltrationPoint exfiltrationPoint in this._extractMarkers.Keys)
				{
					this.UpdateExtractStatus(exfiltrationPoint, exfiltrationPoint.Status);
				}
			}
		}

		// Token: 0x0600020B RID: 523 RVA: 0x0000B704 File Offset: 0x00009904
		public void OnShowInRaid(MapView map)
		{
			if (this._extractMarkers.Count == 0)
			{
				this.AddExtractMarkers(map);
			}
			foreach (ExfiltrationPoint exfiltrationPoint in this._extractMarkers.Keys)
			{
				this.UpdateExtractStatus(exfiltrationPoint, exfiltrationPoint.Status);
				exfiltrationPoint.OnStatusChanged += this.UpdateExtractStatus;
			}
		}

		// Token: 0x0600020C RID: 524 RVA: 0x0000B788 File Offset: 0x00009988
		public void OnHideInRaid(MapView map)
		{
			foreach (ExfiltrationPoint exfiltrationPoint in this._extractMarkers.Keys)
			{
				exfiltrationPoint.OnStatusChanged -= this.UpdateExtractStatus;
			}
		}

		// Token: 0x0600020D RID: 525 RVA: 0x0000B7EC File Offset: 0x000099EC
		public void OnRaidEnd(MapView map)
		{
			this.TryRemoveMarkers();
		}

		// Token: 0x0600020E RID: 526 RVA: 0x0000B7F4 File Offset: 0x000099F4
		public void OnMapChanged(MapView map, MapDef mapDef)
		{
			foreach (ExfiltrationPoint extract in Enumerable.ToList<ExfiltrationPoint>(this._extractMarkers.Keys))
			{
				this.TryRemoveMarker(extract);
				this.TryAddMarker(map, extract);
			}
		}

		// Token: 0x0600020F RID: 527 RVA: 0x0000B85C File Offset: 0x00009A5C
		public void OnDisable(MapView map)
		{
			this.TryRemoveMarkers();
		}

		// Token: 0x06000210 RID: 528 RVA: 0x0000B864 File Offset: 0x00009A64
		private void AddExtractMarkers(MapView map)
		{
			GameWorld instance = Singleton<GameWorld>.Instance;
			Player player = GameUtils.GetMainPlayer();
			IEnumerable<ExfiltrationPoint> enumerable;
			if (GameUtils.IsScavRaid())
			{
				enumerable = Enumerable.Where<ScavExfiltrationPoint>(instance.ExfiltrationController.ScavExfiltrationPoints, (ScavExfiltrationPoint p) => p.isActiveAndEnabled && p.InfiltrationMatch(player));
			}
			else
			{
				enumerable = Enumerable.Where<ExfiltrationPoint>(instance.ExfiltrationController.ExfiltrationPoints, (ExfiltrationPoint p) => p.isActiveAndEnabled && p.InfiltrationMatch(player));
			}
			foreach (ExfiltrationPoint extract in enumerable)
			{
				this.TryAddMarker(map, extract);
			}
		}

		// Token: 0x06000211 RID: 529 RVA: 0x0000B90C File Offset: 0x00009B0C
		private void TryRemoveMarkers()
		{
			foreach (ExfiltrationPoint extract in Enumerable.ToList<ExfiltrationPoint>(this._extractMarkers.Keys))
			{
				this.TryRemoveMarker(extract);
			}
		}

		// Token: 0x06000212 RID: 530 RVA: 0x0000B96C File Offset: 0x00009B6C
		private void UpdateExtractStatus(ExfiltrationPoint extract, EExfiltrationStatus status)
		{
			if (!this._extractMarkers.ContainsKey(extract))
			{
				return;
			}
			MapMarker mapMarker = this._extractMarkers[extract];
			if (!this._showExtractStatusInRaid)
			{
				mapMarker.Color = Settings.ExtractDefaultColor.Value;
				return;
			}
			EExfiltrationStatus status2 = extract.Status;
			if (status2 == EExfiltrationStatus.NotPresent)
			{
				mapMarker.Color = Settings.ExtractClosedColor.Value;
				return;
			}
			if (status2 != EExfiltrationStatus.UncompleteRequirements)
			{
				mapMarker.Color = Settings.ExtractOpenColor.Value;
				return;
			}
			mapMarker.Color = Settings.ExtractHasRequirementsColor.Value;
		}

		// Token: 0x06000213 RID: 531 RVA: 0x0000B9F4 File Offset: 0x00009BF4
		private void TryAddMarker(MapView map, ExfiltrationPoint extract)
		{
			if (this._extractMarkers.ContainsKey(extract))
			{
				return;
			}
			MapMarkerDef markerDef = new MapMarkerDef
			{
				Category = "Extract",
				ImagePath = "Markers/exit.png",
				Text = extract.Settings.Name.BSGLocalized(),
				Position = MathUtils.ConvertToMapPosition(extract.transform)
			};
			MapMarker value = map.AddMapMarker(markerDef);
			this._extractMarkers[extract] = value;
			this.UpdateExtractStatus(extract, extract.Status);
		}

		// Token: 0x06000214 RID: 532 RVA: 0x0000BA78 File Offset: 0x00009C78
		private void TryRemoveMarker(ExfiltrationPoint extract)
		{
			if (!this._extractMarkers.ContainsKey(extract))
			{
				return;
			}
			extract.OnStatusChanged -= this.UpdateExtractStatus;
			this._extractMarkers[extract].ContainingMapView.RemoveMapMarker(this._extractMarkers[extract]);
			this._extractMarkers.Remove(extract);
		}

		// Token: 0x06000215 RID: 533 RVA: 0x0000BAD5 File Offset: 0x00009CD5
		public void OnShowOutOfRaid(MapView map)
		{
		}

		// Token: 0x06000216 RID: 534 RVA: 0x0000BAD7 File Offset: 0x00009CD7
		public void OnHideOutOfRaid(MapView map)
		{
		}

		// Token: 0x0400011B RID: 283
		private const string _extractCategory = "Extract";

		// Token: 0x0400011C RID: 284
		private const string _extractImagePath = "Markers/exit.png";

		// Token: 0x0400011D RID: 285
		private bool _showExtractStatusInRaid = true;

		// Token: 0x0400011E RID: 286
		private Dictionary<ExfiltrationPoint, MapMarker> _extractMarkers = new Dictionary<ExfiltrationPoint, MapMarker>();
	}
}
