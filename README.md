# Touch Simulator For Unity
Simulate multi-touch using mouse in Unity Editor. `TouchSimulator` allows mouse left click and right click to send Vector2 data needed to simulate a pinch input in a smarthphone screen.

![Alt Text](TouchSimulator.gif)

## Dependencies
This script is using Unity's InputSystem package.

## How to Use
1. Import `TouchSimulator.cs` to your Unity project.
2. Create a `Canvas` with its `RenderMode` to `Screen Space - Overlay`
3. The component `Canvas Scaler`'s `UI Scale Mode` to `Scale With Screen Size`
4. Add `TouchSimulator` to the canvas.
5. As the child of the canvas, create an empty gameObject, and name it "Touch". As the children of Touch, make two empty gameObjects, and name them "Primary" and "Secondary". 

        Canvas
        └ Touch
            └ Primary
            └ Secondary
    You may want to add `Image` component to see them.

6. In `TouchSimulator`, assign Touch to `Touch` field, Primary to `Pinch Primary`, and Secondary to `Pinch Secondary`.
7. Subscribe to any of `TouchSimulator`'s delegates:
    - `OnTapInput`
    - `OnTapSecondInput`
    - `OnDeltaInput`
    - `OnPositionInput`
    - `OnPinchInput`

For more reference, check the example `CameraController.cs` which is used to control the camera movement in 2D game.

For more reference on how to actually implement pinch input in Unity, check this article:
https://www.arcaneshift.com/blog/2023/06/19/pinch-and-scroll-with-unitys-new-input-system/
