using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMouse : MonoBehaviour
{
    StackData heldItem = StackData.none;
    GameObject icon;
    PlayerInventory inventory;
    // Start is called before the first frame update
    void Start()
    {
        inventory = this.GetComponent<PlayerInventory>();
        icon = inventory.slot.GetComponent<SlotIcon>().createSelf(inventory.hiddenInventory.transform, inventory.gameObject,inventory.inventory, 101);
        Destroy(icon.GetComponent<Image>());
        Destroy(icon.GetComponent<Button>());
    }
    public void clickedOn(int id)
    {
        if (heldItem.amount <= 0) {
            heldItem.addItems(ref inventory.inventory.inventory[id]);
            heldItem.updateSlot(icon);
            Debug.Log(heldItem.item);
            inventory.upDateInventory();
        }
        else
        {
            inventory.inventory.inventory[id].addItems(ref heldItem);
            heldItem.updateSlot(icon);
            inventory.upDateInventory();
        }
    }
    // Update is called once per frame
    void Update()
    {
        Vector3 loc =inventory.hiddenInventory.transform.parent.transform.TransformPoint(Input.mousePosition);
        Vector3 pos = Camera.allCameras[1].ScreenToWorldPoint(Input.mousePosition);
        pos.z = 0;
        icon.transform.position =pos;
    }
}
