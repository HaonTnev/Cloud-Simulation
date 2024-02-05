    using OpenCover.Framework.Model;using Unity.VisualScripting;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.Rendering;
    using UnityEngine.Serialization;
    using Random = UnityEngine.Random;

/*
This file holds our 3D Arrays and the CA logic which is needed for the simulation. 
The Simulation is done using the "CloudCell" struct.
I work with 3 distinct 3D Araays. Two of which "cloudCells" & "nextGenBuffer" are used to perform the actual logic and hold the Structs. 
While "cubes" is just there for visualization of the simulation. A thing I struggled with in the .compute approach. 
This code is mainly composed by stuff I wrote in the former mentioned .comute approaches.
*/


public class CloudCell
{
    public bool hum;
    public bool act;
    public bool cld;

   public bool Equals(CloudCell other)
    {
        return this.cld == other.cld && this.act == other.act && this.hum == other.hum;
    }
// no position vector needed. Position is defined by the 3D Array index.
}

    public class CloudSetup : MonoBehaviour
    {
        public CloudCell[,,] cloudCells;
        public CloudCell[,,] nextGenBuffer;
        public CloudCell[,,] coxZucker;
        

        public GameObject[,,] cubes; // Just for visualization

        public bool vizualize = true; // Will be used to toggle the visualization eventually

        [SerializeField] public enum _SimulationMethod
        { 
            SixCell, 
            SevenCell, 
            TenCell
        }

        [FormerlySerializedAs("simulationMethodMethod")] public _SimulationMethod simulationMethod;
        private Vector3Int[] sevenCellNeighborhood =
        {
            new Vector3Int(1, 0, 0),
            new Vector3Int(-1, 0, 0),
            new Vector3Int(0, 2, 0),
            new Vector3Int(0, 1, 0),
            new Vector3Int(0, -1, 0),
            new Vector3Int(0, 0, 1),
            new Vector3Int(0, 0, -1),
        };

        private Vector3Int[] sixCellNeighborhood = //Predefined set off offsets to use in the getNeighbors function
        {
            new Vector3Int(1, 0, 0),
            new Vector3Int(-1, 0, 0),
            new Vector3Int(0, 1, 0),
            new Vector3Int(0, -1, 0),
            new Vector3Int(0, 0, 1),
            new Vector3Int(0, 0, -1),

        };

        private Vector3Int [] tenCellNeighborhood = {
            new Vector3Int(1, 0, 0),
            new Vector3Int(-1, 0, 0),
            new Vector3Int(2, 0, 0),
            new Vector3Int(-2, 0, 0),
            new Vector3Int(0, 1, 0),
            new Vector3Int(0, -1, 0),
            new Vector3Int(0, 0, 1),
            new Vector3Int(0, 0, -1),
            new Vector3Int(0, 0, 2),
            new Vector3Int(0, 0, -2)
        };

    [SerializeField] private Material mat;

    // Define Size of simulation space
   
    [Range(3,70),Tooltip("Horizontal size of the simulation field.")]
    public int xSize;
    [Range(3,70),Tooltip("Vertical size of the simulation field.")]
    public int ySize;
    [Range(3,70),Tooltip("Diagonal size of the simulation field.")]
    public int zSize;

    // Start is called before the first frame update
    void Start()
    {
        Random.seed = 1337;
        print(xSize*ySize*zSize);
        SetupSimulationSpace();
        InstantiateArrays();
        cloudCells[xSize/2, ySize/2, zSize/2].act = true; // Cloud seed
        
    }

     private int iteration = 0;
    void FixedUpdate()
    {
        
            Automaton();
            UpdateToNextGen();
            if (vizualize)
            {
                VizualizeCloudCells();
            }
            print("updated");
        

        //Debug.Log("Updated");
       iteration++;
        //print(iteration);


    }

    private void UpdateToNextGen()
    {
        coxZucker = cloudCells;
        cloudCells = nextGenBuffer;
        nextGenBuffer = coxZucker;
    }

    private void Automaton()
    {
        for (int x = 0; x < xSize; x++)
        {
            for (int y = 0; y < ySize; y++)
            {
                for (int z = 0; z < zSize; z++)
                {
                    if (cloudCells[x, y, z].act == false && cloudCells[x, y, z].hum == true && HasNeighbours(new Vector3Int(x, y, z))) // update .act state
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
                    /*if (cloudCells[x,y,z].Equals(nextGenBuffer[x,y,z]))
                    {
                        continue;
                    }*/
                    GameObject cube = cubes[x, y, z];
                    MeshRenderer meshrenderer = cube.GetComponent<MeshRenderer>();
                    cube.SetActive(false);
                    if (cloudCells[x,y,z].hum)
                    {
                        cube.SetActive(false);
                        //meshrenderer.material.color = Color.red;
                        //meshrenderer.material.color.WithAlpha(100);
                    }
                    if (cloudCells[x,y,z].act)
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
    private bool HasNeighbours(Vector3Int pos)
    {
        if (simulationMethod==_SimulationMethod.SixCell)
        {
            foreach(Vector3Int offset in sixCellNeighborhood)
            {
                Vector3Int toLook = new Vector3Int(pos.x + offset.x, pos.y + offset.y,pos.z + offset.z);
            
                if (toLook.x >= 0 && toLook.y >= 0 && toLook.z >= 0 && toLook.x <= xSize - 1 && toLook.y <= ySize - 1 && toLook.z <= zSize - 1) // check if you are out of bounds
                {
                    if (cloudCells[toLook.x, toLook.y, toLook.z].act)
                    {
                        
                        return true;
                    }
                }
            }
        }
        if (simulationMethod==_SimulationMethod.SevenCell)
        {
            foreach(Vector3Int offset in sevenCellNeighborhood)
            {
                Vector3Int toLook = new Vector3Int(pos.x + offset.x, pos.y + offset.y,pos.z + offset.z);
            
                if (toLook.x >= 0 && toLook.y >= 0 && toLook.z >= 0 && toLook.x <= xSize - 1 && toLook.y <= ySize - 1 && toLook.z <= zSize - 1) // check if you are out of bounds
                {
                    if (cloudCells[toLook.x, toLook.y, toLook.z].act)
                    {
                        return true;
                    }
                }
            }
        }
        if (simulationMethod==_SimulationMethod.TenCell)
        {
            foreach(Vector3Int offset in sixCellNeighborhood)
            {
                Vector3Int toLook = new Vector3Int(pos.x + offset.x, pos.y + offset.y,pos.z + offset.z);
            
                if (toLook.x >= 0 && toLook.y >= 0 && toLook.z >= 0 && toLook.x <= xSize - 1 && toLook.y <= ySize - 1 && toLook.z <= zSize - 1) // check if you are out of bounds
                {
                    if (cloudCells[toLook.x, toLook.y, toLook.z].act)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
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
                        
                    float rng = Random.value * 2.0f;
                       
                    if (rng<1)
                    {
                        toMake.hum = true;
                    }else toMake.hum = false;

                    toMake.act = false;
                    toMake.cld = false;

                    cloudCells[x, y, z] = toMake;
                    nextGenBuffer[x, y, z] = new CloudCell();
                    if (vizualize)
                    {
                        GameObject newCube = MakeCube(x, y, z);
                        cubes[x, y, z] = newCube;
                    }
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
    private void SetupSimulationSpace() // setup the gameObject with correct transforms
    {
        gameObject.transform.localScale = new Vector3(xSize, ySize, zSize);     
        
    }
    void OnDrawGizmos()
    {
        Handles.color=Color.red;
        Vector3 bounds = new Vector3(xSize, ySize, zSize);
        var objectpos = gameObject.transform.position;
        Vector3 center = new Vector3(objectpos.x,objectpos.y, objectpos.z);
        Handles.DrawWireCube(center, bounds);
            
    }
}
