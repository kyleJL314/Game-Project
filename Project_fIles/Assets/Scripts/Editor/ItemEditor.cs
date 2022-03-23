using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
/*
[CustomEditor(typeof(Items))]
public class ItemEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        Items objectData = (Items)target;
        GameObject temp = objectData.gameObject;

        if (GUILayout.Button("Generate Object"))
        {
            Mesh mesh = objectData.CreateMesh();
            mesh.RecalculateNormals();
            MeshFilter meshFilter = temp.GetComponent<MeshFilter>();
            MeshRenderer meshRenderer = temp.GetComponent<MeshRenderer>();
            if (!meshFilter)
            {
                meshFilter = temp.AddComponent<MeshFilter>();
                meshRenderer = temp.AddComponent<MeshRenderer>();
            }
            meshFilter.mesh = mesh;
            Material[] materials = new Material[2];
            materials[0] = objectData.Opaquematerial;
            materials[1] = objectData.Transparentmaterial;
            meshRenderer.materials = materials;

        }
    }
}
*/