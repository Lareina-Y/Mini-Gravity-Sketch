using UnityEngine;

namespace XRC.Assignments.Project.G09
{
    [CreateAssetMenu(fileName = "MeshEditConfig", menuName = "Mesh Manipulation/Config")]
    public class MeshEditConfig : ScriptableObject
    {
        public GameObject vertexVisualizerPrefab;
        public float vertexVisualizerScale = 0.02f;
    }
}