using UnityEngine;

public class TerrainOffset : MonoBehaviour
{
    [Range(0f, 1f)]
    public float offset = 0.05f;

    [ContextMenu("Raise All Terrains")]
    void RaiseAllTerrainsMethod()
    {
        Terrain[] terrains = Terrain.activeTerrains;

        foreach (Terrain terrain in terrains)
        {
            TerrainData data = terrain.terrainData;

            int width = data.heightmapResolution;
            int height = data.heightmapResolution;

            float[,] heights = data.GetHeights(0, 0, width, height);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    heights[y, x] = Mathf.Clamp01(heights[y, x] + offset);
                }
            }

            data.SetHeights(0, 0, heights);
        }

        Debug.Log($"Raised {terrains.Length} terrain tiles.");
    }
}
