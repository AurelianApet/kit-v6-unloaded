using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WeaponAnimationController : MonoBehaviour { 

				//   ####    WARNING !!!    ####    \\
		/*

			At the time of this writing, all Toolkit 2D animated structures have to be instantiated in-game, 
			by user interaction or script loading, and not manually placed in the scene with the editor; 
			If they are, the animations will desynchronize/not work properly

	 	*/

	//goldBig, goldSmall,goldSmallShadow, both, bothshadow, bow, bowshadow
	public tk2dSpriteAnimator[] animatedSpritesAnim = new tk2dSpriteAnimator[1];

	private string 
		cw = "CW",
		ccw = "CCW",
		clipFire,
		clipReload;

	//Fire0 - Fire11, RotateCCW, RotateCW
	//"GoldBig","GoldSmall","GoldSmall", "Both","Both","Loaded","Loaded"

	//As it is now, the script is configured for objects with multiple rotating components; however, the animations can apply to other systems as 
	//well, as long as the animations are duplicate with a CW/CCW (clock-wise, counter-clockwise) extension - even if they're not rotating,
	//the animation in playing forwards/backwards

	private string[] 
		//rotateCW and rotateCCW are the names of the animation clips
		animatedSprites = new string[1]{"rotate"},		//You can include more animated sprites here, for composed animated in synch onjects - 
														//for instance, the cannon and it's base, rotating in synch; 
														//also, this is the name of the Animation, minus the direction extension 
														//- cw(clockwise) or ccw(counter-clockwise)
		currentClip = new string[1]; 					//animatedSprites + ext(CCW/CW) plays the animations for each component - usualy a rotation


	//we use arrays and not simple values in case you intend to have several synchronized animated components
	//each of these will have a certain number of frames (different) and a last(current) frame
	//the last frame is used to translate between clockwise and counter-clockwise rotations

	public int[] 
	fireAngles = new int[1]{11},//if there are 12 angles (0-11), put this at 11
	lastFrame = new int[1];

	public bool //make private
		previousCW = true, 
		rotatingCW = false, 
		rotatingCCW = false,
		firing = false,
		hasFired = false,
		fireNew = false;
		
	private int 
		currentFrame, 		//always translated into a cw frame
		targetFrame,		//always translated into a cw frame
		prevTargetFrame,
		anglesNo;			//used to rotate in the proper direction (avoid going "the long way")  

	public GameObject targetOb;

	//Test AutoFire

	private float 
		fireTimer = 0,
		fireTime = 0.1f;

	private Component cannonController, sextant; 

	// Use this for initialization
	void Start () {

		cannonController = GetComponent<TurretControllerFrameEvent> ();
		sextant = transform.GetComponentInChildren<Sextant> ();
		anglesNo = ((Sextant)sextant).angles.Length;

		animatedSpritesAnim[0].AnimationEventTriggered +=  TargetAcquired;// 6 AnimationEventHandler

		RotateCW ();//short burst to register the animations correctly - otherwise throws a harmless error every now and then
		Stop ();

		//StartCoroutine ("InitWeapon"); // to correct some animation desynchronizations; not necessary any more
		//InvokeRepeating ("UpdateTarget", 2f, 2f);
	}

							// ### TESTING START ### \\

	public void UpdateTarget() //auto acquire targets and attack closest - for testing purposes with scripted moving targets
	{
		((Sextant)sextant).fire = false;
		string type = "Target";
		GameObject[] targets = GameObject.FindGameObjectsWithTag (type);
		
		if(targets.Length == 0)return;
		
		List<Vector2> closeTargets = new List<Vector2> ();
		
		
		for (int i = 0; i < targets.Length; i++) 
		{
			if(Vector2.Distance(targets[i].transform.position,transform.position)<1000)
				closeTargets.Add(new Vector2(Vector2.Distance(targets[i].transform.position, transform.position),i));
		}
		
		if (closeTargets.Count == 0)
			return;
		
		closeTargets.Sort(delegate (Vector2 d1,Vector2 d2){return d1.x.CompareTo(d2.x);});
		
		/*
		for (int i = 0; i < closeTargets.Count; i++) 
		{
			if(closeTargets[i] != null)
			target = targets [(int)closeTargets [i].y].transform.position;
			break;
		}
		*/
		
		targetOb = targets [(int)closeTargets [0].y];
		
		((Sextant)sextant).targetOb = targetOb;
		//((Rotor)rotor).targetOb = targetOb;
		((Sextant)sextant).fire = true;
	}

	private void ManualFireControl()
	{
		if (Input.GetKey(KeyCode.A)) 
		{
			RotateCW();
		}
		if (Input.GetKey(KeyCode.D)) 
		{
			RotateCCW();
		}
		if (Input.GetKey(KeyCode.W)) 
		{
			Fire ();
		}		
		if (Input.GetKey(KeyCode.S)) 
		{
			Stop ();
		}
	}
	
	private void TestAutoFire()
	{
		fireTimer += Time.deltaTime;
		if(fireTimer>fireTime)
		{
			fireTimer = 0;
			int i= Random.Range(0,3);
			
			switch (i) 
			{
			case 0:
				RotateCW();
				break;
			case 1:
				RotateCCW();
				break;
			case 2:
				Fire ();
				break;
			case 3:
				Stop ();
				break;			
			}			
		}
	}
							// ### TESTING END ### \\

	private IEnumerator InitWeapon()
	{
		yield return new WaitForSeconds (3.0f);
		RotateCW ();
		Stop ();
		animatedSpritesAnim[0].AnimationEventTriggered +=  TargetAcquired;
	}
	// Update is called once per frame
	void Update () 
	{	
		//ManualFireControl ();
		//TestAutoFire ();
	}

	/*
	public void UpdateTarget()
	{
		StartCoroutine ("FireNewTarget");
	}
	*/

	private IEnumerator FireNewTarget()
	{
		yield return new WaitForSeconds (0.1f);
		//((Sextant)sextant).elapsedTime = 0;
		((Sextant)sextant).fire = true;
	}

	private void ExecuteFire()//frame 2=frame 6
	{
		firing = true;

		int frame;
		if (previousCW)
			frame = lastFrame [0]; //keep for multiple synchronized animations
		else
			frame = fireAngles [0] - lastFrame [0]; //keep for multiple synchronized animations

		clipFire = "fire" + frame;
		//clipReload = "Reload" + frame;

		animatedSpritesAnim[0].Play(clipFire);

		//animatedSpritesAnim[0].AnimationCompleted = Reload;	//uncomment if you have a reload clip

		SkipToEndFire();										//comment if you have a reload clip

	}

	/*
	System.Action < tk2dSpriteAnimator, tk2dSpriteAnimationClip, int > 	
	AnimationEventTriggered

	Animation callback. This is called when the frame displayed has 
	tk2dSpriteAnimationFrame.triggerEvent set. The triggering frame 
	index is passed through, and the eventInfo / Int / Float can be 
	extracted. Parameters (caller, currentClip, currentFrame) 
	Set to null to clear. 
	 */

	private void SkipToEndFire()
	{
		((TurretControllerFrameEvent)cannonController).Fire();
		hasFired = true;	
		firing = false;
		animatedSpritesAnim [0].AnimationCompleted = null;	

	}

	private void Reload(tk2dSpriteAnimator animator, tk2dSpriteAnimationClip clip)
	{	
		((TurretControllerFrameEvent)cannonController).Fire();
		animatedSpritesAnim[0].Play(clipReload);		
		animatedSpritesAnim[0].AnimationCompleted = EndFire;
	}

	private void EndFire(tk2dSpriteAnimator animator, tk2dSpriteAnimationClip clip)
	{
		hasFired = true;	
		firing = false;
		animatedSpritesAnim [0].AnimationCompleted = null;		
	}


	private void TranslateCurrentFrame(int frameNo)
	{
		/*
		Pay atention at the way the rotation animations are made; the ccw (counter-clockwise) rotation
		starts at the next frame compared to the cw frame 0 - the SE direction
		The cw and ccw rotation animations do not start/end at the same angle !!!
		The 2 rotation animation clips are continuing each other!!!	
		*/

		if(previousCW)
			currentFrame = frameNo;
		else 
			currentFrame = fireAngles[0]-frameNo;
	}

	void  TargetAcquired(tk2dSpriteAnimator animator, tk2dSpriteAnimationClip clip, int frameNo)
	{
		TranslateCurrentFrame (frameNo);

		if(currentFrame == targetFrame && fireNew)
		{
			fireNew = false;	
			Fire ();			
		}
	}

	private IEnumerator LateFire()
	{
		yield return new WaitForSeconds (0.1f);	
		fireNew = false;
		Fire ();
	}

	public void FireAtFrame(int newTargetFrame)
	{
		if (firing||fireNew)
			return;

		animatedSpritesAnim[0].AnimationEventTriggered +=  TargetAcquired;//AnimationEventHandler

		((TurretControllerFrameEvent)cannonController).Aim ();
		targetFrame = newTargetFrame;

		if (targetFrame == currentFrame)//fire second time from same position
		{	
			StartCoroutine ("LateFire");
			fireNew = true;
			return;
		}

		bool rot_cw = true;
		int difFrame = targetFrame - currentFrame;//receives new fireat frame
		if(difFrame<0) rot_cw = false;

		//anglesNo

		if (currentFrame>anglesNo/2 && targetFrame<anglesNo/4||//6 3
			(currentFrame<anglesNo/4 && targetFrame>anglesNo/2))//3 6 
		{
			rot_cw = !rot_cw;
		}

		if (rot_cw)
		{
			RotateCW ();
		}
		else
		{
			RotateCCW ();
		}

		prevTargetFrame = targetFrame;
		fireNew = true;
	
	}

	private void RotateCW()
	{	
		if (rotatingCW||firing)
						return;

		bool afterFire = false;

		if (!previousCW)
		{
			previousCW = true;
			if(!hasFired)
				TranslateFrames ();
			else
			{
				afterFire = true;
			}
		}

		Play ("CW");

		if (afterFire) 
		{
			TranslateFrames();
			Play ("CW");
		}

		rotatingCW = true; rotatingCCW = false; hasFired = false; 
			
	}

	private void RotateCCW()
	{
		if (rotatingCCW||firing)
						return;

		bool afterFire = false;

		if (previousCW)
		{
			previousCW = false;
			if(!hasFired)
				TranslateFrames ();
			else
			{	
				afterFire = true;
			}
		}

		Play ("CCW");

		if (afterFire) 
		{
			TranslateFrames();		
			Play ("CCW");
		}

		rotatingCCW = true; rotatingCW = false; hasFired = false;

	}

	private void Play(string ext)
	{
		for (int i = 0; i < animatedSpritesAnim.Length; i++) 
		{
			currentClip[i] = animatedSprites[i]+ext;
			animatedSpritesAnim[i].PlayFromFrame(currentClip[i],lastFrame[i]);
		}
	}

	private void TranslateFrames()
	{
		for (int i = 0; i < animatedSpritesAnim.Length; i++) 
		{
			lastFrame[i] = fireAngles[i]- animatedSpritesAnim [i].CurrentFrame;	
		}
	}

	private void RecordLastFrame()
	{
		for (int i = 0; i < animatedSpritesAnim.Length; i++) 
		{
			animatedSpritesAnim[i].Stop();	
			lastFrame[i] = animatedSpritesAnim [i].CurrentFrame;	
		}
	}

	private void Stop()
	{
		if (hasFired||firing) return;		// hasfired don't overwrite with fire frames 

		rotatingCCW = false; rotatingCW = false;

		RecordLastFrame ();
	}

	private void Fire()
	{
		if (firing)	return;
		Stop();
		ExecuteFire();
	}

	public void StopAnimations()
	{
		StartCoroutine ("LateStopAnimations");
	}

	private IEnumerator LateStopAnimations()
	{
		yield return new WaitForSeconds (0.5f);
		for (int i = 0; i < animatedSpritesAnim.Length; i++) 
		{
			animatedSpritesAnim[i].Stop();			
		}
	}
	/*
	void OnGUI()//to test 
		//Do not overlap manual commands with auto target- you will desynch the animation due to
		//some consecutive delay for fire in place ()
	{

	if (GUI.Button (new Rect (5, 5, 45, 20), "Stop")) 
		{
			Stop();
		}
	if (GUI.Button (new Rect (60, 5, 45, 20), "CW")) 
		{
			RotateCW();
		}
	if (GUI.Button (new Rect (115, 5, 45, 20), "CCW")) 
		{
			RotateCCW();
		}

		if (GUI.Button (new Rect (170, 5, 45, 20), "Fire")) 
		{
			Fire();
		}

		/*
		if (GUI.Button (new Rect (260, 5, 100, 20), "FireAt3")) 
		{
			FireAtFrame(3);
		}
		*/
	//}
	
}


