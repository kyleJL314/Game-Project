using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Threading;

public class World : MonoBehaviour
{
	public int worldSeed = 10;

	public int planetRadiuse = 100;

	public float altitudeScale = 0.01f;

	public float altitudeHeight = 30;

	public float biomeTransition = .2f;

	public float planetGravity = 9f;

	Vector3 planetOffset = new Vector3(-4000, 30000, 8000);

	public List<IBiome> biomes = new List<IBiome>();

	//public WorldStorage chunks = new WorldStorage();

	public Queue<int3> chunksToGenerate = new Queue<int3>();


	public LinkedList<int3> chunksWaitingForStructure = new LinkedList<int3>();


	WorldBiomeData biomeData;

	AltidudeData altidudeData;

	BlockData blockdata;

	void Start()
	{
		

		blockdata = this.GetComponent<BlockData>();
		//chunks = blockdata.chunks;

		biomeData = new WorldBiomeData(this);

		altidudeData = new AltidudeData(altitudeHeight,altitudeScale, planetOffset, this);

		

		biomes.Add(new TestBiome(this,blockdata));
		biomes.Add(new TestBiome2(this,blockdata));
		biomes.Add(new Ocean(this,blockdata));
		altidudeData.initialize();
		biomeData.initialize();

		

	}
	void Update()
	{
		
		MakeChunkStructure();


	}

    public void PlaceItem(int3 loc, int itemId,Rotation rotation)
	{
		for(int i=0;i< GameData.items[itemId].voxels.Length;i++)
		{
			int3 voxelLocation = loc + GameData.items[itemId].voxels[i].pos.rotationTansformation(rotation);
			if (blockdata.chunks.getChunk(voxelLocation.voxelToChunk()).voxelInfo.VoxelAt(voxelLocation.converToVoxelInChunkFromVoxelInWold()).ItemId == 0)
			{
				blockdata.chunks.getChunk(voxelLocation.voxelToChunk()).voxelInfo.VoxelAt(voxelLocation.converToVoxelInChunkFromVoxelInWold()) = new ChunkVoxel(rotation, itemId, i);
			}
		}
	}


	void MakeChunkStructure()
    {
		while (chunksToGenerate.Count != 0)
		{
			lock (chunksToGenerate)
			{
				int3 currentChunk = chunksToGenerate.Dequeue();
				if (blockdata.chunks.getChunk(currentChunk) != null)
				{
					Debug.LogWarning("double chunck being generated");
				}
				else
				{
					blockdata.chunks.getChunk(currentChunk) = new Chunk(this,blockdata, currentChunk);
				}

				//tasks.Enqueue(new generateTerrain(chunks.getChunk(currentChunk), this));
			}
		}
	}

	public SidedAlttitude GetAlttitude(int3 loc)
    {
		int side = 0;
		int height = 0;
		int alttitude = 0;


		int sideAlttitide = getAlttitudeBySide(loc, VoxelData.yPositive);
		int sideHeight = loc.y - sideAlttitide; 
		if(height< sideHeight)
        {
			alttitude = sideAlttitide;
			height = sideHeight;
			side = VoxelData.yPositive;
			
		}
		sideAlttitide = getAlttitudeBySide(loc, VoxelData.yNegitive);
		sideHeight = -loc.y - getAlttitudeBySide(loc, VoxelData.yNegitive);
		if (height < sideHeight)
		{
			alttitude = sideAlttitide;
			height = sideHeight;
			side = VoxelData.yNegitive;
		}
		sideAlttitide = getAlttitudeBySide(loc, VoxelData.xPositive);
		sideHeight = loc.x - getAlttitudeBySide(loc, VoxelData.xPositive);
		if (height < sideHeight)
		{
			alttitude = sideAlttitide;
			height = sideHeight;
			side = VoxelData.xPositive;
		}
		sideAlttitide = getAlttitudeBySide(loc, VoxelData.xNegitive);
		sideHeight = -loc.x - getAlttitudeBySide(loc, VoxelData.xNegitive);
		if (height < sideHeight)
		{
			alttitude = sideAlttitide;
			height = sideHeight;
			side = VoxelData.xNegitive;
		}
		sideAlttitide = getAlttitudeBySide(loc, VoxelData.zPositive);
		sideHeight = loc.z - getAlttitudeBySide(loc, VoxelData.zPositive);
		if (height < sideHeight)
		{
			alttitude = sideAlttitide;
			height = sideHeight;
			side = VoxelData.zPositive;
		}
		sideAlttitide = getAlttitudeBySide(loc, VoxelData.zNegitive);
		sideHeight = -loc.z - getAlttitudeBySide(loc, VoxelData.zNegitive);
		if (height < sideHeight)
		{
			alttitude = sideAlttitide;
			height = sideHeight;
			side = VoxelData.zNegitive;
			
		}
		return new SidedAlttitude(height,side, alttitude);

	} 
	public int getAlttitudeBySide(int3 loc,int side)
	{

		return altidudeData.getAltitudeBySide(loc, side);
	}

