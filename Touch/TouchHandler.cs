using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

/* 
*** TOUCH HANDLER CLASS ***
Aggregates all of the Input.touches and creates a new TouchInstance for each,
then stores them in TouchHandler.touches[]

[ TouchInstance ] - contains all of the extended properties such as 'action',
'tapCount', 'distanceTraveled', etc...

[ TouchExtension ] - extends Input.touches with the getExtended method,
returns TouchInstance or TouchInstance[]

[ TouchCreator ] - is used solely for converting mouse clicks to touch events

 */



[DisallowMultipleComponent]
[ScriptOrder(-1000)]
[RequireComponent (typeof(UnityEngine.EventSystems.StandaloneInputModule))]
public class TouchHandler : MonoBehaviour{

	[Header("Options")]
	[Space(10,order=2)]

	public static TouchHandler currentInstance;
	public bool dontDestroyOnLoad = false;
	public bool displayStatsInEditor = false;

	[Tooltip("Time allowed between taps before reset")]
	public float tapTimeout = 0.15f;
	public static float _tapTimeout;

	[Range(0.01f, 1f)]
	public float longPressTime = 0.5f;
	public static float _longPressTime;

	public UnitTypes measureUnits = UnitTypes.Centimeter;
	public static UnitTypes _measureUnits;

	[Tooltip("Speed (measureUnits/Second) a touch can travel before swipe action")]
	public float swipeThreshold = 50f;
	public static float _swipeThreshold;

	[Tooltip("Distance a touch can move before drag action")]
	public float dragThreshold = 0.5f;
	public static float _dragThreshold;

	public static float _pinchDistance;
	public static float _pinchRatio;

	public Sprite touchSprite;

	public enum actions{None, Down, LongPress, Tap, Swipe, Drag}; 
	public enum directions{None, Up, Down, Left, Right};
	public enum UnitTypes{Millimeter, Centimeter, Inch, Pixel};

	public static Touch[] touches = new Touch[0];
	public static List<TouchInstance> _touchCache = new List<TouchInstance>();

	// Use mouse to simulate touches
	public bool simulateTouchWithMouse = false;
	public TouchCreator[] tc = new TouchCreator[2];
	private Touch fakeTouch, simulatePinch;

	void Awake () {
		if (dontDestroyOnLoad){
			// SINGLETON
			if( currentInstance == null ) {
				currentInstance = this;
				DontDestroyOnLoad(this.gameObject);
			}else{
				Destroy( this.gameObject );
			}
		}
#if UNITY_EDITOR
		if (simulateTouchWithMouse)
			Input.simulateMouseWithTouches = false;
#endif
	}

	void Start () {
		screenPixelsPerCm = Screen.dpi; //initialize
		_measureUnits = measureUnits;
		_tapTimeout = tapTimeout;
		_longPressTime = longPressTime;
		_dragThreshold = UnitsToPixels(dragThreshold);
		_swipeThreshold = swipeThreshold;
		tc[0] = new TouchCreator();
		tc[1] = new TouchCreator();
		fakeTouch = tc[0].CreateEmpty();
		simulatePinch = tc[1].CreateEmpty();
		if (Input.touchSupported && Input.multiTouchEnabled)
			simulateTouchWithMouse = false;
	}

	public void AssignTouches(){
		if (!simulateTouchWithMouse){
			touches = (Touch[])Input.touches.Clone ();
		}else{
			//simulate touch with mouse
			if(Input.GetKeyDown(KeyCode.LeftControl)){
				simulatePinch = tc[1].Begin(1);
			}else if (Input.GetKey(KeyCode.LeftControl)){
				simulatePinch = tc[1].Update (true);
			}else if (Input.GetKeyUp(KeyCode.LeftControl)){
				simulatePinch = tc[1].End();
			}else{
				simulatePinch = new Touch();
			}

			if(Input.GetMouseButtonDown(0)){
				fakeTouch = tc[0].Begin ();
				touches = new Touch[]{fakeTouch, simulatePinch};
			}else if (Input.GetMouseButton(0)){
				fakeTouch = tc[0].Update ();
				touches[0] = fakeTouch;
				touches[1] = simulatePinch;
			}else if (Input.GetMouseButtonUp(0)){
				fakeTouch = tc[0].End();
				touches[0] = fakeTouch;
				if (simulatePinch.fingerId == 1){
					simulatePinch = tc[1].End ();
					touches[1] = simulatePinch;
				}
			}else{
				fakeTouch = tc[0].CreateEmpty ();
				touches = (Touch[])Input.touches.Clone ();
			}
		}
	}

