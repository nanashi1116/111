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
using EFT.Interactive.SecretExfiltrations;

namespace DynamicMaps.DynamicMarkers
{
	// Token: 0x0200003A RID: 58
	public class SecretMarkerProvider : IDynamicMarkerProvider
	{
		// Token: 0x17000054 RID: 84
		// (get) Token: 0x06000259 RID: 601 RVA: 0x0000CDAD File Offset: 0x0000AFAD
		// (set) Token: 0x0600025A RID: 602 RVA: 0x0000CDB8 File Offset: 0x0000AFB8
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
				foreach (SecretExfiltrationPoint secretExfiltrationPoint in this._secretMarkers.Keys)
				{
					this.UpdateSecretExtractStatus(secretExfiltrationPoint, secretExfiltrationPoint.Status);
				}
			}
		}

		// Token: 0x0600025B RID: 603 RVA: 0x0000CE28 File Offset: 0x0000B028
		public void OnShowInRaid(MapView map)
		{
			if (this._secretMarkers.Count == 0)
			{
				this.AddSecretMarkers(map);
			}
			foreach (SecretExfiltrationPoint secretExfiltrationPoint in this._secretMarkers.Keys)
			{
				this.UpdateSecretExtractStatus(secretExfiltrationPoint, secretExfiltrationPoint.Status);
				secretExfiltrationPoint.OnStatusChanged += this.UpdateSecretExtractStatus;
			}
		}

		// Token: 0x0600025C RID: 604 RVA: 0x0000CEAC File Offset: 0x0000B0AC
		public void OnHideInRaid(MapView map)
		{
			foreach (SecretExfiltrationPoint secretExfiltrationPoint in this._secretMarkers.Keys)
			{
				secretExfiltrationPoint.OnStatusChanged -= this.UpdateSecretExtractStatus;
			}
		}

		// Token: 0x0600025D RID: 605 RVA: 0x0000CF10 File Offset: 0x0000B110
		public void OnRaidEnd(MapView map)
		{
			this.TryRemoveMarkers();
		}

		// Token: 0x0600025E RID: 606 RVA: 0x0000CF18 File Offset: 0x0000B118
		public void OnMapChanged(MapView map, MapDef mapDef)
		{
			foreach (SecretExfiltrationPoint secretExfiltrationPoint in Enumerable.ToList<SecretExfiltrationPoint>(this._secretMarkers.Keys))
			{
				this.TryRemoveMarker(secretExfiltrationPoint);
				this.TryAddMarker(map, secretExfiltrationPoint);
			}
		}

		// Token: 0x0600025F RID: 607 RVA: 0x0000CF80 File Offset: 0x0000B180
		public void OnDisable(MapView map)
		{
			this.TryRemoveMarkers();
		}

		// Token: 0x06000260 RID: 608 RVA: 0x0000CF88 File Offset: 0x0000B188
		public void RefreshMarkers(MapView map)
		{
			foreach (SecretExfiltrationPoint secretExfiltrationPoint in Enumerable.ToList<SecretExfiltrationPoint>(this._secretMarkers.Keys))
			{
				this.TryRemoveMarker(secretExfiltrationPoint);
				this.TryAddMarker(map, secretExfiltrationPoint);
			}
		}

		// Token: 0x06000261 RID: 609 RVA: 0x0000CFF0 File Offset: 0x0000B1F0
		private void AddSecretMarkers(MapView map)
		{
			GameWorld instance = Singleton<GameWorld>.Instance;
			GameUtils.GetMainPlayer();
			foreach (SecretExfiltrationPoint point in ((IEnumerable<SecretExfiltrationPoint>)instance.ExfiltrationController.SecretExfiltrationPoints))
			{
				this.TryAddMarker(map, point);
			}
		}

		// Token: 0x06000262 RID: 610 RVA: 0x0000D050 File Offset: 0x0000B250
		private void TryAddMarker(MapView map, SecretExfiltrationPoint point)
		{
			if (this._secretMarkers.ContainsKey(point))
			{
				return;
			}
			MapMarkerDef markerDef = new MapMarkerDef
			{
				Category = "Secret",
				ImagePath = "Markers/exit.png",
				Text = point.Settings.Name.BSGLocalized(),
				Position = MathUtils.ConvertToMapPosition(point.transform),
				Color = Settings.SecretPointColor.Value
			};
			MapMarker value = map.AddMapMarker(markerDef);
			this._secretMarkers[point] = value;
			this.UpdateSecretExtractStatus(point, point.Status);
		}

		// Token: 0x06000263 RID: 611 RVA: 0x0000D0E4 File Offset: 0x0000B2E4
		private void TryRemoveMarkers()
		{
			foreach (SecretExfiltrationPoint secret in Enumerable.ToList<SecretExfiltrationPoint>(this._secretMarkers.Keys))
			{
				this.TryRemoveMarker(secret);
			}
		}

		// Token: 0x06000264 RID: 612 RVA: 0x0000D144 File Offset: 0x0000B344
		private void UpdateSecretExtractStatus(ExfiltrationPoint point, EExfiltrationStatus status)
		{
			if (!this._secretMarkers.ContainsKey(point as SecretExfiltrationPoint))
			{
				return;
			}
			MapMarker mapMarker = this._secretMarkers[point as SecretExfiltrationPoint];
			if (!this._showExtractStatusInRaid)
			{
				mapMarker.Color = Settings.SecretPointColor.Value;
				return;
			}
			EExfiltrationStatus status2 = point.Status;
			if (status2 == EExfiltrationStatus.UncompleteRequirements)
			{
				mapMarker.Color = Settings.ExtractHasRequirementsColor.Value;
				return;
			}
			if (status2 == EExfiltrationStatus.Countdown)
			{
				mapMarker.Color = Settings.ExtractOpenColor.Value;
				return;
			}
			if (status2 == EExfiltrationStatus.Hidden)
			{
				mapMarker.Color = Settings.SecretPointColor.Value;
				return;
			}
			mapMarker.Color = Settings.SecretPointColor.Value;
		}

		// Token: 0x06000265 RID: 613 RVA: 0x0000D1E8 File Offset: 0x0000B3E8
		private void TryRemoveMarker(SecretExfiltrationPoint secret)
		{
			if (!this._secretMarkers.ContainsKey(secret))
			{
				return;
			}
			secret.OnStatusChanged -= this.UpdateSecretExtractStatus;
			this._secretMarkers[secret].ContainingMapView.RemoveMapMarker(this._secretMarkers[secret]);
			this._secretMarkers.Remove(secret);
		}

		// Token: 0x06000266 RID: 614 RVA: 0x0000D245 File Offset: 0x0000B445
		public void OnShowOutOfRaid(MapView map)
		{
		}

		// Token: 0x06000267 RID: 615 RVA: 0x0000D247 File Offset: 0x0000B447
		public void OnHideOutOfRaid(MapView map)
		{
		}

		// Token: 0x04000137 RID: 311
		private const string SecretCategory = "Secret";

		// Token: 0x04000138 RID: 312
		private const string SecretImagePath = "Markers/exit.png";

		// Token: 0x04000139 RID: 313
		private bool _showExtractStatusInRaid = true;

		// Token: 0x0400013A RID: 314
		private Dictionary<SecretExfiltrationPoint, MapMarker> _secretMarkers = new Dictionary<SecretExfiltrationPoint, MapMarker>();
	}
}
