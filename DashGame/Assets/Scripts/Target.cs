using UnityEngine;

public class Target : MonoBehaviour
{
    Animator animator;
    Transform portal;
    Vector3 defaultTargetSize = new Vector3(0.23f, 0.23f, 1);
    Vector3 currentPoint;
    Vector3 nextPoint;
    Vector3[] travelPath, tempPath;

    int aRandomNum;
    float travelSpeed;
    int pointCounter = 0;
    float smallestTargestSize = 0.03f; //smallest a target will get when it grows and shrinks
    float growShrinkSpeed;
    bool hit = false;
    bool inUse = false;
    bool growShrink = false;
    bool travelOnPath = false;
    bool shrink = false;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        portal = transform.Find("PortalSprite");
    }

    public void Shrink()
    {
        travelOnPath = false;
        growShrink = false;
        shrink = true;
    }

    public void Shrink2() //used for other game modes to make transitions less weird
    {
        transform.parent = null;
        travelOnPath = false;
        growShrink = false;
        shrink = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        hit = true;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        hit = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        hit = true;
    }

    public void Spawn(Transform parent, Vector2 pos, Vector3 scale, bool travel, bool growshrink, Vector3[] path = null, float travelspeed = 0, float growshrinkspeed = 0)
    {
        inUse = true;

        transform.localScale = scale;
        transform.localPosition = pos;
        transform.parent = parent;

        if (travel)
        {
            aRandomNum = Random.Range(0, 10); //if greater than 4 will travel in normal, order if smaller than 5 will travel in reverse order

            if (aRandomNum > 4)
            {
                tempPath = path;
                System.Array.Reverse(tempPath);
                travelPath = tempPath;
            }
            else
            {
                travelPath = path;
            }
            travelSpeed = travelspeed;

            int randPos = Random.Range(0, travelPath.Length - 1);
            pointCounter = randPos + 1;
            transform.localPosition = travelPath[randPos];

            travelOnPath = true;
        }

        if (growshrink)
        {
            growShrinkSpeed = growshrinkspeed;
            growShrink = true;
        }
    }

    private void Update()
    {
        animator.SetBool("Shrink", shrink);
    }

    private void FixedUpdate()
    {     
        if (growShrink)
        {
            transform.localScale = new Vector3(((defaultTargetSize.x - smallestTargestSize) / 2 + smallestTargestSize) + (Mathf.Sin(Time.time * growShrinkSpeed)
                    * ((defaultTargetSize.x - smallestTargestSize) / 2)), ((defaultTargetSize.x - smallestTargestSize) / 2 + smallestTargestSize)
                    + (Mathf.Sin(Time.time * growShrinkSpeed) * ((defaultTargetSize.x - smallestTargestSize) / 2)),
                    0.575f + Mathf.Sin(Time.time * growShrinkSpeed) * ((1 - 0.15f) / 2)); //float values in z scale make particle system particle look normal when they shrink
        }

        if (travelOnPath)
        {
            nextPoint = travelPath[pointCounter];
            transform.localPosition = Vector2.MoveTowards(transform.localPosition, nextPoint, Time.deltaTime * travelSpeed);
            if (transform.localPosition == nextPoint)
            {
                pointCounter += 1;
                if (pointCounter > travelPath.Length - 1)
                {
                    pointCounter = 0;
                }
            }
        }
    }

    public void StopUsing()
    {
        growShrink = false;
        travelOnPath = false;
        hit = false;
        inUse = false;
        shrink = false;

        gameObject.SetActive(false);
    }

    public void SetColor(Color col)
    {
        for (int i = 0; i < portal.childCount; i++)
        {
            if (portal.GetChild(i).GetComponent<SpriteRenderer>() != null)
            {
                portal.GetChild(i).GetComponent<SpriteRenderer>().color = col;
            }

            if (portal.GetChild(i).GetComponent<ParticleSystem>() != null)
            {
                ParticleSystem.MainModule mainMod = portal.GetChild(i).GetComponent<ParticleSystem>().main;
                mainMod.startColor = col;
            }
        }
    }

    public bool InUse
    {
        get
        {
            return inUse;
        }
    }

    public bool Moving
    {
        get
        {
            return travelOnPath;
        }
    }

    public float TravelSpeed
    {
        get
        {
            return travelSpeed;
        }
    }
}
