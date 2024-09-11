using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class TreeGenerator : MonoBehaviour
{
    [Header("Trees")]
    [SerializeField] private int m_width;
    [SerializeField] private int m_height;
    [SerializeField] private int m_numberOFTrees;
    [SerializeField] private float m_minDistBetweenTrees;
    [Header("Trunks")]
    [SerializeField] private Vector2 m_minmaxTrunkScale;
    [Header("Branches")]
    [SerializeField] private Vector3 m_maxAngles;
    [SerializeField] private Vector2 m_minmaxBranchHeight;
    [SerializeField] private Vector2 m_minmaxNumberOfBranch;
    [SerializeField] private Vector2 m_minmaxBranchScale;
    [Header("Prefabs")]
    [SerializeField] private GameObject m_trunk; 
    [SerializeField] private GameObject m_branch;

    private List<GameObject> m_Trees;
    void Start()
    {
        m_Trees = new List<GameObject>();
        GenerateTree();
    }
    private void GenerateTree()
    {
        for (int i = 0; i < m_numberOFTrees; i++)
        {
            GameObject trunk = Instantiate(m_trunk, transform);
            Vector3 pos = RandomizePos();
            trunk.transform.SetLocalPositionAndRotation(pos, Quaternion.Euler(0f, Random.Range(-360, 360), 0f));
            float s = Random.Range(m_minmaxTrunkScale.x, m_minmaxTrunkScale.y);
            trunk.transform.GetChild(0).localScale = new Vector3(1f, s, 1f);
            m_Trees.Add(trunk);
            StartCoroutine(AddBranches(trunk, s));
        }
    }
    private Vector3 RandomizePos()
    {
        Vector3 pos = new(Random.Range(-m_width * 0.5f, m_width * 0.5f), 0f, Random.Range(-m_height * 0.5f, m_height * 0.5f));
        if (!CheckDistances(pos)) RandomizePos();
        return pos;
    }
    private bool CheckDistances(Vector3 pos)
    {
        foreach (var t in m_Trees)
        {
            if (Vector3.Distance(t.transform.localPosition, pos) < m_minDistBetweenTrees) return false;
        }
        return true;
    }
    private IEnumerator AddBranches(GameObject trunk, float s)
    {
        int numBranches = (int)Random.Range(m_minmaxNumberOfBranch.x, m_minmaxNumberOfBranch.y);
        for (int i = 0; i < numBranches; i++)
        {
            GameObject branch = Instantiate(m_branch, trunk.transform);
            branch.transform.SetLocalPositionAndRotation(new Vector3(0f, Random.Range(m_minmaxBranchHeight.x, m_minmaxBranchHeight.y * s), 0f), Quaternion.Euler(Random.Range(-m_maxAngles.x, m_maxAngles.x), Random.Range(-m_maxAngles.y, m_maxAngles.y), Random.Range(-m_maxAngles.z, m_maxAngles.z)));
            float s2 = Random.Range(m_minmaxBranchScale.x, m_minmaxBranchScale.y);
            branch.transform.GetChild(0).localScale = new Vector3(1f, s2, 1f);
            yield return null;
        }
        yield return null;
    }
    //////////////////////////////////////////////////////////////////////////////////////////////////////////
    public int vertsPerPoint;
    public int vertDistFromPoint;
    private List<Point> points = new();
    private List<Vector3[]> verts = new();
    private void TracePath()
    {
        for (int i = 0; i < points.Count; i++)
        {
            points[i].direction = i != 0 ? Vector3.Lerp(points[i].direction, points[i + 1].position - points[i].position, 0.5f) : points[i + 1].position - points[i].position;
            GenerateVerts(points[i]);
            verts.Add(points[i].verts);
        }
    }
    private void GenerateVerts(Point p)
    {
        Vector3[] verts = new Vector3[vertsPerPoint];
        float angleStep = 360f / vertsPerPoint;
        Vector3 right = Vector3.Cross(p.direction, Vector3.forward).normalized;
        if (right == Vector3.zero) { right = Vector3.Cross(p.direction.normalized, Vector3.right); }
        Vector3 forward = Vector3.Cross(p.direction.normalized, right).normalized;
        for (int i = 0; i < vertsPerPoint; i++)
        {
            float angle = i * angleStep * Mathf.Deg2Rad;
            Vector3 vert = p.position + (Mathf.Cos(angle) * right + Mathf.Sin(angle) * forward) * vertDistFromPoint;
            verts[i] = vert;
        }
        p.verts = verts;
    }
    private void GenerateMeshFromVerts()
    {
        List<Vector3> vertices = new();
        List<int> tris = new();

        int loopcount = verts.Count;

        foreach (Vector3[] loop in verts)
        {
            vertices.AddRange(loop);
        }

    }
    private class Point
    {
        public Vector3 position;
        public Vector3 direction;
        public Vector3[] verts;
    }
}
// Generate meshes
/*
 * ordered array of points
 * grab the direction at each point by lerping between the direction from the last point and the direction towards the new point
 * generate x number of vertecies around the point using the calculated direction
 * repeat for all points
 * generate a mesh by connecting all the vertecies
 */