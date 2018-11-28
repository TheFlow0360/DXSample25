using System;
using System.Windows;

namespace DXSample25
{
    public partial class MapView
    {
        public MapView()
        {
            InitializeComponent();
            Loaded += async (sender, args) =>
            {
                if (DataContext is MapViewModel viewModel)
                {
                    await viewModel.Init();
                }
            };
        }

        private void Map_OnLoaded(Object sender, RoutedEventArgs e)
        {
            if (DataContext is MapViewModel viewModel)
            {
                viewModel.ApplyCenterPoint(false);
            }
        }
    }
}
