using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Sextant : MonoBehaviour{		
	    
	//[HideInInspector]
	public GameObject targetOb;
	private Vector3 target;

	public bool 
		fire = false;

	private float 
		turretRotSpeed = 0.5f,
		shootRate = 1.0f,
    	elapsedTime;

	private int 
		angle, 
		dif, 
		difMod;

	//make sure you adjust the pivot on the sprites so the rotation center is the same as the rotor; 
	//select all relevant sprites in 2DToolkit SpriteCollection anchor/custom/apply
	//12 angles {160, 125, 65, 25, -5, -30, -55, -80, -105, -130, -150, -175}
	//8 angles {}

	public int[] angles = new int[12]{160, 125, 65, 25, -5, -30, -55, -80, -105, -130, -150, -175};//rotor/sextant at 160 degrees - frame 0 facing SE
	public int anglePrecision = 24;

	private Component weaponAnimController;

    void Start()
    {
		weaponAnimController = GetComponent<WeaponAnimationController> ();
    }

    void Update()
	{        
		if(fire)//Input.GetMouseButtonDown(0)
		{
			elapsedTime += Time.deltaTime;
			if (elapsedTime >= shootRate)
			{
				UpdateTarget();
				Rotate();
				elapsedTime = 0.0f;
			}
		}
    }
    
	private void Rotate()//this calculates angle and passes it to the animator
	{	
		Vector3 targetDir = gameObject.transform.position - target;

		if (targetDir != Vector3.zero)
		{
			angle = (int)(Mathf.Atan2(targetDir.y, targetDir.x) * Mathf.Rad2Deg+10);

			int targetFrame = 0;

			for (int i = 0; i < angles.Length; i++) //cycle through angles looking for nearest value
			{
				dif = angle - angles[i];
				difMod = Mathf.Abs(dif);

				if(difMod<=anglePrecision)
				{
					//print (difMod.ToString());
					targetFrame = i;
					break;
				}
			}
			//print ("fire at frame " + targetFrame.ToString());	
			((WeaponAnimationController)weaponAnimController).UpdateTarget();
			((WeaponAnimationController)weaponAnimController).FireAtFrame(targetFrame);// all these frames are cw frames

    	}
	}

	private void UpdateTarget()
	{
		if(targetOb!=null)
		target = targetOb.transform.position;
	}

}

/*
Frame Rotation Angles
160, 125, 65, 25, -5, -30, -55, -80, -105, -130, -150, -175
BREAKING POINT -170 = 190
*/
