using UnityEngine;
using System.Collections;

public class Example2D : MonoBehaviour {
	
	public bool grapple = false;
	public GameObject zoomPanel;

	private RaycastHit2D pinchHit;
	private int[] layersToIgnore = {8};
	private Color lerpedColor = Color.white;
	private string text = "Touch Module Example 2D";
	
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
			text = "Touch Module Example 2D";
		
		// Iterate pulse color - (used if object is touched by user)
		lerpedColor = Color.Lerp(Color.white, Color.magenta, Mathf.PingPong(Time.time, 1));
		
		// Check if user is pinching
		if (Input.touches.IsPinching()){
			// Scale 3D object by pinching it
			if (Input.touches.GetPinchRayHit2D(out pinchHit, layersToIgnore)){
				text = string.Format("Pinching - scale {0}", pinchHit.transform.name);
				pinchHit.transform.localScale += Vector3.one * Input.touches.GetPinchRatioDelta() * 500f;
				pinchHit.rigidbody.gameObject.GetComponent<Renderer>().material.color = lerpedColor;
			// Zoom in/out by pinching vacant area of screen
			}else{
				text = string.Format ("Pinching - zoom camera");
				zoomPanel.transform.localScale += Vector3.one * Input.touches.GetPinchRatioDelta();
			}
			
		// Check for a Two-Finger Swipe
		}else if (Input.touches.GetDoubleSwipeDirection() != TouchHandler.directions.None)
		{	
			// Camera Pan left/right
			if(Input.touches.GetDoubleSwipeDirection() == TouchHandler.directions.Right)
			{
				text = string.Format("Two Finger Swipe - Pan Right");
				zoomPanel.transform.localPosition += Vector3.right * -10f;
			}else{
				text = string.Format("Two Finger Swipe - Pan Left");
				zoomPanel.transform.localPosition += Vector3.left * -10f;
			}
			
		// All Other Actions
		}else{
			CheckActions();
		}
	}
	
	void CheckActions(){
		// Iterate Through All Touches + cast rays from each touch
		foreach(TouchInstance t in Input.touches.CheckRayHit2D(layersToIgnore)){
			if (t.raycastHit2D.rigidbody != null){
				
				// Set color if object is touched by user
				t.raycastHit2D.rigidbody.gameObject.GetComponent<Renderer>().material.color = lerpedColor;

				switch (t.action){
					
				// Drag objects
				case TouchHandler.actions.Drag:
					// Make sure user is still touching
					if (!t.phase.IsDone()){
						text = string.Format("Drag (Grapple Off) - {0}", t.raycastHit2D.transform.name);
						t.raycastHit2D.rigidbody.MovePosition(new Vector2(t.raycastHit2D.point.x, t.raycastHit2D.point.y));
						t.raycastHit2D.rigidbody.velocity = t.raycastHit2D.rigidbody.velocity.normalized;

						// use this to keep hold of object, even when moving too fast for object to update
						if (grapple){
							text = string.Format("Drag (Grapple On) - {0}", t.raycastHit2D.transform.name);
							text += "\nGrapple keeps hold of the object,\neven when moving too fast for object to update.";
							t.GrappleRayHit();
						}
						// use this to avoid accidentally swiping
						t.overrideAction = TouchHandler.actions.Drag;
					}else{
						// let go of any grappled items
						t.UngrappleRayHit();
					}
					break;
					
				// Rotate Clockwise by continually tapping
				case TouchHandler.actions.Tap:
					text = string.Format("Tapping (count {0}) - {1}", t.tapCount, t.raycastHit2D.transform.name);
					t.raycastHit2D.rigidbody.AddTorque(3f);
					break;
					
				// Rotate Anti-Clockwise with Longpress
				case TouchHandler.actions.LongPress:
					text = string.Format("Long Press - {0}", t.raycastHit2D.transform.name);
					t.raycastHit2D.rigidbody.AddTorque(-3f);
					if (grapple){
						t.GrappleRayHit();
					}
					break;
					
				// Swipe to add force to object
				case TouchHandler.actions.Swipe:
					text = "Swipe";
					t.raycastHit2D.rigidbody.AddForceAtPosition(t.velocity * 10, t.raycastHit2D.point);
					break;
				}
			}
		}
	}
	
}
