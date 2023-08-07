using UnityEngine;
using System.Collections;

public class StructureSelector : BaseSelector {//attached to each building as an invisible 2dtoolkit button
	
	// Use this for initialization
	void Start () {
		InitializeComponents ();
		InitializeSpecificComponents ();
	}

	public void ReSelect()
	{
		if(((Relay)relay).delay||((Relay)relay).pauseInput) return;

		((StructureTween)tween).Tween();
		//((SoundFX)soundFX).Click();

		if(!battleMap)
		{		
			if(!((StructureCreator)structureCreator).isReselect &&
				!((Relay)relay).pauseInput)
				{

				if (messageNotification != null&&((MessageNotification)messageNotification).isReady) 
					{
					((MessageNotification)messageNotification).FadeOut ();
					resourceGenerator.Harvest (structureIndex);
					return;
					}

					((BaseCreator)structureCreator).isReselect = true;						
					int childrenNo = gameObject.transform.childCount;//grass was parented last
					((BaseCreator)structureCreator).OnReselect(gameObject, gameObject.transform.GetChild (childrenNo-1).gameObject, structureType);	
				}
		}
		else if(structureClass=="Building"||structureClass=="Weapon")//the target select on the battle map
		{
			((Helios)helios).selectedBuildingIndex = structureIndex;
			if(((Helios)helios).DeployedUnits.Count == 0)return; //ignore if there are no units deployed
	
			int assignedToGroup = -1;
			bool userSelect = false;  //auto or user target select

			for (int i = 0; i <= ((Helios)helios).instantiationGroupIndex; i++) //((BattleProc)battleProcSc).userSelect.Length
			{			
				if(((Helios)helios).userSelect[i])
				{
					assignedToGroup = i;
					((Helios)helios).userSelect[i] = false;
					userSelect = true;
					break;
				}
			}

			if(!userSelect)
			{
				assignedToGroup = ((Helios)helios).FindNearestGroup(transform.position);//designate a group to attack this building
			}

			if(assignedToGroup == -1) return;

			if(((Helios)helios).targetBuildingIndex[assignedToGroup] != structureIndex)//if this building is not already the target of the designated group
			{
				switch (assignedToGroup) 
				{
				case 0:
					((Helios)helios).Select0();
					break;

				case 1:
					((Helios)helios).Select1();
					break;

				case 2:
					((Helios)helios).Select2();
					break;

				case 3:
					((Helios)helios).Select3();
					break;
				}

				((Helios)helios).targetBuildingIndex[assignedToGroup] = structureIndex;	//pass relevant info to BattleProc for this new target building		
				((Helios)helios).targetCenter[assignedToGroup] = transform.position;
				((Helios)helios).FindSpecificBuilding();
				((Helios)helios).updateTarget[assignedToGroup] = true;
				((Helios)helios).pauseAttack[assignedToGroup] = true;
			}

		}
	}

}
