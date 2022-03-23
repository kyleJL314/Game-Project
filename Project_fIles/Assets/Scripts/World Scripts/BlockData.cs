using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockData : MonoBehaviour
{
	public WorldStorage chunks = new WorldStorage();

	public Queue<int3> chunksToGenerate = new Queue<int3>();

	Queue<int3> chunksToRender = new Queue<int3>();
	public Queue<Chunk> chunksWaitingForMesh = new Queue<Chunk>();

	Vector3 playerlocation = new Vector3(0, 0, 0);
	int3 chunkPlayerLastIn = new int3(100, 100, 100);
	public int3 chunkPlayerIsIn
	{
		get
		{
			return new int3(playerlocation).voxelToChunk();
		}
	}
	// Start is called before the first frame update
	public GameObject player;

	World world;
	void Start()
    {
		world = this.gameObject.GetComponent<World>();
	}

    // Update is called once per frame
    void Update()
    {
		playerlocation = player.transform.localPosition;
		for (int i = 0; i < 3; i++)
		{
			if (chunksWaitingForMesh.Count != 0)
			{
				lock (chunksWaitingForMesh)
				{
					Chunk temp = chunksWaitingForMesh.Dequeue();
					lock (temp)
					{
						temp.CreateMesh();

					}
				}
			}
		}
		if (chunkPlayerIsIn != chunkPlayerLastIn)
		{
			chunkPlayerLastIn = chunkPlayerIsIn;
			getChunksToLoad();
			loadChunks();
		}
	}
	void getChunksToLoad()
	{
		for (int x = chunkPlayerIsIn.x - VoxelData.viewDistance; x <= chunkPlayerIsIn.x + VoxelData.viewDistance; x++)
		{
			for (int y = chunkPlayerIsIn.y - VoxelData.viewDistance; y <= chunkPlayerIsIn.y + VoxelData.viewDistance; y++)
			{
				for (int z = chunkPlayerIsIn.z - VoxelData.viewDistance; z <= chunkPlayerIsIn.z + VoxelData.viewDistance; z++)
				{
					chunksToRender.Enqueue(new int3(x, y, z));
				}
			}
		}
	}
	void loadChunks()
    {
		if (world)
		{
			world.getChunksToGenerate(chunksToRender);

		}
	}
	public Vector3 GlobalSpaceToBlockLocation(Vector3 globallocation)
	{
		return transform.TransformPoint(new int3(transform.InverseTransformPoint(globallocation)).converToVector3() + new Vector3(.5f, .5f, .5f));
	}
	public void PlaceItem(int3 loc, int itemId, Rotation rotation)
	{
		GenerateItem(loc, itemId, rotation);
		int3 chunkloc = loc.voxelToChunk();
		for (int x = -1; x <= 1; x++)
		{
			for (int y = -1; y <= 1; y++)
			{
				for (int z = -1; z <= 1; z++)
				{
					
					lock(chunks.getChunk(chunkloc + new int3(x, y, z)))
                    {
						chunks.getChunk(chunkloc + new int3(x, y, z)).RenderChunk();
						chunks.getChunk(chunkloc + new int3(x, y, z)).CreateMesh();
					}
					
				}
			}
		}

	}
	public void GenerateItem(int3 loc, int itemId, Rotation rotation)
	{
		for (int i = 0; i < GameData.items[itemId].voxels.Length; i++)
		{
			int3 voxelLocation = loc + GameData.items[itemId].voxels[i].pos.rotationTansformation(rotation);
			if (chunks.getChunk(voxelLocation.voxelToChunk()).voxelInfo.VoxelAt(voxelLocation.converToVoxelInChunkFromVoxelInWold()).ItemId == 0)
			{
				chunks.getChunk(voxelLocation.voxelToChunk()).voxelInfo.VoxelAt(voxelLocation.converToVoxelInChunkFromVoxelInWold()) = new ChunkVoxel(rotation, itemId, i);
			}
		}
	}
	public void BreakItem(int3 loc)
	{
		ChunkVoxel temp = voxelAt(loc);
		DestroyItem(loc);
		int3 chunkloc = loc.voxelToChunk();

		for (int x = -1; x <= 1; x++)
		{
			for (int y = -1; y <= 1; y++)
			{
				for (int z = -1; z <= 1; z++)
				{
					
					lock(chunks.getChunk(chunkloc + new int3(x, y, z)))
                    {
						chunks.getChunk(chunkloc + new int3(x, y, z)).RenderChunk();
						chunks.getChunk(chunkloc + new int3(x, y, z)).CreateMesh();
					}
					
				}
			}
		}
		
		ItemData itemData = GameData.items[temp.ItemId];
		GameObject dropedItem = itemData.ItemDrop();
		if (dropedItem != null)
		{
			placeItemAt(loc, dropedItem);
		}

	}
	public void placeItemAt(int3 loc, GameObject item)
	{
		GameObject groundItem = Instantiate(item, loc.converToVector3(), Quaternion.identity);
		groundItem.GetComponent<ItemData>().applyMesh();
	}
	public void DestroyItem(int3 loc)
	{
		ChunkVoxel temp = voxelAt(loc);
		ItemData itemData = GameData.items[temp.ItemId];
		int3 originPos = loc - itemData.voxels[temp.voxelID].pos.rotationTansformation(temp.rotation);
		for (int i = 0; i < itemData.voxels.Length; i++)
		{
			int3 voxelLocation = originPos + itemData.voxels[i].pos.rotationTansformation(temp.rotation);
			ChunkVoxel voxel = chunks.getChunk(voxelLocation.voxelToChunk()).voxelInfo.VoxelAt(voxelLocation.converToVoxelInChunkFromVoxelInWold());
			if (voxel.rotation == temp.rotation && voxel.ItemId == temp.ItemId && voxel.voxelID == i)
			{
				chunks.getChunk(voxelLocation.voxelToChunk()).voxelInfo.VoxelAt(voxelLocation.converToVoxelInChunkFromVoxelInWold()) = new ChunkVoxel(new Rotation(0, 0), 0, 0);
			}

		}




	}

	public ChunkVoxel voxelAt(int3 voxelLocation)
	{
		if (chunks.getChunk(voxelLocation.voxelToChunk()) != null)
		{
			return chunks.getChunk(voxelLocation.voxelToChunk()).voxelInfo.VoxelAt(voxelLocation.converToVoxelInChunkFromVoxelInWold());
		}
        else if(world)
        {
			return world.getTerrainVoxel(voxelLocation);
        }
        else
        {
			return new ChunkVoxel(new Rotation(0, 0), 0, 0);
        }
	}
	/*
	public int breakObjectAt(int3 voxelLocation)
	{
		int voxelID = voxelAt(voxelLocation).voxelID;
		chunks.getChunk(voxelLocation.voxelToChunk()).voxelInfo.VoxelAt(voxelLocation.converToVoxelInChunkFromVoxelInWold()) = new ChunkVoxel(new Rotation(0, 0), 0, 0);
		return voxelID;
	}
	*/
	public void addChunk(Chunk chunk)
    {
		chunks.getChunk(chunk.coord) = chunk;
    }

	
}
