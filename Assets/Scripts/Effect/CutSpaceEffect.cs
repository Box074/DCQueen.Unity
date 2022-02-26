using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CutSpaceEffect : MonoBehaviour
{
    [System.Serializable]
    public class CutSpaceLine
    {
        public Vector2 from;
        public Vector2 to;
    }
    public List<CutSpaceLine> lines = new List<CutSpaceLine>();
    public float cutSize = 0;
    public Camera cam;
    public Material renderMat;
    private void Start()
    {
        if (Application.isEditor) renderMat = new Material(Shader.Find("DCQ/CutSpaceEffect"));
        cam = GetComponent<Camera>();
    }

    private float GetKB(Vector2 p0, Vector2 p1, out float b)
    {
        float tk = p0.x - p1.x;
        float k = (p0.y - p1.y) / (tk == 0 ? 1 : tk);
        b = p0.y - (k * p1.x);
        return k;
    }
    private float GetX(float k, float b, float y)
    {
        return (y - b) / k;
    }
    private float GetY(float k, float b, float x)
    {
        return k * x + b;
    }
    private void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (cutSize == 0 || lines.Count == 0)
        {
            Graphics.Blit(src, dest);
            return;
        }
        var temp = new RenderTexture(src);
        bool allSkip = true;
        bool lastUseTemp = false;
        //float z = cam.ViewportToWorldPoint(Vector3.zero).z;
        var size = new Vector2(cam.orthographicSize * 2, cam.orthographicSize * 2);
        var camB = new Rect();
        camB.size = size;
        camB.center = cam.transform.position;
        for (var i = 0; i < lines.Count; i++)
        {
            var v = lines[i];

            Material mat = renderMat;

            float ck = GetKB(v.from - camB.min, v.to - camB.min, out var cb);
            Vector2 top;
            Vector2 buttom;
            if (ck > 0)
            {
                top.x = GetX(ck, cb, camB.size.y);
                top.y = camB.size.y;
                if (top.x > camB.size.x)
                {
                    Debug.Log($"Top: {top}({camB})");
                    top.x = camB.size.x;
                    top.y = GetY(ck, cb, top.x);
                    Debug.Log($"Top2: {top}({camB})");
                    if (top.y > camB.size.y || top.y < 0) continue;
                }
                else if (top.x < 0) continue;

                buttom.x = GetX(ck, cb, 0);
                buttom.y = 0;
                if (buttom.x < 0)
                {
                    Debug.Log($"Buttom: {buttom}({camB})");
                    buttom.x = 0;
                    buttom.y = GetY(ck, cb, 0);
                    Debug.Log($"Buttom2: {buttom}({camB})");
                    if (buttom.y < 0 || buttom.y > camB.size.y) continue;
                }
                else if (buttom.x > camB.size.x) continue;
            }
            else
            {
                top.x = GetX(ck, cb, camB.size.y);
                top.y = camB.size.y;
                if (top.x < 0)
                {
                    Debug.Log($"Top: {top}({camB})");
                    top.x = 0;
                    top.y = GetY(ck, cb, top.x);
                    Debug.Log($"Top2: {top}({camB})");
                    if (top.y > camB.size.y || top.y < 0) continue;
                }
                else if (top.x > camB.size.x) continue;

                buttom.x = GetX(ck, cb, 0);
                buttom.y = 0;
                if (buttom.x > camB.size.x)
                {
                    Debug.Log($"Buttom: {buttom}({camB})");
                    buttom.x = camB.size.x;
                    buttom.y = GetY(ck, cb, buttom.x);
                    Debug.Log($"Buttom2: {buttom}({camB})");
                    if (buttom.y < 0 || buttom.y > camB.size.y) continue;
                }
                else if (buttom.x < 0) continue;
            }

            var from = new Vector2(top.x / camB.size.x, top.y / camB.size.y);
            var to = new Vector2(buttom.x / camB.size.x, buttom.y / camB.size.y);

            Debug.DrawLine(top + (Vector2)camB.min, buttom + (Vector2)camB.min, Color.red);

            Debug.Log($"From: {from} To: {to}");

            float k = GetKB(from, to, out var b);

            float begX = from.x;
            float begY = from.y;
            mat.SetFloat("_CutX", begX);
            mat.SetFloat("_CutY", begY);

            float endX = to.x;
            float endY = to.y;
            mat.SetFloat("_CutXTo", endX);
            mat.SetFloat("_CutYTo", endY);

            float loffset = 0;
            float roffset = 0;
            if (k < 0)
            {
                loffset = cutSize / src.width;
            }
            else
            {
                roffset = cutSize / src.width;
            }
            mat.SetFloat("_LOffset", loffset);
            mat.SetFloat("_ROffset", roffset);

            if (i < lines.Count - 1)
            {
                if (!lastUseTemp) Graphics.Blit(src, temp, mat, -1);
                else Graphics.Blit(temp, src, mat, -1);
            }
            else
            {
                if (!lastUseTemp) Graphics.Blit(temp, dest, mat, -1);
                else Graphics.Blit(src, dest, mat, -1);
            }
            lastUseTemp = !lastUseTemp;
            allSkip = false;
        }
        //temp.Release();
        if (allSkip)
        {
            Debug.Log("All Skip");
            Graphics.Blit(src, dest);
        }
    }
}