	void Update () {

		AssignTouches();

		//compare _touchCache to latest touches
		foreach(Touch t in touches){
			if (!_touchCache.Exists ( x => x.fingerId == t.fingerId )){
				_touchCache.Add (new TouchInstance(t));
				_touchCache.OrderBy(x => x.fingerId);
			}
		}

		//update TouchInstances in reverse 
		//so they can remove themselves when finished
		if (_touchCache.Count > 0){
			for(int i = _touchCache.Count - 1; i >= 0; i--){
				_touchCache[i].Update ();
			}
		}

		Pinch.Update();

	}

#if UNITY_EDITOR
	void OnGUI()
	{
		if (displayStatsInEditor){
			Color fontColor = Color.white;
			int w = Screen.width, h = Screen.height;
			
			GUIStyle style = new GUIStyle();
			Rect rect = new Rect(5, 5, w, h / 50);
			style.alignment = TextAnchor.UpperLeft;
			style.fontSize = h / 50;
			style.normal.textColor = fontColor;

			string text = string.Format("Is Pinching:\t{0}", Pinch.active);
			text += string.Format("\nPinch Dist.:\t{0:0.0}px", Pinch.distance);
			text += string.Format("\nPinch Ratio:\t{0:0%}", Pinch.ratio);
			text += "\n----------------------------------";
			int y = h/10;
			GUI.Label(rect, text, style);

			Texture t = touchSprite.texture;
			Rect tr = touchSprite.textureRect;
			Rect r = new Rect(tr.x / t.width, tr.y / t.height, tr.width / t.width, tr.height / t.height );
			for (int i = 0; i < _touchCache.Count; i++){
				rect = new Rect(5, (h / 50) * i * 9 + y, w, h / 50);
				style.alignment = TextAnchor.UpperLeft;
				style.fontSize = h / 50;
				style.normal.textColor = fontColor;
				text = string.Format("Touch ID:\t{0}", _touchCache[i].fingerId);
				if (_touchCache.Count > 0){
					text += string.Format("\nPhase:\t{0}", _touchCache[i].phase);
					text += string.Format("\nAction:\t{0}", _touchCache[i].action);
					text += string.Format("\nTaps:\t{0}", _touchCache[i].tapCount);
					text += string.Format("\nDistance:\t{0}", _touchCache[i].distanceTraveled);
					text += string.Format("\nSpeed:\t{0}", _touchCache[i].speed);
					text += string.Format("\nTime:\t{0:0.00} s.", _touchCache[i].totalPressTime);
					text += "\n----------------------------------";
				}
				GUI.Label(rect, text, style);
				GUI.DrawTextureWithTexCoords(new Rect(_touchCache[i].currentPos.x - (t.width/2), Screen.height - _touchCache[i].currentPos.y - (t.height/2), tr.width, tr.height), t, r);
			}
		}
	}
#endif
	
	//HELPERS
	private const float inchesToCentimeters = 2.54f;
	private static float _screenPixelsPerCm = 0f;
	public static float screenPixelsPerCm
	{
		get
		{
			return _screenPixelsPerCm;
		}
		
		set
		{
			
			float setDpi = value;
			float fallbackDpi = 72f;
			#if UNITY_ANDROID
			// Android MDPI setting fallback
			// http://developer.android.com/guide/practices/screens_support.html
			fallbackDpi = 160f;
			#elif (UNITY_WP8 || UNITY_WP8_1 || UNITY_WSA || UNITY_WSA_8_0)
			// Windows phone is harder to track down
			// http://www.windowscentral.com/higher-resolution-support-windows-phone-7-dpi-262
			fallbackDpi = 92f;
			#elif UNITY_IOS
			// iPhone 4-6 range
			fallbackDpi = 326f;
			#endif
			
			_screenPixelsPerCm = setDpi == 0f ? fallbackDpi / inchesToCentimeters : setDpi / inchesToCentimeters;
		}
	}

