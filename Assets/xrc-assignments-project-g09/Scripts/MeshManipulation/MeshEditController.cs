using UnityEngine;
using System.Collections.Generic;
using UnityEngine.XR.Interaction.Toolkit;
using MeshManipulation.UI;

namespace MeshManipulation
{
    public class MeshEditController : MonoBehaviour
    {
        [Header("Visualizer Settings")]
        [SerializeField] private GameObject vertexVisualizerPrefab;
        [SerializeField] private float vertexVisualizerScale = 0.02f;

        private Mesh originalMesh;
        private Mesh instanceMesh;
        private MeshFilter meshFilter;

        private Transform visualizersContainer;
        
        // Vertex data structure
        private class VertexGroup
        {
            public Vector3 Position;
            public List<int> MeshVertexIndices;
            public GameObject Visualizer;

            public VertexGroup(Vector3 position, GameObject visualizer)
            {
                Position = position;
                Visualizer = visualizer;
                MeshVertexIndices = new List<int>();
            }
        }

        private List<VertexGroup> vertexGroups = new List<VertexGroup>();
        private Dictionary<GameObject, VertexGroup> visualizerToGroup = new Dictionary<GameObject, VertexGroup>();

        private MeshEditConfig config;
        
        private BoxCollider originalBoxCollider;
        private MeshCollider meshCollider;
        private bool isEditing = false;

        private WireframeRenderer wireframeRenderer;

        public event System.Action OnVertexPositionChanged;

        public void Initialize()
        {
            if (vertexVisualizerPrefab == null)
            {
                Debug.LogError("Vertex visualizer prefab is not assigned!");
                return;
            }

            meshFilter = GetComponent<MeshFilter>();
            originalMesh = meshFilter.sharedMesh;
            
            // Create mesh instance
            instanceMesh = Instantiate(originalMesh);
            meshFilter.mesh = instanceMesh;  // Use instance instead of sharedMesh
            
            // Check if visualizers container already exists
            visualizersContainer = transform.Find("Edit Mode Visualizers");
            if (visualizersContainer != null)
            {
                Debug.Log("Edit Mode Visualizers already exists!");
                return;
            }
            
            // Create container
            visualizersContainer = new GameObject("Edit Mode Visualizers").transform;
            visualizersContainer.SetParent(transform);
            visualizersContainer.localPosition = Vector3.zero;
            visualizersContainer.localRotation = Quaternion.identity;
            visualizersContainer.localScale = Vector3.one;

            // Save original BoxCollider reference
            originalBoxCollider = GetComponent<BoxCollider>();
            if (originalBoxCollider != null)
            {
                // Disable BoxCollider at the beginning of edit mode
                originalBoxCollider.enabled = false;
                
                // Add MeshCollider
                meshCollider = gameObject.AddComponent<MeshCollider>();
                meshCollider.sharedMesh = instanceMesh;
                meshCollider.convex = true;
            }

            isEditing = true;
            InitializeVertexGroups();
            HideAll();

            wireframeRenderer = gameObject.AddComponent<WireframeRenderer>();
            wireframeRenderer.Initialize(this);
        }

        private void InitializeVertexGroups()
        {
            if (instanceMesh == null) return;

            Vector3[] vertices = instanceMesh.vertices;
            const float epsilon = 0.0001f;

            for (int i = 0; i < vertices.Length; i++)
            {
                Vector3 vertex = vertices[i];
                VertexGroup group = null;

                // Find existing vertex group
                foreach (var existingGroup in vertexGroups)
                {
                    if (Vector3.Distance(existingGroup.Position, vertex) < epsilon)
                    {
                        group = existingGroup;
                        break;
                    }
                }

                if (group == null)
                {
                    GameObject visualizer = Instantiate(vertexVisualizerPrefab, visualizersContainer);
                    visualizer.transform.localPosition = vertex;
                    visualizer.transform.localScale = Vector3.one * vertexVisualizerScale;

                    var positionTracker = visualizer.AddComponent<VertexPositionTracker>();
                    positionTracker.OnPositionChanged += (newPosition) => UpdateVertexPosition(visualizer, newPosition);

                    if (visualizer.TryGetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>(out var grabInteractable))
                    {
                        var currentLayers = grabInteractable.interactionLayers.value;
                        if ((currentLayers & InteractionLayerMask.GetMask("SphereSelectVertex")) == 0)
                        {
                            currentLayers |= InteractionLayerMask.GetMask("SphereSelectVertex");
                            grabInteractable.interactionLayers = currentLayers;
                        }

                        grabInteractable.trackPosition = true;
                        grabInteractable.trackRotation = false;
                    }

                    visualizer.SetActive(false);

                    group = new VertexGroup(vertex, visualizer);
                    vertexGroups.Add(group);
                    visualizerToGroup[visualizer] = group;
                }

                group.MeshVertexIndices.Add(i);
            }
        }

