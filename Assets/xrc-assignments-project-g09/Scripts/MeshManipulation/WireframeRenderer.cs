using UnityEngine;
using System.Collections.Generic;

public class WireframeRenderer : MonoBehaviour
{
    [SerializeField] private Color wireframeColor = Color.white;
    [SerializeField] private float lineWidth = 0.001f;
    [SerializeField] private Material lineMaterial;
    [SerializeField] private bool showOnStart = true;
    [SerializeField] private bool debugMode = true;
    
    private List<LineRenderer> lineRenderers = new List<LineRenderer>();
    private MeshFilter meshFilter;
    private Dictionary<string, Edge> edges = new Dictionary<string, Edge>();

    // 用于存储边的数据结构
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

    private void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        if (meshFilter != null && meshFilter.sharedMesh != null)
        {
            if (debugMode)
            {
                Debug.Log($"[WireframeRenderer] Found mesh on {gameObject.name} with {meshFilter.sharedMesh.vertexCount} vertices");
            }
            Initialize();
            SetVisible(showOnStart);
        }
        else if (debugMode)
        {
            Debug.LogWarning($"[WireframeRenderer] No MeshFilter or mesh found on {gameObject.name}");
        }
    }

    private void Update()
    {
        UpdateLinePositions();
    }

    public void Initialize()
    {
        if (meshFilter == null || meshFilter.sharedMesh == null) return;
        
        ClearExistingLines();
        CreateWireframe();
    }

    private void CreateWireframe()
    {
        Mesh mesh = meshFilter.sharedMesh;
        int[] triangles = mesh.triangles;

        if (debugMode)
        {
            Debug.Log($"[WireframeRenderer] Creating wireframe for mesh with {mesh.vertexCount} vertices and {triangles.Length/3} triangles");
        }

        // 遍历所有三角形
        for (int i = 0; i < triangles.Length; i += 3)
        {
            ProcessTriangleEdges(triangles[i], triangles[i + 1], triangles[i + 2]);
        }

        // 创建线条渲染器
        foreach (var edge in edges.Values)
        {
            CreateLine(edge);
        }

        if (debugMode)
        {
            Debug.Log($"[WireframeRenderer] Created {lineRenderers.Count} line renderers");
        }

        // 初始化所有线条位置
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
        // 创建唯一的边标识符
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
        // 确保边的key是唯一的，不管顶点的顺序如何
        return start < end ? $"{start}-{end}" : $"{end}-{start}";
    }

    private void CreateLine(Edge edge)
    {
        GameObject lineObj = new GameObject("WireframeLine");
        lineObj.transform.SetParent(transform);
        
        LineRenderer line = lineObj.AddComponent<LineRenderer>();
        
        // 如果没有指定材质，使用URP的Line材质
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
        
        // 使用世界空间坐标
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

            // 将本地坐标转换为世界坐标
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

    public void SetVisible(bool visible)
    {
        foreach (var line in lineRenderers)
        {
            if (line != null)
            {
                line.enabled = visible;
            }
        }
    }

    private void OnDestroy()
    {
        ClearExistingLines();
    }
}