	public bool isVoxelAt(int3 loc)
    {
		BiomeInfo biome = biomeData.getBiomeAt(loc);
		float blockStrength =0;

        for (int i =0;i<biome.biomes.Count;i++)
        {
			FloatInt temp = biome.biomes[i];

			blockStrength += biomes[temp.Int].blockStrength(loc)* temp.Float;
	
        }
		return 0<blockStrength;
	}
	public ChunkVoxel getTerrainVoxel(int3 loc)
    {
		return biomes[getStrogestBiome(loc).Int].getVoxel(loc);
	}
	public FloatInt getStrogestBiome(int3 loc)
    {

		return biomeData.getStrongestBiome(loc);
	}

    public void getChunksToGenerate(Queue<int3> chunksToGenerate)
    {
        if (chunksToGenerate == null)
        {
			return;
        }
		
        while (chunksToGenerate.Count >0)
        {
			int3 chunkToRender = chunksToGenerate.Dequeue();

			lock (chunksWaitingForStructure)
			{
				if ((blockdata.chunks.getChunk(chunkToRender) == null || !blockdata.chunks.getChunk(chunkToRender).hasStructure) && !chunksWaitingForStructure.Contains(chunkToRender))
				{
					GameData.jobMain.tasks.Enqueue(new GenerateObjecets(chunkToRender, this,blockdata));
				}
			}
			
		}
    }

	private class WorldBiomeData
    {
		int radius;
		int size;
		BiomeInfo[,,] biomeData;
		World world;
        public WorldBiomeData( World _world)
        {
			world = _world;
			
			radius =(Mathf.CeilToInt(world.planetRadiuse + world.altitudeHeight));
			size = 2 * radius;
			
			biomeData = new BiomeInfo[6, size, size];
        }
		public void initialize()
        {
			for(int x = -radius; x < radius; x++)
            {
				for (int z = -radius; z < radius; z++)
				{
					biomeData[VoxelData.yPositive,x+radius,z+radius] = getBiome(new int3(x,radius,z));
				}
			}
			for (int x = -radius; x < radius; x++)
			{
				for (int z = -radius; z < radius; z++)
				{
					biomeData[VoxelData.yNegitive, x + radius, z + radius] = getBiome(new int3(x, -radius, z));
				}
			}
			for (int y = -radius; y < radius; y++)
			{
				for (int z = -radius; z < radius; z++)
				{
					biomeData[VoxelData.xPositive, y + radius, z + radius] = getBiome(new int3(radius, y, z));
				}
			}
			for (int y = -radius; y < radius; y++)
			{
				for (int z = -radius; z < radius; z++)
				{
					biomeData[VoxelData.xNegitive, y + radius, z + radius] = getBiome(new int3(-radius, y, z));
				}
			}
			for (int x = -radius; x < radius; x++)
			{
				for (int y = -radius; y < radius; y++)
				{
					biomeData[VoxelData.zPositive, x + radius, y + radius] = getBiome(new int3(x, y, radius));
				}
			}
			for (int x = -radius; x < radius; x++)
			{
				for (int y = -radius; y < radius; y++)
				{
					biomeData[VoxelData.zNegitive, x + radius, y + radius] = getBiome(new int3(x, y, -radius));
				}
			}



		}
		BiomeInfo getBiome(int3 loc)
        {
			LinkedList<FloatInt> biomes = new LinkedList<FloatInt>();
			FloatInt strongestBiome = FindStrogestBiome(loc);
			float biomeTransition = strongestBiome.Float - world.biomeTransition;

			for (int i = 0; i < world.biomes.Count; i++)
            {
				float biomeStrength = world.biomes[i].getBiomWeight(loc) - biomeTransition;
                if (biomeStrength > 0)
                {
					biomes.AddFirst(new FloatInt(i, biomeStrength));
					
				}
			}
			
			return new BiomeInfo(biomes.ToArray());
        }

