using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Inventory", menuName= "Inventory System/Inventory" )]
public class Inventory : ScriptableObject
{
   public List<InventorySlotFact> Facts = new List<InventorySlotFact>();
   public List<InventorySlotScroll> Scrolls = new List<InventorySlotScroll>();

   public void AddFact(ItemObject fact){
     Facts.Add(new InventorySlotFact(fact));
   }

   public void AddScroll(DefaultScroll scroll){
      Scrolls.Add(new InventorySlotScroll(scroll));
   }
}
[System.Serializable]
public class InventorySlotFact{
   public ItemObject item;
   public bool isDisplayed;

   public InventorySlotFact(ItemObject _item){
      item = _item;
      isDisplayed = false;
   }

}

[System.Serializable]
public class InventorySlotScroll{
   public DefaultScroll item;
   public bool isDisplayed;

   public InventorySlotScroll( DefaultScroll _item){
      item = _item;
      isDisplayed = false;
   }
}



