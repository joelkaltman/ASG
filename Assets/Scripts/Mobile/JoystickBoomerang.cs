using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class JoystickBoomerang : Joystick {

	private bool pressed;

	public override void Initialize(){
		this.pressed = false;
	}

	public override void OnPointerDown(PointerEventData eventData)
	{
		this.pressed = true;
	}

	public override void OnPointerUp(PointerEventData eventData)
	{
		this.pressed = false;
	}

	public override void OnDrag(PointerEventData data)
	{
	}

	public override Vector2 CurrentValues(){
		return new Vector2 (0, 0);
	}

	public override bool CanShoot(){
		return this.pressed;
	}
}