		FloatInt FindStrogestBiome(int3 loc)
		{
			int strongestBiome = 0;
			float strongestBiomeWeight = 0;
			for (int i = 0; i < world.biomes.Count; i++)
			{
				float tempStrongestBiomeWeight = world.biomes[i].getBiomWeight(loc);
				if (tempStrongestBiomeWeight > strongestBiomeWeight)
				{
					strongestBiomeWeight = tempStrongestBiomeWeight;
					strongestBiome = i;
				}
			}
			return new FloatInt(strongestBiome, strongestBiomeWeight);
		}

		public FloatInt getStrongestBiome(int3 loc)
        {

			List<FloatInt> biomeAt = getBiomeAt(loc).biomes;
			int strongestBiome = 0;
			float strongestBiomeWeight = 0;
			for (int i = 0; i < biomeAt.Count; i++)
			{
				float tempStrongestBiomeWeight = biomeAt[i].Float;
				if (tempStrongestBiomeWeight > strongestBiomeWeight)
				{
					strongestBiomeWeight = tempStrongestBiomeWeight;
					strongestBiome = i;
				}
			}
			return new FloatInt(biomeAt[strongestBiome].Int, strongestBiomeWeight);
		}

		public BiomeInfo getBiomeAt(int3 loc)
        {
			int side = loc.direction();
			Vector2Int importantLoc = loc.getLocOnSide(side);
			importantLoc.x += radius;
			importantLoc.y += radius;
			if (importantLoc.x >= size)
			{
				importantLoc.x = size - 1;

			}
			else if (importantLoc.x < 0)
			{
				importantLoc.x = 0;

			}
			if (importantLoc.y >= size)
			{
				importantLoc.y = size - 1;

			}
			else if (importantLoc.y < 0)
			{
				importantLoc.y = 0;

			}
			return biomeData[side, importantLoc.x, importantLoc.y];
		}
	}

	private class AltidudeData
    {
		int radius;
		float altidudeHeight;
		float altidudeScale;
		int size;
		Vector3 offSet;
		int[,,] altidudeData;
		World world;
		public AltidudeData(float _altidudeHeight,float _altidudeScale, Vector3 _offSet, World _world)
		{

			world = _world;
			radius = (Mathf.CeilToInt(world.planetRadiuse + world.altitudeHeight));
			size = 2 * radius;

			
			
		
			altidudeScale = _altidudeScale;
			altidudeHeight = _altidudeHeight;
			altidudeData = new int[6, size, size];
			offSet = _offSet;
		}

		public int getAltitudeBySide(int3 loc, int side)
        {
			Vector2Int coords = loc.getLocOnSide(side);
			coords.x += radius;
			coords.y += radius;
			if (coords.x <0)
            {
				coords.x = 0;
            }
			else if (coords.x >= size)
            {
				coords.x = size - 1;
            }
			if (coords.y < 0)
			{
				coords.y = 0;
			}
			else if (coords.y >= size)
			{
				coords.y = size - 1;
			}
			
			return altidudeData[side, coords.x, coords.y];
		}
		public void initialize()
		{
			for (int x = -radius; x < radius; x++)
			{
				for (int z = -radius; z < radius; z++)
				{
					altidudeData[VoxelData.yPositive, x + radius, z + radius] = findAlttitude(new int3(x, radius, z));
				}
			}
			for (int x = -radius; x < radius; x++)
			{
				for (int z = -radius; z < radius; z++)
				{
					altidudeData[VoxelData.yNegitive, x + radius, z + radius] = findAlttitude(new int3(x, -radius, z));
				}
			}
			for (int y = -radius; y < radius; y++)
			{
				for (int z = -radius; z < radius; z++)
				{
					altidudeData[VoxelData.xPositive, y + radius, z + radius] = findAlttitude(new int3(radius, y, z));
				}
			}
			for (int y = -radius; y < radius; y++)
			{
				for (int z = -radius; z < radius; z++)
				{
					altidudeData[VoxelData.xNegitive, y + radius, z + radius] = findAlttitude(new int3(-radius, y, z));
				}
			}
			for (int x = -radius; x < radius; x++)
			{
				for (int y = -radius; y < radius; y++)
				{
					altidudeData[VoxelData.zPositive, x + radius, y + radius] = findAlttitude(new int3(x, y, radius));
				}
			}
			for (int x = -radius; x < radius; x++)
			{
				for (int y = -radius; y < radius; y++)
				{
					altidudeData[VoxelData.zNegitive, x + radius, y + radius] = findAlttitude(new int3(x, y, -radius));
				}
			}




		}
		int findAlttitude(int3 loc)
		{
			Vector3 location = loc.converToVector3();

			return (int)(world.planetRadiuse + altidudeHeight * (randomData.PerlinNoise3D(location, altidudeScale, offSet) * 2-1));
		}
	}

