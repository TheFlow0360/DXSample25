using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace DXSample25.Map
{
    public class MapFocusCommand : ICommand
    {
        private readonly List<IMapFocusable> _components = new List<IMapFocusable>();

        public Boolean CanExecute(Object parameter = null)
        {
            return _components.Count > 0;
        }

        public void Execute(Object parameter = null)
        {
            if (!CanExecute())
            {
                return;
            }
            if (parameter is MapFocusCommandParams p)
            {
                _components.ForEach(focusable => focusable.FocusCoordinates(p.Latitude, p.Longitude, p.WithZoomAdjust));
            }
        }

        public event EventHandler CanExecuteChanged;

        public void AddComponent(IMapFocusable component)
        {
            _components.Add(component);
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        public void RemoveComponent(IMapFocusable component)
        {
            _components.Remove(component);
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public struct MapFocusCommandParams
    {
        public Double Latitude { get; set; }

        public Double Longitude { get; set; }

        public Boolean WithZoomAdjust { get; set; }
    }

    public interface IMapFocusable
    {
        void FocusCoordinates(Double latitude, Double longitude, Boolean withZoomAdjust = false);
    }
}