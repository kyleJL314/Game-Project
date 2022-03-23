using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
public class Items : MonoBehaviour
{
	public ObjectVoxel[] voxels;

	public Material Opaquematerial;
	public Material Transparentmaterial;

	public int3 smallBond;
	public int3 largeBond;

    public void FixedUpdate()
    {
        
    }
    public void Start()
    {
		createBond();
    }
    public void createBond()
	{
		int smallX = 0;
		int largeX = 0;

		int smallY = 0;
		int largeY = 0;

		int smallZ = 0;
		int largeZ = 0;
		for (int i = 0; i < voxels.Length; i++)
		{
			if (voxels[i].x < smallX)
			{
				smallX = voxels[i].x;

			}
			if (voxels[i].x > largeX)
			{
				largeX = voxels[i].x;

			}

			if (voxels[i].y < smallY)
			{
				smallY = voxels[i].y;

			}
			if (voxels[i].y > largeY)
			{
				largeY = voxels[i].y;

			}

			if (voxels[i].z < smallZ)
			{
				smallZ = voxels[i].z;

			}
			if (voxels[i].z > largeZ)
			{
				largeZ = voxels[i].z;

			}

		}

		smallBond = new int3(smallX, smallY, smallZ);
		largeBond = new int3(largeX, largeY, largeZ);

	}

	public void applyMesh()
    {

		Mesh mesh = CreateMesh();
		mesh.RecalculateNormals();
		MeshFilter meshFilter = this.GetComponent<MeshFilter>();
		MeshRenderer meshRenderer = this.GetComponent<MeshRenderer>();
		MeshCollider meshCollider = this.GetComponent<MeshCollider>();
		if (!meshFilter)
		{
			meshFilter = this.gameObject.AddComponent<MeshFilter>();
			meshRenderer = this.gameObject.AddComponent<MeshRenderer>();
		}
		meshFilter.mesh = mesh;
		meshCollider.sharedMesh = meshFilter.mesh;
		Material[] materials = new Material[2];
		materials[0] = this.Opaquematerial;
		materials[1] = this.Transparentmaterial;
		meshRenderer.materials = materials;
	}
	
	public Mesh CreateMesh()
	{

		int vertexIndex = 0;

		List<Vector3> vertices = new List<Vector3>();

		List<int> OpaqueTriangles = new List<int>();
		List<int> TransparentTriangles = new List<int>();

		List<Color> colors = new List<Color>();

		RenderObject();

		Mesh temp = new Mesh();
		temp.vertices = vertices.ToArray();
		temp.subMeshCount = 2;

		temp.SetTriangles(OpaqueTriangles.ToArray(), 0);
		temp.SetTriangles(TransparentTriangles.ToArray(), 1);
		temp.colors = colors.ToArray();

		temp.RecalculateNormals();

		return temp;

		void CreateOpaqueFace(Vector3 pos, int p, Color color)
		{

			vertices.Add(pos + VoxelData.voxelVerts[VoxelData.voxelTris[p, 0]]);
			vertices.Add(pos + VoxelData.voxelVerts[VoxelData.voxelTris[p, 1]]);
			vertices.Add(pos + VoxelData.voxelVerts[VoxelData.voxelTris[p, 2]]);
			vertices.Add(pos + VoxelData.voxelVerts[VoxelData.voxelTris[p, 3]]);

			colors.Add(color);
			colors.Add(color);
			colors.Add(color);
			colors.Add(color);

			OpaqueTriangles.Add(vertexIndex);
			OpaqueTriangles.Add(vertexIndex + 1);
			OpaqueTriangles.Add(vertexIndex + 2);
			OpaqueTriangles.Add(vertexIndex + 2);
			OpaqueTriangles.Add(vertexIndex + 1);
			OpaqueTriangles.Add(vertexIndex + 3);
			vertexIndex += 4;

		}

		void RenderOpaqueBlock(ObjectVoxel block)
		{
			for (int p = 0; p < 6; p++)
			{
				Vector3 pos = new Vector3(block.x, block.y, block.z);
				ObjectVoxel blockNext = IsBlockAt(new int3(pos + VoxelData.faceChecks[p]));
				if (blockNext == null)
				{

					CreateOpaqueFace(pos, p, block.color);
				}
				else if (blockNext.isTansparent)
				{
					CreateOpaqueFace(pos, p, block.color);
				}
			}
		}
		void CreateTransparentFace(Vector3 pos, int p, Color color)
		{

			vertices.Add(pos + VoxelData.voxelVerts[VoxelData.voxelTris[p, 0]]);
			vertices.Add(pos + VoxelData.voxelVerts[VoxelData.voxelTris[p, 1]]);
			vertices.Add(pos + VoxelData.voxelVerts[VoxelData.voxelTris[p, 2]]);
			vertices.Add(pos + VoxelData.voxelVerts[VoxelData.voxelTris[p, 3]]);

			colors.Add(color);
			colors.Add(color);
			colors.Add(color);
			colors.Add(color);

			TransparentTriangles.Add(vertexIndex);
			TransparentTriangles.Add(vertexIndex + 1);
			TransparentTriangles.Add(vertexIndex + 2);
			TransparentTriangles.Add(vertexIndex + 2);
			TransparentTriangles.Add(vertexIndex + 1);
			TransparentTriangles.Add(vertexIndex + 3);
			vertexIndex += 4;

		}

		void RenderTransparentBlock(ObjectVoxel block)
		{
			for (int p = 0; p < 6; p++)
			{
				Vector3 pos = new Vector3(block.x, block.y, block.z);
				ObjectVoxel blockNext = IsBlockAt(new int3(pos + VoxelData.faceChecks[p]));
				if (blockNext == null)
				{
					CreateTransparentFace(pos, p, block.color);
				}
				else if (blockNext.isTansparent)
				{
					if (blockNext.color != block.color)
					{
						CreateTransparentFace(pos, p, block.color);
					}
				}
			}
		}

		ObjectVoxel IsBlockAt(int3 pos)
		{
			for (int i = 0; i < voxels.Length; i++)
			{

				if (voxels[i].x == pos.x && voxels[i].y == pos.y && voxels[i].z == pos.z)
				{
					return voxels[i];
				}
			}
			return null;
		}
		void RenderObject()
		{
			for (int i = 0; i < voxels.Length; i++)
			{
				if (voxels[i].isTansparent)
				{
					RenderTransparentBlock(voxels[i]);
				}
				else
				{
					RenderOpaqueBlock(voxels[i]);
				}
			}

		}
	}
}
*/