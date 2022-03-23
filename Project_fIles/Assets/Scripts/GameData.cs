using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData : MonoBehaviour
{
    public ItemData[] _items;
    public Material _OpaqueMaterial;
    public  Material _TranparentMaterial;

    public static JobManager jobMain;
    public static ItemData[] items;
    public static Material TranparentMaterial;
    public static Material OpaqueMaterial;
    private void Awake()
    {
        jobMain = GetComponent<JobManager>();
        items = _items;

        TranparentMaterial = _TranparentMaterial;
        OpaqueMaterial = _OpaqueMaterial;
        for(int i = 0;i<items.Length;i++)
        {
            ItemData itemi = items[i];
            if (itemi != null)
            {

                itemi.createBond();
                itemi.group = new StackData(i, 0);
            }
        }
    }
 

}
