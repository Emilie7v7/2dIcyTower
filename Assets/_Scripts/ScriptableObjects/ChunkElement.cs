using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace _Scripts.ScriptableObjects
{
    // Base class for all chunk elements
[CreateAssetMenu(fileName = "ChunkElement", menuName = "Chunk Generation/Chunk Element")]
public abstract class ChunkElement : ScriptableObject
{
    public string elementName;
    public int layerOrder;  // For controlling generation order
    public Tilemap tilemap; // Reference to the tilemap
    public TileBase[] tiles; // Available tiles for this element

    // Abstract method that each element type must implement
    public abstract void Generate(int width, int height, ChunkGenerationData data);
}

// Data container for sharing generation info between elements
public class ChunkGenerationData
{
    public Dictionary<Vector3Int, bool> occupiedPositions = new Dictionary<Vector3Int, bool>();
    // Add any other shared data needed during generation
}

// Example: Background Element
[CreateAssetMenu(fileName = "BackgroundElement", menuName = "Chunk Generation/Background Element")]
public class BackgroundElement : ChunkElement
{
    [Header("Background Settings")]
    public float noiseScale = 0.1f;
    public float threshold = 0.5f;

    public override void Generate(int width, int height, ChunkGenerationData data)
    {
        // Your background generation logic
    }
}

// Example: Wall Element
[CreateAssetMenu(fileName = "WallElement", menuName = "Chunk Generation/Wall Element")]
public class WallElement : ChunkElement
{
    [Header("Wall Settings")]
    public int wallThickness = 4;
    public bool generateCorners = true;

    public override void Generate(int width, int height, ChunkGenerationData data)
    {
        // Your wall generation logic
    }
}

// Example: Platform Element
[CreateAssetMenu(fileName = "PlatformElement", menuName = "Chunk Generation/Platform Element")]
public class PlatformElement : ChunkElement
{
    [Header("Platform Settings")]
    public float spawnChance = 0.3f;
    public int minLength = 3;
    public int maxLength = 8;
    public int minSpacing = 3;

    public override void Generate(int width, int height, ChunkGenerationData data)
    {
        // Your platform generation logic
    }
}

// Chunk Configuration ScriptableObject
[CreateAssetMenu(fileName = "ChunkConfiguration", menuName = "Chunk Generation/Chunk Configuration")]
public class ChunkConfiguration : ScriptableObject
{
    public Vector2Int chunkSize = new Vector2Int(100, 100);
    public List<ChunkElement> elements = new List<ChunkElement>();
}

}