using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class renderChunk : Itask
{
    Chunk chunk;
    BlockData data;
    int3 loc;
    public override bool Equals(object o)
    {
        renderChunk objectToCompare = o as renderChunk;
        if (objectToCompare != null)
        {
            return chunk == objectToCompare.chunk;
        }
        else
        {
            return false;
        }
    }

    public renderChunk(Chunk _chunk, BlockData _data)
    {
        chunk = _chunk;
        data = _data;

        loc = chunk.coord;
    }
    public int priority()
    {
        int3 disFromPlayer =data.chunkPlayerIsIn - loc;

        return disFromPlayer.abs()+2;
    }

    public bool ready()
    {
        return true;
    }

    public void start()
    {
        lock (chunk)
        {
            chunk.RenderChunk();
        }

        if (!data.chunksWaitingForMesh.Contains(chunk))
        {
            lock (data.chunksWaitingForMesh)
            {
                data.chunksWaitingForMesh.Enqueue(chunk);
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
