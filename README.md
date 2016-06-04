# Unity-Touch
An extended Touch input module for Unity

### Install
1. Download and import the asset package located in this repository (or grab the c# files)
2. Select <i>Assets/import package/custom package...</i>
3. Select Touch.Unity
4. Drag/drop TouchHandler.cs to your EventSystem
5. It's good to go!

### Setup
**_TouchHandler component option breakdown_**

1. **Dont Destroy on Load (checkbox)**: Keep the event system when loading a new scene.
2. **Display Stats in Editor (checkbox)**: Show relevant data for all touches plus a relevant marker for the location of each touch.
3. **Tap Timeout (textbox)**: Amount of time to give the user between taps. If time is exceeded, tap will be registered as new touch.
4. **Long Press Time (slider)**: Amount of time user needs to press in one place to register a long press.
5. **Measure Units (dropdown)**: Centimeter is default. Automatically converts thresholds to correct unit based on pixel density of device.
This is to help ensure a consistent experience on a multitude of devices.
6. **SwipeThreshold (textbox)**: Movement speed required to consider a touch (drag) event as a swipe
7. **DragThreshold (textbox)**: Distance in MeasureUnits required for the touch to move in order to be considered a drag event. Don't set
this to 0 because there is always a small amount of movement at the beginning of every touch event.
8. **TouchSprite (spritebox)**: Sprite to use for touch events when _Display Stats In Editor_ is enabled.
9. **Simulate Touch with Mouse (checkbox)**: Use the mouse to simulate 1 finger touch events. Press Left+Ctrl while clicking to simulate
a basic pinch event.

### Touch Actions
- Tap: rapidly tap screen to add to the tap count
- Long Press: Press and hold in one place to activate
- Drag: Drag finger across screen
- Pinch: (2 fingers only) Move closer and further to change pinch radius
- Swipe: Drag finger across screen rapidly
- 2-Finger Swipe: Ditto, but with 2 fingers

### Accessing the data
**_Most of the data can be accessed through the Input.touches extension modules_**

#####Helpers
- **_float/int/vector2/vector3_.PixelsToTouchUnits()**: Converts # of pixels to selected Touch Measure Units (centimeter by default)
- **Input.touches.GetDoubleSwipeDirection()**: _Returns TouchHandler.directions_, the cardinal direction of the double swipe (type: TouchHandler.directions)

#####Pinch
- **Input.touches.SetPinchDelay(_float_)**: Set the Time that both touches must exceed before pinch begins
- **Input.touches.IsPinching()**: _Returns bool_, user is pinching or not
- **Input.touches.GetPinchDistance()**: _returns float_, change in distance since start of pinch (+ pinch bigger, - pinch smaller)
- **Input.touches.GetPinchDistanceDelta()**: _returns float_, change in distance since last change
- **Input.touches.GetPinchRatio()**: _returns float_, ratio of change in distance since start ( >1.0 pinch bigger, <1.0 pinch smaller)
- **Input.touches.GetPinchRatioDelta()**: _returns float_, ratio change in distance since last change
- **Input.touches.GePinchRayHit3D(_out RaycastHit, layersToIgnore_)**: _returns bool_, raycasts at center of pinch. Layers to Ignore is
comma delemited integers _(ex. Input.touches.GetPinchRayHit3d(out rHit, 8,9,10) will ignore all colliders on layers 8,9,10)_
- **Input.touches.GePinchRayHit2D(_out RaycastHit, layersToIgnore_)**: _returns bool_, raycasts at center of pinch. Layers to Ignore is
comma delemited integers _(ex. Input.touches.GetPinchRayHit2d(out rHit, 8,9,10) will ignore all colliders on layers 8,9,10)_
- **Input.touches.GetPinchPhase()**: _returns touchphase_, (began, moved, stationary, ended)

#####Touch Events
- **Input.touches.CheckRayHit3D()**: _returns TouchInstance[]_, causes all current touches to raycast3D. Each touch instance will store the
data in touchInstance.raycastHit.
- **Input.touches.CheckRayHit2D()**: _returns TouchInstance[]_, causes all current touches to raycast2D. Each touch instance will store the
data in touchInstance.raycastHit2D.
- **Input.touches[_int_].GetAction()**: _returns TouchHandler.actions_, for single touch event
- **Input.touches[_int_].GetTapCount()**: _returns int_, tap count for touch event
- **Input.touches[_int_].GetSwipeDirection()**: _returns TouchHandler.directions_, (left, right, up, down, none)
- **Input.touches[_int_].GetPressTime()**: _returns float_, get current press time of touch event
- **Input.touches.GetRaw()**: _returns TouchInstance[]_, access all extended public variables/methods of each touch event
- **Input.touches[_int_].GetRaw()**: _returns TouchInstance_, access all extended public variables/methods of single touch event
