using UnityEngine;
using Newtonsoft.Json;

namespace BepinexModCompatibilityLayer.Classes {
  public class GuiLayoutWindow {
    private int _preferredWindowId;
    private GUI.WindowFunction _windowFunction;
    private Rect _windowRect;
    private string _windowTitle;
    private PermanentValues _properties;
    
    public int PreferredWindowId => _preferredWindowId;

    public string WindowTitle {
      get => _windowTitle;
      set => _windowTitle = value;
    }

    internal GuiLayoutWindow(int preferredWindowId, GUI.WindowFunction windowFunction, Rect windowRect, string windowTitle) {
      _properties = new PermanentValues {
        StartingWindowId = preferredWindowId
      };
      _preferredWindowId = preferredWindowId;
      _windowFunction = windowFunction;
      _windowRect = windowRect;
      _windowTitle = windowTitle;
    }
    
    internal string SaveWindow() {
      _properties.X = _windowRect.x;
      _properties.Y = _windowRect.y;
      _properties.Width = _windowRect.width;
      _properties.Height = _windowRect.height;
      _properties.WindowTitle = _windowTitle;
      return JsonConvert.SerializeObject(_properties);
    }
    
    internal void LoadWindow(string properties) {
      _properties = JsonConvert.DeserializeObject<PermanentValues>(properties);
      _preferredWindowId = _properties.StartingWindowId;
      _windowRect = new Rect(_properties.X, _properties.Y, _properties.Width, _properties.Height);
      _windowTitle = _properties.WindowTitle;
    }
    
    public void Draw() {
      GUILayout.Window(_preferredWindowId, _windowRect, _windowFunction, _windowTitle);
    }

    private class PermanentValues {
      internal int StartingWindowId;
      internal float X;
      internal float Y;
      internal float Width;
      internal float Height;
      internal string WindowTitle;
    }
  }
}