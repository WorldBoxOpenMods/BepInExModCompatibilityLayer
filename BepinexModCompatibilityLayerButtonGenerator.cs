using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BepinexModCompatibilityLayer {
  /// <summary>
  /// This class is used to avoid overlap between button positioned at a static location. 
  /// It's recommended to know how Tuples work for using this.
  /// </summary>
  public class BepinexModCompatibilityLayerButtonGenerator {
    internal BepinexModCompatibilityLayerButtonGenerator() {
      _ignoredRects = new List<Rect>();
    }

    private static readonly List<(Rect buttonFrame, int signature)> OccupiedLocations = new List<(Rect buttonFrame, int signature)>();
    internal List<Rect> IgnoredRects => _ignoredRects.ToList();
    private readonly List<Rect> _ignoredRects;
    private static int _signature;
    private static int _garbageCollectionCounter;

    internal void AddOccupiedLocation(Rect buttonFrame, int signature) {
      OccupiedLocations.Add((buttonFrame, signature));
    }

    /// <summary>
    /// This method returns a list of all occupied locations. It might be useful but you likely won't have to use it.
    /// </summary>
    /// <returns>The list containing all of the Rects and their associated IDs. 
    /// -1 as the signature means that Unity added the Rect.</returns>
    public IEnumerable<(Rect buttonFrame, int signature)> GetOccupiedLocations() {
      return OccupiedLocations.ToList();
    }

    /// <summary>
    /// Use this method instead of GUI.Button. It will automatically avoid overlap with other buttons.
    /// </summary>
    /// <param name="buttonFrame">Your preferred location.</param>
    /// <param name="text">The text that should be in the button.</param>
    /// <param name="signature">The signature from the last time where you generated a button. Pass along 0 if you don't have it.</param>
    /// <returns>The methods returns a Tuple containing the modified version of your Rect, the ID that has been assigned to your location, 
    /// and a bool stating whether or not the generated button has been pressed by the user.</returns>
    public (Rect modifiedFrame, int id, bool buttonClicked) CreateButton(Rect buttonFrame, string text, int signature) {
      Rect FailureModifier(Rect rect, Rect collidedRect) {
        rect.y += collidedRect.height;
        return rect;
      }

      return CreateButton(buttonFrame, text, signature, FailureModifier);
    }

    /// <summary>
    /// Use this method instead of GUI.Button. It will automatically avoid overlap with other buttons.
    /// </summary>
    /// <param name="buttonFrame">Your preferred location.</param>
    /// <param name="text">The text that should be in the button.</param>
    /// <param name="signature">The signature from the last time where you generated a button. Pass along 0 if you don't have it.</param>
    /// <param name="failureModifier">This method get executed every time where your suggested location isn't valid due to overlap. 
    /// It receives your Rect, and has to return a modified Rect.</param>
    /// <returns>The methods returns a Tuple containing the modified version of your Rect, the ID that has been assigned to your location, 
    /// and a bool stating whether or not the generated button has been pressed by the user.</returns>
    public (Rect modifiedFrame, int id, bool buttonClicked) CreateButton(Rect buttonFrame, string text, int signature, Func<Rect, Rect> failureModifier) {
      ++_garbageCollectionCounter;
      if (ShouldAddRectToOccupiedLocations(ref buttonFrame, signature, failureModifier)) {
        OccupiedLocations.Add((buttonFrame, signature == 0 ? ++_signature : signature));
      }

      _ignoredRects.Add(buttonFrame);
      bool buttonClicked = GUI.Button(new Rect(buttonFrame), text);
      _ignoredRects.Remove(buttonFrame);
      if (_garbageCollectionCounter % 6000 == 0) {
        _garbageCollectionCounter = 0;
        OccupiedLocations.RemoveAll(occupiedLocation => !occupiedLocation.buttonFrame.Overlaps(new Rect(0, 0, Screen.width, Screen.height)));
        GC.Collect();
      }

      return (buttonFrame, signature == 0 ? _signature : signature, buttonClicked);
    }

    /// <summary>
    /// Use this method instead of GUI.Button. It will automatically avoid overlap with other buttons.
    /// </summary>
    /// <param name="buttonFrame">Your preferred location.</param>
    /// <param name="text">The text that should be in the button.</param>
    /// <param name="signature">The signature from the last time where you generated a button. Pass along 0 if you don't have it.</param>
    /// <param name="failureModifier">This method get executed every time where your suggested location isn't valid due to overlap. 
    /// It receives your Rect, the Rect that your Rect collided with, and has to return your modified Rect.</param>
    /// <returns>The methods returns a Tuple containing the modified version of your Rect, the ID that has been assigned to your location, 
    /// and a bool stating whether or not the generated button has been pressed by the user.</returns>
    public (Rect modifiedFrame, int id, bool buttonClicked) CreateButton(Rect buttonFrame, string text, int signature, Func<Rect, Rect, Rect> failureModifier) {
      ++_garbageCollectionCounter;
      if (ShouldAddRectToOccupiedLocations(ref buttonFrame, signature, failureModifier)) {
        OccupiedLocations.Add((buttonFrame, ++_signature));
      }

      _ignoredRects.Add(buttonFrame);
      bool buttonClicked = GUI.Button(new Rect(buttonFrame), text);
      _ignoredRects.Remove(buttonFrame);
      if (_garbageCollectionCounter % 6000 == 0) {
        _garbageCollectionCounter = 0;
        OccupiedLocations.RemoveAll(occupiedLocation => !occupiedLocation.buttonFrame.Overlaps(new Rect(0, 0, Screen.width, Screen.height)));
        GC.Collect();
      }

      return (buttonFrame, signature == 0 ? _signature : signature, buttonClicked);
    }

    private static bool ShouldAddRectToOccupiedLocations(ref Rect buttonFrame, int signature, Func<Rect, Rect> failureModifier) {
      Rect buttonFrameValue;
      bool validLocationFound;
      bool shouldAddRect = true;
      do {
        buttonFrameValue = new Rect(buttonFrame);
        validLocationFound = true;
        foreach ((Rect buttonFrame, int signature) signedLocation in OccupiedLocations.Where(signedLocation => buttonFrameValue.Overlaps(signedLocation.buttonFrame))) {
          if (signedLocation.signature != signature) {
            buttonFrame = failureModifier(buttonFrame);
            validLocationFound = false;
          } else {
            shouldAddRect = false;
          }
        }
      } while (!validLocationFound);

      return shouldAddRect;
    }

    private static bool ShouldAddRectToOccupiedLocations(ref Rect buttonFrame, int signature, Func<Rect, Rect, Rect> failureModifier) {
      Rect buttonFrameValue;
      bool validLocationFound;
      bool shouldAddRect = true;
      do {
        buttonFrameValue = new Rect(buttonFrame);
        validLocationFound = true;
        foreach ((Rect buttonFrame, int signature) signedLocation in OccupiedLocations.Where(signedLocation => buttonFrameValue.Overlaps(signedLocation.buttonFrame))) {
          if (signedLocation.signature != signature) {
            buttonFrame = failureModifier(buttonFrame, signedLocation.buttonFrame);
            validLocationFound = false;
          } else {
            shouldAddRect = false;
          }
        }
      } while (!validLocationFound);

      return shouldAddRect;
    }
  }
}