using UnityEngine;
using UnityEngine.EventSystems;
using System.Reflection;
using System.Collections.Generic;

public class TouchCreator
{
	static BindingFlags flag = BindingFlags.Instance | BindingFlags.NonPublic;
	static Dictionary<string, FieldInfo> fields;
	
	public object touch;


	public float deltaTime { get { return ((Touch)touch).deltaTime; } set { fields["m_TimeDelta"].SetValue(touch, value); } }
	public int tapCount { get { return ((Touch)touch).tapCount; } set { fields["m_TapCount"].SetValue(touch, value); } }
	public TouchPhase phase { get { return ((Touch)touch).phase; } set { fields["m_Phase"].SetValue(touch, value); } }
	public Vector2 deltaPosition { get { return ((Touch)touch).deltaPosition; } set { fields["m_PositionDelta"].SetValue(touch, value); } }
	public int fingerId { get { return ((Touch)touch).fingerId; } set { fields["m_FingerId"].SetValue(touch, value); } }
	public Vector2 position { get { return ((Touch)touch).position; } set { fields["m_Position"].SetValue(touch, value); } }
	public Vector2 rawPosition { get { return ((Touch)touch).rawPosition; } set { fields["m_RawPosition"].SetValue(touch, value); } }

	private Vector2 lastPosition;

	public Touch Update(bool simulatePinch = false)
	{
		//PHASE
		if (deltaPosition.magnitude > EventSystem.current.pixelDragThreshold){
			phase = TouchPhase.Moved;
		}else{
			phase = TouchPhase.Stationary;
		}
		//DELTA TIME/POSITION
		deltaTime = Time.deltaTime;
		if (simulatePinch){
			position = new Vector3(Screen.width - Input.mousePosition.x, Input.mousePosition.y, 0);
		}else{
			position = Input.mousePosition;
		}
		rawPosition = position;
		deltaPosition = (lastPosition != Vector2.zero) ? (position - lastPosition) : Vector2.zero;
		lastPosition = position;
		return (Touch)touch;

	}

	public Touch CreateEmpty()
	{
		phase = TouchPhase.Canceled;
		position = Vector2.zero;
		rawPosition = Vector2.zero;
		deltaPosition = Vector2.zero;
		tapCount = 0;
		fingerId = -1;
		return (Touch)touch;

	}

	public Touch Begin(int setFingerId = 0)
	{
		phase = TouchPhase.Began;
		deltaTime = 0f;
		if (setFingerId != 0){
			position = new Vector3(Screen.width - Input.mousePosition.x, Input.mousePosition.y, 0);
		}else{
			position = Input.mousePosition;
		}
		deltaPosition = Vector2.zero;
		lastPosition = Input.mousePosition;
		tapCount++;
		fingerId = setFingerId;
		return (Touch)touch;
	}

	public Touch End()
	{
		phase = TouchPhase.Ended;

		return (Touch)touch;
	}
	
	public TouchCreator()
	{
		touch = new Touch();
	}
	
	static TouchCreator()
	{
		fields = new Dictionary<string, FieldInfo>();
		foreach(var f in typeof(Touch).GetFields(flag))
		{
			fields.Add(f.Name, f);
		}


	}
}