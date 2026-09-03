/****************************************************************************
 * Copyright ©2021 Khoa Nguyen and Quan Dang. Adapted from CSE 457 Modeler by
 * Brian Curless. All rights reserved. Permission is hereby granted to
 * students registered for University of Washington CSE 457.
 * No other use, copying, distribution, or modification is permitted without
 * prior written consent. Copyrights for third-party components of this work
 * must be honored.  Instructors interested in reusing these course materials
 * should contact the authors below.
 * Khoa Nguyen: https://github.com/akkaneror
 * Quan Dang: https://github.com/QuanGary
 ****************************************************************************/
using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Mathf;

/// <summary>
/// SurfaceOfRevolution is responsible for generating a mesh given curve points.
/// </summary>

#if (UNITY_EDITOR)
public class SurfaceOfRevolution : MonoBehaviour
{
    private Mesh mesh;

    private List<Vector2> curvePoints;
    private int _mode;
    private int _numCtrlPts;
    private readonly string _curvePointsFile = "curvePoints.txt";
    private Vector3[] normals;
    private int[] triangles;
    private Vector2[] UVs;
    private Vector3[] vertices;

    private int subdivisions;
    public TextMeshProUGUI subdivisionText;

    private void Start()
    {
        subdivisions = 16;
        subdivisionText.text = "Subdivision: " + subdivisions.ToString();
    }

    private void Update()
    {
    }

    public void Initialize()
    {
        // Create an empty mesh
        mesh = new Mesh();
        mesh.indexFormat =
            UnityEngine.Rendering.IndexFormat.UInt32; // Set Unity's max number of vertices for a mesh to be ~4 billion
        GetComponent<MeshFilter>().mesh = mesh;

        // Load curve points
        ReadCurveFile(_curvePointsFile);

        // Invalid number of control points
        if (_mode == 0 && _numCtrlPts < 4 || _mode == 1 && _numCtrlPts < 2) return;
        
        // Calculate and draw mesh
        ComputeMeshData();
        UpdateMeshData();
    }


