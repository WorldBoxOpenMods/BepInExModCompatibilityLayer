using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BepinexModCompatibilityLayer {
  /// <summary>
  /// The purpose of this class is to make sure that windows generated with the Unity method 
  /// GUILayout.Window do not have duplicate IDs.
  /// </summary>
  public class BepinexModCompatibilityLayerWindowIdSeparator {
    internal BepinexModCompatibilityLayerWindowIdSeparator() { }
    private static readonly List<(int id, GUI.WindowFunction signature)> WindowIds = new List<(int id, GUI.WindowFunction signature)>();

    /// <summary>
    /// This method adds a window ID to the list of window IDs that are already in use. You won't ever have to use 
    /// this since it's automatically called when creating a window.
    /// </summary>
    /// <param name="id">The Window ID that you want to add to the list.</param>
    /// <param name="signature">Your method for creating the GUI inside of the window. 
    /// The utility uses it as a way of connecting any given ID to it's owner</param>
    public void AddWindowId(int id, GUI.WindowFunction signature) {
      if (WindowIds.Any(t => t == (id, signature))) {
        return;
      }

      WindowIds.Add((id, signature));
    }

    /// <summary>
    /// This methods deletes a window ID from the list of window IDs that are already in use. 
    /// You can use this if you won't need a window ID again and want to free it up for other mods.
    /// </summary>
    /// <param name="id">The Window ID that you want to remove from the list.</param>
    /// <param name="signature">Your method for creating the GUI inside of the window. 
    /// The utility uses it as a way of connecting any given ID to it's owner</param>
    public void RemoveWindowId(int id, GUI.WindowFunction signature) {
      WindowIds.Remove((id, signature));
    }

    /// <summary>
    /// This method is for getting every Window ID that is currently in use.
    /// </summary>
    /// <returns>The return value is an array with every used Window ID, without the associated signatures.</returns>
    public int[] GetWindowIds() {
      int[] windowIds = new int[WindowIds.Count];
      for (int i = 0; i < WindowIds.Count; ++i) {
        windowIds[i] = WindowIds[i].id;
      }

      return windowIds;
    }

    /// <summary>
    /// This method is for getting a valid Window ID based on your preferred ID.
    /// </summary>
    /// <param name="preferredId">The ID that you preferably want to get in the utility.</param>
    /// <param name="signature">Your method for creating the GUI in the window. 
    /// The method needs it to know if the preferred ID belongs to you if it was used before.</param>
    /// <returns>The return value is the ID that the utility ended up finding for you.</returns>
    public int GetValidWindowId(int preferredId, GUI.WindowFunction signature) {
      while (true) {
        bool validIdFound = true;
        for (int i = 0; i < WindowIds.Count; ++i) {
          if (WindowIds[i].id == preferredId) {
            if (signature != null && WindowIds[i].signature != signature) {
              ++preferredId;
              validIdFound = false;
            }
          }
        }

        if (validIdFound) {
          return preferredId;
        }
      }
    }
  }
}