	/// <summary>
	/// Convert Units to pixels based on screen dpi
	/// </summary>
	/// <returns>float</returns>
	/// <param name="units"> The number of units to convert</param>
	public static float UnitsToPixels(float units){
		float conversion = 1f;
		
		switch (_measureUnits)
		{
		case UnitTypes.Centimeter:
			conversion = _screenPixelsPerCm;
			break;
		case UnitTypes.Millimeter:
			conversion = _screenPixelsPerCm / 10f;
			break;
		case UnitTypes.Inch:
			conversion = _screenPixelsPerCm * 2.54f;
			break;
		case UnitTypes.Pixel:
			conversion = 1f;
			break;
		}
		
		return units * conversion;
	}
	
	/// <summary>
	/// Convert pixels to Units based on screen dpi
	/// </summary>
	/// <returns>float</returns>
	public static float PixelsPerUnit(){
	
		switch (_measureUnits)
		{
		case UnitTypes.Centimeter:
			return _screenPixelsPerCm;
		case UnitTypes.Millimeter:
			return _screenPixelsPerCm * 10f;
		case UnitTypes.Inch:
			return _screenPixelsPerCm / 2.54f;
		case UnitTypes.Pixel:
		default:
			return 1f;
		}	
	}

	//  RAYCAST FOR TOUCHED OBJECTS
	/// <summary>
	/// <para>Returns TouchInstance[].</para>
	/// <para>Same method as: Input.touches.GetRayHit3D();</para>
	/// </summary>
	/// <returns>The ray hit3 d.</returns>
	public static TouchInstance[] GetRayHit3D(int[] layerMask){
		foreach(TouchInstance t in _touchCache){
			t.CheckRayHit(layerMask);
		}
		return _touchCache.ToArray();
	}

	//  RAYCAST FOR TOUCHED OBJECTS
	/// <summary>
	/// <para>Returns TouchInstance[].</para>
	/// <para>Same method as: Input.touches.GetRayHit3D();</para>
	/// </summary>
	/// <returns>The ray hit3 d.</returns>
	public static TouchInstance[] GetRayHit2D(int[] layerMask){
		foreach(TouchInstance t in _touchCache){
			t.CheckRayHit2D(layerMask);
		}
		return _touchCache.ToArray();
	}

	//  TWO FINGER SWIPE
	/// <summary>
	/// <para>Get direction of two finger swipe.</para>
	/// <para>Returns directions.None if not applicable.</para>
	/// </summary>
	public static class DoubleSwipe{
		public static TouchHandler.directions Direction(){
			if (_touchCache.Count == 2 &&
			    _touchCache[0].action == TouchHandler.actions.Swipe &&
			    _touchCache[1].action == TouchHandler.actions.Swipe){
				return _touchCache[0].swipeDirection;
			}

			return TouchHandler.directions.None;
		}
	}

	//  PINCH CLASS
	/// <summary>
	/// <para>Pinch Class, contains properties:</para>
	/// <para>[TouchPhase]phase, [bool]active, [float]pinchDelay</para>
	/// <para>[float](delta)distance, [float](delta)ratio</para>
	/// </summary>
	public static class Pinch{

		public static TouchPhase phase = TouchPhase.Ended;
		public static bool active = false;
		public static float pinchDelay = 0.2f;
		public static float distance = 0f;
		public static float ratio = 1f;

		private static float lastDistance = 0f;
		private static float lastRatio = 1f;

		
		public static float deltaDistance{
			get{
				float delta = distance - lastDistance;
				return delta;
			}
		}

		public static float deltaRatio{
			get{
				float delta = ratio - lastRatio;
				return delta;
			}
		}

		/// <summary>
		/// Update pinch properties
		/// </summary>
		public static void Update(){

			active = GetPinchState();
			distance = GetDistance();
			ratio = GetRatio();

			if (active){
				if (phase == TouchPhase.Ended){
					phase = TouchPhase.Began;
				}else{
					if (Math.Abs(deltaDistance) > TouchHandler._dragThreshold){
						phase = TouchPhase.Moved;
					}else{
						phase = TouchPhase.Stationary;
					}
				}
			}
		}

