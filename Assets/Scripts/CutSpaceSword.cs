using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutSpaceSword : MonoBehaviour
{
    public List<GameObject> lineS = new List<GameObject>();
    public GameObject cutPrefab;
    public float cutWidth;
    public GameObject target;
    public GameObject hc;
    public Material effectMat;
    public CutSpaceEffect CutSpaceEffect
    {
        get
        {
            if(_cutSpaceEffect == null)
            {
                _cutSpaceEffect = Camera.main.GetComponent<CutSpaceEffect>();
                if(_cutSpaceEffect == null)
                {
                    _cutSpaceEffect = Camera.main.gameObject.AddComponent<CutSpaceEffect>();
                    _cutSpaceEffect.renderMat = effectMat;
                }
            }
            return _cutSpaceEffect;
        }
    }
    private CutSpaceEffect _cutSpaceEffect;
    public bool trigger;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(trigger)
        {
            trigger = false;
            DoAttack();
        }
    }

    public GameObject DoAttack()
    {
        return DoAttack(hc.transform.position, target.transform.position);
    }
    private void OnEnable() {
        StartCoroutine(FireLoop());
    }
    private void OnDisable() {
        StopAllCoroutines();
    }
    private IEnumerator FireLoop()
    {
        yield return null;
        while(true)
        {
            yield return null;
            if(lineS.Count == 0) continue;
            yield return new WaitForSeconds(1.25f);
            yield return Fire();
        }
    }
    public IEnumerator Fire()
    {
        CutSpaceEffect.enabled = true;
        CutSpaceEffect.cutSize = cutWidth;
        foreach (var v in lineS)
        {
            v.transform.Find("N").gameObject.SetActive(false);
            v.transform.Find("T").gameObject.SetActive(true);
            v.GetComponent<Collider2D>().enabled = true;
        }

        yield return new WaitForSeconds(0.15f);
        foreach (var v in lineS)
        {
            v.GetComponent<Collider2D>().enabled = false;
        }
        float t = 0;
        while (t <= 0.5f)
        {
            yield return null;
            t += Time.deltaTime;
            CutSpaceEffect.cutSize = Mathf.Lerp(cutWidth, 0, Mathf.Min(t * 2, 1));
            foreach (var v in lineS)
            {
                v.transform.Find("N").gameObject.SetActive(false);
                v.transform.Find("T").gameObject.SetActive(true);
                v.transform.localScale = new Vector3(Mathf.Lerp(1, 0, Mathf.Min(t * 2, 1)), 1, 1);
            }
        }
        foreach(var v in lineS) Destroy(v);
        lineS.Clear();
        CutSpaceEffect.lines.Clear();
    }

    public GameObject DoAttack(Vector2 heroPos, Vector2 target)
    {
        Bounds bb = new Bounds(heroPos, new Vector3(80,80));
        var lines = CutSpaceEffect.lines;
        Vector2 p0 = new Vector2(Random.Range(bb.min.x, bb.max.x), bb.max.y + 10);
        Vector2 p1;
        float p1y = bb.min.y - 20;
        float k = 0;
        float b = 0;
        if (target != null)
        {
            var need = target;
            float s = p0.x - need.x;
            k = (p0.y - need.y) / (s == 0 ? 1 : s);
            b = p0.y - (k * p0.x);
        }
        else
        {
            float x = Random.Range(bb.min.x, bb.max.x);
            float s = p0.x - x;
            k = (p0.y - p1y) / (s == 0 ? 1 : s);
            b = p0.y - (k * p0.x);
        }
        p1 = new Vector2((k == 0) ? 0 : ((p1y - b) / k), p1y);
        lines.Add(new CutSpaceEffect.CutSpaceLine()
        {
            from = p0,
            to = p1
        });
        GameObject line = Instantiate(cutPrefab);
        lineS.Add(line);
        float ly = (bb.max.y - p1y) / 2 + p1y;
        float lx = (k == 0) ? 0 : ((ly - b) / k);
        line.transform.position = new Vector3(lx, ly, 0);
        float angle = Mathf.Atan2(p0.y - p1.y, p0.x - p1.x) * Mathf.Rad2Deg - 90;
        line.transform.eulerAngles = new Vector3(0, 0, angle);
        return line;
    }
}
