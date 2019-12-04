using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Inventory", menuName= "Inventory System/Inventory" )]
public class Inventory : ScriptableObject
{
   public List<InventorySlot<ItemObject>> Facts = new List<InventorySlot<ItemObject>>();
   public List<InventorySlot<DefaultScroll>> Scrolls = new List<InventorySlot<DefaultScroll>>();

   public void AddFact(ItemObject fact){
     Facts.Add(new InventorySlot<ItemObject>(fact));
   }

   public void AddScroll(DefaultScroll scroll){
      Scrolls.Add(new InventorySlot<DefaultScroll>(scroll));
   }
}

   [System.Serializable]
public class InventorySlot<T>
{
   public T  item;
   public bool isDisplayed;

   public InventorySlot( T _item){
      item = _item;
      isDisplayed = false;
   }
}
