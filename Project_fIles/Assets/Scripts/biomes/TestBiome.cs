using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestBiome : IBiome
{
    World world;
    BlockData data;
    public float blockStrength(int3 loc)
    {
        return world.GetAlttitude(loc).depthBelow;
    }

    public float getBiomWeight(int3 location)
    {
        return .5f;
    }

    public ChunkVoxel getVoxel(int3 loc)
    {

        
        if(world.isVoxelAt(loc))
        {
            return new ChunkVoxel(new Rotation(0, 0), 0, 0);
        }
       
        return new ChunkVoxel(new Rotation(0, 0), 1,0);
    }

    Structure IBiome.GetStructure(int3 pos,int seed)
    {
        if (isValidObject(pos))
        {
            Structure tmep = new Structure();
            tmep.addObject(new int3(0,0,0) , 4, new Rotation(0,0));
            tmep.addObject(new int3(0, 3, 0), 4, new Rotation(VoxelData.xPositive, 0));
            //tmep.addObject(new int3(0, 4, 0), 4, new Rotation(3,0));

            return tmep;
        }
        return null;
    }
    public bool isValidObject(int3 pos)
    {

        int3 sidepos = pos + int3.side(pos.direction() - pos.direction() % 2 * 2 + 1);
        int3 sideChunk = sidepos.voxelToChunk();
        if (!data.voxelAt(sidepos).isAir()&& data.voxelAt(pos).isAir())
        {
            return true;
        }

        return false;
    }

    public TestBiome(World _world,BlockData _data)
    {
        data = _data;
        world = _world;
    }


}
