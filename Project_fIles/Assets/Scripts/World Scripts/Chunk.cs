using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk
{
    public ChunkStorage voxelInfo = new ChunkStorage();

    public bool hasTerrain = false;
    public bool isBeingWorkedOn = false;
    public bool hasStructure = false;
    public bool isRendering = false;
    MeshRenderer meshRenderer;
    MeshFilter meshFilter;
    MeshCollider meshCollider;
    List<Color> colors = new List<Color>();
    List<Vector3> vertices = new List<Vector3>();
    List<int> OpaqueTriangles = new List<int>();
    List<int> TransparentTriangles = new List<int>();


    Material[] materials = new Material[2];
    int vertexIndex = 0;


    World world;
    public BlockData data;
    public int3 coord;
    GameObject chunkObject;

    public Chunk(World _world, BlockData _data,int3 _coord)
    {
        world = _world;
        data = _data;
        coord = _coord;
        chunkObject = new GameObject();

        meshFilter = chunkObject.AddComponent<MeshFilter>();
        meshRenderer = chunkObject.AddComponent<MeshRenderer>();
        meshCollider = chunkObject.AddComponent<MeshCollider>();
        materials[0] = GameData.OpaqueMaterial;
        materials[1] = GameData.TranparentMaterial;
        meshRenderer.materials = materials;

        chunkObject.transform.SetParent(world.transform.Find("Chunks"));
        chunkObject.transform.localPosition = new Vector3(coord.x * VoxelData.chunkSize, coord.y * VoxelData.chunkSize, coord.z * VoxelData.chunkSize);
        chunkObject.transform.rotation = world.transform.rotation;
        chunkObject.name = coord.ToString();

    }

    public void RenderChunk()
    {
        clearMesh();
        for (int x = 0; x < VoxelData.chunkSize; x++)
        {
            for (int y = 0; y < VoxelData.chunkSize; y++)
            {
                for (int z = 0; z < VoxelData.chunkSize; z++)
                {
                    ChunkVoxel voxel = voxelInfo.VoxelAt(new int3(x, y, z));
                    if (!voxel.isAir())
                    {
                        if (!voxel.isTransparent())
                        {
                            RenderBlockOpaque(new int3(x, y, z));
                        }
                        else
                        {
                            RenderBlockTransparent(new int3(x, y, z));
                        }
                    }

                }
            }
        }


    }

    void RenderBlockOpaque(int3 block)
    {
        for (int p = 0; p < 6; p++)
        {
            Vector3 pos = block.converToVector3();
            if (!IsAirAt(new int3(pos + VoxelData.faceChecks[p])) || voxelInfo.VoxelAt(new int3(pos + VoxelData.faceChecks[p])).isTransparent())
            {
          
                CreateOpaqueFace(pos, p, voxelInfo.VoxelAt(block).GetColor());
            }

        }
    }

    void CreateOpaqueFace(Vector3 pos, int p, Color color)
    {

        vertices.Add(pos + VoxelData.voxelVerts[VoxelData.voxelTris[p, 0]]);
        vertices.Add(pos + VoxelData.voxelVerts[VoxelData.voxelTris[p, 1]]);
        vertices.Add(pos + VoxelData.voxelVerts[VoxelData.voxelTris[p, 2]]);
        vertices.Add(pos + VoxelData.voxelVerts[VoxelData.voxelTris[p, 3]]);


        colors.Add(voxelInfo.VoxelAt(new int3(pos)).GetColor());
        colors.Add(voxelInfo.VoxelAt(new int3(pos)).GetColor());
        colors.Add(voxelInfo.VoxelAt(new int3(pos)).GetColor());
        colors.Add(voxelInfo.VoxelAt(new int3(pos)).GetColor());


        OpaqueTriangles.Add(vertexIndex);
        OpaqueTriangles.Add(vertexIndex + 1);
        OpaqueTriangles.Add(vertexIndex + 2);
        OpaqueTriangles.Add(vertexIndex + 2);
        OpaqueTriangles.Add(vertexIndex + 1);
        OpaqueTriangles.Add(vertexIndex + 3);
        vertexIndex += 4;
    }

    void RenderBlockTransparent(int3 block)
    {
        for (int p = 0; p < 6; p++)
        {
            Vector3 pos = block.converToVector3();
            ChunkVoxel blockNext = BlockAt(new int3(pos + VoxelData.faceChecks[p]));
            if (blockNext.isAir())
            {
                CreateTransparentFace(pos, p, voxelInfo.VoxelAt(block).GetColor());
            }
        }
    }

    void CreateTransparentFace(Vector3 pos, int p, Color color)
    {

        vertices.Add(pos + VoxelData.voxelVerts[VoxelData.voxelTris[p, 0]]);
        vertices.Add(pos + VoxelData.voxelVerts[VoxelData.voxelTris[p, 1]]);
        vertices.Add(pos + VoxelData.voxelVerts[VoxelData.voxelTris[p, 2]]);
        vertices.Add(pos + VoxelData.voxelVerts[VoxelData.voxelTris[p, 3]]);


        colors.Add(voxelInfo.VoxelAt(new int3(pos)).GetColor());
        colors.Add(voxelInfo.VoxelAt(new int3(pos)).GetColor());
        colors.Add(voxelInfo.VoxelAt(new int3(pos)).GetColor());
        colors.Add(voxelInfo.VoxelAt(new int3(pos)).GetColor());


        TransparentTriangles.Add(vertexIndex);
        TransparentTriangles.Add(vertexIndex + 1);
        TransparentTriangles.Add(vertexIndex + 2);
        TransparentTriangles.Add(vertexIndex + 2);
        TransparentTriangles.Add(vertexIndex + 1);
        TransparentTriangles.Add(vertexIndex + 3);
        vertexIndex += 4;
    }

    void clearMesh()
    {
        vertexIndex = 0;
        vertices.Clear();
        colors.Clear();
        //uvs.Clear();
        TransparentTriangles.Clear();
        OpaqueTriangles.Clear();
    }

    bool IsAirAt(int3 pos)
    {

        if (IsVoxelInChunk(pos))
        {
            return !voxelInfo.VoxelAt(pos).isAir();
        }
        else
        {
            return false;
        }
    }
    ChunkVoxel BlockAt(int3 pos)
    {
        if (IsVoxelInChunk(pos))
        {
            return voxelInfo.VoxelAt(pos);
        }
        else
        {
            return data.voxelAt(pos.converToVoxelInWorldFromVoxelInChunk(coord));
        }
    }
    bool IsVoxelInChunk(int3 pos)
    {
        if (pos.x < 0 || pos.x >= VoxelData.chunkSize || pos.y < 0 || pos.y >= VoxelData.chunkSize || pos.z < 0 || pos.z >= VoxelData.chunkSize)
        {
            return false;

        }
        else
        {
            return true;

        }
    }

    public void GenerateTerrian()
    {
        for (int x = 0; x < VoxelData.chunkSize; x++)
        {
            for (int y = 0; y < VoxelData.chunkSize; y++)
            {
                for (int z = 0; z < VoxelData.chunkSize; z++)
                {
                    voxelInfo.VoxelAt(new int3(x, y, z)) = world.getTerrainVoxel(new int3(x,y,z).converToVoxelInWorldFromVoxelInChunk(coord));
                }
            }
        }
        if (hasTerrain)
        {
            Debug.LogWarning("double terrian at"+coord);
        }

    }
    int objectSeedforChunk()
    {
        
        System.Random random = new System.Random(coord.x);
        int y = random.Next() + coord.y;
        random = new System.Random(coord.y);
        int z = random.Next() + coord.z;
        random = new System.Random(coord.z);
        return random.Next();

    }
    int3[] objectLocations()
    {
        System.Random random = new System.Random(objectSeedforChunk());
        const int num = 500;
        int3[] loc = new int3[num];
        for (int i = 0; i < num; i++)
        {
            loc[i] = new int3(random.Next(0, VoxelData.chunkSize), random.Next(0, VoxelData.chunkSize), random.Next(0, VoxelData.chunkSize));
        }
        return loc;
    }
    public void generateObjects()
    {
        int3[] loc = objectLocations();
        for (int i = 0; i < loc.Length; i++)
        {
            int biome = world.getStrogestBiome(loc[i].converToVoxelInWorldFromVoxelInChunk(coord)).Int;
            Structure structure = world.biomes[biome].GetStructure(loc[i].converToVoxelInWorldFromVoxelInChunk(coord), 0);
            if (structure != null)
            {
                placeStructure(structure, loc[i], new Rotation(loc[i].converToVoxelInWorldFromVoxelInChunk(coord).direction(),i));

            }


        }
    }
    void placeStructure(Structure structure,int3 pos,Rotation rotation)
    {
        
        for(int i = 0; i < structure.numberOfObjects; i++)
        {
            Rotation relitiveRotation = rotation.combine(structure.rotations[i]);
            world.PlaceItem(pos.converToVoxelInWorldFromVoxelInChunk(coord)+structure.relitiveLocation[i].rotationTansformation(rotation),structure.itemsIds[i], relitiveRotation);
        }
       // voxelInfo.VoxelAt(pos) = new ChunkVoxel(0, 0, 5, 0);
    }


    public void CreateMesh()
    {
        
        Mesh mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.subMeshCount = 2;

        mesh.SetTriangles(OpaqueTriangles.ToArray(),0);
        mesh.SetTriangles(TransparentTriangles.ToArray(), 1);

        mesh.colors = colors.ToArray();
       // mesh.uv = uvs.ToArray();

        mesh.RecalculateNormals();

        meshFilter.mesh = mesh;
        meshCollider.sharedMesh = meshFilter.mesh;

    }


}




[System.Serializable]
public class ChunkStorage
{
    ChunkVoxel[,,] voxels;
    public int3 location;
    public ref ChunkVoxel VoxelAt(int3 pos)
    {
        return ref voxels[pos.x, pos.y, pos.z];
    }
    public ChunkStorage()
    {
        voxels = new ChunkVoxel[VoxelData.chunkSize, VoxelData.chunkSize, VoxelData.chunkSize];
    }

}

public struct ChunkVoxel
{
    public Rotation rotation;

    public ushort ItemId;
    public ushort voxelID;

    public Color GetColor()
    {
        return GameData.items[ItemId].voxels[voxelID].color;
    }
    public int3 GetRealitiveLocation()
    {
        return new int3(0, 0, 0);
    }
    public bool isAir()
    {
        if (ItemId == 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public bool isTransparent()
    {
        return GameData.items[ItemId].voxels[voxelID].isTansparent;
    }
    public bool isSolid()
    {
        return true;
    }
    
        public ChunkVoxel(Rotation _rotation, int _objectID, int _voxelID)
    {
        ItemId = (ushort)_objectID;
        voxelID = (ushort)_voxelID;
        rotation = _rotation;
    }
}