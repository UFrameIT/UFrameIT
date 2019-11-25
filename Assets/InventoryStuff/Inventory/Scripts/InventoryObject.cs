using System.Collections;
using System.Collections.Generic;
using UnityEngine;

   [CreateAssetMenu(fileName = "New Inventory", menuName= "Inventory System/Inventory" )]
public class InventoryObject : ScriptableObject
{
   public List<InventorySlot> Facts = new List<InventorySlot>();
   public List<InventorySlot> Scrolls = new List<InventorySlot>();

   public void AddFact(ItemObject fact){
     Facts.Add(new InventorySlot(fact));
   }

   public void AddScroll(ItemObject scroll){
      Scrolls.Add(new InventorySlot(scroll));
   }
}

   [System.Serializable]
public class InventorySlot
{
   public ItemObject item;
   public bool isDisplayed;

   public InventorySlot( ItemObject _item){
      item = _item;
      isDisplayed = false;
   }
}
