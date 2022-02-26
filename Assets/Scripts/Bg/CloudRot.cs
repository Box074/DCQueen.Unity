using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudRot : MonoBehaviour
{
    public GameObject cloud;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    private float lastRottime;
    // Update is called once per frame
    void Update()
    {
        if(Time.time - lastRottime > 0.05f)
        {
            lastRottime = Time.time;
            cloud.transform.Rotate(0, 0, 0.01f);
        }
    }
}
