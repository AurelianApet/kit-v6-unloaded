using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TurretControllerBase : MonoBehaviour {

	public GameObject projectilePf;
	
	protected Transform 
		turretTr,
		projectileSpawnPointTr;

	[HideInInspector]
	public GameObject targetOb;//try to remove this var

	//Bullet shooting rate
	protected float 
		fireRate = 2.0f,
		elapsedTime;

	private float turretRotSpeed = 2.0f, turretSearchSpeed =0.05f;


	private int currentTargetIndex;
	protected Vector3 currentTargetPosition;	
	public List<GameObject> targetList = new List<GameObject> ();

	//[HideInInspector]
	public bool fire = false;

	public bool randomRotationFinished = true;
	private Vector3 randomTargetPosition;

	protected Component soundFx;

	void Start () {
		
	}

	protected void InitializeComponents()
	{
		turretTr = transform.Find("Turret");
		projectileSpawnPointTr = turretTr.Find("Tip");
		soundFx = GameObject.Find ("SoundFX").GetComponent<SoundFX> ();
		//InvokeRepeating ("CheckSearchStuck", 10, 10);
	}

	void Update () {		

	}
		
	public void Aim()
	{
		//UpdateTarget();
		Rotate();
	}

	protected void Search()
	{
		SearchRotate ();
	}

	protected void UpdateTarget()
	{
		if (targetList.Count == 0)
		{
			fire = false;
			return;
		}

		if (targetList [0] != null)
			currentTargetPosition = targetList [0].transform.position;
		else
			targetList.Remove (targetList [0]);
		
	}

	private void Rotate()				//this rotates the 3d Turret
	{
		Vector3 targetDir = turretTr.position - currentTargetPosition;
		
		if (targetDir != Vector3.zero)
		{
			
			float angle = Mathf.Atan2(targetDir.y, targetDir.x) * Mathf.Rad2Deg + 10;
			
			//turretTr.rotation = Quaternion.AngleAxis(angle, Vector3.forward); 								//instant rotation
			
			turretTr.rotation = Quaternion.Slerp(turretTr.rotation, 
			                                    Quaternion.AngleAxis(angle, Vector3.forward), 
			                                     Time.deltaTime * turretRotSpeed);						//slow gradual rotation					
		}
	}

	private void SearchRotate()
	{	
		if (randomRotationFinished) 
		{
			float rnd = Random.Range (-2000, 2000);
			randomTargetPosition = turretTr.position + new Vector3(rnd,rnd,rnd);
			randomRotationFinished = false;
			StartCoroutine ("NewRandomSearch");
		}
		else
		{
			float angle = Mathf.Atan2(randomTargetPosition.y, randomTargetPosition.x) * Mathf.Rad2Deg + 10;
			//print (angle.ToString ());
			//turretTr.rotation = Quaternion.AngleAxis(angle, Vector3.forward); 								//instant rotation

			turretTr.rotation = Quaternion.Slerp(turretTr.rotation, 
				Quaternion.AngleAxis(angle, Vector3.forward), 
				Time.deltaTime * turretSearchSpeed);						//slow gradual rotation					
		}
	}

	private IEnumerator NewRandomSearch()// the coroutine won't survive the the editor window loosing focus 
	{
		yield return new WaitForSeconds (10);
		randomRotationFinished = true;	
	}

	public void Fire()
	{
		((SoundFX)soundFx).CannonFire();   
		LaunchProjectile ();
	}

	protected void LaunchProjectile()
	{
		((SoundFX)soundFx).CannonFire();   
		//Vector3 pos = new Vector3 (projectileSpawnPointTr.position.x, projectileSpawnPointTr.position.y, currentTargetPosition.z);
		Instantiate(projectilePf, projectileSpawnPointTr.position, projectileSpawnPointTr.rotation);
	}
		
	public void AddTarget(GameObject target)
	{
		targetList.Add (target);
	}
	
	public void RemoveTarget(GameObject target)
	{
		targetList.Remove (target);
	}
}
