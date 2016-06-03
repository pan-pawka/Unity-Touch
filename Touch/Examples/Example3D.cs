using UnityEngine;
using System.Collections;

public class Example3D : MonoBehaviour {

	public bool grapple = false;

	private RaycastHit pinchHit;
	private int[] layersToIgnore = {8};
	private Color lerpedColor = Color.white;
	private string text = "Touch Module Example 3D";

	public void setGrapple(bool tempGrapple){
		grapple = tempGrapple;
	}

	void OnGUI()
	{

		Color fontColor = Color.white;
		
		GUIStyle style = new GUIStyle();
		Rect rect = new Rect(0, 5, Screen.width, 75);
		style.alignment = TextAnchor.UpperCenter;
		style.fontSize = 35;
		style.normal.textColor = fontColor;
		GUI.Label(rect, text, style);

	}

	void Update () {

		if (!text.Contains("Swipe"))
			text = "Touch Module Example 3D";

		// Iterate pulse color - (used if object is touched by user)
		lerpedColor = Color.Lerp(Color.white, Color.magenta, Mathf.PingPong(Time.time, 1));

		// Check if user is pinching
		if (Input.touches.IsPinching()){
			// Scale 3D object by pinching it
			if (Input.touches.GetPinchRayHit3D(out pinchHit, layersToIgnore)){
				text = string.Format("Pinching - scale {0}", pinchHit.transform.name);
				pinchHit.transform.localScale = Vector3.one * Input.touches.GetPinchRatio();
				pinchHit.rigidbody.gameObject.GetComponent<Renderer>().material.color = lerpedColor;
			// Zoom in/out by pinching vacant area of screen
			}else{
				text = string.Format ("Pinching - zoom camera");
				Camera.main.fieldOfView = (Input.touches.GetPinchRatio()) * 60f;
			}
			
		// Check for a Two-Finger Swipe
		}else if (Input.touches.GetDoubleSwipeDirection() != TouchHandler.directions.None)
		{	
			// Camera Pan left/right
			if(Input.touches.GetDoubleSwipeDirection() == TouchHandler.directions.Right)
			{
				text = string.Format("Two Finger Swipe - Pan Right");
				Camera.main.transform.Rotate(Vector3.up * Time.deltaTime * 90);
			}else{
				text = string.Format("Two Finger Swipe - Pan Left");
				Camera.main.transform.Rotate(Vector3.down * Time.deltaTime * 90);
			}
			
		// All Other Actions
		}else{
			CheckActions();
		}
	}

	void CheckActions(){
		// Iterate Through All Touches + cast rays from each touch
		foreach(TouchInstance t in Input.touches.CheckRayHit3D(layersToIgnore)){
			if (t.raycastHit.rigidbody != null){

				// Set color if object is touched by user
				t.raycastHit.rigidbody.gameObject.GetComponent<Renderer>().material.color = lerpedColor;

				switch (t.action){

				// Drag objects
				case TouchHandler.actions.Drag:
					text = string.Format("Drag (Grapple Off) - {0}", t.raycastHit.transform.name);
					t.raycastHit.rigidbody.MovePosition(new Vector3(t.raycastHit.point.x, t.raycastHit.rigidbody.position.y, t.raycastHit.point.z));
					//use this to keep hold of object, even when moving too fast for object to update
					if (grapple){
						text = string.Format("Drag (Grapple On) - {0}", t.raycastHit.transform.name);
						text += "\nGrapple keeps hold of the object,\neven when moving too fast for object to update.";
						t.GrappleRayHit();
					}
					//use this to avoid accidentally swiping
					t.overrideAction = TouchHandler.actions.Drag;
					break;

				// Rotate Clockwise by continually tapping
				case TouchHandler.actions.Tap:
					text = string.Format("Tapping (count {0}) - {1}", t.tapCount, t.raycastHit.transform.name);
					t.raycastHit.rigidbody.transform.Rotate(Vector3.up * Time.deltaTime * 90);
					break;

				// Rotate Anti-Clockwise with Longpress
				case TouchHandler.actions.LongPress:
					text = string.Format("Long Press - {0}", t.raycastHit.transform.name);
					t.raycastHit.rigidbody.transform.Rotate(Vector3.down * Time.deltaTime * 90);
					break;
				
				// Swipe to add force to object
				case TouchHandler.actions.Swipe:
					text = "Swipe";
					t.raycastHit.rigidbody.AddForce(t.velocity * 5);
					t.raycastHit.rigidbody.AddRelativeTorque(t.velocity * 5);
					break;
				}
			}
		}
	}
	
}
