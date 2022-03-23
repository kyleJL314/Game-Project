using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*public class generateTerrain : Itask
{
    public Chunk chunk;
    World world;
    public generateTerrain(Chunk _chunk, World _world)
    {
        chunk = _chunk;
        world = _world;
    }

    bool Itask.ready()
    {
        return true;
    }

    void Itask.start()
    {
        chunk.GenerateTerrian();
        chunk.RenderChunk();
        lock (world.chunksWaitingForMesh)
        {
            world.chunksWaitingForMesh.Enqueue(chunk);
        }
    }
}*/