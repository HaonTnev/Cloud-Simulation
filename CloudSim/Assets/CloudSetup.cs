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

    // Define Size of simulation space
    public int xSize;
    public int ySize;
    public int zSize;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
        
        Material mat = new Material();
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
