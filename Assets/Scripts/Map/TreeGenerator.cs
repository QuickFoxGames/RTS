using UnityEngine;
using System.Collections.Generic;
public class TreeGenerator : MonoBehaviour
{
    [SerializeField] private Material m_mat;
    [SerializeField] private List<Branch> m_branches;
    public int m_seed = 42;
    private void Start()
    {
        Random.InitState(m_seed);
        for (int i = 0; i < m_branches.Count; i++)
        {
            GenerateBranch(m_branches[i]);
            m_branches[i].StoreLastValues();
            if (!m_branches[i].m_rootTransform && i != 0)
            {
                GameObject root = new GameObject();
                root.AddComponent<MeshFilter>();
                root.AddComponent<MeshRenderer>();
                root.transform.SetParent(m_branches[i - 1].m_rootTransform);
                m_branches[i].m_rootTransform = root.transform;
            }
            CreateMeshForBranch(m_branches[i]);
            GetRandomPointOnMesh(m_branches[i].m_mesh, out Vector3 pos, out Vector3 nrml);
            if (i != 0)
                m_branches[i].m_rootTransform.SetLocalPositionAndRotation(pos, Quaternion.LookRotation(nrml));
        }
    }
    void GetRandomPointOnMesh(Mesh mesh, out Vector3 randomPoint, out Vector3 randomNormal)
    {
        Vector3[] vertices = mesh.vertices;
        Vector3[] normals = mesh.normals;
        int[] triangles = mesh.triangles;

        int triangleIndex = Random.Range(0, triangles.Length / 3) * 3;
        int index0 = triangles[triangleIndex];
        int index1 = triangles[triangleIndex + 1];
        int index2 = triangles[triangleIndex + 2];

        Vector3 v0 = vertices[index0];
        Vector3 v1 = vertices[index1];
        Vector3 v2 = vertices[index2];

        Vector3 barycentricCoords = GetRandomBarycentricCoordinates();
        randomPoint = v0 * barycentricCoords.x + v1 * barycentricCoords.y + v2 * barycentricCoords.z;

        Vector3 n0 = normals[index0];
        Vector3 n1 = normals[index1];
        Vector3 n2 = normals[index2];
        randomNormal = n0 * barycentricCoords.x + n1 * barycentricCoords.y + n2 * barycentricCoords.z;

        randomNormal.Normalize();
    }
    Vector3 GetRandomBarycentricCoordinates()
    {
        float u = Random.value;
        float v = Random.value;
        if (u + v > 1)
        {
            u = 1 - u;
            v = 1 - v;
        }
        return new Vector3(1 - u - v, u, v);
    }
    private void Update()
    {
        for (int i = 0; i < m_branches.Count; i++)
        {
            if (m_branches[i].HasValuesChanged())
            {
                RecreateBranch(m_branches[i], m_branches[i != 0 ? i - 1 : 0]);
            }
        }
    }
    private void RecreateBranch(Branch branch, Branch lastBranch)
    {
        branch.m_segmentVertices.Clear();
        Random.InitState(m_seed);
        GenerateBranch(branch);
        branch.StoreLastValues();
        CreateMeshForBranch(branch);
    }
    /*private void GenerateBranch(Branch branch)
    {
        float segmentLength = branch.m_branchLength / (branch.m_numMainSegments - 1);

        Vector3 currentPosition = Vector3.zero;
        Vector3 currentUpDirection = Vector3.up;

        List<Vector3> mainSegmentPositions = new List<Vector3>();
        List<Vector3> mainUpDirections = new List<Vector3>();
        for (int i = 0; i < branch.m_numMainSegments; i++)
        {
            float segmentPercent = (float)i / (branch.m_numMainSegments - 1);
            float scale = Mathf.Lerp(branch.m_bottomScale, branch.m_topScale, segmentPercent);
            if (i > 0)
            {
                float randomXAngle = Random.Range(-branch.m_maxXRotation, branch.m_maxXRotation);
                float randomYAngle = Random.Range(-branch.m_maxYRotation, branch.m_maxYRotation);
                Quaternion randomRotation = Quaternion.Euler(randomXAngle, randomYAngle, 0);
                currentUpDirection = randomRotation * currentUpDirection;
            }
            Vector3 nextPosition = currentPosition + currentUpDirection.normalized * segmentLength;

            mainSegmentPositions.Add(currentPosition);
            mainUpDirections.Add(currentUpDirection);
            currentPosition = nextPosition;
        }
        for (int i = 0; i < branch.m_numMainSegments - 1; i++)
        {
            Vector3 p0 = mainSegmentPositions[i];
            Vector3 p3 = mainSegmentPositions[i + 1];
            Vector3 p1 = p0 + mainUpDirections[i] * segmentLength * 0.5f;
            Vector3 p2 = p3 - mainUpDirections[i + 1] * segmentLength * 0.5f;
            for (int t = 0; t <= branch.m_numSubSegments; t++)
            {
                float tValue = (float)t / branch.m_numSubSegments;
                Vector3 bezierPoint = CalculateBezierPoint(tValue, p0, p1, p2, p3);
                Vector3 bezierUpDirection = Vector3.Slerp(mainUpDirections[i], mainUpDirections[i + 1], tValue);

                float bezierScale = Mathf.Lerp(
                    Mathf.Lerp(branch.m_bottomScale, branch.m_topScale, (float)i / (branch.m_numMainSegments - 1)),
                    Mathf.Lerp(branch.m_bottomScale, branch.m_topScale, (float)(i + 1) / (branch.m_numMainSegments - 1)),
                    tValue);
                branch.m_segmentVertices.Add(GenerateCircle(bezierPoint, bezierUpDirection, bezierScale, branch.m_numVerts));
            }
        }
    }*/
    private Vector3 CalculateBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        float uuu = uu * u;
        float ttt = tt * t;

