    using Unity.VisualScripting;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.Rendering;
    using Random = UnityEngine.Random;

/*
This file holds our 3D Arrays and the CA logic which is needed for the simulation. 
The Simulation is done using the "CloudCell" struct.
I work with 3 distinct 3D Araays. Two of which "cloudCells" & "nextGenBuffer" are used to perform the actual logic and hold the Structs. 
While "cubes" is just there for visualization of the simulation. A thing I struggled with in the .compute approach. 
This code is mainly composed by stuff I wrote in the former mentioned .comute approaches.
*/


public struct CloudCell
{
public bool hum;
public bool act;
public bool cld;
// no position vector needed. Position is defined by the 3D Array index.
}

public class CloudSetup : MonoBehaviour
{
    public CloudCell[ , , ] cloudCells;
    public CloudCell[ , , ] nextGenBuffer;

    public GameObject [ , , ] cubes; // Just for visualization

    public bool vizualize = true; // Will be used to toggle the visualization eventually

    private Vector3Int[] sixCellNeighberhood = //Predefined set off offsets to use in the getNeighbors function
    {
        new Vector3Int(1, 0, 0),
        new Vector3Int(-1, 0, 0),
        new Vector3Int(0, 1, 0),
        new Vector3Int(0, -1, 0),
        new Vector3Int(0, 0, 1),
        new Vector3Int(0, 0, -1),

    };
    [SerializeField] private Material mat;

    // Define Size of simulation space
    public int xSize;
    public int ySize;
    public int zSize;

    // Start is called before the first frame update
    void Start()
    {
        print(xSize*ySize*zSize);
        InstantiateArrays();
        cloudCells[xSize/2, ySize/2, zSize/2].act = true; // Cloud seed
        // foreach (var offset in sixCellNeighberhood)
        // {
        //     Debug.Log(offset);
        // }
       
    }

    // Update is called once per frame

    private int iteration = 0;
    void FixedUpdate()
    {
        Automaton();
        UpdateToNextGen();
        VizualizeCloudCells();
        //Debug.Log("Updated");
        iteration++;
        print(iteration);


    }

    private void UpdateToNextGen()
    {
        cloudCells = nextGenBuffer;
    }

    private void Automaton()
    {
        for (int x = 0; x < xSize; x++)
        {
            for (int y = 0; y < ySize; y++)
            {
                for (int z = 0; z < zSize; z++)
                {
                    
                    var numNeighbors = GetNeighbours(new Vector3Int(x, y, z));
                   // print(numNeighbors);
                    if (cloudCells[x, y, z].act == false && cloudCells[x, y, z].hum == true && numNeighbors > 0) // update .act state
                    {
                        nextGenBuffer[x, y, z].hum = false;
                        nextGenBuffer[x, y, z].act = true;
                        nextGenBuffer[x, y, z].cld = false;
                    }

                    else if (cloudCells[x, y, z].act == true || cloudCells[x, y, z].cld == true) // update .cld state
                    {
                        nextGenBuffer[x, y, z].hum = false;
                        nextGenBuffer[x, y, z].act = false;
                        nextGenBuffer[x, y, z].cld = true;
                    }

                    else if (cloudCells[x, y, z].hum == true)
                    {
                        nextGenBuffer[x, y, z].hum = true;
                        nextGenBuffer[x, y, z].act = false;
                        nextGenBuffer[x, y, z].cld = false;
                    }
                }
            }
        }
    }

    private void VizualizeCloudCells()
    {
        for (int x = 0; x < xSize; x++)
        {
            for (int y = 0; y < ySize; y++)
            {
                for (int z = 0; z < zSize; z++)
                {
                    GameObject cube = cubes[x, y, z];
                    MeshRenderer meshrenderer = cube.GetComponent<MeshRenderer>();
                    cube.SetActive(false);
                    if (cloudCells[x,y,z].hum)
                    {
                        cube.SetActive(false);
                        meshrenderer.material.color = Color.red;
                    }
                    else if(cloudCells[x,y,z].act) 
                    {
                       cube.SetActive(true);
                        meshrenderer.material.color = Color.green;
                    }
                    else if(cloudCells[x,y,z].cld) 
                    {
                        cube.SetActive(true);
                        meshrenderer.material.color = Color.blue;
                    }
                    

                    cube.GetComponent<MeshRenderer>().material.color = meshrenderer.material.color;    
                    cubes[x, y, z] = cube;
                }
            }
        }
    }
    private int GetNeighbours(Vector3Int pos)
    {
        int num = 0 ;
        foreach(Vector3Int offset in sixCellNeighberhood)
        {
            Vector3Int toLook = new Vector3Int(pos.x + offset.x, pos.y + offset.y,pos.z + offset.z);
            
            if (toLook.x >= 0 && toLook.y >= 0 && toLook.z >= 0 && toLook.x <= xSize - 1 && toLook.y <= ySize - 1 && toLook.z <= zSize - 1) // check if you are out of bounds
            {
                if (cloudCells[toLook.x, toLook.y, toLook.z].act)
                {
                    num++;
                }
            }
        }
        return num; 
    }
    
    private void InstantiateArrays()
    {
        cloudCells = new CloudCell[xSize, ySize, zSize];
        nextGenBuffer = new CloudCell[xSize,ySize,zSize];
        cubes = new GameObject[xSize, ySize, zSize];
        
        for (int x = 0; x < xSize; x++)
        {
            for (int y = 0; y < ySize; y++)
            {
                for (int z = 0; z < zSize; z++)
                {
                    // Initially the act & cld fields are empty (false) and only the hum field is occupied (some are true)
                    // there will be ""Cloud seeds planted in the act field
             
                   
                    CloudCell toMake = new CloudCell();
                        
                    float rng = Random.Range(0,2);
                       
                    if (rng<1)
                    {
                        toMake.hum = true;
                    }else toMake.hum = false;

                    toMake.act = false;
                    toMake.cld = false;

                    cloudCells[x, y, z] = toMake;
                        
                    GameObject newCube = MakeCube(x,y,z);
                    cubes[x, y, z] = newCube;
                }
            }
        }
    }
    
    private GameObject MakeCube(int x, int y, int z)
    {
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
       
        // Set Values of the cube       
        cube.transform.parent = this.gameObject.transform;                       
        cube.transform.position = new Vector3(x,y,z);
        
        //cube.name = new string(x,y,z.ToString());
        
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