    /// <summary>
    /// Computes the surface revolution mesh given the curve points and the number of radial subdivisions.
    /// 
    /// Inputs:
    /// curvePoints : the list of sampled points on the curve.
    /// subdivisions: the number of radial subdivisions
    /// 
    /// Outputs:
    /// vertices : a list of `Vector3` containing the vertex positions
    /// normals  : a list of `Vector3` containing the vertex normals. The normal should be pointing out of
    ///            the mesh.
    /// UVs      : a list of `Vector2` containing the texture coordinates of each vertex
    /// triangles: an integer array containing vertex indices (of the `vertices` list). The first three
    ///            elements describe the first triangle, the fourth to sixth elements describe the second
    ///            triangle, and so on. The vertex must be oriented counterclockwise when viewed from the 
    ///            outside.
    /// </summary>
    private void ComputeMeshData()
    {
        // TODO: Compute and set vertex positions, normals, UVs, and triangle faces
        // You will want to use curvePoints and subdivisions variables, and you will
        // want to change the size of these arrays

        int vertexCount = curvePoints.Count * (subdivisions + 1);
        int triangleCount = (subdivisions) * 6 * (curvePoints.Count - 1);

        vertices = new Vector3[vertexCount];
        normals = new Vector3[vertexCount];
        UVs = new Vector2[vertexCount];
        triangles = new int[triangleCount];
        int count = 0;

        for (int i = 0; i < curvePoints.Count; i++) {

            for (int j = 0; j < subdivisions + 1; j++) {

                Vector2 currVertex = curvePoints[i];
                float currAngle = PI * 2f * j / subdivisions;
                float x = currVertex.x;
                float y = currVertex.y;

                float finalX = Cos(currAngle) * x + 0 * y + Sin(currAngle) * 0;
                float finalY = y;
                float finalZ = -Sin(currAngle) * x + 0 * y + Cos(currAngle) * 0;

                
                vertices[count] = new Vector3(finalX, finalY, finalZ);
                count++;
            }

        }

        int normalCount = 0;

        for (int i = 0; i < curvePoints.Count; i++) {

            for (int j = 0; j < subdivisions + 1; j++) {

                Vector2 tangent = new Vector2(0f, 0f);
                Vector3 z = new Vector3(0f, 0f, 1);
                Vector3 normal = new Vector3(0f, 0f, 0f);
                Vector2 currVertex = curvePoints[i];
                float currAngle = PI * 2f * j / subdivisions;

                if (i == curvePoints.Count - 1) {
                    tangent = curvePoints[i] - curvePoints[i - 1];

                } else if (i == 0) {
                    tangent = curvePoints[i + 1] - curvePoints[i];

                } else {
                    tangent = curvePoints[i + 1] - curvePoints[i - 1];

                }
                tangent = tangent.normalized;
                normal = Vector3.Cross(z, tangent);

                float finalX = Cos(currAngle) * normal.x + 0 * normal.y + Sin(currAngle) * normal.z;
                float finalY = normal.y;
                float finalZ = -Sin(currAngle) * normal.x + 0 * normal.y + Cos(currAngle) * normal.z;

                
                normals[normalCount] = new Vector3(finalX, finalY, finalZ).normalized;
                normalCount++;

            }
        }

        int uvCount = 0;
        float[] numerator = new float[curvePoints.Count];

        for (int d = 0; d < curvePoints.Count; d++) {

            if (d == 0) {
                numerator[d] = 0;
            } else {
                numerator[d] = numerator[d - 1] + Vector2.Distance(curvePoints[d - 1], curvePoints[d]);
            }
        }

        for (int i = 0; i < numerator.Length; i++) {
            for (int j = 0; j < subdivisions + 1; j++) {
                Vector2 currVertex = curvePoints[i];
                float currAngle = PI * 2f * j / subdivisions;
                float denominator = numerator[curvePoints.Count - 1];
                float v = numerator[i] / denominator;
                float u = 1 - (currAngle / (2f * PI));

                Vector2 uvVector = new Vector2(u, v);
                
                UVs[uvCount] = uvVector;
                uvCount++;
            }
        }

        int cornerCount = 0;
        for (int i = 0; i < curvePoints.Count - 1; i++) {
            for(int j = 0; j < subdivisions; j++) {

                int nextRow = i + 1;
                int nextCol = j + 1;
                int vertCount = subdivisions + 1;

                int botRight = (vertCount * i) + nextCol;
                int botLeft = (vertCount * i) + j;
                int topRight = (vertCount * nextRow) + nextCol;
                int topLeft = (vertCount * nextRow) + j;

                triangles[cornerCount] = botLeft;
                triangles[cornerCount + 1] = topLeft;
                triangles[cornerCount + 2] = topRight;
                triangles[cornerCount + 3] = botLeft;
                triangles[cornerCount + 4] = topRight;
                triangles[cornerCount + 5] = botRight;

                cornerCount += 6;
            }
        }

    }

    private void UpdateMeshData()
    {
        // Assign data to mesh
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.normals = normals;
        mesh.triangles = triangles;
        mesh.uv = UVs;
    }

    // Export mesh as an asset
    public void ExportMesh()
    {
        string path = EditorUtility.SaveFilePanel("Save Mesh Asset", "Assets/ExportedMesh/", mesh.name, "asset");
        if (string.IsNullOrEmpty(path)) return;
        path = FileUtil.GetProjectRelativePath(path);
        AssetDatabase.CreateAsset(mesh, path);
        AssetDatabase.SaveAssets();
    }

    public void SubdivisionValueChanged(Slider slider)
    {
        subdivisions = (int)slider.value;
        subdivisionText.text = "Subdivision: " + subdivisions.ToString();
    }
    
    private void ReadCurveFile(string file)
    {
        curvePoints = new List<Vector2>();
        string line;

        var f =
            new StreamReader(file);
        if ((line = f.ReadLine()) != null)
        {
            var curveData = line.Split(' ');
            _mode = Convert.ToInt32(curveData[0]);
            _numCtrlPts = Convert.ToInt32(curveData[1]);
        }

        while ((line = f.ReadLine()) != null)
        {
            var curvePoint = line.Split(' ');
            var x = float.Parse(curvePoint[0]);
            var y = float.Parse(curvePoint[1]);
            curvePoints.Add(new Vector2(x, y));
        }

        f.Close();
    }
}
#endif