        Vector3 p = uuu * p0; // first term
        p += 3 * uu * t * p1; // second term
        p += 3 * u * tt * p2; // third term
        p += ttt * p3; // fourth term

        return p;
    }
    private List<Vector3> GenerateCircle(Vector3 center, Vector3 upDirection, float radius, int numVerts)
    {
        List<Vector3> circlePoints = new List<Vector3>();
        Vector3 axis1 = Vector3.Cross(upDirection, Vector3.right).normalized;
        if (axis1 == Vector3.zero)
        {
            axis1 = Vector3.Cross(upDirection, Vector3.forward).normalized;
        }
        Vector3 axis2 = Vector3.Cross(upDirection, axis1).normalized;
        float angleStep = 360f / numVerts;
        for (int i = 0; i < numVerts; i++)
        {
            float angleRad = Mathf.Deg2Rad * angleStep * i;
            Vector3 pointOnCircle = center + radius * (Mathf.Cos(angleRad) * axis1 + Mathf.Sin(angleRad) * axis2);
            circlePoints.Add(pointOnCircle);
        }
        return circlePoints;
    }
    private void GenerateBranch(Branch branch)
    {
        float segmentLength = branch.m_branchLength / (branch.m_numMainSegments - 1);

        Vector3 currentPosition = Vector3.zero;
        Vector3 currentUpDirection = Vector3.up;

        List<Vector3> mainSegmentPositions = new List<Vector3>();
        List<Vector3> mainUpDirections = new List<Vector3>();

        // Generate the main segment positions and directions
        for (int i = 0; i < branch.m_numMainSegments; i++)
        {
            float segmentPercent = (float)i / (branch.m_numMainSegments - 1);
            float scale = Mathf.Lerp(branch.m_bottomScale, branch.m_topScale, segmentPercent);

            if (i > 0)
            {
                float randomXAngle = Random.Range(-branch.m_maxXRotation, branch.m_maxXRotation);
                float randomYAngle = Random.Range(-branch.m_maxYRotation, branch.m_maxYRotation);
                Quaternion randomRotation = Quaternion.Euler(randomXAngle, randomYAngle, 0);
                currentUpDirection = randomRotation * currentUpDirection;
            }

            Vector3 nextPosition = currentPosition + currentUpDirection.normalized * segmentLength;
            mainSegmentPositions.Add(currentPosition);
            mainUpDirections.Add(currentUpDirection);
            currentPosition = nextPosition;
        }

        // Generate vertices for each segment
        for (int i = 0; i < branch.m_numMainSegments - 1; i++)
        {
            Vector3 p0 = mainSegmentPositions[i];
            Vector3 p3 = mainSegmentPositions[i + 1];
            Vector3 p1 = p0 + mainUpDirections[i] * segmentLength * 0.5f;
            Vector3 p2 = p3 - mainUpDirections[i + 1] * segmentLength * 0.5f;

            for (int t = 0; t < branch.m_numSubSegments; t++)
            {
                float tValue = (float)t / branch.m_numSubSegments;
                Vector3 bezierPoint = CalculateBezierPoint(tValue, p0, p1, p2, p3);
                Vector3 bezierUpDirection = Vector3.Slerp(mainUpDirections[i], mainUpDirections[i + 1], tValue);

                float bezierScale = Mathf.Lerp(
                    Mathf.Lerp(branch.m_bottomScale, branch.m_topScale, (float)i / (branch.m_numMainSegments - 1)),
                    Mathf.Lerp(branch.m_bottomScale, branch.m_topScale, (float)(i + 1) / (branch.m_numMainSegments - 1)),
                    tValue);

                // Avoid adding the same vertices if the points overlap between rings
                List<Vector3> circle = GenerateCircle(bezierPoint, bezierUpDirection, bezierScale, branch.m_numVerts);
                branch.m_segmentVertices.Add(circle);
            }
        }
    }
    private void CreateMeshForBranch(Branch branch)
    {
        Mesh mesh = new Mesh
        {
            name = branch.m_name
        };

        List<Vector3> vertices = new List<Vector3>();  // List of unique vertices
        List<int> triangles = new List<int>();  // List of triangle indices
        List<Vector3> normals = new List<Vector3>();  // List of normals

        int vertsPerSegment = branch.m_numVerts;  // Number of vertices per segment (ring)
        int segmentCount = branch.m_segmentVertices.Count;  // Number of segments (rings)

        // Only add vertices once per ring and avoid duplicates between rings
        for (int i = 0; i < segmentCount; i++)
        {
            for (int j = 0; j < vertsPerSegment; j++)
            {
                vertices.Add(branch.m_segmentVertices[i][j]);
            }
        }

        // Initialize normals for each vertex (same count as vertices)
        normals.AddRange(new Vector3[vertices.Count]);

        // Generate triangles between consecutive segments
        for (int i = 0; i < segmentCount - 1; i++)  // Loop through segments
        {
            for (int j = 0; j < vertsPerSegment; j++)  // Loop through vertices in each segment
            {
                // Current vertex index in the current segment
                int currentIndex = i * vertsPerSegment + j;

                // Corresponding vertex index in the next segment (same relative position)
                int nextIndex = (i + 1) * vertsPerSegment + j;

                // Wrap around for the next vertex in the ring (to form the quad face)
                int nextJ = (j + 1) % vertsPerSegment;  // Wrap to the first vertex when at the last vertex

                // Indices for the triangle formed between this segment and the next
                int currentNextJ = i * vertsPerSegment + nextJ; // Next vertex in the current segment
                int nextNextJ = (i + 1) * vertsPerSegment + nextJ; // Next vertex in the next segment

                // Triangle 1: current vertex, next vertex in the current segment, next vertex in the next segment
                triangles.Add(currentIndex);
                triangles.Add(nextNextJ);  // Swap the order of these two to fix winding
                triangles.Add(nextIndex);

                // Triangle 2: current vertex, next vertex in the next segment, next vertex in the current segment
                triangles.Add(currentIndex);
                triangles.Add(currentNextJ);  // Swap the order of these two to fix winding
                triangles.Add(nextNextJ);

                // Compute face normals for both triangles
                Vector3 normal1 = CalculateNormal(vertices[currentIndex], vertices[nextNextJ], vertices[nextIndex]);
                Vector3 normal2 = CalculateNormal(vertices[currentIndex], vertices[currentNextJ], vertices[nextNextJ]);

                // Add these normals to the vertices (accumulate for smoothing)
                normals[currentIndex] += normal1 + normal2;
                normals[nextIndex] += normal1;
                normals[nextNextJ] += normal1 + normal2;
                normals[currentNextJ] += normal2;
            }
        }

        // Normalize all the accumulated normals for smooth shading
        for (int i = 0; i < normals.Count; i++)
        {
            normals[i] = normals[i].normalized;
        }

        // Apply vertices, triangles, and normals to the mesh
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.normals = normals.ToArray();  // Use smoothed normals
        // Assign the mesh to a MeshFilter component
        if (!branch.m_rootTransform.gameObject.TryGetComponent<MeshFilter>(out var meshFilter))
        {
            meshFilter = branch.m_rootTransform.gameObject.AddComponent<MeshFilter>();
        }
        meshFilter.mesh = mesh;

        // Optionally assign a MeshRenderer component if it does not exist
        if (!branch.m_rootTransform.gameObject.TryGetComponent<MeshRenderer>(out var meshRenderer))
        {
            meshRenderer = branch.m_rootTransform.gameObject.AddComponent<MeshRenderer>();
            meshRenderer.material = m_mat;
        }
        branch.m_mesh = mesh;
    }
    private Vector3 CalculateNormal(Vector3 v0, Vector3 v1, Vector3 v2)
    {
        Vector3 side1 = v1 - v0;
        Vector3 side2 = v2 - v0;
        return Vector3.Cross(side1, side2).normalized;
    }

    /*private void OnDrawGizmos()
    {
        if (m_branches == null || m_branches.Count == 0) return;
        Gizmos.color = Color.green;
        foreach (var branch in m_branches)
        {
            if (branch.m_segmentVertices == null) break;
            for (int i = 0; i < branch.m_segmentVertices.Count; i++)
            {
                List<Vector3> currentSegment = branch.m_segmentVertices[i];

                for (int j = 0; j < currentSegment.Count; j++)
                {
                    int nextIndex = (j + 1) % currentSegment.Count;
                    Gizmos.DrawLine(currentSegment[j], currentSegment[nextIndex]);
                }
                if (i < branch.m_segmentVertices.Count - 1)
                {
                    List<Vector3> nextSegment = branch.m_segmentVertices[i + 1];

                    Vector3 currentSegmentCenter = GetCenterPoint(currentSegment);
                    Vector3 nextSegmentCenter = GetCenterPoint(nextSegment);

                    Gizmos.color = Color.red;
                    Gizmos.DrawLine(currentSegmentCenter, nextSegmentCenter);
                    Gizmos.color = Color.green;
                }
            }
        }
    }
    private Vector3 GetCenterPoint(List<Vector3> segment)
    {
        Vector3 center = Vector3.zero;
        foreach (var point in segment)
        {
            center += point;
        }
        return center / segment.Count;
    }*/
}
[System.Serializable]
public class Branch
{
    public string m_name;

