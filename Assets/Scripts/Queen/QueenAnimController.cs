using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class QueenAnimController : CommonAnimControl
{
    public bool useNormalMap;
    public TextAsset headTrack;
    public GameObject head;
    public SpriteAtlas normalAtlas;
    public RawImage testImage;
    private Vector3 trackPos0 = new Vector3(-0.32100001f, 0.574999988f);
    private Vector3 trackPos1 = new Vector3(-0.395000011f, 0.620000005f);
    private Dictionary<string, Dictionary<string, List<int>>> headC = null;
    private Dictionary<string, Sprite> normalMap = new Dictionary<string, Sprite>();
    protected override void OnUpdateSprite()
    {
        base.OnUpdateSprite();
        if(headC == null)
        {
            headC = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, List<int>>>>(headTrack.text);
        }
        
        var h = headC[currentAnim];
        if(h == null) return;
        var hp = h["Bip001 Head"];
        var id = (currentAnimFrame - 1) * 3;
        if(hp.Count <= id + 2) return;
        var x = hp[id] - 215;
        var y = hp[id + 1] - 197;
        head.transform.localPosition = new Vector3(x * 0.009250000125f + trackPos0.x,
            y * (-0.009250000125f) + trackPos0.y);

        if(!useNormalMap) return;
        currentAnimFrame--;
        var normalName = GetSpriteName();
        currentAnimFrame++;
        if(!normalMap.TryGetValue(normalName, out var v))
        {
            var t = normalAtlas.GetSprite(normalName);
            if(t == null) return;
            v = t;
            normalMap.Add(normalName, v);
        }
        var tex = v.texture;
        var r = v.textureRect;
        
        //r.position = v.textureRectOffset;
        var rw = new Rect(r.xMin / tex.width, r.yMin / tex.height, r.width / tex.width, r.height / tex.height);
        mainRenderer.material.SetTexture("_NormalMap", tex);
        mainRenderer.material.SetVector("_NormalMapRect", 
            new Vector4(rw.xMin, rw.yMin, rw.width, rw.height)
            );
        if(testImage != null)
        {
            testImage.texture = tex;
            testImage.uvRect = rw;
        }
        //Debug.Log($"{r.ToString()}({v.texture.width}, {v.texture.height})");
        //Vector3(-0.32100001, 0.574999988, -1); 215, 197, 0
        //Vector3(-0.395000011,0.620000005,-1); 207, 193, 0
    }
}
