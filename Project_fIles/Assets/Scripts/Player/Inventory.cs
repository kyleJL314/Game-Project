using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory 
{
    public static readonly Vector3 displayPos = new Vector3(0,0,-100);
    public StackData[] inventory;

    public Inventory(int size)
    {
        inventory = new StackData[size];

    }
    public void addItems(ref StackData itemsToAdd)
    {
        for(int i = 0; i < inventory.Length; i++)
        {
            inventory[i].addItems(ref itemsToAdd);
            if (itemsToAdd.item == 0)
            {
                break;
            }
        }
    }

}


[System.Serializable]
public struct StackData
{
    public int item;
    public int amount;
    public static StackData none = new StackData(0,0);
    public StackData(int _item, int _amount)
    {
        item = _item;
        amount = _amount;
    }
    public void addItems(ref StackData itemsToAdd)
    {
        if (itemsToAdd.item == 0)
        {
            return;
        }
        if (item != 0)
        {
            if (itemsToAdd.item == item)
            {
                if (GameData.items[itemsToAdd.item].stackSize < amount + itemsToAdd.amount)
                {
                    itemsToAdd.amount = amount + itemsToAdd.amount - GameData.items[itemsToAdd.item].stackSize;
                    amount = GameData.items[itemsToAdd.item].stackSize;
                    
                }
                else
                {
                    amount += itemsToAdd.amount;
                    itemsToAdd = StackData.none;

                }
            }
        }
        else
        {
            this = itemsToAdd;
            itemsToAdd = StackData.none;
            
        }

    }

    public bool removeItems(int count)
    {
        amount -= count;
        if (amount <= 0)
        {
            amount = 0;
            item = 0;
            return true;
        }
        return false;
    }

    public void updateSlot( GameObject slot)
    {
        ItemData currentObject = slot.transform.GetComponentInChildren<ItemData>();
        Text itemCounter = slot.transform.GetComponentInChildren<Text>(true);
        if (currentObject)
        {
           
            if (currentObject.group.item == item)
            {
                
                itemCounter.text = amount.ToString();
            }
            else
            {
                Object.Destroy(currentObject.transform.parent.gameObject);
                GameObject itemDisplay = createDisplayItem();
                if (itemDisplay)
                {
                    itemCounter.gameObject.SetActive(true);
                    itemCounter.text = amount.ToString();
                    itemDisplay.transform.SetParent(slot.transform);
                    itemDisplay.transform.localPosition = Inventory.displayPos;
                }
                else
                {
                    itemCounter.text = "0";
                }
            }

        }
        else
        {
            GameObject itemDisplay = createDisplayItem();
            if (itemDisplay)
            {
                itemCounter.gameObject.SetActive(true);
                itemCounter.text = amount.ToString();
                itemDisplay.transform.SetParent(slot.transform);
                itemDisplay.transform.localPosition = Inventory.displayPos;
                itemDisplay.transform.localScale = Vector3.one*1000;
            }
        }

    }

    public GameObject createDisplayItem()
    {
        if (item == 0) return null;
        GameObject itemDisplay = new GameObject();

        itemDisplay.transform.rotation = GameData.items[item].rotation;
        GameObject itemMesh = Object.Instantiate(GameData.items[item].gameObject, Vector3.zero, Quaternion.identity);
        ItemData itemData = itemMesh.GetComponent<ItemData>();
        itemData.applyMesh();
        itemDisplay.layer = 5;
        itemMesh.layer = 5;
        itemMesh.transform.SetParent(itemDisplay.transform);
        Object.Destroy(itemMesh.GetComponent<Rigidbody>());
        Object.Destroy(itemMesh.GetComponent<MeshCollider>());

        itemMesh.transform.localRotation = Quaternion.identity;
        float scale = calcualteScale(itemData.largeBond ,itemData.smallBond);


        Vector3 pos = -(itemData.largeBond - itemData.smallBond).converToVector3().normalized/3.4f;
      
       // pos.z -= .1f;
        itemMesh.transform.localPosition = pos;

        itemMesh.transform.localScale = Vector3.one / scale;



        return itemDisplay;
        
    }
    float calcualteScale(int3 large, int3 small)
    {
        return   (large - small).converToVector3().magnitude*1.5f;
    }
}