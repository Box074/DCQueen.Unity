using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HKTool.Unity;

public class QueenController : MonoBehaviour
{
    public int nextAction = 0;
    public int prevAction = 0;
    public GameObject target;


    public QueenAnimController animController;
    public CommonAnimControl effectCtrl;
    public HealthManager hm;
    public System.Action OnDeath;
    public bool facingRight;
    public BoxCollider2D battelArea;
    public Rigidbody2D rig;
    public Collider2D col;
    public BoxCollider2D gdamageBox;
    public BoxCollider2D respawnArea;
    public AudioSource audioSource;
    public GameObject killPetPoint;
    public bool shouldBreakSkill;
    [Header("Cut Space(deprecated)")]
    public CutSpaceEffect cutSpaceEffect;
    public float cutSpaceWidth;
    public Texture2D cutSpaceTex;
    public GameObject cutPrefab;


    [Header("Skill HitBox")]
    public GameObject estocHeadDB;
    public GameObject estocFeetDB;
    public GameObject estocArcDB;
    public GameObject overshieldDB;
    public GameObject lungeDB;
    public GameObject poisonCloudDB;
    public GameObject cutGrenadeDB;
    public GameObject shockWaveDB;
    public GameObject groundStompDB;
    [Header("Audio")]
    public AudioClip enm_queen_vocal1;
    public AudioClip enm_queen_ccq_release1;
    public AudioClip enm_queen_vocal2;
    public AudioClip enm_queen_ccq_release2;
    public AudioClip enm_queen_vocal3;
    public AudioClip enm_queen_ccq_release3;
    public AudioClip enm_queen_vocal10;
    public AudioClip enm_queen_ccso_release;
    public AudioClip enm_queen_lunge_charge;
    public AudioClip enm_queen_lunge_release;
    public AudioClip enm_queen_vocal6;
    public AudioClip enm_queen_dispel_aoe_release;
    public AudioClip enm_queen_grab_voiceless;
    public AudioClip enm_queen_disable_grenade_release;
    public AudioClip enm_queen_split_charge;
    public AudioClip enm_queen_split_release;
    public AudioClip enm_queen_shockwave_charge;
    public AudioClip enm_queen_shockwave_release;
    public AudioClip weapon_shield_charge;
    public AudioClip weapon_shield_block;
    [Header("Other")]
    public bool canDamage;
    public float invTime;
    public bool touchWall;
    public bool touchRight;
    public int hp;
    private void Turn(bool fright)
    {
        if (fright)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        facingRight = fright;
    }
    private void FaceTarget()
    {
        if (target.transform.position.x < transform.position.x) Turn(false);
        else Turn(true);
    }
    private void Awake()
    {
        hm = gameObject.GetComponent<HealthManager>();

    }
    private void Start()
    {
        cutSpaceEffect = Camera.main.GetComponent<CutSpaceEffect>();
        if (cutSpaceEffect == null)
        {
            cutSpaceEffect = Camera.main.gameObject.AddComponent<CutSpaceEffect>();
        }
        cutSpaceEffect.enabled = false;
    }
    private bool CTarget(Vector2 pos, float minDis, float maxDis)
    {
        var v0 = (Vector2)target.transform.position - pos;
        return v0.x >= minDis && v0.x <= maxDis;
    }
    private IEnumerator DashAction(float speed, string anim, float maxTime = float.MaxValue)
    {
        var v = new Vector2(speed * (facingRight ? 1 : -1), 0);
        rig.velocity = v;
        yield return null;
        while (animController.IsPlaying(anim) && (rig.velocity.x > 0.1f || rig.velocity.x < -0.1f)
             && (!touchWall || (touchWall && touchRight == facingRight)))
        {
            maxTime -= Time.deltaTime;
            if (maxTime <= 0) break;
            rig.velocity = v;
            if (col.bounds.min.x < battelArea.bounds.min.x)
            {
                transform.position = new Vector3(battelArea.bounds.min.x,
                    transform.position.y, 0);

                break;
            }
            if (col.bounds.max.x > battelArea.bounds.max.x)
            {
                transform.position = new Vector3(battelArea.bounds.max.x,
                    transform.position.y, 0);

                break;
            }
            yield return null;
        }
        rig.velocity = Vector2.zero;
    }
    private IEnumerator Fall()
    {
        if (animController.IsPlaying("fall"))
        {
            animController.PlayLoop("fall");
        }
        yield return null;
        while (true)
        {
            yield return null;
            if (col.bounds.min.y < gdamageBox.bounds.min.y)
            {
                transform.position = new Vector3(
                    Random.Range(respawnArea.bounds.min.x, respawnArea.bounds.max.x), respawnArea.bounds.max.y, 0);
                hp -= (int)(hp * 0.03f);
                break;
            }
        }
    }
    private IEnumerator EstocHead()
    {
        audioSource.PlayOneShot(enm_queen_vocal1);
        yield return animController.PlayWait("loadEstocHead");
        audioSource.PlayOneShot(enm_queen_ccq_release1);
        animController.Play("estocHead");

        effectCtrl.Play("fxEstocHead");
        effectCtrl.gameObject.SetActive(true);

        yield return animController.WaitToFrame(2);
        estocHeadDB.SetActive(true);
        yield return animController.Wait("estocHead");
        estocHeadDB.SetActive(false);
        yield return animController.PlayWait("estocHeadIdle");
    }
    private IEnumerator EstocFeet()
    {
        audioSource.PlayOneShot(enm_queen_vocal2);
        yield return animController.PlayWait("loadEstocFeet");
        audioSource.PlayOneShot(enm_queen_ccq_release2);
        animController.Play("estocFeet");

        effectCtrl.Play("fxEstocFeet");
        effectCtrl.gameObject.SetActive(true);

        yield return animController.WaitToFrame(2);
        estocFeetDB.SetActive(true);
        yield return animController.Wait("estocFeet");
        estocFeetDB.SetActive(false);
        yield return animController.PlayWait("estocFeetIdle");
    }
    private IEnumerator EstocArc()
    {
        audioSource.PlayOneShot(enm_queen_vocal3);
        yield return animController.PlayWait("loadEstocArc");
        audioSource.PlayOneShot(enm_queen_ccq_release3);
        animController.Play("estocArc");

        effectCtrl.Play("fxEstocArc");
        effectCtrl.gameObject.SetActive(true);

        yield return animController.WaitToFrame(2);
        estocArcDB.SetActive(true);
        yield return animController.Wait("estocArc");
        estocArcDB.SetActive(false);
        yield return animController.PlayWait("estocArcIdle");
    }
    private IEnumerator OverShield()
    {
        audioSource.PlayOneShot(enm_queen_vocal10);
        yield return animController.PlayWait("loadOvershield");

        animController.Play("overshield");

        yield return animController.WaitToFrame(14);
        audioSource.PlayOneShot(enm_queen_ccso_release);
        overshieldDB.SetActive(true);

        effectCtrl.Play("fxOvershield");
        effectCtrl.gameObject.SetActive(true);

        yield return animController.WaitToFrame(25);
        overshieldDB.SetActive(false);
        yield return animController.Wait("overshield");
    }
    private IEnumerator GroundStomp()
    {
        effectCtrl.Play("fxGroundStomp");
        effectCtrl.gameObject.SetActive(true);

        audioSource.PlayOneShot(weapon_shield_charge);
        yield return animController.PlayWait("loadGroundStomp");
        audioSource.PlayOneShot(weapon_shield_block);
        animController.Play("groundStomp");

        yield return effectCtrl.WaitToFrame(8);
        groundStompDB.SetActive(true);
        yield return effectCtrl.Wait();
        groundStompDB.SetActive(false);
        yield return animController.Wait();
    }
    private IEnumerator Lunge()
    {
        audioSource.PlayOneShot(enm_queen_lunge_charge);
        yield return animController.PlayWait("idleLoadLunge");

        lungeDB.SetActive(true);
        audioSource.PlayOneShot(enm_queen_lunge_release);
        animController.Play("lunge");
        yield return DashAction(45, "lunge", 0.35f);

        lungeDB.SetActive(false);
        yield return animController.PlayWait("lungeIdle");
    }
    private IEnumerator PoisonCloud()
    {
        audioSource.PlayOneShot(enm_queen_vocal6);
        effectCtrl.Play("fxPoisonCloud");
        effectCtrl.gameObject.SetActive(true);
        yield return animController.PlayWait("loadPoisonCloud");
        audioSource.PlayOneShot(enm_queen_dispel_aoe_release);
        animController.Play("poisonCloud");
        poisonCloudDB.SetActive(true);
        yield return animController.WaitToFrame(12);
        poisonCloudDB.SetActive(false);
        yield return animController.Wait("poisonCloud");
    }
    private IEnumerator CutGrenade()
    {
        audioSource.PlayOneShot(enm_queen_grab_voiceless);
        yield return animController.PlayWait("loadCutGrenade");
        animController.Play("cutGrenade");

        effectCtrl.Play("fxCutGrenade");
        effectCtrl.gameObject.SetActive(true);

        audioSource.PlayOneShot(enm_queen_disable_grenade_release);
        cutGrenadeDB.SetActive(true);
        yield return animController.WaitToFrame(15);
        cutGrenadeDB.SetActive(false);
        yield return animController.Wait("cutGrenade");
    }
    private IEnumerator ShockWave()
    {
        audioSource.PlayOneShot(enm_queen_shockwave_charge);
        effectCtrl.Play("fxShockwave");
        effectCtrl.gameObject.SetActive(true);

        yield return animController.PlayWait("loadShockWave");
        audioSource.PlayOneShot(enm_queen_shockwave_release);
        shockWaveDB.SetActive(true);
        animController.Play("shockWave");
        yield return effectCtrl.WaitToFrame(21);
        shockWaveDB.SetActive(false);
        yield return animController.Wait();
    }
    private IEnumerator DashToHero()
    {
        yield return null;
        FaceTarget();
        animController.PlayLoop("dash");
        yield return null;
        while (!CTarget(transform.position, 0, 1.5f) && (!touchWall || (touchWall && touchRight != facingRight)))
        {
            yield return null;
            FaceTarget();
            if (facingRight)
            {
                rig.velocity = new Vector2(15, 0);
            }
            else
            {
                rig.velocity = new Vector2(-15, 0);
            }
        }
        rig.velocity = Vector2.zero;
        yield return animController.PlayWait("dashIdle");
    }
    private IEnumerator DashToHeroDef()
    {
        yield return null;
        FaceTarget();
        animController.PlayLoop("dash");
        yield return null;
        while (!CTarget(transform.position, 0, 1.5f) && (!touchWall || (touchWall && touchRight != facingRight)))
        {
            yield return null;
            FaceTarget();
            if (facingRight)
            {
                rig.velocity = new Vector2(15, 0);
            }
            else
            {
                rig.velocity = new Vector2(-15, 0);
            }
        }
        rig.velocity = Vector2.zero;
        yield return animController.PlayWait("dashIdleDEFENSE");
    }
    private float backDashCoolTime = 0;
    private IEnumerator BackDash()
    {
        if(Time.time - backDashCoolTime < 2.5f)
        {
            nextAction = 4;
            yield break;
        }
        backDashCoolTime = Time.time;
        yield return null;
        FaceTarget();
        animController.PlayLoop("backDash");
        yield return null;
        while (CTarget(transform.position, 0, 3.5f) && (!touchWall || (touchWall && touchRight == facingRight)))
        {
            yield return null;
            FaceTarget();
            if (facingRight)
            {
                rig.velocity = new Vector2(-15, 0);
            }
            else
            {
                rig.velocity = new Vector2(15, 0);
            }
        }
        rig.velocity = Vector2.zero;
        yield return animController.PlayWait("backDashIdleDEFENSE");
    }
    //TODO
    private IEnumerator Massivecuts()
    {

        yield return animController.PlayWait("idleMassivecuts");
        animController.PlayLoop("massivecuts");
        int count = Random.Range(5, 10);
        Bounds bb = battelArea.bounds;
        var lines = cutSpaceEffect.lines;
        lines.Clear();
        List<GameObject> lineS = new List<GameObject>();
        for (int i = 0; i < count; i++)
        {
            Vector2 p0 = new Vector2(Random.Range(bb.min.x, bb.max.x), bb.max.y + 10);
            Vector2 p1;
            float p1y = bb.min.y - 20;
            float k = 0;
            float b = 0;
            if (target != null)
            {
                var need = target.transform.position;
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
            audioSource.PlayOneShot(enm_queen_split_charge);
            yield return new WaitForSeconds(0.75f);
            audioSource.PlayOneShot(enm_queen_split_release);
            cutSpaceEffect.enabled = true;
            cutSpaceEffect.cutSize = cutSpaceWidth;
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
                cutSpaceEffect.cutSize = Mathf.Lerp(cutSpaceWidth, 0, Mathf.Min(t * 2, 1));
                foreach (var v in lineS)
                {
                    v.transform.Find("N").gameObject.SetActive(false);
                    v.transform.Find("T").gameObject.SetActive(true);
                    v.transform.localScale = new Vector3(Mathf.Lerp(1, 0.5f, Mathf.Min(t * 2, 1)), 1, 1);
                }
            }
            if (i != count - 1)
            {
                foreach (var v in lineS)
                {
                    v.transform.Find("N").gameObject.SetActive(true);
                    v.transform.Find("T").gameObject.SetActive(false);
                    v.transform.localScale = Vector3.one;
                }
            }
            else
            {
                while (t <= 0.25f)
                {
                    foreach (var v in lineS)
                    {
                        v.transform.localScale = new Vector3(Mathf.Lerp(1, 0, Mathf.Min(t * 4, 1)), 1, 1);
                    }
                }
            }
            cutSpaceEffect.enabled = false;
            yield return new WaitForSeconds(0.15f);
        }
        foreach (var v in lineS) Destroy(v);
        yield return animController.PlayWait("massivecutsIdle");
    }
    private IEnumerator KillPet(GameObject pet)
    {
        foreach(var v in pet.GetComponents<Component>())
        {
            if(!(v is Transform) && !(v is Renderer))
            {
                Destroy(v);
            }
        }
        yield return animController.PlayWait("idleBulletStop");
        animController.PlayLoop("bulletStop");
        while((killPetPoint.transform.position - pet.transform.position).sqrMagnitude > 1)
        {
            pet.transform.position = Vector2.MoveTowards(pet.transform.position, killPetPoint.transform.position,
                0.15f);
            yield return null;
        }
        animController.Play("killPet");
        yield return animController.WaitToFrame(14);
        Destroy(pet);
        yield return animController.Wait();
    }
    float killPetCoolTime = 0;
    private IEnumerator KillPetSkill()
    {
        if(Time.time - killPetCoolTime < 3)
        {
            nextAction = 11;
            yield break;
        }
        killPetCoolTime = Time.time;
        foreach(var v in FindObjectsOfType<GameObject>(false))
        {
            if(v.name.StartsWith("Grimmchild") || v.name.StartsWith("Weaverling"))
            {
                yield return KillPet(v);
                yield break;
            }
        }
        nextAction = 11;
    }
    private IEnumerator AfterSkill()
    {
        if(nextAction != 0) yield break;
        yield return null;
        FaceTarget();
        if(target.transform.position.y > col.bounds.max.y && prevAction != 12)
        {
            nextAction = 12;
            yield break;
        }
        bool lastIdle = prevAction == 0 || endIdle.Any(x => x == prevAction);

        if (lastIdle)
        {
            if (CTarget(transform.position, 0, 1.5f) && (!touchWall || (touchWall && touchRight == facingRight)))
            {
                nextAction = idleNearSkill[Random.Range(0, idleNearSkill.Length)];
            }
            else
            {
                var r = Random.Range(0, 100);
                if(r < 55) nextAction = dash[Random.Range(0, dash.Length)];
                else if(r < 88) nextAction = idleFarSkill[Random.Range(0, idleFarSkill.Length)];
                else
                {
                    nextAction = 13;
                }
            }
            yield break;
        }
        else
        {
            if (CTarget(transform.position, 0, 1.5f) && (!touchWall || (touchWall && touchRight == facingRight)))
            {
                nextAction = shieldSkill[Random.Range(0, shieldSkill.Length)];
            }
            else
            {
                nextAction = dash[Random.Range(0, dash.Length)];
            }
        }
    }
    private int[] idleNearSkill = new int[]{
        2, 4, 8, 9, 7
    };
    private int[] dash = new int[]{
        10, 11, 10, 10, 11, 7
    };
    private int[] idleFarSkill = new int[]{
        7
    };
    private int[] idleSkill = new int[]{
        2, 4, 7, 8, 9
    };
    private int[] shieldSkill = new int[]{
        1, 2, 3, 5, 6,
    };
    private int[] endShield = new int[]{
        5, 6, 9, 11, 13
    };
    private int[] endIdle = new int[]{
        1, 2, 3, 4, 7, 8, 10
    };
    private IEnumerator Die()
    {
        yield return animController.PlayWait("death");
        animController.PlayLoop("deathLoop");
        OnDeath?.Invoke();
        hm.isDead = true;
        hm.hp = 0;
    }
    private IEnumerator HitFlash()
    {
        yield return null;
        animController.mainRenderer.material.SetFloat("_Flash", 0.75f);
        yield return new WaitForSeconds(0.15f);
        animController.mainRenderer.material.SetFloat("_Flash", 0);
    }
    private IEnumerator SkillTest()
    {
        prevAction = nextAction;
        nextAction = 0;
        switch (prevAction)
        {
            case 1:
                yield return EstocHead();
                break;
            case 2:
                yield return EstocFeet();
                break;
            case 3:
                yield return OverShield();
                break;
            case 4:
                yield return EstocArc();
                break;
            case 5:
                yield return PoisonCloud();
                break;
            case 6:
                yield return CutGrenade();
                break;
            case 7:
                yield return Lunge();
                break;
            case 8:
                yield return ShockWave();
                break;
            case 9:
                yield return GroundStomp();
                break;
            case 10:
                yield return DashToHero();
                break;
            case 11:
                yield return DashToHeroDef();
                break;
            case 12:
                yield return BackDash();
                break;
            case 13:
                yield return KillPetSkill();
                break;
            default:
                break;
        }
    }
    int lastHP = 0;
    private void Update() {
        touchWall = false;
        if(col.bounds.center.x < battelArea.bounds.min.x)
        {
            touchWall = true;
            touchRight = false;
            transform.position += new Vector3(battelArea.bounds.min.x - col.bounds.center.x, 0);
        }
        if(col.bounds.center.x > battelArea.bounds.max.x)
        {
            touchWall = true;
            touchRight = true;
            transform.position -= new Vector3(col.bounds.center.x - battelArea.bounds.max.x, 0);
        }
        if(hm.hp < lastHP)
        {
            StartCoroutine(HitFlash());
        }
        lastHP = hm.hp;
    }
    private IEnumerator LifeLoop()
    {
        yield return null;
        animController.PlayLoop("idle");
        while(target == null) yield return null;
        yield return new WaitForSeconds(1);
        while (true)
        {
            yield return null;
            yield return SkillTest();
            yield return AfterSkill();
            if(nextAction == 7 && prevAction == 7)
            {
                nextAction = 10;
            }
            else if(nextAction == prevAction && (nextAction == 10 || nextAction == 11))
            {

            }
            if(nextAction == prevAction && nextAction != 0 && nextAction != 4) continue;
            yield return null;
            if(hm.hp <= 0)
            {
                yield return Die();
                yield break;
            }
            if (!animController.IsPlaying("idle")) animController.PlayLoop("idle");
        }
    }
    private void OnEnable()
    {
        StartCoroutine(LifeLoop());
    }
}
