using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCreatMaterial : MonoBehaviour {

    [ContextMenuItem("test","test")]
    public Material Target;
    public Texture2D texture;

    public int TextureSize = 256;
    float floatSize;

    void test()
    {
        floatSize = (float)TextureSize;
        //if (texture == null)
        {
            texture = new Texture2D(TextureSize, TextureSize);
            
            generate();
            //texture.SetPixels(0,0,256,256, new Color[] { Color.grey });
            Target.SetTexture("_BumpMap ", texture);
        }
    }

	void generate()
    {
        var up = new Color(128,128,255);
        for (int i = 0; i < TextureSize; i++)
            for (int j = 0; j < TextureSize; j++)
                texture.SetPixel(i, j, up );//Color.white * Mathf.PerlinNoise((float)i / floatSize, (float)j / floatSize));
    }
}
