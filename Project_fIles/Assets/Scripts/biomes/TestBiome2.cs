using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestBiome2 : IBiome
{
    World world;
    BlockData data;
    public float blockStrength( int3 loc)
    {
        return (float)(world.GetAlttitude(loc).depthBelow-7);
    }

    public float getBiomWeight( int3 loc)
    {
        return randomData.PerlinNoise3D((loc.cubeLoc(loc.direction(),world.planetRadiuse).converToVector3()),.02f,new Vector3(10000,-1000,2000) );

    }

    public ChunkVoxel getVoxel(int3 loc)
    {

        if (world.isVoxelAt(loc))
        {
            return new ChunkVoxel(new Rotation(0,0), 0, 0);
        }

        return new ChunkVoxel(new Rotation(0, 0), 2, 0);
    }

    Structure IBiome.GetStructure(int3 pos,int seed)
    {
        return null;
    }

    public TestBiome2(World _world,BlockData _data)
    {
        data = _data;
        world = _world;
    }
}
