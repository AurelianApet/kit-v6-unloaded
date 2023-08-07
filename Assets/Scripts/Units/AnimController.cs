﻿using UnityEngine;
using System.Collections;

public class AnimController : MonoBehaviour {  //animation controller for dobbits/soldiers

	tk2dSpriteAnimator animator;

	//public tk2dSpriteAnimation attackAnimation, buildAnimation, idleAnimation, walkAnimation;

	public tk2dSpriteAnimation spriteAnimation;
	
	private string action,direction;

	public string soldierType = "";//EventTrigger

	//private Component soundFX; 

	void Start () 
	{
		animator = GetComponent<tk2dSpriteAnimator>();

		action = "Walk";
		direction = "W";

		//soundFX = GameObject.Find("SoundFX").GetComponent<SoundFX>();// //connects to SoundFx - a sound source near the camera

		if (soldierType == "EventTrigger")
		{
			animator.AnimationEventTriggered += FireWeapon;
		}

	}

	void Update () {

		//Manual controller to check animations
		/*
		bool animChanged = false;
		if(Input.anyKey){ animChanged = true; }

		if(Input.GetKey(KeyCode.F)){ direction = "E"; }
		else if(Input.GetKey(KeyCode.R)){ direction = "NE"; }
		else if(Input.GetKey(KeyCode.E)){ direction = "N"; }
		else if(Input.GetKey(KeyCode.W)){ direction = "NW"; }
		else if(Input.GetKey(KeyCode.S)){ direction = "W"; }
		else if(Input.GetKey(KeyCode.Z)){ direction = "SW"; }
		else if(Input.GetKey(KeyCode.X)){ direction = "S"; }
		else if(Input.GetKey(KeyCode.C)){ direction = "SE"; }		

		if(Input.GetKey(KeyCode.I)){action = "Idle"; animator.Library = idleAnimation;		}
		else if(Input.GetKey(KeyCode.O)){action = "Walk"; animator.Library = walkAnimation;		}
		else if(Input.GetKey(KeyCode.K)){action = "Attack"; animator.Library = attackAnimation;		}
		else if(Input.GetKey(KeyCode.L)){action = "Build"; animator.Library = buildAnimation;		}

		if(animChanged){UpdateCharacterAnimation();}
		*/
	}

	//order: change anim / turn /update char anim 


	public void ChangeAnim(string anim)
	{
		action = anim; 
		/*
		switch (anim) 
		{
		case "Idle":
			animator.Library = idleAnimation;
			break;
		case "Walk":
			animator.Library = walkAnimation;
			break;
		case "Attack":
			animator.Library = attackAnimation;
			break;
		case "Build":
			animator.Library = buildAnimation;	
			break;
		}	
		*/
	}


	public void Turn(string dir){direction = dir; }

	public void UpdateCharacterAnimation()
	{
		animator.Play(action + "_" + direction);
	}


	void  FireWeapon(tk2dSpriteAnimator animator, tk2dSpriteAnimationClip clip, int frameNo)
	{

		//if (soldierType == "EventTrigger")
		//((SoundFX)soundFX).CopFire ();
		//.CannonFire ();
	}


}