		/// <summary>
		/// Gets the state of the pinch.
		/// </summary>
		/// <returns><c>true</c>, if user is pinching, <c>false</c> otherwise.</returns>
		private static bool GetPinchState(){
			if (_touchCache.Count == 2){
				return (_touchCache[1].action != TouchHandler.actions.Swipe &&
				          _touchCache[0].action != TouchHandler.actions.Swipe) &&
					(_touchCache[0].totalPressTime > pinchDelay &&
					 _touchCache[1].totalPressTime > pinchDelay);
			}

			phase = TouchPhase.Ended;
			return false;
			
		}
		
		/// <summary>
		/// <para>Gets Distance of pinch in pixels.</para>
		/// <para><c>Positive: </c>Touches moving away from each other.</para>
		/// <para><c>Negative: </c>Touches moving toward each other.</para>
		/// </summary>
		/// <returns>float</returns>
		private static float GetDistance(){
			if (active){
				lastDistance = distance;
				return Vector2.Distance(_touchCache[0].currentPos, _touchCache[1].currentPos) - 
						Vector2.Distance (_touchCache[0].startPos, _touchCache[1].startPos);
			}
			lastDistance = 0;
			return 0;
		}

		/// <summary>
		/// <para>Gets % that pinch has changed since start.</para>
		/// <para> </para>
		/// <para><c>1.0: </c> No change.</para>
		/// <para><c>Greater 1: </c>Touches moving away from each other.</para>
		/// <para><c>Less 1: </c>Touches moving toward each other.</para>
		/// </summary>
		/// <returns>float</returns>
		private static float GetRatio(){
			if (active){
				lastRatio = ratio;
				return Vector2.Distance(_touchCache[0].currentPos, _touchCache[1].currentPos) / 
						Vector2.Distance (_touchCache[0].startPos, _touchCache[1].startPos);
			}
			lastRatio = 1;
			return 1;
		}

		// CHECK IF USER IS PINCHING SOMETHING
		/// <summary>
		/// Raycast from center of pinch.
		/// </summary>
		/// <returns><c>true</c>, if ray hits a collider, <c>false</c> otherwise.</returns>
		/// <param name="hit">RaycastHit</param>
		public static bool Raycast3D(out RaycastHit hit, int[] layerMask){

			Vector2 center = Vector2.one;
			Ray ray;

			if (active){
				center = (_touchCache[0].currentPos + _touchCache[1].currentPos) / 2;

				ray = Camera.main.ScreenPointToRay(center);
				int tempMask = 0;
				foreach(int t in layerMask){
					tempMask |= (1 << t);
				}
				return Physics.Raycast(ray, out hit, Mathf.Infinity, ~tempMask);
			}else{
				/* this part is only here because 'hit'
				 * must be assigned before returning.
				 * we may accidentally hit something,
				 * but will always return false */
				center *= (-1000000);
				ray = Camera.main.ScreenPointToRay(center);

				Physics.Raycast(ray, out hit, Mathf.Infinity);
				return false;
			}
		}

		// CHECK IF USER IS PINCHING SOMETHING
		/// <summary>
		/// Raycast from center of pinch.
		/// </summary>
		/// <returns><c>true</c>, if ray hits a collider, <c>false</c> otherwise.</returns>
		/// <param name="hit">RaycastHit</param>
		public static bool Raycast2D(out RaycastHit2D hit, int[] layerMask){
			Vector2 center = Vector2.one;
			
			if (active){
				center = Camera.main.ScreenToWorldPoint((_touchCache[0].currentPos + _touchCache[1].currentPos) / 2);
				hit = Physics2D.Raycast(center, Vector2.zero);
				return hit.collider != null;
			}else{
				/* this part is only here because 'hit'
				 * must be assigned before returning.
				 * we may accidentally hit something,
				 * but will always return false */
				center *= (-1000000);
				hit = Physics2D.Raycast(center, Vector2.zero);
				return false;
			}
		}
	}
	
}

