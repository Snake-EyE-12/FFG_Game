using UnityEngine;
using System.Collections.Generic;

public class MeshCombiner : MonoBehaviour
{
    public void CombineMeshesPerMaterial()
    {
        // Find all MeshRenderers
        MeshRenderer[] meshRenderers = GetComponentsInChildren<MeshRenderer>();
        Dictionary<Material, List<MeshFilter>> materialToMeshFilters = new Dictionary<Material, List<MeshFilter>>();

        // Group meshes by material
        foreach (MeshRenderer mr in meshRenderers)
        {
            MeshFilter mf = mr.GetComponent<MeshFilter>();
            if (mf == null || mf.sharedMesh == null) continue;

            Material mat = mr.sharedMaterial;
            if (!materialToMeshFilters.ContainsKey(mat))
                materialToMeshFilters[mat] = new List<MeshFilter>();

            materialToMeshFilters[mat].Add(mf);
        }

        // combine each material group separately
        foreach (var kvp in materialToMeshFilters)
        {
            Material mat = kvp.Key;
            List<MeshFilter> mfs = kvp.Value;

            CombineInstance[] combine = new CombineInstance[mfs.Count];
            for (int i = 0; i < mfs.Count; i++)
            {
                combine[i].mesh = mfs[i].sharedMesh;
                combine[i].transform = mfs[i].transform.localToWorldMatrix;
            }

            Mesh combinedMesh = new Mesh();
            combinedMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
            combinedMesh.CombineMeshes(combine);

            GameObject combinedObj = new GameObject("Combined_" + mat.name);
            combinedObj.transform.SetParent(transform);
            combinedObj.transform.localPosition = Vector3.zero;

            MeshFilter mfCombined = combinedObj.AddComponent<MeshFilter>();
            mfCombined.sharedMesh = combinedMesh;

            MeshRenderer mrCombined = combinedObj.AddComponent<MeshRenderer>();
            mrCombined.sharedMaterial = mat;

            MeshCollider mc = combinedObj.AddComponent<MeshCollider>();
            mc.sharedMesh = combinedMesh;
        }

        // Disable originals
        foreach (MeshRenderer mr in meshRenderers)
        {
            mr.gameObject.SetActive(false);
        }
    }
}
