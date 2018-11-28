using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Docking;
using DevExpress.Xpf.Docking.Base;

namespace DXSample25
{
    public partial class MainWindow
    {
        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        public MainWindow()
        {
            InitializeComponent();
        }

        private IntPtr ExternalHandle { get; set; } = IntPtr.Zero;

        private DocumentPanel HostView { get; set; }

        private DocumentPanel MapView { get; set; }

        private void OpenExternalView_OnItemClick(Object sender, ItemClickEventArgs e)
        {
            var filename = "DXSampleFile.txt";
            File.WriteAllText(filename, "This is the hosted Editor window displaying the test file.");
            var process = Process.Start(filename);
            Task.Delay(100).Wait();
            ExternalHandle = process?.MainWindowHandle ?? IntPtr.Zero;
        }

        private void OpenHostView_OnItemClick(Object sender, ItemClickEventArgs e)
        {
            if (HostView == null)
            {
                var view = new HostView(ExternalHandle);
                HostView = new DocumentPanel();
                HostView.Caption = "Host View";
                HostView.Content = view;
                DocumentGroup.Add(HostView);
            }
            DockLayoutManager.Activate(HostView);
        }

        private void OpenMapView_OnItemClick(Object sender, ItemClickEventArgs e)
        {
            if (MapView == null)
            {
                var viewModel = new MapViewModel();
                var view = new MapView();
                view.DataContext = viewModel;
                MapView = new DocumentPanel();
                MapView.Caption = "Map View";
                MapView.Content = view;
                DocumentGroup.Add(MapView);
            }
            DockLayoutManager.Activate(MapView);
        }

        private void MainWindow_OnClosed(Object sender, EventArgs e)
        {
            if (HostView != null)
            {
                HostView.Closed = true;
            }
        }

        private void DockLayoutManager_OnDockItemClosed(Object sender, DockItemClosedEventArgs e)
        {
            foreach (var item in e.AffectedItems)
            {
                if (item == HostView)
                {
                    HostView = null;
                } 
                else if (item == MapView)
                {
                    MapView = null;
                }
            }
        }
    }
}
