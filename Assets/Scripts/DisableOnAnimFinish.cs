using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableOnAnimFinish : MonoBehaviour
{
    public CommonAnimControl animControl;
    public bool isDestroy;
    private void Update()
    {
        if (animControl != null)
        {
            if (animControl.isStop)
            {
                if (!isDestroy) gameObject.SetActive(false);
                else Destroy(gameObject);
            }
        }
    }
}
