/*using System;
using System.Runtime.InteropServices;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public struct Cell
{
    public Vector3 pos;
    public int alive;
    public int aliveFor;
    public Vector4 col;
};
public class Cloud : MonoBehaviour
{
  
    
    public ComputeShader shader;
    public ComputeBuffer cells;
    public ComputeBuffer nextGen;
    
    public Cell[] CloudCells;
    public Cell[] testingshiiit;
    public int[ ] nextGenCPU;
    public GameObject[] CellCubes;
    
    public int xSize;
    public int ySize;
    public int zSize;

    [SerializeField] private Material mat;
    private void Start()
    {
        
        Debug.Log(xSize*ySize*zSize );
        InstantiateArrays();
        
        CreateBuffer();
        
        
    }

 

    private void Update()
    {
        // if (Input.GetMouseButtonUp(0))
        // {
        //     foreach (var cell in CloudCells)
        //     {
        //         Debug.Log(cell.aliveFor);
        //     }
        // }
    }

    private void FixedUpdate()
    {
      
       Debug.Log("fixedUpdate was executed");

        
            shader.Dispatch(0, xSize / 8, ySize / 8, zSize / 8);
            nextGen.GetData(nextGenCPU);
            cells.GetData(CloudCells);
            int index = 0;
            for (int x = 0; x < xSize; x++)
            {
                for (int y = 0; y < ySize; y++)
                {
                    for (int z = 0; z < zSize; z++)
                    {
                        Cell cell = CloudCells[index];
                   
                        GameObject cellCube = CellCubes[index];
                        //cellCube.GetComponent<Renderer>().material.color = toLook.col;
                        if (nextGenCPU[index] == 1)
                        {
                            cellCube.SetActive(true);
                        }cellCube.SetActive(false);
                        

                        index++;
                    }
                }
            }
    }
    private void CreateBuffer()
    {
        shader.SetInt("_xSize", xSize);
        shader.SetInt("_ySize", ySize);
        shader.SetInt("_zSize", zSize);
        shader.SetInt("_bufferLength", xSize*ySize*zSize);
        int stride = Marshal.SizeOf(typeof(int));
        nextGen = new ComputeBuffer(nextGenCPU.Length, stride);
        nextGen.SetData(nextGenCPU);
        shader.SetBuffer(0, "nextGen", nextGen);
        
        stride = Marshal.SizeOf(typeof(Cell));
        cells = new ComputeBuffer(CloudCells.Length, stride);
        cells.SetData(CloudCells);
        
        shader.SetBuffer(0, "cells", cells);
    }
    
    private void InstantiateArrays()
    {
        CloudCells = new Cell[xSize* ySize* zSize];
        CellCubes = new GameObject[xSize* ySize* zSize];
        nextGenCPU = new int[xSize*ySize*zSize];
        testingshiiit = new Cell[xSize * ySize * zSize];
        int index = 0; 
        for (int x = 0; x < xSize; x++)
        {
            for (int y = 0; y < ySize; y++)
            {
                for (int z = 0; z < zSize; z++)
                {
             
                   
                        Cell toMake = new Cell();
                        toMake.pos = new Vector3(x, y, z);
                        toMake.col = new Vector4(0, 0, 0, 0);
                        
                        
                        float rng = Random.Range(0, 2);
                        if (rng<1)
                        {
                            toMake.alive = 1;
                        }else toMake.alive = 0;

                        
                        GameObject newCube = MakeCube(toMake);
                        CellCubes[index] = newCube;
                        if (toMake.alive == 1)
                        {
                            nextGenCPU[index] = 1;
                        }else nextGenCPU[index] = 0;
                        
                        CloudCells[index] = toMake;
                        index++;
                }
                
            }
            
        }
    }
    private GameObject MakeCube(Cell cell)
    {
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
       
        // Set Values of the cube                              
        cube.transform.position = cell.pos;
        cube.transform.parent = this.gameObject.transform;
        cube.name = new string(cell.pos.ToString());
        
        // Performance operations. get rid of unneeded features and enable perfomancewise needed ones
        Destroy(cube.GetComponent<BoxCollider>());
        
       // cube.isStatic = true;
        Material material = new Material(mat);
        MeshRenderer meshRenderer = cube.GetComponent<MeshRenderer>();
        meshRenderer.shadowCastingMode = ShadowCastingMode.Off;
        meshRenderer.receiveShadows = false;
        meshRenderer.allowOcclusionWhenDynamic = false;

        meshRenderer.material = material;
        
        material.enableInstancing = true; 
        
        return cube;
    }
        void OnDrawGizmos()
        {
            Handles.color=Color.red;
            Vector3 bounds = new Vector3(xSize, ySize, zSize);
            var objectpos = gameObject.transform.position;
            Vector3 center = new Vector3(objectpos.x+(bounds.x/2),objectpos.y+(bounds.y/2), objectpos.z+(bounds.z/2));
            Handles.DrawWireCube(center, bounds);
            
        }
}

*/
    // The CPU implementation with 3d arrays
    using System.Runtime.InteropServices;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.Rendering;
    using Random = UnityEngine.Random;

    public struct Cell
    {
        public Vector3 pos;
        public bool alive;
        public int aliveFor;
        public Vector4 col;
    };
    public class Cloud : MonoBehaviour
    {

        private Cell[, , ] CloudCells;
    private bool[, , ] CellsBuffer;
    private GameObject[,,] CellCubes;
    
    public int xSize;
    public int ySize;
    public int zSize;

    [SerializeField] private Material mat;
    private void Start()
    {
        Debug.Log(xSize*ySize*zSize );
        InstantiateArrays();
 
    }

    private void FixedUpdate()
    {
        for (int x = 0; x < xSize; x++)
        {
            for (int y = 0; y < ySize; y++)
            {
                for (int z = 0; z < zSize; z++)
                {
                    Cell toLook = CloudCells[x, y, z];
                    GameObject cellCube = CellCubes[x, y, z];
                    
                    // Vizualize by activating and deactivating the cubes
                    if (cellCube != null)
                    {
                        if (toLook.alive)
                        {
                            cellCube.SetActive(true);
                        }else cellCube.SetActive(false);
                    }

                  
                    
                    // the CA logic. writen into the buffer to be set all at once
                    var num = GetNeighbours(toLook, new Vector3Int(x, y, z));
                    if (toLook.alive && (num < 5 || num > 6))
                    {
                        CellsBuffer[x,y,z] = false;
                        toLook.aliveFor = 0 ;
                    }

                    else if (toLook.alive && (num >= 4 && num <= 7))
                    {
                        CellsBuffer[x,y,z] = true;
                        toLook.aliveFor++;
                    }
                    else if (toLook.alive == false && (num == 4 || num == 5))
                    {
                        CellsBuffer[x,y,z] = true;
                        toLook.aliveFor++;
                    }

                   // cellCube.GetComponent<Material>().color = new Color(2 * toLook.aliveFor, 0, 0, 0);
                }
            }
        }

        // Update CloudCells with the next generation
        for (int x = 0; x < xSize; x++)
        {
            for (int y = 0; y < ySize; y++)
            {
                for (int z = 0; z < zSize; z++)
                {
                    CloudCells[x, y, z].alive = CellsBuffer[x, y, z];
                }
            }
        }
    }
    
    private int GetNeighbours(Cell cell, Vector3Int pos)
    {
        int num = 0 ;
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1 ; j++)
            {
                for (int k = -1; k <= 1; k++)
                {
                    Vector3Int toLook = new Vector3Int(pos.x + i, pos.y + j, pos.z + k);
                    
                    if (i == 0 && j == 0 && k == 0)
                        continue;

                    if (toLook.x >= 0 && toLook.y >= 0 && toLook.z >= 0 && toLook.x <= xSize - 1 && toLook.y <= ySize - 1 && toLook.z <= zSize - 1) // check if you are out of bounds 
                    {
                        
                        if (CloudCells[toLook.x, toLook.y, toLook.z].alive)
                        {
                            num++;
                                
                        }
 
                    }
                }
            }
        }
       // Debug.Log(num);
        return num; 
    }

 
    private void InstantiateArrays()
    {
        CloudCells = new Cell[xSize, ySize, zSize];
        CellCubes = new GameObject[xSize, ySize, zSize];
        CellsBuffer = new bool[xSize,ySize,zSize];
        
        for (int x = 0; x < xSize; x++)
        {
            for (int y = 0; y < ySize; y++)
            {
                for (int z = 0; z < zSize; z++)
                {
             
                   
                        Cell toMake = new Cell();
                        toMake.pos = new Vector3(x, y, z);
                        float rng = Random.Range(0,2);
                       
                        if (rng<1)
                        {
                            toMake.alive = true;
                        }else toMake.alive = false;

                        CloudCells[x, y, z] = toMake;
                        
                        GameObject newCube = MakeCube(toMake);
                        CellCubes[x, y, z] = newCube;
                        CellsBuffer[x, y, z] = toMake.alive;

                }
                
            }
            
        }
    }
    private GameObject MakeCube(Cell cell)
    {
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
       
        // Set Values of the cube                              
        cube.transform.position = cell.pos;
        cube.transform.parent = this.gameObject.transform;
        cube.name = new string(cell.pos.ToString());
        
        // Performance operations. get rid of unneeded features and enable perfomancewise needed ones
        Destroy(cube.GetComponent<BoxCollider>());
        
       // cube.isStatic = true;
        
        MeshRenderer meshRenderer = cube.GetComponent<MeshRenderer>();
        meshRenderer.shadowCastingMode = ShadowCastingMode.Off;
        meshRenderer.receiveShadows = false;
        meshRenderer.allowOcclusionWhenDynamic = false;

        meshRenderer.material = mat;
        
        mat.enableInstancing = true; 
        
        return cube;
    }
        void OnDrawGizmos()
        {
            Handles.color=Color.red;
            Vector3 bounds = new Vector3(xSize, ySize, zSize);
            var objectpos = gameObject.transform.position;
            Vector3 center = new Vector3(objectpos.x+(bounds.x/2),objectpos.y+(bounds.y/2), objectpos.z+(bounds.z/2));
            Handles.DrawWireCube(center, bounds);
            
        }
}