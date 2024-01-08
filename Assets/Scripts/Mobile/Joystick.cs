using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class Joystick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
	public int MovementRange = 100;
	public string horizontalAxisName = "Horizontal"; // The name given to the horizontal axis for the cross platform input
	public string verticalAxisName = "Vertical"; // The name given to the vertical axis for the cross platform input

	[HideInInspector] public Vector3 m_StartPos;

    public void Start()
    {
		this.Initialize ();
    }

	public virtual void Initialize(){
		m_StartPos = transform.position;
	}

	public virtual void OnDrag(PointerEventData data)
	{
		Vector3 dif = new Vector3(data.position.x, data.position.y, 0) - m_StartPos;
		float magnitude = dif.magnitude;
		dif.Normalize ();
		float newMagnitude = Mathf.Clamp (magnitude, -MovementRange, MovementRange);

		transform.position = m_StartPos + dif * newMagnitude;
	}


	public virtual void OnPointerUp(PointerEventData data)
	{
		transform.position = m_StartPos;
	}


	public virtual void OnPointerDown(PointerEventData data) { }

	public virtual Vector2 getJoystickCurrentValues(){
		float valueX = (this.transform.position.x - this.m_StartPos.x) / MovementRange;
		float valueY = (this.transform.position.y - this.m_StartPos.y) / MovementRange;
		return new Vector2 (valueX, valueY);
	}

	public void Reset(){
		transform.position = m_StartPos;
	}

	public virtual bool canShoot(){
		if (this.transform.position.x != this.m_StartPos.x && this.transform.position.y != this.m_StartPos.y) {
			return true;
		}
		return false;
	}
}