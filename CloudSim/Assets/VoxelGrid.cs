using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class VoxelGrid : MonoBehaviour
{
    private Vector3 _boundingBox;

    public float voxelSize;
    
    public ComputeBuffer VoxelBuffer;
    // Start is called before the first frame update
    void Start()
    {
        _boundingBox = GetComponent<BoxCollider>().size;
        _boundingBox *= 2;
        voxelSize = 0.5f;
        Debug.Log(_boundingBox+" "+ voxelSize);
       // GetVoxelAmount();
        VoxelBuffer = new ComputeBuffer(GetVoxelAmount(), 32);

    }

    int GetVoxelAmount()
    {
        var toReturn = ((_boundingBox.x * _boundingBox.y * _boundingBox.z)/ voxelSize);
        Debug.Log(toReturn);
        return (int)toReturn;
    }
    
}