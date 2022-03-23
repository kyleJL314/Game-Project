using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBiome 
{

    float getBiomWeight(int3 location);

    float blockStrength(int3 location);
    ChunkVoxel getVoxel(int3 location);

    Structure GetStructure(int3 pos,int seed);


}
