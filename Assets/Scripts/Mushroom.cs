using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mushroom : MonoBehaviour
{
    [SerializeField]
    private uint
        _layers,
        _roundness;

    [SerializeField]
    private float
        _thickness,
        _heightDetail;

    private MeshFilter   _mf;
    private MeshRenderer _mr;

    [SerializeField]
    private Material _mat;

    void Start ()
    {
        _mf = gameObject.AddComponent<MeshFilter>();
        _mr = gameObject.AddComponent<MeshRenderer>();
        BuildVertices();
    }

    private void Update()
    {
        //BuildVertices();
    }

    void BuildVertices()
    {
        Vector3[] vertices = new Vector3[_layers * _roundness];

        float half = (1.0f / _roundness) * Mathf.PI;

        for (int j = 0; j < _layers; j++)
        {
            for (int i = 0; i < _roundness; i++)
            {
                float extraRot = half * (j % 2); // for the rotation on uneven layers
                float rotation = (float)i / _roundness * 2 * Mathf.PI + extraRot;

                // rotate point
                Vector3 point = new Vector3(Mathf.Sin(rotation) * _thickness, _heightDetail * j, Mathf.Cos(rotation) * _thickness);
                point += transform.position;

                // add point to vertices
                vertices[i + j * _roundness] = point;
            }
        }

        int[] triangles = new int[6 * _roundness * _layers]; // 3 vertices * 2 triangles per layer

        for (int k = 0; k < _layers; k++) // for each layer
        {
            int kPos = k * 6 * (int)_roundness;
            int kVert = (k * (int)_roundness) / 2;

            for (int j = 0; j < _roundness; j++) // for each bottom triangle
            {
                for (int i = 0; i < 3; i++) // for each vertice
                {
                    int aPos = kPos + j*3 + i;

                    if (i == 2) // vertice is one more layer above current
                    {
                        triangles[aPos] = kVert + j + (int)_roundness;
                        continue;
                    }

                    triangles[aPos] = kVert + (j + i) % (int)_roundness; // set vertice on current layer
                }
            }


            for (int j = 0; j < _roundness; j++) // for each upper triangle
            {
                for (int i = 0; i < 3; i++)
                {
                    int aPos = kPos + j*3 + i + (3 * (int)_roundness);
                    int start = j % (int)_roundness;

                    if (i == 0) // if the first vertice, get bottom vertce
                    {
                        triangles[aPos] = kVert + start;
                        continue;
                    }

                    int next = start + (int)_roundness + 1 - i; // get vertices one layer above

                    if (next < _roundness) // check for false vertice
                        next += (int)_roundness;

                    triangles[aPos] = kVert + next;
                }
            }
        }

        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        _mf.mesh = mesh;
        _mr.material = _mat;
        
    }

    private void OnValidate()
    {
        if (!Application.isPlaying)
            return;

        BuildVertices();
    }

    void CalculateTriangles()
    {

    }
}
