using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gravity : MonoBehaviour
{

    Transform parent;
    World planet;
    Rigidbody rb;

    public bool rotate = true;
    void Start()
    {
        parent = this.transform.parent;
        if (parent) planet = this.transform.parent.gameObject.GetComponent<World>();
        else this.transform.SetParent(GameObject.Find("GameData").transform);

        rb = this.GetComponent<Rigidbody>();
        testForParentPlanet();
    }

    // Update is called once per frame
    void testForParentPlanet()
    {
        foreach (Transform child in transform.parent)
        {
            World world = child.GetComponent<World>();
            if (world)
            {
                this.transform.SetParent(world.transform);
            }
        }

    }
    void FixedUpdate()
    {
        testForParentPlanet();
        planet = this.transform.parent.gameObject.GetComponent<World>();
        
        if (planet)
        {

            if (rotate)
            {
                updateRotation();
            }

            appltGravity();
        }
   
        


    }
    void updateRotation()
    {
        switch (new int3(transform.localPosition).direction())
        {
            case VoxelData.yPositive:
                transform.rotation = Quaternion.FromToRotation(transform.up, planet.transform.up) * transform.rotation;
                break;
            case VoxelData.yNegitive:
                transform.rotation = Quaternion.FromToRotation(transform.up, -planet.transform.up) * transform.rotation;
                break;
            case VoxelData.xPositive:
                transform.rotation = Quaternion.FromToRotation(transform.up, planet.transform.right) * transform.rotation;
                break;
            case VoxelData.xNegitive:
                transform.rotation = Quaternion.FromToRotation(transform.up, -planet.transform.right) * transform.rotation;
                break;
            case VoxelData.zPositive:
                transform.rotation = Quaternion.FromToRotation(transform.up, planet.transform.forward) * transform.rotation;
                break;
            case VoxelData.zNegitive:
                transform.rotation = Quaternion.FromToRotation(transform.up, -planet.transform.forward) * transform.rotation;
                break;
        }
    }
    void appltGravity()
    {
        Vector3 gravity = planet.transform.TransformDirection(int3.side(new int3(transform.localPosition).direction()).converToVector3())*-1*planet.planetGravity;
        rb.AddForce(gravity,ForceMode.Acceleration);

        
    }

}
