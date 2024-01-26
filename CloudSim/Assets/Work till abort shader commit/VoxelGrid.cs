using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

struct Voxel
{
    public Vector3 pos;
    public Color col;
}

public class VoxelGrid : MonoBehaviour
{
    public ComputeShader cloudShader;
    public RenderTexture renderTexture;
    
    private Vector3 _boundingBox;

    public float voxelSize;
    
    public ComputeBuffer VoxelBuffer;

    private Voxel[] voxels;
    void Start()
    {

       // voxels = new Voxel[GetVoxelAmount()];
        
        _boundingBox = GetComponent<BoxCollider>().size;
        _boundingBox *= 2;
        voxelSize = 0.5f;
        Debug.Log(_boundingBox+" "+ voxelSize);
       // GetVoxelAmount();
       PopulateVoxels();
    }

    int GetVoxelAmount()
    {
        var toReturn = ((_boundingBox.x * _boundingBox.y * _boundingBox.z)/ voxelSize);
        Debug.Log(toReturn);
        return (int)toReturn;
    }

    private void PopulateVoxels()
    {
        int voxelAmount = GetVoxelAmount();
        for (int i = 0; i < voxelAmount; i++)
        {
          //  voxels[i] = new Voxel();
            //voxels[i].pos = PosFromIndex(i);
           // Debug.Log(PosFromIndex(i)+", "+ i );
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);

            var position = cube.transform.position;
            position = PosFromIndex(i);
            cube.transform.position = position;
                
            cube.GetComponent<Renderer>().material.color = new Color(Mathf.Clamp(position.x,0, 1), Mathf.Clamp(position.y,0, 1),Mathf.Clamp(position.z,0, 1), 0f);
            
            cube.transform.parent = this.gameObject.transform;
        }
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (renderTexture== null)
        {
            renderTexture = new RenderTexture(256, 256, 24);
            renderTexture.enableRandomWrite = true;
            renderTexture.Create();
        }
         
        cloudShader.SetTexture(0,"Result", renderTexture);
        cloudShader.SetFloat("res", renderTexture.width);
        cloudShader.Dispatch(0, renderTexture.width/8, renderTexture.height/8, 1);
        
        Graphics.Blit(renderTexture, destination);
    }

    private void CreateVoxelBuffer()
    {
        VoxelBuffer = new ComputeBuffer(GetVoxelAmount(), 32);
        VoxelBuffer.SetData(voxels);
        
        cloudShader.SetBuffer(0, "name", VoxelBuffer);
        cloudShader.SetFloat("res", voxels.Length);
        cloudShader.Dispatch(0,voxels.Length/ 8, voxels.Length/8, voxels.Length/1);

        VoxelBuffer.GetData(voxels);
    }
    private Vector3 PosFromIndex(int index)
    {
        float x = index % _boundingBox.x;
        float y = (index / _boundingBox.x) % _boundingBox.y;
        float z = (index / (_boundingBox.x * _boundingBox.y));
        
        Vector3 pos = new Vector3(x, y, z);
        pos = pos * voxelSize - _boundingBox;
        
        return pos;
    }
}