using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Rotation
{
    [SerializeField]
    byte data;
    
    public int face
    {
        get
        {
            return data / 4;
        }
        set{
            data = (byte)(rotation + value%6 * 4);
        
        }
    }
    public int rotation
    {
        get
        {
            return data % 4;
        }
        set
        {
            data = (byte)(face * 4 + (value+8) % 4);
        }
    }
    public static bool operator ==(Rotation c1, Rotation c2)
    {
        return (c1.data == c2.data);
    }
    public static bool operator !=(Rotation c1, Rotation c2)
    {
        return (c1.data != c2.data);
    }
    public override bool Equals(object o)
    {
        if (this == (Rotation)o)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public override int GetHashCode()
    {
        return 0;
    }

    public Rotation(int _face,int _rotation)
    {
        data = 0;
        face = _face;
        rotation = _rotation;
    }

    public Rotation combine(Rotation otherRotation)
    {
        Rotation finalRotation = otherRotation;
        finalRotation = finalRotation.rotateAroundY(this.rotation);
        switch (this.face)
        {
            case VoxelData.yPositive:
                
                break;
            case VoxelData.yNegitive:
                finalRotation = finalRotation.rotateAroundX(2);
                
                break;
            case VoxelData.xPositive:
                finalRotation = finalRotation.rotateAroundZ(1);
                break;
            case VoxelData.xNegitive:
                finalRotation = finalRotation.rotateAroundZ(3);
                break;
            case VoxelData.zPositive:
                
                finalRotation = finalRotation.rotateAroundX(1);
                break;
            case VoxelData.zNegitive:
      
                finalRotation = finalRotation.rotateAroundX(3);
                break;
        }
        //finalRotation.face = VoxelData.zPositive;
        return finalRotation;
        
    }
    public Rotation rotateAroundX(int numberOfTimes)
    {
        Rotation finalRotation = this;
        for (int i = 0; i < numberOfTimes; i++)
        {
            switch (finalRotation.face)
            {
                case VoxelData.yPositive:
                    finalRotation.face = VoxelData.zPositive;
                    break;
                case VoxelData.yNegitive:
                    finalRotation.face = VoxelData.zNegitive;
                    finalRotation.rotation += 2;
                    break;
                case VoxelData.xPositive:
                    finalRotation.rotation++;
                    break;
                case VoxelData.xNegitive:
                    finalRotation.rotation--;
                    break;
                case VoxelData.zPositive:
                    finalRotation.face = VoxelData.yNegitive;
                    finalRotation.rotation += 2;
                    break;
                case VoxelData.zNegitive:
                    finalRotation.face = VoxelData.yPositive;

                    
                    break;

            }
        }
        return finalRotation;
    }
    public Rotation rotateAroundY(int numberOfTimes)
    {
        Rotation finalRotation = this;
        for (int i = 0; i < numberOfTimes; i++)
        {
            switch (finalRotation.face)
            {
                case VoxelData.yPositive:
                    finalRotation.rotation--;
                    break;
                case VoxelData.yNegitive:
                    finalRotation.rotation++;
                    break;
                case VoxelData.xPositive:
                    finalRotation.face = VoxelData.zPositive;
                    finalRotation.rotation--;
                    break;
                case VoxelData.xNegitive:
                    finalRotation.face = VoxelData.zNegitive;
                   finalRotation.rotation--;
                    break;
                case VoxelData.zPositive:
                    finalRotation.face = VoxelData.xNegitive;
                    finalRotation.rotation--;
                    break;
                case VoxelData.zNegitive:
                    finalRotation.face = VoxelData.xPositive;
                    finalRotation.rotation--;
                    break;

            }
        }
        return finalRotation;
    }
    public Rotation rotateAroundZ(int numberOfTimes)
    {
        Rotation finalRotation = this;
        for (int i = 0; i < numberOfTimes; i++)
        {
            switch (finalRotation.face)
            {
                case VoxelData.yPositive:
                    finalRotation.face = VoxelData.xPositive;
                    break;
                case VoxelData.yNegitive:
                    finalRotation.face = VoxelData.xNegitive;
                    break;
                case VoxelData.xPositive:
                    finalRotation.face = VoxelData.yNegitive;
                    break;
                case VoxelData.xNegitive:
                    finalRotation.face = VoxelData.yPositive;
                    break;
                case VoxelData.zPositive:
                    finalRotation.rotation--;
                    break;
                case VoxelData.zNegitive:
                    finalRotation.rotation++;
                    break;

            }
        }
        return finalRotation;
    }
 
}
