using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ocean : IBiome
{
    World world;
    BlockData data;
    int oceanHeight;
    public Ocean(World _world,BlockData _data)
    {
        data = _data;
        world = _world;
        oceanHeight = world.planetRadiuse;
    }
    public float blockStrength( int3 loc)
    {
        return (float)(1);
    }

    public float getBiomWeight( int3 loc)
    {
        if(world.GetAlttitude(loc).alttitude<= oceanHeight) return  1;
        return 0;

    }

    public ChunkVoxel getVoxel( int3 loc)
    {


        if (world.GetAlttitude(loc).depthBelow<=0)
        {
            return new ChunkVoxel(new Rotation(0, 0), 1, 0);
        }
        else if (loc.LargeAbs()<= oceanHeight)
        {
            return new ChunkVoxel(new Rotation(0, 0), 3, 0);
        }
        else
        {
            return new ChunkVoxel(new Rotation(0, 0), 0, 0);
        }

  
    }

    Structure IBiome.GetStructure(int3 loc,int seed)
    {
        return null;
    }
}
