using System;
using System.Windows;
using DevExpress.Map;
using DevExpress.Xpf.Map;

namespace DXSample25.Map
{
    public class MyVectorLayer : VectorLayer
    {
        static MyVectorLayer()
        {
            MapItem.IsHitTestVisibleProperty.OverrideMetadata(typeof(ShapeTitle), new PropertyMetadata(false));
            SelectedItemProperty.OverrideMetadata(typeof(MyVectorLayer), new PropertyMetadata(SelectedItemPropertyChangedCallback));
            SelectedItemsProperty.OverrideMetadata(typeof(MyVectorLayer), new PropertyMetadata(SelectedItemsPropertyChangedCallback));
        }

        private static void SelectedItemPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is MyVectorLayer instance)
            {
                instance.OnSelectedItemChangedInternal(e.NewValue);
            }
        }

        private static void SelectedItemsPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is MyVectorLayer instance)
            {
                instance.OnSelectedItemsChangedInternal(e.NewValue);
            }
        }

        public event EventHandler SelectedItemChanged;

        public event EventHandler SelectedItemsChanged;

        public CoordPoint SelectedItemCoords { get; set; }

        private void OnSelectedItemChangedInternal(Object newValue)
        {
            OnSelectedItemChanged(newValue);
            SelectedItemChanged?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnSelectedItemChanged(Object newValue)
        {
            if (newValue is MapPushpin item)
            {
                SelectedItemCoords = item.Location;
            }
        }

        private void OnSelectedItemsChangedInternal(Object newValue)
        {
            OnSelectedItemsChanged(newValue);
            SelectedItemsChanged?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnSelectedItemsChanged(object newValue)
        {
        }
    }
}