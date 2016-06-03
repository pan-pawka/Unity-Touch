using UnityEngine;
using System.Collections;


public static class TouchExtension
{

	/* ------------ HELPER METHODS ------------ */

	/// <summary>
	/// Check if Touch is finished
	/// </summary>
	/// <returns><c>true</c> if TouchPhase = Ended || Canceled; otherwise, <c>false</c>.</returns>
	/// <param name="tPhase">TouchPhase</param>
	public static bool IsDone(this TouchPhase tPhase)
	{
		return (tPhase == TouchPhase.Ended || tPhase == TouchPhase.Canceled);
	}

	/// <summary>
	/// Pixels to touch units.
	/// </summary>
	/// <returns><c>Float</c> pixels converted to TouchHandler.MeasureUnits</returns>
	/// <param name="pixels">[int]Pixels</param>
	public static float PixelsToTouchUnits(this int pixels){
		return pixels / TouchHandler.PixelsPerUnit();
	}

	/// <summary>
	/// Pixels to touch units.
	/// </summary>
	/// <returns><c>Float</c> pixels converted to TouchHandler.MeasureUnits</returns>
	/// <param name="pixels">[float]Pixels</param>
	public static float PixelsToTouchUnits(this float pixels){
		return pixels / TouchHandler.PixelsPerUnit();
	}

	/// <summary>
	/// Pixels to touch units.
	/// </summary>
	/// <returns><c>Float</c> pixels converted to TouchHandler.MeasureUnits</returns>
	/// <param name="pixels">[Vector2]Pixels</param>
	public static Vector2 PixelsToTouchUnits(this Vector2 pixels){
		return pixels / TouchHandler.PixelsPerUnit();
	}

	/// <summary>
	/// Pixels to touch units.
	/// </summary>
	/// <returns><c>Float</c> pixels converted to TouchHandler.MeasureUnits</returns>
	/// <param name="pixels">[Vector3]Pixels</param>
	public static Vector3 PixelsToTouchUnits(this Vector3 pixels){
		return pixels / TouchHandler.PixelsPerUnit();
	}

	/// <summary>
	/// <para>Returns direction of two-fingered swipe</para>
	/// <para> </para>
	/// <para>Default Return: TouchHandler.directions.None</para>
	/// </summary>
	/// <returns>The double swipe direction.</returns>
	/// <param name="touches">Input.touches.DoubleSwipeDirection()</param>
	public static TouchHandler.directions GetDoubleSwipeDirection(this Touch[] touches){
		return TouchHandler.DoubleSwipe.Direction();
	}


	/* ------------ PINCH METHODS ------------ */

	/// <summary>
	/// Set delay before pinch becomes active.
	/// </summary>
	/// <param name="pinchDelay">Time that both touches must exceed before pinch begins</param>
	public static void SetPinchDelay(this Touch[] touches, float pinchDelay = 0.2f){
		TouchHandler.Pinch.pinchDelay = pinchDelay;
	}

	/// <summary>
	/// Determines if user is pinching.
	/// </summary>
	/// <returns><c>true</c> if is active the specified pinchDelay; otherwise, <c>false</c>.</returns>
	/// <param name="pinchDelay">time to allow the user to perform some other action</param>
	public static bool IsPinching(this Touch[] touches){
		return TouchHandler.Pinch.active;
	}

	/// <summary>
	/// <para>Gets Distance of pinch in pixels.</para>
	/// <para><c>Positive: </c>Touches moving away from each other.</para>
	/// <para><c>Negative: </c>Touches moving toward each other.</para>
	/// </summary>
	/// <returns>(float) distance in pixels</returns>
	public static float GetPinchDistance(this Touch[] touches){
		return TouchHandler.Pinch.distance;
	}

	/// <summary>
	/// <para>Gets Distance of pinch in pixels since last update.</para>
	/// <para><c>Positive: </c>Touches moving away from each other.</para>
	/// <para><c>Negative: </c>Touches moving toward each other.</para>
	/// </summary>
	/// <returns>(float) distance in pixels</returns>
	public static float GetPinchDistanceDelta(this Touch[] touches){
		return TouchHandler.Pinch.deltaDistance;
	}

	/// <summary>
	/// <para>Gets % that pinch has changed since start.</para>
	/// <para> </para>
	/// <para><c>Less than 1: </c>Touches moving toward each other.</para>
	/// <para><c>Greater than 1: </c>Touches moving away from each other.</para>
	/// </summary>
	/// <returns>(float) percentage pinch has changed since start</returns>
	public static float GetPinchRatio(this Touch[] touches){
		return TouchHandler.Pinch.ratio;
	}

	/// <summary>
	/// <para>Gets % that pinch has changed since last update.</para>
	/// <para> </para>
	/// <para><c>Less than 1: </c>Touches moving toward each other.</para>
	/// <para><c>Greater than 1: </c>Touches moving away from each other.</para>
	/// </summary>
	/// <returns>(float) percentage pinch has changed since start</returns>
	public static float GetPinchRatioDelta(this Touch[] touches){
		return TouchHandler.Pinch.deltaRatio;
	}

