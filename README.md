# Life_Under_Water
Repository for MED801 semester project.

## How to use
When initally setting up this project in a new environment, it needs to be calibrated. run the Depth View scene. Hit play, followed by spacebar. This should remove the white canvas, and you should see a camera feed from the Kinect, with two boxes. In the PR_MeasureDepth object, adjust the Top, Bottom, Left and Right cutoffs, hitting spacebar after each change to apply them. This will change the "play area" in which the system measures depth data.

Set the Depth Sensitivity to 1, and adjust the wall depth until it is a value just below where more markers appear in the area. 

Adjust the DepthSensitivity, to give a buffer area where objects will be detected above the floor.

## Development Notes
Based on Andrew Connell's Tutorial available here: https://www.youtube.com/watch?reload=9&v=6EkQA3GakFI

Install Kinect SDK and Download Kinect Unity Pro Packages from: https://developer.microsoft.com/en-us/windows/kinect

import the Kinect.versionNo.unitypackage to the Unity Project.