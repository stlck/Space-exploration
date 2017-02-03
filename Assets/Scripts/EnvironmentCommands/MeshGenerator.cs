using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ProceduralToolkit;

public class MeshGenerator
{

    public LocationStation Test;
    public Mesh output;
    int[,] tiles;
    int count;
    public ProceduralBlocks BlockGenerator;
    public int Size = 50;

    public void newGen()
    {
        vertices = new List<Vector3>();
        triangles = new List<int>();
        //tiles = Test.MapTiles;
        tiles = BlockGenerator.GenerateMap(Size, Size);
        count = 0;
        for(int i = 1; i < Test.Size-2; i++)
            for(int j = 1; j < Test.Size-2; j++)
            {

                var sub = new[] {
                    tiles[i - 1, j],
                    tiles[i, j - 1],
                    tiles[i + 1, j],
                    tiles[i, j + 1] };
                switch (tiles[i, j])
                {
                    case 0:
                    case 2:
                    case 3:
                        //Debug.Log(i + " x " + j + " " + tiles[i,j] + +tiles[i, j]);
                        if (//tiles[i - 1, j] != 1 && tiles[i, j-1] != 1 && tiles[i + 1, j] != 1 && tiles[i, j + 1] != 1
                            sub.Count(m => m == 1) <= 1)
                        {
                            //MeshDraft.Quad(Vector3.right * i + Vector3.forward * j, Vector3.one * Test.TileSize, Vector3.one * Test.TileSize);
                            createGround(i, j);
                        }
                        else if (sub[0] != 1 && sub[1] != 1)//tiles[i - 1, j] != 1 && tiles[i, j - 1] != 1)
                            createTriangle(
                                Vector3.right * i + Vector3.forward * j,
                                Vector3.right * i + Vector3.forward * (j + 1),
                                Vector3.right * (i + 1) + Vector3.forward * j);
                        else if (sub[2] != 1 && sub[3] != 1)//tiles[i + 1, j] != 1 && tiles[i, j + 1] != 1)
                            createTriangle(
                                Vector3.right * (i + 1) + Vector3.forward * j,
                                Vector3.right * i + Vector3.forward * (j + 1),
                                Vector3.right * (i + 1) + Vector3.forward * (j + 1));
                        else if (sub[1] != 1 && sub[2] != 1)//tiles[i + 1, j] != 1 && tiles[i, j - 1] != 1)
                            createTriangle(
                                Vector3.right * (i) + Vector3.forward * (j),
                                Vector3.right * (i +1 ) + Vector3.forward * (j + 1),
                                Vector3.right * (i + 1) + Vector3.forward * (j));
                        //createGround(i, j);
                        else if (sub[0] != 1 && sub[3] != 1)//tiles[i-1, j] != 1 && tiles[i, j + 1] != 1)
                            createTriangle(
                                Vector3.right * (i) + Vector3.forward * (j),
                                Vector3.right * (i) + Vector3.forward * (j+1),
                                Vector3.right * (i + 1) + Vector3.forward * (j + 1));
                        //createGround(i, j);
                        break;
                    case 1:
                        break;
                }
            }

        output = new Mesh();
        //GetComponent<MeshFilter>().mesh = mesh;

        output.vertices = vertices.ToArray();
        output.triangles = triangles.ToArray();
        output.RecalculateNormals();
    }

    public void newGen2()
    {
        vertices = new List<Vector3>();
        triangles = new List<int>();
        //tiles = Test.MapTiles;
        tiles = BlockGenerator.GenerateMap(Size, Size);
        count = 0;
        output = new Mesh();
        var draft = new MeshDraft(output);

        for (int i = 1; i < Test.Size - 2; i++)
            for (int j = 1; j < Test.Size - 2; j++)
            {
                if(tiles[i,j] != 1)
                {
                    draft.Add(MeshDraft.Quad(Vector3.right *i + Vector3.forward * j, Vector3.right * Test.TileSize, Vector3.forward * Test.TileSize));
                }
            }

        output = draft.ToMesh();
    }

    void createTriangle(Vector3 f, Vector3 f2, Vector3 f3)
    {
        vertices.Add(Test.TileSize * (f));
        int i1 = count++;
        vertices.Add(Test.TileSize * (f2));
        int i2 = count++;
        vertices.Add(Test.TileSize * (f3));
        int i3 = count++;

        triangles.Add(i1);
        triangles.Add(i2);
        triangles.Add(i3);
    }

    void createGround(int i, int j)
    {
        // 2            2   4
        //
        // 1    3           3
        vertices.Add(Test.TileSize * (Vector3.right * i + Vector3.forward * j));
        int i1 = count++;
        vertices.Add(Test.TileSize * (Vector3.right * i + Vector3.forward * (j + 1)));
        int i2 = count++;
        vertices.Add(Test.TileSize * (Vector3.right * (i + 1) + Vector3.forward * j));
        int i3 = count++;

        vertices.Add(Test.TileSize * (Vector3.right * (i + 1) + Vector3.forward * j));
        int i4 = i3; count++;
        vertices.Add(Test.TileSize * (Vector3.right * i + Vector3.forward * (j + 1)));
        int i5 = i2; count++;
        vertices.Add(Test.TileSize * (Vector3.right * (i + 1) + Vector3.forward * (j + 1)));
        int i6 = count++;

        triangles.Add(i1);
        triangles.Add(i2);
        triangles.Add(i3);
                      
        triangles.Add(i4);
        triangles.Add(i5);
        triangles.Add(i6);
    }
    
    public MeshFilter walls;

     List<Vector3> vertices;
     List<int> triangles;
    
}