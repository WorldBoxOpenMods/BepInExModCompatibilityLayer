# BepInExModCompatiblityLayer

BepInExModCompatibilityLayer is a library mod which allows different mods using the IMGUI system of Unity to create their primary UI toggle buttons in a way which ensures that buttons don't overlap with each other and also ads a way to ensure that the ID used for any given IMGUI window isn't identical to the ID of another window.

## Building

The project, as uploaded on this repo, cannot be built out of the box. It assumes that all of it's dependency DLLs like BepInEx and required Unity DLL files are located within a Libraries directory in the parent directory of BepInExModCompatiblityLayer.
To be able to build it, you need to either replicate this file structure or edit the .csproj file to contain dependency paths which work on your device.
