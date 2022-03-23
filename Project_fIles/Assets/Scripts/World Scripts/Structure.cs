using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class Structure
{
    public int numberOfObjects=0;
    public List<Rotation> rotations= new List<Rotation>();
    public List<int> itemsIds= new List<int>();
    public List<int3> relitiveLocation= new List<int3>();

    public void addObject(int3 loc,int objectId, Rotation rotation)
    {
        numberOfObjects++;
        rotations.Add(rotation);
        itemsIds.Add(objectId);
        relitiveLocation.Add(loc);
    }
}