    public float m_bottomScale;
    public float m_topScale;
    public float m_branchLength;
    public float m_maxXRotation;
    public float m_maxYRotation;

    public int m_numMainSegments;
    public int m_numVerts;
    public int m_numSubSegments;

    public Transform m_rootTransform;

    public List<List<Vector3>> m_segmentVertices = new();
    public Mesh m_mesh;
    #region Last values
    private float m_lastBottomScale;
    private float m_lastTopScale;
    private float m_lastBranchLength;
    private float m_lastMaxXRotation;
    private float m_lastMaxYRotation;
    private int m_lastNumMainSegments;
    private int m_lastNumVerts;
    private int m_lastNumSubSegments;
    #endregion
    public bool HasValuesChanged()
    {
        return m_bottomScale != m_lastBottomScale ||
               m_topScale != m_lastTopScale ||
               m_branchLength != m_lastBranchLength ||
               m_maxXRotation != m_lastMaxXRotation ||
               m_maxYRotation != m_lastMaxYRotation ||
               m_numMainSegments != m_lastNumMainSegments ||
               m_numVerts != m_lastNumVerts ||
               m_numSubSegments != m_lastNumSubSegments;
    }
    public void StoreLastValues()
    {
        m_lastBottomScale = m_bottomScale;
        m_lastTopScale = m_topScale;
        m_lastBranchLength = m_branchLength;
        m_lastMaxXRotation = m_maxXRotation;
        m_lastMaxYRotation = m_maxYRotation;
        m_lastNumMainSegments = m_numMainSegments;
        m_lastNumVerts = m_numVerts;
        m_lastNumSubSegments = m_numSubSegments;
    }
}