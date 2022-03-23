using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct int3
{
	public int x;
	public int y;
	public int z;
	public static int3 up => new int3(0, 1, 0);
	public static int3 down => new int3(0, -1, 0);
	public static int3 right => new int3(1, 0, 0);
	public static int3 left => new int3(-1, 0, 0);
	public static int3 forward => new int3(0, 0, 1);
	public static int3 back => new int3(0, 0, -1);

	public static int3 side(int side)
    {
		switch (side)
		{
			case 0:
				return new int3(0, 1, 0);
			case 1:
				return new int3(0, -1, 0);	
			case 2:
				return new int3(1, 0, 0);	
			case 3:
				return new int3(-1, 0, 0);
			case 4:
				return new int3(0, 0, 1);
			case 5:
				return new int3(0, 0, -1);
			default:
				Debug.LogError(side+"Not vaild Side");
				return new int3(0,0,0);
				
		}

	}

	public int3(int _x, int _y, int _z)
	{
		x = _x;
		y = _y;
		z = _z;
	}
	public int3(Vector3 pos)
	{
		x = Mathf.FloorToInt(pos.x);
		y = Mathf.FloorToInt(pos.y);
		z = Mathf.FloorToInt(pos.z);
	}


	public int3 voxelToChunk()
	{
		return new int3(Mathf.FloorToInt((float)x / (float)VoxelData.chunkSize), Mathf.FloorToInt((float)y / (float)VoxelData.chunkSize), Mathf.FloorToInt((float)z / (float)VoxelData.chunkSize));
	}
	public int3 converToVoxelFromChunk(int3 voxelLocInChunk)
	{
		return new int3(x * VoxelData.chunkSize + voxelLocInChunk.x, y * VoxelData.chunkSize + voxelLocInChunk.y, z * VoxelData.chunkSize + voxelLocInChunk.z);
	}
	public int3 converToVoxelInWorldFromVoxelInChunk(int3 Chunk)
	{
		return new int3(Chunk.x * VoxelData.chunkSize + x, Chunk.y * VoxelData.chunkSize + y, Chunk.z * VoxelData.chunkSize + z);
	}
	public int3 converToVoxelInChunkFromVoxelInWold()
	{
		int3 chunk = this;
		chunk = chunk.voxelToChunk();
		return new int3(x - VoxelData.chunkSize * chunk.x, y - VoxelData.chunkSize * chunk.y, z - VoxelData.chunkSize * chunk.z);
	}
	public int3 converToVoxelInChunkFromVoxelInWold(int3 chunk)
	{
		return new int3(x - VoxelData.chunkSize * chunk.x, y - VoxelData.chunkSize * chunk.y, z - VoxelData.chunkSize * chunk.z);
	}
	public Vector3 converToVector3()
	{
		return new Vector3(x, y, z);
	}

	public Vector2Int getLocOnSide(int side)
    {
		
		switch (side)
		{
			case VoxelData.yPositive:
				return new Vector2Int(x,z);
				
			case VoxelData.yNegitive:
				return new Vector2Int(x, z);

			case VoxelData.xPositive:
				return new Vector2Int(y,z);
				
			case VoxelData.xNegitive:
				return new Vector2Int(y, z);
				
			case VoxelData.zPositive:
				return new Vector2Int(x, y);
			
			case VoxelData.zNegitive:
				return new Vector2Int(x,y);
			
			default:
				Debug.LogError(side + " side is not a side");
				return new Vector2Int(0,0);

		}
		
	}

	public int direction()
    {
		int xAbs = x;
		int yAbs = y;
		int zAbs = z;

		if (x < 0) xAbs = -x;
		if (y < 0) yAbs = -y;
		if (z < 0) zAbs = -z;

        if (xAbs > zAbs)
        {
            if (xAbs > yAbs)
            {
                if (x < 0)
                {
					return VoxelData.xNegitive;
                }
                else
                {
					return VoxelData.xPositive;
                }
            }
            else
            {
				if (y < 0)
				{
					return VoxelData.yNegitive;
				}
				else
				{
					return VoxelData.yPositive;
				}
			}
        }
        else
        {
            if (zAbs > yAbs)
            {
				if (z < 0)
				{
					return VoxelData.zNegitive;
				}
				else
				{
					return VoxelData.zPositive;
				}
			}
            else
            {
				if (y < 0)
				{
					return VoxelData.yNegitive;
				}
				else
				{
					return VoxelData.yPositive;
				}
			}
        }

	}

	

	public int3 cubeLoc(int side,int radi)
    {
		int3 temp = this;
		switch (side)
		{
			case VoxelData.yPositive:
				temp.y = radi;
				break;
			case VoxelData.yNegitive:
				temp.y = -radi;
				break;
			case VoxelData.xPositive:
				temp.x = radi;
				break;
			case VoxelData.xNegitive:
				temp.x = -radi;
				break;
			case VoxelData.zPositive:
				temp.z = radi;
				break;
			case VoxelData.zNegitive:
				temp.z = -radi;
				break;
		}
		return temp;
	}

	public int3 rotationTansformation(Rotation rotaion)
    {
		int3 temp = this;
        switch (rotaion.rotation)
        {
			case 1:
				temp.x = z;
				temp.z = -x;
				break;
			case 2:
				temp.x = -x;
				temp.z = -z;
				break;
			case 3:
				temp.x = -z;
				temp.z = x;
				break;
		}
		int3 temp2 = temp;
		switch (rotaion.face)
        {
			case VoxelData.yPositive:

				break;
			case VoxelData.yNegitive:
				temp.y = -temp2.y;
				temp.x = -temp2.x;
				break;
			case VoxelData.xPositive:
				temp.x = temp2.y;
				temp.y = -temp2.x;
				break;
			case VoxelData.xNegitive:
				temp.x =-temp2.y;
				temp.y = temp2.x;
				break;
			case VoxelData.zPositive:
				temp.z = temp2.y;
				temp.y = -temp2.z;
				break;
			case VoxelData.zNegitive:
				temp.z = -temp2.y;
				temp.y = temp2.z;
				break;

		}
		return temp;
    }

	public void log()
	{
		Debug.Log(this.x + "," + this.y + "," + this.z);
	}
	public static bool operator ==(int3 c1, int3 c2)

	{
		if (c1.x == c2.x && c1.y == c2.y && c1.z == c2.z)
		{
			return true;
		}
		return false;

	}
	public static bool operator !=(int3 c1, int3 c2)
	{
		return !(c1 == c2);
	}

	// uncomment the Equals function to resolve  
	public override bool Equals(object o)
	{
		if (this == (int3)o)
		{
			return true;
		}
        else
        {
			return false;
        }
	}
	public int abs()
    {
		return Mathf.Abs(x) + Mathf.Abs(y) + Mathf.Abs(z);

	}
	public int LargeAbs() {
		return Mathf.Max(Mathf.Abs(x), Mathf.Max(Mathf.Abs(y), Mathf.Abs(z)));
	}
	public override int GetHashCode()
	{
		return 0;
	}
	public override string ToString()
	{
		return x + "," + y + "," + z;
	}
	public static int3 operator +(int3 c1, int3 c2)
	{
		return new int3(c1.x + c2.x, c1.y + c2.y, c1.z + c2.z);
	}
	public static int3 operator -(int3 c1, int3 c2)
	{
		return new int3(c1.x - c2.x, c1.y - c2.y, c1.z - c2.z);
	}
	public static int3 operator *(int3 c1, float c2)
	{
		return new int3(new Vector3(c1.x * c2, c1.y * c2, c1.z * c2));
	}

	public static int3 operator /(int3 c1, float c2)
	{
		return new int3(new Vector3(c1.x / c2, c1.y / c2, c1.z / c2));
	}

	public static int3 operator %(int3 c1, int c2)
	{
		return new int3(c1.x % c2, c1.y % c2, c1.z % c2);
	}
}

public struct SidedAlttitude
{
	public int alttitude;
	public int depthBelow;
	public int side;
	public SidedAlttitude(int _depthrBelow,int _side, int _alttitude)
    {
		alttitude = _alttitude;
		depthBelow = _depthrBelow;
		side = _side;
    }
}

[System.Serializable]
public class ItemVoxel
{
	public int x;

	public int y;

	public int z;

	public bool isTansparent;

	public int3 pos
	{
		get { return new int3(x, y, z); }
	}

	public Color color;
}
public struct FloatInt
{
	public int Int;
	public float Float;
	public FloatInt(int _Int, float _Float)
	{
		Int = _Int;
		Float = _Float;
	}

}