using UnityEngine;

public class Target : MonoBehaviour
{
    Animator animator;
    Transform portal;
    Vector3 defaultTargetSize = new Vector3(0.23f, 0.23f, 1);
    Vector3 currentPoint;
    Vector3 nextPoint;
    Vector3[] travelPath, tempPath;
    AudioSource portalHum; // the idle hum of the portal when it is on screen
    Transform portalSpriteTransform;
    Vector2 vectorNearFloor;

    AudioManager audioManager;

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
    float cameraHeight;
    float idleVolumeMax;
    float maxDistanceFromFloor; // the farthest possible distance a transform can be from the floor
    float currentIdleVolume;
    float currentMaxIdleVolume;
    float spawnTime = 1; //time it take for the targetspawn anim to finish
    float t = 0;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        portal = transform.Find("PortalSprite");
        portalHum = GetComponent<AudioSource>();
        idleVolumeMax = portalHum.volume;
        portalSpriteTransform = transform.GetChild(0);
    }

    private void Start()
    {
        audioManager = AudioManager.Instance;

        cameraHeight = Camera.main.orthographicSize;
        vectorNearFloor = new Vector2(0, 1 - cameraHeight);
        maxDistanceFromFloor = Vector2.Distance(new Vector2(Camera.main.aspect * cameraHeight,cameraHeight), vectorNearFloor) + 0.7f; //added 0.7f so that target can still be heard if a lil of screen
    }

    private void OnEnable()
    {
        TargetController.ChangeSpeedImmediately += ChangeSpeedImmediately;

        portalHum.volume = 0;
    }

    private void OnDisable()
    {
        TargetController.ChangeSpeedImmediately -= ChangeSpeedImmediately;
    }

    public void Shrink()
    {
        travelOnPath = false;
        growShrink = false;
        shrink = true;
        currentIdleVolume = portalHum.volume;
    }

    public void Shrink2() //used for other game modes to make transitions less weird
    {
        transform.parent = null;
        travelOnPath = false;
        growShrink = false;
        shrink = true;
        currentIdleVolume = portalHum.volume;
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

        t = 0;
        currentMaxIdleVolume = idleVolumeMax - idleVolumeMax * Mathf.Clamp((Vector2.Distance(transform.position, vectorNearFloor) / maxDistanceFromFloor), 0, 1);

        portalHum.Play();

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

        if(transform.position.y < -(cameraHeight + 0.3))
        {
            portalHum.volume = 0;
        }
        else if(t <= spawnTime)
        {
            t += Time.deltaTime/spawnTime;
            portalHum.volume = Mathf.Lerp(0, idleVolumeMax - idleVolumeMax * Mathf.Clamp((Vector2.Distance(transform.position, vectorNearFloor) / maxDistanceFromFloor), 0, 1), t);
        }
        else if(!shrink)
        {
            // Eq. for normalizing data between 0 and 1 is: (x-min(x)) / (max(x)-min(x))
            portalHum.volume = idleVolumeMax - idleVolumeMax * Mathf.Clamp((Vector2.Distance(transform.position, vectorNearFloor) / maxDistanceFromFloor),0,1);
        }
        else
        {
            portalHum.volume = Mathf.Clamp(portalSpriteTransform.localScale.x - 0.2f, 0, currentIdleVolume);
            //if(idleSound.volume == 0)
            //{
            //    idleSound.Stop();
            //}
        }
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
        transform.parent = null;

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

    void ChangeSpeedImmediately(float speed)
    {
        travelSpeed = speed;
    }

    public void PlaySpawnSound()
    {
        if (transform.position.y < cameraHeight && transform.position.y > -cameraHeight)
        {
            audioManager.PlayMiscSound("portalSpawn");
        }
    }

    public void PlayShrinkSound()
    {
        if (transform.position.y < cameraHeight && transform.position.y > -cameraHeight)
        {
            audioManager.PlayMiscSound("portalShrink");
        }
    }
}
