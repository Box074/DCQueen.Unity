using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShaderManager : MonoBehaviour
{
    public Material mat;
    private void Awake()
    {
        if (mat == null)
        {
            mat = new Material(Shader.Find("Sprites/Default"));
        }
        foreach (var v in GetComponents<Renderer>())
        {
            if (v.sharedMaterial == null || v.material == null) v.material = v.sharedMaterial = mat;
        }
        foreach (var v in GetComponentsInChildren<Renderer>(true))
        {
            if (v.sharedMaterial == null)
            {
                
                v.material = v.sharedMaterial = mat;
            }
        }
    }
}
