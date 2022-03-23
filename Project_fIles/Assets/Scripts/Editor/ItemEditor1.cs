using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

[CustomEditor(typeof(ItemData))]
public class ItemEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        ItemData itemData = (ItemData)target;
        GameObject temp = itemData.gameObject;

        if (GUILayout.Button("Generate Item"))
        {
            Mesh mesh = itemData.CreateMesh();
            mesh.RecalculateNormals();
            MeshFilter meshFilter = temp.GetComponent<MeshFilter>();
            MeshRenderer meshRenderer= temp.GetComponent<MeshRenderer>();
            if (!meshFilter)
            {
                meshFilter = temp.AddComponent<MeshFilter>();
                meshRenderer = temp.AddComponent<MeshRenderer>();
            }
            meshFilter.mesh = mesh;
            Material[] materials = new Material[2];
            materials[0] = itemData.Opaquematerial;
            materials[1] = itemData.Transparentmaterial;
            meshRenderer.materials = materials;

        }
    }
}
