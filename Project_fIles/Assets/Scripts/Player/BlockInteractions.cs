using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockInteractions : MonoBehaviour
{
    PlayerInventory inventory;
    public Transform playerDirection;
    public GameObject placeIndicator;
    RaycastHit hit;
    bool place = false;
    bool breako = false;
    bool IndicatorUsed = false;
    bool rotateCommand = false;
    bool faceCommand = false;
    Rotation currentRotatedObject = new Rotation(1,3);
    
    private void Start()
    {
        inventory = this.GetComponent<PlayerInventory>();
        
    }
    private void Update()
    {
        if (!GetComponent<PlayerInventory>().isOpen)
        {
            if (!breako) breako = Input.GetButtonDown("Fire1");
            if (!place) place = Input.GetButtonDown("Fire2");

            if (!rotateCommand) rotateCommand = Input.GetButtonDown("RotateObject");

            if (!faceCommand) faceCommand = Input.GetButtonDown("FaceObject");
        }
    }
    void FixedUpdate()
    {

        if (IndicatorUsed)
        {
            IndicatorUsed = false;
        }
        else
        {
            placeIndicator.SetActive(false);
        }


        if (rotateCommand)
        {
            currentRotatedObject.rotation++;
            rotateCommand = false;
        }
        if (faceCommand)
        {
            currentRotatedObject.face++;
            faceCommand = false;
        }
    }
    public void breakblock()
    {
        if (!breako)
        {
            return;
        }
        if (Physics.Raycast(playerDirection.position, playerDirection.TransformDirection(Vector3.forward), out hit, Mathf.Infinity))
        {
            Debug.DrawRay(playerDirection.position, playerDirection.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);

            BlockData worldhit = hit.collider.gameObject.GetComponentInParent<BlockData>();

            if (worldhit != null)
            {

                Vector3 blockhit = hit.point + hit.normal * -.1f;
                

                worldhit.BreakItem(new int3(worldhit.transform.InverseTransformPoint(blockhit)));

            }

        }
        else
        {
            Debug.DrawRay(playerDirection.position, playerDirection.TransformDirection(Vector3.forward) * 1000, Color.white);

        }
        breako = false;
    }
    public bool placeBlock(int placeObjectId)
    {
        if (!place)
        {
            return false;
        }
        if (Physics.Raycast(playerDirection.position, playerDirection.TransformDirection(Vector3.forward), out hit, Mathf.Infinity))
        {
            Debug.DrawRay(playerDirection.position, playerDirection.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);

            BlockData worldhit = hit.collider.gameObject.GetComponentInParent<BlockData>();
            
            if (worldhit != null)
            {

                Vector3 blockhit = hit.point + hit.normal * .1f;
                int3 offset = offsetcalcuator(hit.normal,worldhit, placeObjectId, currentRotatedObject);

                worldhit.PlaceItem(new int3(worldhit.transform.InverseTransformPoint(blockhit))+offset, placeObjectId, currentRotatedObject);

            }

        }
        else
        {
            Debug.DrawRay(playerDirection.position, playerDirection.TransformDirection(Vector3.forward) * 1000, Color.white);

        }
        place = false;
        return true;
    }
    public void updateIndicator(int placeObjectId)
    {
        placeIndicator.SetActive(true);
        IndicatorUsed = true;

        if (Physics.Raycast(playerDirection.position, playerDirection.TransformDirection(Vector3.forward), out hit, Mathf.Infinity))
        {
            Debug.DrawRay(playerDirection.position, playerDirection.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);

            BlockData worldhit = hit.collider.gameObject.GetComponentInParent<BlockData>();

            if (worldhit != null)
            {

                Vector3 blockhit = hit.point + hit.normal * .1f;
                int3 offset = offsetcalcuator(hit.normal, worldhit, placeObjectId, currentRotatedObject);
                //placeIndicator.transform.rotation = worldhit.transform.rotation;
                placeIndicator.transform.position = worldhit.GlobalSpaceToBlockLocation(blockhit)+offset.converToVector3();
                placeIndicator.transform.SetParent(worldhit.transform);
                createMesh(placeObjectId);
                
                placeIndicator.transform.localRotation = Quaternion.AngleAxis(currentRotatedObject.rotation * 90, new Vector3(0, 1, 0));
                if (currentRotatedObject.face == VoxelData.yNegitive)
                {
                    placeIndicator.transform.localRotation = Quaternion.FromToRotation(new Vector3(0, 1, 0), int3.side(VoxelData.xPositive).converToVector3()) * placeIndicator.transform.localRotation;
                    placeIndicator.transform.localRotation = Quaternion.FromToRotation(new Vector3(1, 0, 0), int3.side(currentRotatedObject.face).converToVector3()) * placeIndicator.transform.localRotation;
                }
                else
                {
                    placeIndicator.transform.localRotation = Quaternion.FromToRotation(new Vector3(0, 1, 0), int3.side(currentRotatedObject.face).converToVector3()) * placeIndicator.transform.localRotation;
                }
            }

        }
        else
        {
            Debug.DrawRay(playerDirection.position, playerDirection.TransformDirection(Vector3.forward) * 1000, Color.white);

        }
    
    }
   
    
    void createMesh(int placeObjectId)
    {

        ItemData objectData = GameData.items[placeObjectId];
        GameObject temp = placeIndicator.transform.GetChild(0).gameObject;
        Mesh mesh = objectData.CreateMesh();
        mesh.RecalculateNormals();
        MeshFilter meshFilter = temp.GetComponent<MeshFilter>();
        MeshRenderer meshRenderer = temp.GetComponent<MeshRenderer>();
        if (!meshFilter)
        {
            meshFilter = temp.AddComponent<MeshFilter>();
            meshRenderer = temp.AddComponent<MeshRenderer>();
        }
        meshFilter.mesh = mesh;
        Material[] materials = new Material[2];
        materials[0] = objectData.Opaquematerial;
        materials[1] = objectData.Transparentmaterial;
        meshRenderer.materials = materials;
    }
    public int3 offsetcalcuator(Vector3 normal,BlockData world,int objectId,Rotation objectRotation)
    {
        return new int3(0, 0, 0);
        /*
        Vector3 localNorm = world.transform.InverseTransformDirection(normal);
        
        int3 smallFinal = GameData.objects[objectId].smallBond.rotationTansformation(objectRotation);
        int3 largFinal = GameData.objects[objectId].largeBond.rotationTansformation(objectRotation);
        switch(new int3(localNorm).direction())
        {
            case (VoxelData.yPositive):
                return new int3(0, -1 * Mathf.Min(smallFinal.y, largFinal.y), 0);

            case (VoxelData.yNegitive):
                return new int3(0, -1 * Mathf.Max(smallFinal.y, largFinal.y), 0);

            case (VoxelData.xPositive):
                return new int3(-1 * Mathf.Min(smallFinal.x, largFinal.x),0, 0);
            case (VoxelData.xNegitive):
                return new int3(-1 * Mathf.Max(smallFinal.x, largFinal.x), 0, 0);
            case (VoxelData.zPositive):
                return new int3(0, 0, -1 * Mathf.Min(smallFinal.z, largFinal.z));

            case (VoxelData.zNegitive):
                return new int3(0,0, -1 * Mathf.Max(smallFinal.z, largFinal.z));

        }
        return new int3(0, 0, 0);*/

    }

}
