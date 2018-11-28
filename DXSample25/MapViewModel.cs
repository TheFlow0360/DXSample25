using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DevExpress.Mvvm;
using DevExpress.Xpf.Map;
using DXSample25.Map;

namespace DXSample25
{
    public class MapViewModel : ViewModelBase
    {
        public MapViewModel()
        {
            CenterPoint = new GeoPoint(52.1205333, 11.6276237);
            PerimeterRadiusInKilometer = 10;
            
            Task.Delay(TimeSpan.FromMilliseconds(100)).Wait();
            CenterPoint = new GeoPoint(52.2205333, 11.5276237);
            ZoomLevel = 11;
        }

        public async Task Init()
        {
            CenterPoint = new GeoPoint(52.1205333, 11.6276237);
            await UpdateLocations();
            ApplyCenterPoint(true);
        }

        private GeoPoint CenterPoint
        {
            get => GetProperty(() => CenterPoint);
            set => SetProperty(() => CenterPoint, value);
        }

        public Double ZoomLevel
        {
            get => GetProperty(() => ZoomLevel);
            set => SetProperty(() => ZoomLevel, value);
        }

        public Double PerimeterRadiusInKilometer
        {
            get => GetProperty(() => PerimeterRadiusInKilometer);
            set => SetProperty(() => PerimeterRadiusInKilometer, value);
        }

        private BulkObservableCollection<MapLocation> _locations;
        public BulkObservableCollection<MapLocation> Locations => _locations ?? (_locations = new BulkObservableCollection<MapLocation>());

        private MapFocusCommand _focusCommand;
        public MapFocusCommand FocusCommand => _focusCommand ?? (_focusCommand = new MapFocusCommand());

        private MapFocusCommand _centerCommand;
        public MapFocusCommand CenterCommand => _centerCommand ?? (_centerCommand = new MapFocusCommand());

        public void ApplyCenterPoint(Boolean withZoomAdjust)
        {
            var options = new MapFocusCommandParams()
            {
                Latitude = CenterPoint.Latitude,
                Longitude = CenterPoint.Longitude,
                WithZoomAdjust = withZoomAdjust
            };
            CenterCommand.Execute(options);
            FocusCommand.Execute(options);
        }

        public void MoveFocusToPosition(Double latitude, Double longitude)
        {
            var options = new MapFocusCommandParams()
            {
                Latitude = latitude,
                Longitude = longitude
            };
            FocusCommand.Execute(options);
        }

        private async Task UpdateLocations()
        {
            var list = await Task.Factory.StartNew(
                () =>
                {
                    Task.Delay(TimeSpan.FromSeconds(3)).Wait();
                    var locations = new List<MapLocation>()
                    {
                        new MapLocation() {Latitude = 52.156, Longitude = 11.5789},
                        new MapLocation() {Latitude = 52.1454, Longitude = 11.7365},
                        new MapLocation() {Latitude = 52.520008, Longitude = 13.404954},
                        new MapLocation() {Latitude = 51.8883, Longitude = 11.0572},
                        new MapLocation() {Latitude = 52.0667, Longitude = 11.8333},
                        new MapLocation() {Latitude = 52.2667, Longitude = 11.8631}
                    };
                    
                    return locations;
                });
            Locations.Clear();
            Locations.AddRange(list);
        }
    }
}