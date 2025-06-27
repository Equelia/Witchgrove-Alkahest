using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : InteractableItem 
{
	public Animator anim;
	private bool opened = false;
	
	void Start () {
		anim = GetComponent<Animator> ();
	}

	public override void Interact()
	{
		if (!opened)
		{
			anim.SetBool ("DoorOpen", true);
			anim.SetBool ("DoorClose", false);
			opened = true;
		}
		else
		{
			anim.SetBool ("DoorOpen", false);
			anim.SetBool ("DoorClose", true);
		}
	}
}
