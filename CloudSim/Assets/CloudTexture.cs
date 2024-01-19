using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
/*
 * In this approach we try to store the data in a 3D texture where every pixel represents a cell.
 * We attach this script to the camera so we can render it.
 */
public class CloudTexture : MonoBehaviour
{
    public RenderTexture tex;
    public RenderTexture nextGen;
    public ComputeShader shader;
    
    public int xSize;
    public int ySize;
    public int zSize;
    // Start is called before the first frame update
    void Start()
    {
        CreateTex3D();
        
        nextGen = new RenderTexture(xSize, ySize, 0, RenderTextureFormat.Default);// initialize
        nextGen.volumeDepth = zSize;
        nextGen.dimension = TextureDimension.Tex3D; // set the dimension to 3d. although we kinda did that in the last line 
        nextGen.enableRandomWrite = true; // let us write to the texture 
        nextGen.wrapMode = TextureWrapMode.Clamp; // we have a finite number of cell
        nextGen.Create(); // actually creates the render texture xd
        
        shader.SetInt("xSize", xSize);
        shader.SetInt("ySize", ySize);
        shader.SetInt("zSize", zSize);

        float rng = Random.Range(0, 2);

        if (rng < .5f)
        {
            
        }
        
        Graphics.SetRandomWriteTarget(0, tex);
        Graphics.SetRandomWriteTarget(0, nextGen);
        shader.SetTexture(0, "tex", tex);
        shader.SetTexture(0, "nextGen", nextGen);
        shader.Dispatch(0,8,8,8);
    }

    private void CreateTex3D()
    {
        tex = new RenderTexture(xSize, ySize, 0, RenderTextureFormat.Default);// initialize
        tex.volumeDepth = zSize;
        tex.dimension = TextureDimension.Tex3D; // set the dimension to 3d. although we kinda did that in the last line 
        tex.enableRandomWrite = true; // let us write to the texture 
        tex.wrapMode = TextureWrapMode.Clamp; // we have a finite number of cell
        tex.Create(); // actually creates the render texture xd
    }

}
