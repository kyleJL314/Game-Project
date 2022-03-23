using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotIcon : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject player;
    public Inventory inventory;
    public int id;
    public void onClick() {
        //Debug.Log(id);
        player.GetComponent<PlayerMouse>().clickedOn(id);
    }
    public GameObject createSelf(Transform parent,GameObject player,Inventory inventory,int id)
    {

        GameObject slots = Instantiate(this.gameObject, parent);
        SlotIcon slot = slots.GetComponent<SlotIcon>();
        slot.player = player;
        slot.inventory = inventory;
        slot.id = id;
        slots.name = "slot " + id.ToString();
        return slots;
        
    }
}
