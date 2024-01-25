using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cube3DTex : MonoBehaviour
{
    public Texture3D texture3D;

    void Start()
    {
        // Ensure the cube has a Renderer component
        Renderer renderer = GetComponent<Renderer>();
        if (renderer == null)
        {
            Debug.LogError("Renderer component not found on the cube.");
            return;
        }

        // Create a new material using the 3D texture shader
        Material material = new Material(Shader.Find("Custom/3DTextureShader"));
        material.SetTexture("_MainTex", texture3D);

        // Assign the material to the cube
        renderer.material = material;
    }
}