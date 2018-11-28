using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using DevExpress.Xpf.Map;

namespace DXSample25.Map
{
    public class PerimeterLayer : VectorLayer, IMapFocusable
    {
        #region Constructor and Initializiation

        public PerimeterLayer()
        {
            InitializeOptions();
            InitializeData();
        }

        private void InitializeOptions()
        {
            EnableHighlighting = false;
            EnableSelection = false;
            AllowResetSelection = false;
        }

        private void InitializeData()
        {
            Ellipse = new MapEllipse
            {
                Fill = TransparentBrush,
                IsHitTestVisible = false,
                Stroke = ColoredBrush,
                StrokeStyle = new StrokeStyle()
                {
                    Thickness = 4
                }
            };
            var dot = new MapDot()
            {
                Fill = CenterFillBrush,
                Stroke = TransparentBrush,
                Size = 20,
                IsHitTestVisible = false
            };
            CenterMarkers = new List<MapDot>();
            CenterMarkers.Add(dot);
            dot = new MapDot()
            {
                Fill = TransparentBrush,
                Stroke = CenterBorderBrush,
                Size = 32,
                StrokeStyle = new StrokeStyle() { Thickness = 5 },
                IsHitTestVisible = false
            };
            CenterMarkers.Add(dot);
            var storage = new MapItemStorage();
            storage.Items.Add(Ellipse);
            foreach (var marker in CenterMarkers)
            {
                storage.Items.Add(marker);
            }
            this.Data = storage;
        }

        #endregion

        #region Dependency Properties

        public static readonly DependencyProperty RadiusInKmProperty = DependencyProperty.Register(
            nameof(RadiusInKm),
            typeof(Double),
            typeof(PerimeterLayer),
            new PropertyMetadata(10.0, RadiusInKmPropertyChanged));

        private static void RadiusInKmPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var instance = d as PerimeterLayer;
            instance?.CalculateEllipse();
        }

        public Double RadiusInKm
        {
            get => (Double)GetValue(RadiusInKmProperty);
            set => SetValue(RadiusInKmProperty, value);
        }

        public static readonly DependencyProperty PositionProperty = DependencyProperty.Register(
            nameof(Position),
            typeof(GeoPoint),
            typeof(PerimeterLayer),
            new PropertyMetadata(new GeoPoint(52.1205333, 11.6276237), PositionPropertyChanged));

        private static void PositionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var instance = d as PerimeterLayer;
            instance?.CalculateEllipse();
        }

        public GeoPoint Position
        {
            get => (GeoPoint)GetValue(PositionProperty);
            set => SetValue(PositionProperty, value);
        }

        public static readonly DependencyProperty WithCenterPointProperty = DependencyProperty.Register(
            nameof(WithCenterPoint),
            typeof(Boolean),
            typeof(PerimeterLayer),
            new PropertyMetadata(true, WithCenterPointPropertyChangedCallback));

        private static void WithCenterPointPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is PerimeterLayer instance && e.NewValue is Boolean visible)
            {
                foreach (var marker in instance.CenterMarkers)
                {
                    marker.Visible = visible;
                }
            }
        }

        public Boolean WithCenterPoint { get; set; }

        public static readonly DependencyProperty FocusCommandProperty = DependencyProperty.Register(
            nameof(FocusCommand),
            typeof(MapFocusCommand),
            typeof(PerimeterLayer),
            new PropertyMetadata(null, FocusCommandPropertyChangedCallback));

        private static void FocusCommandPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is PerimeterLayer instance) || e.NewValue == null)
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

        private static SolidColorBrush TransparentBrush { get; } = new SolidColorBrush(Colors.Transparent);

        private static SolidColorBrush ColoredBrush { get; } = new SolidColorBrush(Color.FromArgb(0xFF, 0xAC, 0xAC, 0xAC));

        private static SolidColorBrush CenterFillBrush { get; } = new SolidColorBrush(Color.FromArgb(0xFF, 0xAA, 0x19, 0x48));

        private static SolidColorBrush CenterBorderBrush { get; } = new SolidColorBrush(Color.FromArgb(0xFF, 0xAA, 0x19, 0x48));

        private MapEllipse Ellipse { get; set; }

        private List<MapDot> CenterMarkers { get; set; }

        private void CalculateEllipse()
        {
            var diameter = RadiusInKm * 2.0;
            var geoSize = KilometersToGeoSize(Position, new Size(diameter, diameter));
            Ellipse.Location = new GeoPoint(
                Math.Min(90.0, Math.Max(-90.0, Position.Latitude + geoSize.Height / 2.0)),
                Math.Min(180.0, Math.Max(-180.0, Position.Longitude - geoSize.Width / 2.0)));
            foreach (var marker in CenterMarkers)
            {
                marker.Location = Position;
            }

            Ellipse.Width = diameter;
            Ellipse.Height = diameter;
        }

        #endregion

        #region IMapFocusable

        public void FocusCoordinates(Double latitude, Double longitude, Boolean withZoomAdjust = false)
        {
            Position = new GeoPoint(latitude, longitude);
        }

        #endregion
    }
}