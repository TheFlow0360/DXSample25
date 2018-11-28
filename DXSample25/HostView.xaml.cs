using System;
using System.Windows.Controls;
using System.Windows.Documents;

namespace DXSample25
{
    public partial class HostView
    {
        public HostView(IntPtr externalHandle)
        {
            InitializeComponent();
            if (externalHandle != IntPtr.Zero)
            {
                WindowContentArea.Content = new Win32Host(externalHandle);
            }
            else
            {
                WindowContentArea.Content = new TextBlock(new Run("Host Window must be opened first!"));
            }
        }
    }
}
