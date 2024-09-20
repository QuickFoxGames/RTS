using UnityEngine;
using System.Collections.Generic;

public class TreeGenerator : MonoBehaviour
{
    [SerializeField] private int numVertsPerNode;
    [SerializeField] private Vector2 minmaxTrunkNodes;
    [SerializeField] private Vector2 minmaxTrunkLength;
    [SerializeField] private Vector2 minmaxBranchNodes;
    [SerializeField] private Vector2 minmaxBranchLength;
    [SerializeField] private MeshFilter meshFilter;
    private List<Node> m_nodes = new();

    private void Start()
    {
        GenerateTrunk();
    }
    private void GenerateTrunk()
    {
        List<Node> trunkNodes = new()
        {
            new Node(Vector3.zero, 1f) // Add the base node of the trunk
        };
        int numNodes = (int)Random.Range(minmaxTrunkNodes.x, minmaxTrunkNodes.y);
        float h = Random.Range(minmaxTrunkLength.x, minmaxTrunkLength.y);
        float segMin = h / numNodes;
        numNodes--;
        float segMax = h / numNodes;

        for (int i = 1; i < numNodes; i++)
        {
            trunkNodes.Add(new Node(Random.Range(segMin, segMax) * Vector3.up + trunkNodes[i - 1].Position, 1f));
        }

        // Ensure there are at least 2 nodes to create a mesh
        if (trunkNodes.Count >= 2)
        {
            GenerateMesh(trunkNodes);
        }
        else
        {
            Debug.LogWarning("Not enough nodes to generate a mesh.");
        }
    }

    private void GenerateMesh(List<Node> nodes)
    {
        List<Vector3> allVerts = new();
        List<int> triangles = new();

        int vertIndexOffset = 0;

        for (int i = 0; i < nodes.Count; i++)
        {
            Vector3 dir = Vector3.up;
            if (i != 0)
            {
                Vector3 dirFromLast = nodes[i].Position - nodes[i - 1].Position;
                if (i != nodes.Count - 1)
                {
                    Vector3 dirToNext = nodes[i + 1].Position - nodes[i].Position;
                    dir = Vector3.Lerp(dirFromLast, dirToNext, dirToNext.magnitude / (dirFromLast.magnitude + dirToNext.magnitude));
                }
                else
                {
                    dir = dirFromLast;
                }
            }

            GenerateVertsOn(nodes[i], dir);
            allVerts.AddRange(nodes[i].verts);

            if (i > 0)
            {
                int numVerts = nodes[i].verts.Count;
                int baseIndex = vertIndexOffset;

                // Debugging information
                Debug.Log($"Processing node {i} with {numVerts} vertices");
                Debug.Log($"Base index: {baseIndex}");

                // Ensure vertex count is consistent between nodes
                if (numVerts != nodes[i - 1].verts.Count)
                {
                    Debug.LogError("Vertex count mismatch between nodes.");
                    return;
                }

                // Generate triangles between nodes
                for (int j = 0; j < numVerts; j++)
                {
                    int nextIndex = (j + 1) % numVerts;

                    // Check if indices are within bounds
                    if (baseIndex + j >= allVerts.Count ||
                        baseIndex + nextIndex >= allVerts.Count ||
                        baseIndex + numVerts + j >= allVerts.Count ||
                        baseIndex + numVerts + nextIndex >= allVerts.Count)
                    {
                        Debug.LogError($"Triangle index out of bounds: baseIndex={baseIndex}, j={j}, nextIndex={nextIndex}, numVerts={numVerts}");
                        Debug.LogError($"allVerts count: {allVerts.Count}");
                        return;
                    }

                    // Create two triangles for each quad between nodes
                    triangles.Add(baseIndex + j);
                    triangles.Add(baseIndex + nextIndex);
                    triangles.Add(baseIndex + numVerts + j);

                    triangles.Add(baseIndex + nextIndex);
                    triangles.Add(baseIndex + numVerts + nextIndex);
                    triangles.Add(baseIndex + numVerts + j);
                }
            }

            vertIndexOffset += nodes[i].verts.Count;
        }

        Mesh mesh = new()
        {
            vertices = allVerts.ToArray(),
            triangles = triangles.ToArray()
        };

        mesh.RecalculateNormals();
        mesh.name = "Trunk";
        meshFilter.mesh = mesh;
    }

    private void GenerateVertsOn(Node n, Vector3 dir)
    {
        Vector3 perp = Vector3.Cross(dir, Vector3.up);
        if (perp == Vector3.zero)  // Special case where axis is Vector3.up
            perp = Vector3.Cross(dir, Vector3.right);

        float angleStep = 360f / numVertsPerNode;
        n.verts.Clear(); // Ensure verts list is clear before adding new vertices

        for (int i = 0; i < numVertsPerNode; i++)
        {
            float angleInRadians = Mathf.Deg2Rad * angleStep * i;
            Vector3 positionOnCircle = n.scale * (Mathf.Cos(angleInRadians) * perp + Mathf.Sin(angleInRadians) * Vector3.Cross(dir, perp));
            Vector3 position = positionOnCircle + n.Position; // Use n.Position for final position
            n.verts.Add(position);
        }
    }


    public class Node
    {
        public Vector3 Position;
        public float scale;
        public List<Vector3> verts;
        public Node(Vector3 pos, float s)
        {
            Position = pos;
            scale = s;
            verts = new();
        }
    }
}
