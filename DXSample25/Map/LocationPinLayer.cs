using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using DevExpress.Data.Selection;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Map;

namespace DXSample25.Map
{
    public class LocationPinLayer : MyVectorLayer, IMapFocusable
    {
        #region Constructor and Initializiation

        public LocationPinLayer()
        {
            InitializeOptions();
            InitializeData();
        }

        private void InitializeOptions()
        {
            EnableHighlighting = false;
            EnableSelection = true;
            ToolTipEnabled = true;
        }

        private void InitializeData()
        {
            SelectedLocations = new List<MapLocation>();
            DataAdapterInternal = CreateDataAdapter();
            Data = DataAdapterInternal;
        }

        protected virtual DataSourceAdapterBase CreateDataAdapter()
        {
            return new ListSourceDataAdapter
            {
                Mappings = new MapItemMappingInfo()
                {
                    Latitude = nameof(MapLocation.Latitude),
                    Longitude = nameof(MapLocation.Longitude)
                },
                ItemSettings = new MapPushpinSettings(),
                Clusterer = new DistanceBasedClusterer(),
                DataSource = DataSource
            };
        }

        #endregion

        #region Dependency Properties

        public static readonly DependencyProperty DataSourceProperty = DependencyProperty.Register(
            nameof(DataSource),
            typeof(Object),
            typeof(LocationPinLayer),
            new PropertyMetadata(null, DataSourcePropertyChangedCallback));

        private static void DataSourcePropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is LocationPinLayer instance && instance.DataAdapterInternal != null)
            {
                instance.DataAdapterInternal.DataSource = e.NewValue;
            }
        }

        public Object DataSource
        {
            get => GetValue(DataSourceProperty);
            set => SetValue(DataSourceProperty, value);
        }

        public static readonly DependencyProperty SelectedLocationProperty = DependencyProperty.Register(
            nameof(SelectedLocation),
            typeof(MapLocation),
            typeof(LocationPinLayer),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, SelectedLocationPropertyChangedCallback));

        private static void SelectedLocationPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is LocationPinLayer instance)
            {
                instance.OnSelectedLocationChanged(e.NewValue);
            }
        }

        public MapLocation SelectedLocation
        {
            get => (MapLocation)GetValue(SelectedLocationProperty);
            set => SetValue(SelectedLocationProperty, value);
        }

        public static readonly DependencyProperty SelectedLocationsProperty = DependencyProperty.Register(
            nameof(SelectedLocations),
            typeof(IList<MapLocation>),
            typeof(LocationPinLayer),
            new PropertyMetadata(null)
            );

        public IList<MapLocation> SelectedLocations
        {
            get => (IList<MapLocation>)GetValue(SelectedLocationsProperty);
            set => SetValue(SelectedLocationsProperty, value);
        }

        public static readonly DependencyProperty SelectMatchingLocationOnFocusProperty = DependencyProperty.Register(
            nameof(SelectMatchingLocationOnFocus),
            typeof(Boolean),
            typeof(LocationPinLayer),
            new PropertyMetadata(true));

        public Boolean SelectMatchingLocationOnFocus
        {
            get => (Boolean)GetValue(SelectMatchingLocationOnFocusProperty);
            set => SetValue(SelectMatchingLocationOnFocusProperty, value);
        }

        public static readonly DependencyProperty FocusCommandProperty = DependencyProperty.Register(
            nameof(FocusCommand),
            typeof(MapFocusCommand),
            typeof(LocationPinLayer),
            new PropertyMetadata(null, FocusCommandPropertyChangedCallback));

        private static void FocusCommandPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is LocationPinLayer instance) || e.NewValue == null)
            {
                return;
            }
            ((MapFocusCommand)e.OldValue)?.RemoveComponent(instance);
            ((MapFocusCommand)e.NewValue).AddComponent(instance);
        }

        public MapFocusCommand FocusCommand
        {
            get => (MapFocusCommand)GetValue(FocusCommandProperty);
            set => SetValue(FocusCommandProperty, value);
        }

        #endregion

        #region internals

        private DataSourceAdapterBase DataAdapterInternal { get; set; }

        private Boolean SelectionChanging { get; set; }

        private MapLocation GetLocationFromItem(MapItem item)
        {
            return DataAdapterInternal.GetItemSourceObject(item) as MapLocation;
        }

        protected override void OnSelectedItemChanged(Object newItem)
        {
            base.OnSelectedItemChanged(newItem);
            if (SelectionChanging)
            {
                return;
            }
            SelectionChanging = true;
            try
            {
                switch (newItem)
                {
                    case null:
                        SelectedLocation = null;
                        SelectedLocations = new List<MapLocation>();
                        break;
                    case MapLocation location:
                        SelectedLocation = location;
                        var list = new List<MapLocation>()
                                    {
                                        location
                                    };
                        SelectedLocations = list;
                        SelectedItemCoords = new GeoPoint(location.Latitude, location.Longitude);
                        break;
                    case MapItem clusterItem:
                        var locations = clusterItem.ClusteredItems;
                        if (locations != null && locations.Count > 0)
                        {
                            if (locations.All(item => GetLocationFromItem(item) != SelectedLocation))
                            {
                                // wenn keiner der im Cluster enthaltenen Pins der vorher selektierten Location entspricht, hebe die Selektion auf
                                SelectedLocation = null;
                            }
                            list = new List<MapLocation>();
                            locations.ForEach(location => list.Add(GetLocationFromItem(location)));
                            SelectedLocations = list;
                        }
                        break;
                }
            }
            finally
            {
                SelectionChanging = false;
            }
        }

        private void OnSelectedLocationChanged(Object newLocationObj)
        {
            if (SelectionChanging || !(newLocationObj is MapLocation newLocation))
            {
                return;
            }
            SelectMatchingLocation(IsSameLocation);

            Boolean IsSameLocation(MapLocation location)
            {
                return location == newLocation;
            }
        }

        private void SelectMatchingLocation(Predicate<MapLocation> predicate)
        {
            if (DataAdapterInternal.Clusterer?.Items != null &&
                DataAdapterInternal.Clusterer.Items.Where(displayItem => displayItem.ClusteredItems != null && displayItem.ClusteredItems.Count > 0)
                    .Any(displayItem => displayItem.ClusteredItems.Any(item => SelectOnMatch(item, displayItem))))
            {
                return;
            }
            if (DataAdapterInternal.DisplayItems.Any(displayItem => SelectOnMatch(displayItem, displayItem)))
            {
                return;
            }

            Boolean SelectOnMatch(MapItem locationItem, MapItem displayItem)
            {
                var location = GetLocationFromItem(locationItem);
                if (!predicate(location))
                {
                    return false;
                }
                SelectionController.UpdateItemSelection(ElementSelectionMode.Single, ModifierKeys.None, displayItem);
                SelectedItem = Equals(locationItem, displayItem) ? (Object)location : (Object)displayItem;
                return true;
            }
        }

        #endregion

        #region IMapFocusable

        public void FocusCoordinates(Double latitude, Double longitude, Boolean withZoomAdjust = false)
        {
            if (!SelectMatchingLocationOnFocus)
            {
                return;
            }
            SelectMatchingLocation(IsSameCoordinates);

            Boolean IsSameCoordinates(MapLocation location)
            {
                return Math.Abs(location.Latitude - latitude) < (Double.Epsilon * 10) && Math.Abs(location.Longitude - longitude) < (Double.Epsilon * 10);
            }
        }

        #endregion
    }
}