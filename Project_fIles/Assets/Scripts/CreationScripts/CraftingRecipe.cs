using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class CraftingRecipe :MonoBehaviour
{
    // Start is called before the first frame update
    public int catylistID;
    public Structure form;
    public int output;
    public bool testSpot(int3 loc, BlockData world) {
        return true;
    }
    public static int TestForRecipies(int3 loc,int item,GameObject player) {
        return 1;
    }

}