        public void ShowForMode(MeshSelectionUI.SelectionMode mode)
        {
            
            switch (mode)
            {
                case MeshSelectionUI.SelectionMode.Object:
                    HideAll();
                    break;
                case MeshSelectionUI.SelectionMode.Vertex:
                    ShowVertices();
                    ShowWireframe();
                    break;
                case MeshSelectionUI.SelectionMode.Edge:
                    // TODO: ShowEdges();
                    ShowWireframe();
                    break;
                case MeshSelectionUI.SelectionMode.Face:
                    // TODO: ShowFaces();
                    ShowWireframe();
                    break;
            }
        }

        private void ShowVertices()
        {
            foreach (var group in vertexGroups)
            {
                group.Visualizer.SetActive(true);
            }
        }

        private void ShowWireframe()
        {
            wireframeRenderer.SetVisible(true);
        }

        public void HideAll()
        {
            foreach (var group in vertexGroups)
            {
                group.Visualizer.SetActive(false);
            }

            if (wireframeRenderer != null)
            {
                wireframeRenderer.SetVisible(false);
            }
        }

        public void UpdateVertexPosition(GameObject visualizer, Vector3 newPosition)
        {
            if (visualizerToGroup.TryGetValue(visualizer, out VertexGroup group))
            {
                // Update position using world coordinates
                Vector3 worldPosition = visualizer.transform.position;
                group.Position = transform.InverseTransformPoint(worldPosition); // Convert to local coordinates
                UpdateMeshVertices(group);
                
                OnVertexPositionChanged?.Invoke();
            }
        }

        private void UpdateMeshVertices(VertexGroup group)
        {
            if (instanceMesh == null) return;
            
            Vector3[] vertices = instanceMesh.vertices;
            foreach (int index in group.MeshVertexIndices)
            {
                vertices[index] = group.Position;
            }
            
            instanceMesh.vertices = vertices;
            instanceMesh.RecalculateNormals();
            instanceMesh.RecalculateBounds();
        }

        private void OnDestroy()
        {
            if (visualizersContainer != null)
            {
                Destroy(visualizersContainer.gameObject);
            }
            
            // Clean up the instantiated mesh
            if (instanceMesh != null)
            {
                Destroy(instanceMesh);
            }
            
            vertexGroups.Clear();
            visualizerToGroup.Clear();

            // Ensure MeshCollider is cleaned up
            if (meshCollider != null)
            {
                Destroy(meshCollider);
            }
        }

        public void SetConfig(MeshEditConfig newConfig)
        {
            config = newConfig;
            vertexVisualizerPrefab = config.vertexVisualizerPrefab;
            vertexVisualizerScale = config.vertexVisualizerScale;
        }

        // Add a helper method to restore the original mesh
        public void RestoreOriginalMesh()
        {
            if (meshFilter != null && originalMesh != null)
            {
                meshFilter.sharedMesh = originalMesh;
            }
            
            if (instanceMesh != null)
            {
                Destroy(instanceMesh);
                instanceMesh = null;
            }
        }


        public void ExitEditMode()
        {
            if (!isEditing) return;

            isEditing = false;

            ShowForMode(MeshSelectionUI.SelectionMode.Object);
            
            // Use MeshCollider instead of BoxCollider after editing
            if (originalBoxCollider != null)
            {
                // Remove original BoxCollider
                Destroy(originalBoxCollider);
                originalBoxCollider = null;
            }
            
            // Ensure MeshCollider is set correctly
            if (meshCollider != null)
            {
                // Force refresh MeshCollider
                meshCollider.sharedMesh = null;  // Clear first
                meshCollider.sharedMesh = instanceMesh;  // Then set new mesh
                meshCollider.convex = true;
                
                // Update XR Grab Interactable's Colliders
                if (TryGetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>(out var grabInteractable))
                {
                    // Temporarily disable and enable the component to force refresh
                    bool wasEnabled = grabInteractable.enabled;
                    grabInteractable.enabled = false;
                    
                    grabInteractable.colliders.Clear();
                    grabInteractable.colliders.Add(meshCollider);
                    

                    var currentLayers = grabInteractable.interactionLayers.value;
                    if ((currentLayers & InteractionLayerMask.GetMask("SphereSelectObject")) == 0)
                    {
                        currentLayers |= InteractionLayerMask.GetMask("SphereSelectObject");
                        grabInteractable.interactionLayers = currentLayers;
                    }
                    
                    // Re-enable the component
                    grabInteractable.enabled = wasEnabled;
                }
            }

        }

        public Transform VisualizersContainer => visualizersContainer;

    }

    public class VertexPositionTracker : MonoBehaviour
    {
        public event System.Action<Vector3> OnPositionChanged;
        private Vector3 lastPosition;

        private void Start()
        {
            lastPosition = transform.localPosition;
        }

        private void Update()
        {
            if (transform.localPosition != lastPosition)
            {
                lastPosition = transform.localPosition;
                OnPositionChanged?.Invoke(lastPosition);
            }
        }
    }
} 