	/// <summary>
	/// Raycasts at the center of a pinch.
	/// </summary>
	/// <returns><c>true</c>, if pinch center hits collider, <c>false</c> otherwise.</returns>
	/// <param name="touches">Input.Touches.GetPinchRayHit3D(out hit)</param>
	/// <param name="layersToIgnore">comma separated (or integer array) layer #'s to ignore</param> 
	public static bool GetPinchRayHit3D(this Touch[] touches, out RaycastHit hit, params int[] layersToIgnore){
		return TouchHandler.Pinch.Raycast3D(out hit, layersToIgnore);
	}

	/// <summary>
	/// Raycasts at the center of a pinch.
	/// </summary>
	/// <returns><c>true</c>, if pinch center hits collider, <c>false</c> otherwise.</returns>
	/// <param name="touches">Input.Touches.GetPinchRayHit3D(out hit)</param>
	/// <param name="layersToIgnore">comma separated (or integer array) layer #'s to ignore</param> 
	public static bool GetPinchRayHit2D(this Touch[] touches, out RaycastHit2D hit, params int[] layersToIgnore){
		return TouchHandler.Pinch.Raycast2D(out hit, layersToIgnore);
	}

	/// <summary>
	/// Gets the pinch phase.
	/// </summary>
	/// <returns>The pinch phase.</returns>
	/// <param name="touches">Input.touches.GetPinchPhase()</param>;
	public static TouchPhase GetPinchPhase(this Touch[] touches){
		return TouchHandler.Pinch.phase;
	}


	/* ------------ TOUCH INSTANCE METHODS ------------ */

	/// <summary>
	/// All current touches will raycast.
	/// RaycastHit is stored in <TouchInstance.objectTouched>
	/// </summary>
	/// <returns>TouchInstance[]</returns>
	/// <param name="touches">Input.touches.GetRayHit3D()</param>
	/// <param name="layersToIgnore">comma separated (or integer array) layer #'s to ignore</param>
	public static TouchInstance[] CheckRayHit3D(this Touch[] touches, params int[] layersToIgnore){
		return TouchHandler.GetRayHit3D(layersToIgnore);
	}

	/// <summary>
	/// All current touches will raycast.
	/// RaycastHit is stored in <TouchInstance.objectTouched>
	/// </summary>
	/// <returns>TouchInstance[]</returns>
	/// <param name="touches">Input.touches.GetRayHit3D()</param>
	/// <param name="layersToIgnore">comma separated (or integer array) layer #'s to ignore</param>
	public static TouchInstance[] CheckRayHit2D(this Touch[] touches, params int[] layersToIgnore){
		return TouchHandler.GetRayHit2D(layersToIgnore);
	}

	/// <summary>
	/// Returns Touch.action
	/// </summary>
	/// <returns>TouchHandler.actions</returns>
	/// <param name="touch">Input.touches[#].getAction()</param>
	public static TouchHandler.actions GetAction(this Touch touch){
		return touch.GetRaw().action;
	}

	/// <summary>
	/// Returns Touch.tapCount
	/// </summary>
	/// <returns>int</returns>
	/// <param name="touch">Input.touches[#].getTapCount()</param>
	public static int GetTapCount(this Touch touch){
		return touch.GetRaw().tapCount;
	}

	/// <summary>
	/// Gets the swipe direction of a single touch
	/// </summary>
	/// <returns>TouchHandler.directions</returns>
	/// <param name="touch">Input.touches[#].getSwipeDirection()</param>
	public static TouchHandler.directions GetSwipeDirection(this Touch touch){
		return touch.GetRaw().swipeDirection;
	}

	/// <summary>
	/// Gets the current press time of a touch.
	/// to get total press time use: <Input.touches[#].getRaw().totalPressTime>
	/// </summary>
	/// <returns>float</returns>
	/// <param name="touch">Input.touches[#].GetPressTime()</param>
	public static float GetPressTime(this Touch touch){
		return touch.GetRaw().currentPressTime;
	}

	/// <summary>
	/// Gets all extended touch properties.
	/// </summary>
	/// <returns>TouchInstance[]</returns>
	/// <param name="touches">Input.touches.GetRaw()</param>
	public static TouchInstance[] GetRaw(this Touch[] touches){
		return TouchHandler._touchCache.ToArray();
	}

	/// <summary>
	/// Gets extended touch properties for a single touch.
	/// </summary>
	/// <returns>TouchInstance</returns>
	/// <param name="touches">Input.touches[#].GetRaw()</param>
	public static TouchInstance GetRaw(this Touch touch){
		foreach( TouchInstance t in TouchHandler._touchCache)
			if (t.fingerId == touch.fingerId)
				return t;
		return TouchHandler._touchCache[touch.fingerId];
	}
	
}
