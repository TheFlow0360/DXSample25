using System;
using System.Windows;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Map;

namespace DXSample25.Map
{
    public class MyMapControl : MapControl, IMapFocusable
    {
        #region Constructor and Initializiation

        static MyMapControl()
        {
            ZoomLevelProperty.AddPropertyChangedCallback(typeof(MyMapControl), ZoomLevelPropertyChangedCallback);
        }

        public MyMapControl()
        {
            InitializeOptions();
            AddImageLayer();
        }

        private void InitializeOptions()
        {
            MinZoomLevel = 3;
            MaxZoomLevel = 18;
            CenterPoint = new GeoPoint(52.1205333, 11.6276237);
            ZoomLevel = 12;
            ShowSearchPanel = false;
            UseSprings = true;
            ZoomTrackbarOptions = new ZoomTrackbarOptions()
            {
                Orientation = MapZoomTrackbarOrientation.Horizontal,
                Margin = new Thickness(10),
                VerticalAlignment = NavigationElementVerticalAlignment.Bottom
            };
            ScrollButtonsOptions = new ScrollButtonsOptions()
            {
                Visible = false
            };
            ScalePanelOptions = new ScalePanelOptions()
            {
                ShowMilesScale = false,
                Margin = new Thickness(0, -30, 0, 5),
                VerticalAlignment = NavigationElementVerticalAlignment.Bottom
            };
            CoordinatesPanelOptions = new CoordinatesPanelOptions()
            {
                Visible = false
            };
            CacheOptions = new CacheOptions()
            {
                Directory = System.IO.Path.GetTempPath(),
                KeepInterval = TimeSpan.FromHours(1)
            };
        }

        private void AddImageLayer()
        {
            var imageLayer = new ImageLayer
            {
                DataProvider = new OpenStreetMapDataProvider
                {
                    Kind = OpenStreetMapKind.Basic
                }
            };
            Layers.Add(imageLayer);
        }

        #endregion

        #region Dependency Properties

        private static void ZoomLevelPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is MyMapControl instance)
            {
                instance.ZoomLevelChanged?.Invoke(instance, EventArgs.Empty);
            }
        }

        public static readonly DependencyProperty FocusCommandProperty = DependencyProperty.Register(
            nameof(FocusCommand),
            typeof(MapFocusCommand),
            typeof(MyMapControl),
            new PropertyMetadata(null, FocusCommandPropertyChangedCallback));

        private static void FocusCommandPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is MyMapControl instance) || e.NewValue == null)
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

        #region Events

        public event EventHandler ZoomLevelChanged;

        #endregion

        #region IMapFocusable

        public void FocusCoordinates(Double latitude, Double longitude, Boolean withZoomAdjust = false)
        {
            CenterPoint = new GeoPoint(latitude, longitude);
            if (withZoomAdjust)
            {
                ZoomToFitLayerItems(/*Layers.Where(layer => !(layer is PermanentToolTipLayer))*/);
            }
        }

        #endregion
    }
}