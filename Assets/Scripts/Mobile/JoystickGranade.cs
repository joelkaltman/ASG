using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class JoystickGranade : Joystick {

	private bool pressed;

	public override void Initialize(){
		this.m_StartPos = transform.position;
		this.pressed = false;
	}

	public override void OnDrag(PointerEventData data)
	{
		if (!pressed) {
			base.OnDrag (data);
			Invoke ("Stop", 1);
		}
	}

	public override void OnPointerUp(PointerEventData data)
	{
		pressed = false;
		CancelInvoke ();
		base.OnPointerUp (data);
	}

	void Stop(){
		pressed = true;
		this.Reset ();
	}

	public override bool canShoot(){
		float distX = Mathf.Abs (this.transform.position.x - this.m_StartPos.x);
		float distY = Mathf.Abs (this.transform.position.y - this.m_StartPos.y);

		if (distX > MovementRange/2 || distY > MovementRange/2) {
			return true;
		}
		return false;
	}
}
