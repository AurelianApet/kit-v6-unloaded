using UnityEngine;
using System.Collections;

public class TurretControllerFrameEvent : TurretControllerBase{		//frame event based - turns to a certain frame then fires
	
	//[HideInInspector]
	//public GameObject targetOb;
	private Sextant sextant;

	void Start()
	{  
		
		InitializeComponents ();
		sextant = GetComponent<Sextant> ();
		Aim ();

	}

	void Update()
	{
		
		if(fire)//Input.GetMouseButtonDown(0)
		{
			Aim(); 
			elapsedTime += Time.deltaTime;
			if (elapsedTime >= fireRate)
			{
				UpdateTarget ();              
				elapsedTime = 0.0f;		
				if(targetList.Count>0)
				{
					((Sextant)sextant).targetOb = targetList[0];
					((Sextant)sextant).fire = true;
				}
				else
				{
					((Sextant)sextant).fire = false;
				}
				//LaunchProjectile();
			}
		}

	}

	/*
	private void UpdateTarget()
	{
		if(targetOb!=null)
		currentTargetPosition = targetOb.transform.position;
	}
	*/
}
