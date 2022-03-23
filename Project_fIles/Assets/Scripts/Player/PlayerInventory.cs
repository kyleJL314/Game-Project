using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInventory : MonoBehaviour
{
    int hotBarSize = 10;
    int inventorySize = 25;
    float slotOffset = 60f;
    float yHeight = 40f;
    public Inventory inventory;
    public GameObject hotBar;
    public GameObject hiddenInventory;
    public GameObject slot;
    public int SelectedSlot;

    public bool isOpen;
    void Start()
    {
        inventory = new Inventory(hotBarSize + inventorySize);
        createInventory();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Inventory"))
        {
            toggleInventory();
        }
        if (Input.GetKey(KeyCode.Alpha0)) SelectedSlot = 9;
        if (Input.GetKey(KeyCode.Alpha1)) SelectedSlot = 0;
        if (Input.GetKey(KeyCode.Alpha2)) SelectedSlot = 1;
        if (Input.GetKey(KeyCode.Alpha3)) SelectedSlot = 2;
        if (Input.GetKey(KeyCode.Alpha4)) SelectedSlot = 3;
        if (Input.GetKey(KeyCode.Alpha5)) SelectedSlot = 4;
        if (Input.GetKey(KeyCode.Alpha6)) SelectedSlot = 5;
        if (Input.GetKey(KeyCode.Alpha7)) SelectedSlot = 6;
        if (Input.GetKey(KeyCode.Alpha8)) SelectedSlot = 7;
        if (Input.GetKey(KeyCode.Alpha9)) SelectedSlot = 8;
    }
    private void FixedUpdate()
    {
        testItemNear();
        updatedSelectedSlot();
    }
    
    
    void updatedSelectedSlot()
    {
        GameData.items[inventory.inventory[SelectedSlot].item].ItemHeld(this);
    }

    void createInventory()
    {
        int inventoryID = 0;
        
        float startLocation = -(hotBarSize-1)*slotOffset/2;
        for(int i = 0; i < hotBarSize; i++)
        {
            GameObject slots = slot.GetComponent<SlotIcon>().createSelf(hotBar.transform,this.gameObject,inventory, inventoryID);
            slots.GetComponent<RectTransform>().localPosition = new Vector3(startLocation, yHeight, 0);
            startLocation += slotOffset;

            inventoryID++;
        }
        int inventoryWidth= hotBarSize;
        int inventoryRemainder = inventorySize% inventoryWidth;
        int height = Mathf.FloorToInt(inventorySize/inventoryWidth);
        startLocation = -(inventoryRemainder - 1) * slotOffset / 2;

        for (int i =0;i< inventoryRemainder; i++)
        {
            
            GameObject slots = slot.GetComponent<SlotIcon>().createSelf(hiddenInventory.transform, this.gameObject, inventory, inventoryID);
            inventoryID++;


            slots.GetComponent<RectTransform>().localPosition = new Vector3(startLocation, (height + 1) * slotOffset+yHeight, 0);
            //slot.GetComponent<RectTransform>().pivot = new Vector3(startLocation, -.4f,0);
            startLocation += slotOffset;
        }
       
        for (int i = height; i > 0; i--)
        {
            startLocation = -(inventoryWidth - 1) * slotOffset / 2;
            for (int j = 0; j < inventoryWidth; j++)
            {
                GameObject slots = slot.GetComponent<SlotIcon>().createSelf(hiddenInventory.transform, this.gameObject, inventory, inventoryID);

                inventoryID++;

                slots.GetComponent<RectTransform>().localPosition = new Vector3(startLocation, i * slotOffset + yHeight, 0);
                //slot.GetComponent<RectTransform>().pivot = new Vector3(startLocation, -.4f,0);
                startLocation += slotOffset;
            }
        }
       
    }
    public void testItemNear()
    {
        for(int i =0;i<transform.parent.childCount;i++)
        {
            Transform child = transform.parent.GetChild(i);
            ItemData item = child.GetComponent<ItemData>();
            if (item)
            {
              //  Debug.Log("wwerwe");
                if ((item.transform.position - this.transform.position).sqrMagnitude < 30)
                {
                    AddItem(item);
                   
                }
  
            }
        }
  
    }

    public bool AddItem(ItemData item)
    {

        inventory.addItems(ref item.group);
        if (item.group.item == 0)
        {
            Destroy(item.gameObject);
        }
        upDateInventory();


        /*
        GameObject rotationObject = inventory[0].slot.transform.Find("Rotation").gameObject;
        rotationObject.transform.SetParent(inventory[0].slot.transform);
        rotationObject.transform.localPosition = Vector3.zero;
     
        rotationObject.layer = 5;
        item.transform.SetParent(rotationObject.transform);
        item.gameObject.layer = 5;
        
        item.gameObject.GetComponent<Rigidbody>().constraints =RigidbodyConstraints.FreezeAll;
        float scale = (item.largeBond - item.smallBond).converToVector3().magnitude;

        item.transform.localScale = Vector3.one*1000/ scale;
        item.transform.localPosition = new Vector3(-500f / scale, -500f / scale, -500f / scale);
        //item.transform.rotation = Quaternion.Euler(new Vector3(-15, 15, 0));
      */
        return true;

    }
    public void upDateInventory()
    {

        for (int i = 0; i < hotBarSize; i++)
        {
            inventory.inventory[i].updateSlot( hotBar.transform.GetChild(i).gameObject);


        }
        for (int i = 0; i < inventorySize; i++)
        {
           
            inventory.inventory[i+hotBarSize].updateSlot(hiddenInventory.transform.GetChild(i+1).gameObject);


        }


    }
    void toggleInventory()
    {
        if (isOpen)
        {
            closeInventory();
        }
        else
        {
            openInventory();
        }
    }
    void closeInventory()
    {
        isOpen = false;
        hiddenInventory.SetActive(false);
        isOpen = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
    void openInventory()
    {
        Cursor.lockState = CursorLockMode.None;
        isOpen = true;
        hiddenInventory.SetActive(true);
        isOpen = true;
    }
}
