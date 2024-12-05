using UnityEngine;
using System.Collections.Generic;
using MeshManipulation;

public class WireframeRenderer : MonoBehaviour
{
    [SerializeField] private Color wireframeColor = Color.white;
    [SerializeField] private float lineWidth = 0.001f;
    
    
    private List<LineRenderer> lineRenderers = new List<LineRenderer>();
    private MeshFilter meshFilter;
    private Dictionary<string, Edge> edges = new Dictionary<string, Edge>();
    private MeshEditController meshEditController;
    private Material lineMaterial;

    private bool isInialized = false;

    private class Edge
    {
        public int startIndex;
        public int endIndex;
        public LineRenderer lineRenderer;

        public Edge(int start, int end)
        {
            startIndex = start;
            endIndex = end;
        }
    }

    private void OnDestroy()
    {
        if (meshEditController != null)
        {
            meshEditController.OnVertexPositionChanged -= UpdateLinePositions;
        }
        ClearExistingLines();
    }

    public void SetVisible(bool visible)
    {
        Debug.Log("SetVisible: " + visible);
        if (!isInialized) return;

        if (visible)
        {
            UpdateLinePositions();
        }
        
        foreach (var line in lineRenderers)
        {
            if (line != null)
            {
                line.enabled = visible;
            }
        }
    }

    public void Initialize(MeshEditController meshEditController)
    {
        this.meshFilter = GetComponent<MeshFilter>();
        this.meshEditController = meshEditController;

        if (meshEditController == null)
        {
            Debug.LogError("MeshEditController is not assigned!");
        }

        if (meshFilter == null)
        {
            Debug.LogError("MeshFilter is not assigned!");
        }

        if (meshFilter.sharedMesh == null)
        {
            Debug.LogError("Shared mesh is not assigned!");
        }
        
        meshEditController.OnVertexPositionChanged += UpdateLinePositions;
        if (meshFilter == null || meshFilter.sharedMesh == null) return;
        
        ClearExistingLines();
        CreateWireframe();

        isInialized = true;
    }

    private void CreateWireframe()
    {
        Debug.Log("Creating wireframe");
        Mesh mesh = meshFilter.sharedMesh;
        int[] triangles = mesh.triangles;

        for (int i = 0; i < triangles.Length; i += 3)
        {
            ProcessTriangleEdges(triangles[i], triangles[i + 1], triangles[i + 2]);
        }

        foreach (var edge in edges.Values)
        {
            CreateLine(edge);
        }

        UpdateLinePositions();
    }

    private void ProcessTriangleEdges(int v1, int v2, int v3)
    {
        AddEdge(v1, v2);
        AddEdge(v2, v3);
        AddEdge(v3, v1);
    }

    private void AddEdge(int start, int end)
    {
        string edgeKey = CreateEdgeKey(start, end);
        
        if (edges.TryGetValue(edgeKey, out Edge existingEdge))
        {
            // existingEdge.sharedFaces++;
        }
        else
        {
            edges[edgeKey] = new Edge(start, end);
        }
    }

    private string CreateEdgeKey(int start, int end)
    {
        return start < end ? $"{start}-{end}" : $"{end}-{start}";
    }

    private void CreateLine(Edge edge)
    {
        GameObject lineObj = new GameObject("WireframeLine");
        lineObj.transform.SetParent(transform);
        
        LineRenderer line = lineObj.AddComponent<LineRenderer>();
        
        if (lineMaterial == null)
        {
            lineMaterial = new Material(Shader.Find("Universal Render Pipeline/Unlit"));
            lineMaterial.color = wireframeColor;
        }
        
        line.material = lineMaterial;
        line.startColor = wireframeColor;
        line.endColor = wireframeColor;
        
        line.startWidth = lineWidth;
        line.endWidth = lineWidth;
        line.positionCount = 2;
        
        line.useWorldSpace = true;
        
        edge.lineRenderer = line;
        lineRenderers.Add(line);
    }

    private void UpdateLinePositions()
    {
        if (meshFilter == null || meshFilter.sharedMesh == null) return;

        Vector3[] vertices = meshFilter.sharedMesh.vertices;
        
        foreach (var edge in edges.Values)
        {
            if (edge.lineRenderer == null) continue;
            
            Vector3 startWorld = transform.TransformPoint(vertices[edge.startIndex]);
            Vector3 endWorld = transform.TransformPoint(vertices[edge.endIndex]);
            
            edge.lineRenderer.SetPosition(0, startWorld);
            edge.lineRenderer.SetPosition(1, endWorld);
        }
    }

    private void ClearExistingLines()
    {
        foreach (var line in lineRenderers)
        {
            if (line != null)
            {
                Destroy(line.gameObject);
            }
        }
        lineRenderers.Clear();
        edges.Clear();
    }
}