	private class BiomeInfo
    {
		public List<FloatInt> biomes = new List<FloatInt>();
		

		public void addMultipleBiomes(FloatInt[] _biomes)
        {
			biomes.AddRange(_biomes);
        }
		public BiomeInfo(FloatInt[] _biomes)
        {
			biomes.AddRange(_biomes);
		}
    }

}
public class WorldStorage
{
	List<List<List<MegaChunk>>>[] megaChunks = new List<List<List<MegaChunk>>>[8] {
	new List<List<List<MegaChunk>>>(), new List<List<List<MegaChunk>>>(), new List<List<List<MegaChunk>>>(), new List<List<List<MegaChunk>>>(),
	new List<List<List<MegaChunk>>>(), new List<List<List<MegaChunk>>>(), new List<List<List<MegaChunk>>>(), new List<List<List<MegaChunk>>>()};


	public ref Chunk getChunk(int3 chunk)
	{
		int3 megaChunk = chunk / VoxelData.megaChunkSize;
		int quadrent = getQudrentLocation(ref megaChunk);
		getQudrentLocation(ref chunk);
		chunk %= VoxelData.chunkSize;

		while (megaChunks[quadrent].Count <= megaChunk.x)
		{
			lock (megaChunks[quadrent])
			{
				megaChunks[quadrent].Add(new List<List<MegaChunk>>());
			}
		}
		while (megaChunks[quadrent][megaChunk.x].Count <= megaChunk.y)
		{
			lock (megaChunks[quadrent][megaChunk.x])
			{
				megaChunks[quadrent][megaChunk.x].Add(new List<MegaChunk>());
			}
		}
		while (megaChunks[quadrent][megaChunk.x][megaChunk.y].Count <= megaChunk.z)
		{
			lock (megaChunks[quadrent][megaChunk.x][megaChunk.y])
			{
				megaChunks[quadrent][megaChunk.x][megaChunk.y].Add(new MegaChunk());
			}
		}
		return ref megaChunks[quadrent][megaChunk.x][megaChunk.y][megaChunk.z].getChunk(chunk);

	}
	private int getQudrentLocation(ref int3 pos)
	{
		if (pos.x >= 0)
		{
			if (pos.y >= 0)
			{
				if (pos.z >= 0)
				{
					return 0;
				}
				else
				{
					pos.z = -1 * pos.z - 1;
					return 1;
				}
			}
			else
			{
				if (pos.z >= 0)
				{
					pos.y = -1 * pos.y - 1;
					return 2;
				}
				else
				{
					pos.z = -1 * pos.z - 1;
					pos.y = -1 * pos.y - 1;
					return 3;
				}
			}
		}
		else
		{
			if (pos.y >= 0)
			{
				if (pos.z >= 0)
				{
					pos.x = -1 * pos.x - 1;
					return 4;
				}
				else
				{
					pos.z = -1 * pos.z - 1;
					pos.x = -1 * pos.x - 1;
					return 5;
				}
			}
			else
			{
				if (pos.z >= 0)
				{
					pos.x = -1 * pos.x - 1;
					pos.y = -1 * pos.y - 1;
					return 6;
				}
				else
				{
					pos.z = -1 * pos.z - 1;
					pos.y = -1 * pos.y - 1;
					pos.x = -1 * pos.x - 1;
					return 7;
				}
			}
		}
	}
	private class MegaChunk
	{
		Chunk[,,] chunks;
		public ref Chunk getChunk(int3 chunk)
		{
			return ref chunks[chunk.x, chunk.y, chunk.z];

		}
		public MegaChunk()
		{
			chunks = new Chunk[VoxelData.megaChunkSize, VoxelData.megaChunkSize, VoxelData.megaChunkSize];
		}
	}

}