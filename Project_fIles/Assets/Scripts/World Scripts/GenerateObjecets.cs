using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateObjecets : Itask
{
    int3 chunkLoc;
    World world;
    BlockData data;
    public GenerateObjecets(int3 _chunk, World _world,BlockData _data)
    {
        chunkLoc = _chunk;
        world = _world;
        data = _data;
    }
    public override bool Equals(object o)
    {
        GenerateObjecets objectToCompare = o as GenerateObjecets;
        if (objectToCompare != null)
        {
            return chunkLoc == objectToCompare.chunkLoc;
        }
        else
        {
            return false;
        }
    }
    public bool ready()
    {
        return true;
    }
    public int priority()
    {
        int3 disFromPlayer = data.chunkPlayerIsIn - chunkLoc;

        return disFromPlayer.abs();
    }
    public void start()
    {
     
        bool isReady = false;
        while (!isReady)
        {
            isReady = true;
            for (int x = 1; x >= -1; x--)
            {
                for (int y = 1; y >= -1; y--)
                {
                    for (int z = 1; z >= -1; z--)
                    {
                        int3 location = chunkLoc + new int3(x, y, z);
                        Chunk chunk = data.chunks.getChunk(location);
                        if(chunk == null)
                        {
                          
                            lock (world.chunksToGenerate)
                            {
                                if (!world.chunksToGenerate.Contains(location) && data.chunks.getChunk(location) == null)
                                {
                                   
                                    world.chunksToGenerate.Enqueue(location);
                                }
                            }
                            isReady = false;
                        }
                        else if(!chunk.hasTerrain)
                        {
                          
                            isReady = false;
                            if (!chunk.isBeingWorkedOn)
                            {

                                chunk.isBeingWorkedOn = true;
                                chunk.GenerateTerrian();
                                chunk.hasTerrain = true;
                                chunk.isBeingWorkedOn = false;
                            }
                            else
                            {
                                isReady = false;
                            }

                        }
                    }
                }
            }
        }

        Chunk objChunk = data.chunks.getChunk(chunkLoc);
        objChunk.generateObjects();
        objChunk.hasStructure = true;

        lock (world.chunksWaitingForStructure)
        {
            world.chunksWaitingForStructure.Remove(chunkLoc);
        }
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                for (int z = -1; z <= 1; z++)
                {
                    Chunk temp = data.chunks.getChunk(chunkLoc+ new int3(x,y,z));
                    renderChunk chunkToRender = new renderChunk(temp, data);
                    GameData.jobMain.tasks.Enqueue(chunkToRender);
                    /*lock (temp)
                    {
                        temp.RenderChunk();
                    }
                    lock (world.chunksWaitingForMesh)
                    {
                        if (!world.chunksWaitingForMesh.Contains(temp))
                        {
                            world.chunksWaitingForMesh.Enqueue(temp);
                        }
                    }*/
                }
            }
        }

    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public override string ToString()
    {
        return base.ToString();
    }
}
