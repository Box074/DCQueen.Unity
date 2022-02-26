using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class CommonAnimControl : MonoBehaviour
{
    public string nameFormat;
    public int numberL;
    public Material rendererMat;
    public SpriteAtlas atlas;
    public SpriteRenderer[] renderers;
    public SpriteRenderer mainRenderer => (renderers?.Length > 0) ? renderers[0] : null;
    public CommonAnimControl getSpriteIdForm;
    public bool onlySetMainRenderer;
    public float everyFrameTime;
    public int currentAnimFrame;
    public string currentAnim;
    public float ftScale = 1;
    public bool isStop;
    public bool loop;
    public bool autoPlay;
    public bool isPause;
    public bool r;
    float lastFrameTime;
    Dictionary<string, Sprite> spriteCache = new Dictionary<string, Sprite>();
    // Start is called before the first frame update
    void OnEnable()
    {
        if (autoPlay)
        {
            isStop = false;
            currentAnimFrame = 0;
        }
    }
    void Start()
    {
        if (!onlySetMainRenderer)
        {
            foreach (var render in renderers)
            {
                if (rendererMat == null) render.material = new Material(Shader.Find("Sprites/Default"));
                else render.material = rendererMat;
            }
        }
        else
        {
            if (rendererMat == null) mainRenderer.material = new Material(Shader.Find("Sprites/Default"));
            else mainRenderer.material = rendererMat;
        }
    }
    protected virtual string GetSpriteName()
    {
        string spr = currentAnimFrame.ToString();
        if (spr.Length < numberL)
        {
            spr = string.Concat(new string('0', numberL - spr.Length), spr);
        }
        return string.Format(nameFormat, currentAnim, spr);
    }
    public ColliderControl cc;
    protected virtual void OnUpdateSprite()
    {
        cc?.UpdateCollider();
    }

    // Update is called once per frame
    void Update()
    {
        if (getSpriteIdForm == null)
        {
            if (isStop || isPause) return;
            if (Time.time - lastFrameTime < everyFrameTime * ftScale) return;
            lastFrameTime = Time.time;
            string sn = GetSpriteName();
            if(!spriteCache.TryGetValue(sn, out var sprite))
            {
                sprite = atlas.GetSprite(sn);
                spriteCache.Add(sn, sprite);
            }
            if (sprite == null)
            {
                if (!loop || currentAnimFrame == 0 || r)
                {
                    isStop = true;
                    currentAnimFrame = 0;
                }
                else
                {
                    isStop = false;
                    currentAnimFrame = 0;
                    Update();
                }
                return;
            }
            foreach (var render in renderers) render.sprite = sprite;
            if (r) currentAnimFrame--;
            else currentAnimFrame++;
            OnUpdateSprite();
        }
        else
        {
            currentAnim = getSpriteIdForm.currentAnim;
            currentAnimFrame = getSpriteIdForm.currentAnimFrame;
            string sn = GetSpriteName();
            if(!spriteCache.TryGetValue(sn, out var sprite))
            {
                sprite = atlas.GetSprite(sn);
                spriteCache.Add(sn, sprite);
            }
            if(sprite != null)
            {
                foreach (var render in renderers) render.sprite = sprite;
            }
        }
    }

    public void Play(string name, bool stopAndPlay = true)
    {
        r = false;
        isPause = false;
        loop = false;
        isStop = false;
        currentAnim = name;
        currentAnimFrame = 0;
    }
    public void PlayR(string name, int frame)
    {
        Play(name);
        currentAnimFrame = frame;
        r = true;
    }
    public bool IsPlaying(string name)
    {
        return currentAnim == name && !isStop && !isPause;
    }

    public void PlayLoop(string name)
    {
        Play(name);
        loop = true;
    }
    public void Stop()
    {
        currentAnimFrame = 0;
        isStop = true;
        isPause = false;
        r = false;
    }
    public IEnumerator PlayWait(string name)
    {
        Play(name, true);
        isStop = false;
        yield return Wait();
    }
    public IEnumerator Wait()
    {
        while (!isStop && !loop) yield return null;
    }
    public IEnumerator Wait(string name)
    {
        while (IsPlaying(name)) yield return null;
    }
    public IEnumerator WaitToFrame(int tf)
    {
        while (currentAnimFrame < tf && !isStop) yield return null;
    }
    public void Pause()
    {
        isPause = true;
    }
    public void Play()
    {
        r = false;
        if (isPause)
        {
            isPause = false;
        }
        else
        {
            Play(currentAnim, true);
        }
    }
}
