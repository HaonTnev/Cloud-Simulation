using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Cloud : MonoBehaviour
{
    public int[ , , ] CloudCells;
    public int xSize;
    public int ySize;
    public int zSize;
    private void Start()
    {
        CloudCells = new int[xSize,ySize,zSize];
        PopulateCloudCells();
        LoopThroughCloudCells();
       // Debug.Log(CloudCells[1,1,1]);
       Debug.Log(CloudCells.GetLength(0)+", " 
                                        +CloudCells.GetLength(1)+", " 
                                        +CloudCells.GetLength(2));
    }
    
    private void PopulateCloudCells()
    {
        int  k = 0;
        for (int x = 0; x < xSize; x++)
        {
            for (int y = 0; y < ySize; y++)
            {
                for (int z = 0; z < zSize; z++)
                {
                   
                        CloudCells[x, y, z] = k;
                        k++;
                }
                
            }
            
        }
    }
    private void LoopThroughCloudCells()
    {
       
        for (int x = 0; x < xSize; x++)
        {
            for (int y = 0; y < ySize; y++)
            {
                for (int z = 0; z < zSize; z++)
                {

                    GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                                      
                    cube.transform.position = new Vector3(x, y, z);
                    cube.transform.parent = this.gameObject.transform;
                    cube.name = new string(x.ToString()+", "+ y.ToString()+ ", "+z.ToString() );


                }
                
            }
            
        }
    }
}
