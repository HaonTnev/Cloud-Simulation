using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

struct Cube
{
    public Vector3 pos;
    public Color col;
    
};

//[ExecuteAlways]
public class ShaderSetup : MonoBehaviour

{
    [SerializeField] private Vector3 volumePos = new Vector3(0,0,0);
    [SerializeField] private Vector4 volumeExtend = new Vector4(15, 2.5f, 15, 0);
    [SerializeField] private float voxelSize = 0.5f;

    [SerializeField] ComputeShader shader;
    private int voxelAmount;
    void Start()
    {
        voxelAmount = GetVoxelAmount();
        Debug.Log(voxelAmount);
        Debug.Log(shader.FindKernel("CSMain"));
         CreateBuffer();
         ShowVoxels();
        
    }

    private Cube[] voxels;
    private void CreateBuffer()
    { 
        ComputeBuffer voxelBuffer = new ComputeBuffer(voxelAmount, 28);

       voxels = new Cube[voxelAmount];

       for (int i = 0; i < voxelAmount; i++)
       {
           voxels[i] = new Cube();
       }
     //  Debug.Log(voxels[50].pos+"" + " "+voxels[50].col);
       shader.SetFloat("voxelSize", voxelSize);
       shader.SetVector("volumeExtend",volumeExtend);
        
        voxelBuffer.SetData(voxels);
        
        shader.SetBuffer(0, "voxels", voxelBuffer);
        shader.Dispatch(0,64, 1, 1);

        voxelBuffer.GetData(voxels);
        voxelBuffer.Dispose();
      //  Debug.Log(voxels[50].pos+"" + " "+voxels[50].col);
        
    }

    
   private void ShowVoxels()
   {


        foreach (var voxel in voxels)
        {
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.position = voxel.pos;
            cube.GetComponent<MeshRenderer>().material.color = voxel.col;
        }
        
    }
    
    private Vector3 PosFromIndex(int index)
    {
        float x = index % volumeExtend.x;
        float y = (index / volumeExtend.x) % volumeExtend.y;
        float z = (index / (volumeExtend.x * volumeExtend.y));
        
        Vector3 pos = new Vector3(x, y, z);
     //   pos = pos * voxelSize - volumeExtend;
        
        return pos;
    }
    
    int GetVoxelAmount()
    {
        
        var toReturn = ((volumeExtend.x *2) *(volumeExtend.y*2) * (volumeExtend.z)*2);
        Debug.Log((toReturn));
        toReturn = toReturn / voxelSize;
        Debug.Log((toReturn));
        return (int)toReturn;
    }
    
    void OnDrawGizmos()
    {
        Handles.color=Color.red;
        Handles.DrawWireCube(volumePos, volumeExtend);
        
    }
}
