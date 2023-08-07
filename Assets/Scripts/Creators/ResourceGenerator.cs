using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ResourceGenerator : MonoBehaviour {

	private const int noOfEconomyBuildings = 6;		//xml order: Forge Generator Vault Barrel Summon Tatami

	[HideInInspector]//this shows as empty in inspector - probably because it's a custom class?
	public EconomyBuilding[] basicEconomyValues = new EconomyBuilding[noOfEconomyBuildings]; //newly created buildings will request this - to avoid saving a lot of values

	//this shows as empty in inspector - probably because it's a custom class?
	public List<EconomyBuilding> existingEconomyBuildings = new List<EconomyBuilding>();
	public List<MessageNotification> messageNotifications = new List<MessageNotification>();

	private Component stats;

	// Use this for initialization
	void Start () {		
			
		stats = GameObject.Find("Stats").GetComponent<Stats>();	
		Initialize ();
		InvokeRepeating ("RunEconomy", 1, 1);
	}

	private void Initialize()
	{
		for (int i = 0; i < basicEconomyValues.Length; i++) 
		{
			basicEconomyValues [i] = new EconomyBuilding (); 
		}
	}

	private void RunEconomy()
	{
		bool valuesChanged = false;

		for (int i = 0; i < existingEconomyBuildings.Count; i++) 
		{
			string prodType = existingEconomyBuildings [i].ProdType;

			if (prodType != "None") 
			{
				float produce = (float)existingEconomyBuildings [i].ProdPerHour / 3600;

				switch (prodType) {
				case "Gold":
					if (existingEconomyBuildings [i].storedGold + produce < existingEconomyBuildings [i].StoreCap) 
					{
						existingEconomyBuildings [i].ModifyGoldAmount (produce);
						//if((float)existingEconomyBuildings [i].storedGold/existingEconomyBuildings [i].StoreCap>0.1f)//to display when 10% full
						if(existingEconomyBuildings [i].storedGold>1)
						DisplayHarvestNotification (i, existingEconomyBuildings [i].storedGold);					
					}					
					break;
				case "Mana":
					if (existingEconomyBuildings [i].storedMana + produce < existingEconomyBuildings [i].StoreCap) 
					{						
						existingEconomyBuildings [i].ModifyManaAmount (produce);
						//if((float)existingEconomyBuildings [i].storedMana/existingEconomyBuildings [i].StoreCap>0.1f)//to display when 10% full
						if(existingEconomyBuildings [i].storedMana>1)
						DisplayHarvestNotification (i, existingEconomyBuildings [i].storedMana);	
					}
					break;				
				}	
				valuesChanged = true;
			}			
		}
		//if (valuesChanged) UpdateUI();
	}

	public void RegisterMessageNotification(MessageNotification m)
	{
		messageNotifications.Add (m);	//ProductionBuildings.Add (building);
	}

	private void DisplayHarvestNotification(int index, float amount)
	{
		for (int i = 0; i < messageNotifications.Count; i++) {
			if (messageNotifications [i].structureIndex == index) 
			{
				if (!messageNotifications [i].isReady) 
				{
					messageNotifications [i].FadeIn ();
					messageNotifications [i].isReady = true;
				}
				messageNotifications [i].SetLabel (0, "+ " + amount.ToString("0.00"));
				break;
			}
		}
	}
	private void ResetHarvestNotification(int index)
	{
		for (int i = 0; i < messageNotifications.Count; i++) {
			if (messageNotifications [i].structureIndex == index) 
			{
				if (messageNotifications [i].isReady) 
				{
					messageNotifications [i].FadeOut ();
					messageNotifications [i].isReady = false;
				}
				break;
			}
		}
	}
	public void Harvest(int index)
	{
		for (int i = 0; i < existingEconomyBuildings.Count; i++) 
		{
			if (existingEconomyBuildings [i].structureIndex == index) 
			{
				switch (existingEconomyBuildings [i].ProdType) 
				{
				case "Gold":
					((Stats)stats).AddResources ((int)existingEconomyBuildings [i].storedGold, 0, 0);
					existingEconomyBuildings [i].storedGold -= (int)existingEconomyBuildings [i].storedGold;
					break;
				case "Mana":
					((Stats)stats).AddResources (0, (int)existingEconomyBuildings [i].storedMana, 0);
					existingEconomyBuildings [i].storedMana -= (int)existingEconomyBuildings [i].storedMana;
					break;
				}
				ResetHarvestNotification(i);
				((Stats)stats).UpdateUI ();
				break;
			}	
		}
	}
}
