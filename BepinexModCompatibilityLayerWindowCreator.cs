using BepinexModCompatibilityLayer.Classes;
using UnityEngine;

namespace BepinexModCompatibilityLayer {
  public class BepinexModCompatibilityLayerWindowCreator {
    public GuiLayoutWindow CreateWindow(int preferredWindowId, GUI.WindowFunction windowFunction, Rect windowRect, string windowTitle) {
      return new GuiLayoutWindow(preferredWindowId, windowFunction, windowRect, windowTitle);
    }
  }
}