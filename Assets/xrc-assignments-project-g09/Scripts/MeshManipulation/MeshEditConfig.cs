using UnityEngine;

[CreateAssetMenu(fileName = "MeshEditConfig", menuName = "Mesh Manipulation/Config")]
public class MeshEditConfig : ScriptableObject
{
    public GameObject vertexVisualizerPrefab;
    public float vertexVisualizerScale = 0.02f;
} 