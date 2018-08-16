using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetController : MonoBehaviour
{
    public GameObject spawnArea;
    RectTransform spawnAreaRect;
    Vector3[] spawnAreaCorners = new Vector3[4];
    CircleCollider2D collider; //used to account for the offset needed for the radius of the target
    public GameObject TargetPrefab;
    EnemyBehavior ball;
    LevelGenerator LG;
    GameManager game;
    float randomSize;
    Vector3 defaultTargetSize = new Vector3(0.23f, 0.23f, 1);
    Vector3 troubleshootingSize = new Vector3(0.7f, 0.7f, 1);
    bool target1Travel;
    bool target2Travel;
    bool target1Hit, target2Hit;
    public float travelSpeed;
    int pointCounter1, pointCounter2;
    Vector3[] travelPath1;
    Vector3[] travelPath2;
    Vector3 currentPoint1, currentPoint2;
    Vector3 nextPoint1, nextPoint2;
    Target currentTargetInUse;
    Vector3[] tempPath1,tempPath2;
    bool gameRunning;
    bool target1Moving, target2Moving;//are targets moving?
    Transform nextLvl;
    bool growShrink1,growShrink2;
    public float growShrinkSpeed;
    float smallestTargestSize = 0.03f; //smallest a target will get when it grows and shrinks
    Vector3[] nextObstaclePath;
    float targetRadius = 2.53f; //based on 2d circle collider radius
    float targetSpawnOffset;

    public static TargetController Instance;

    public delegate void TargetDelegate();
    public static event TargetDelegate TargetHit;
    public static event TargetDelegate TargetHitAndRichochet;

    float size; //size of the target

    class Target
    {
        public bool inUse;
        public Transform transform;
        public Animator animator;

        public Target(Transform t, Animator anim, Color col)
        {
            transform = t;
            animator = anim;
            this.inUse = true;
            animator.SetBool("InUse",this.inUse);

            Transform portal = transform.Find("PortalSprite");
            for (int i = 0; i< portal.childCount; i++)
            {
                if (portal.GetChild(i).GetComponent<SpriteRenderer>() != null)
                {
                    portal.GetChild(i).GetComponent<SpriteRenderer>().color = col;
                }
            }
        }

        public void StopUsing()
        {
            inUse = false;
        }

        public void Use()
        {
            inUse = true;
        }
    }

    Target[] targets;

    private void Awake()
    {
        Instance = this;
        ball = EnemyBehavior.Instance;
        collider = TargetPrefab.GetComponent<CircleCollider2D>();
        spawnAreaRect = spawnArea.transform as RectTransform;
        spawnAreaRect.GetWorldCorners(spawnAreaCorners);
        target1Travel = false;
        target2Travel = false;
        target1Hit = false;
        target2Hit = false;
        growShrink1 = false;
        growShrink2 = false;
        pointCounter1 = 0;
        pointCounter2 = 0;

        targets = new Target[2];

        for (int i = 0; i < targets.Length; i++)
        {
            GameObject go = Instantiate(TargetPrefab, Vector2.right * -1000, Quaternion.identity);
            go.name = "Target" + i;
            Animator anim = go.GetComponent<Animator>();
            targets[i] = new Target(go.transform, anim, new Color32(255, 248, 57,255)); //lighter yellow color
        }
        targets[0].StopUsing();
        targets[1].Use();
        currentTargetInUse = null;
        gameRunning = false;
        targetSpawnOffset = targetRadius * defaultTargetSize.x;
    }

    private void Start()
    {
        LG = LevelGenerator.Instance;
        game = GameManager.Instance;
    }

    private void OnEnable()
    {
        EnemyBehavior.AbsorbDoneAndRichochet += AbsorbDoneAndRichochet;
        GameManager.GameStarted += GameStarted;
        EnemyBehavior.AbsorbDone += AbsorbDone;
        GameManager.GameOverConfirmed += GameOverConfirmed;
        LevelGenerator.NextLvlGenerated += NextLvlGenerated;
        LevelGenerator.TransitionDone += TransitionDone;
    }

    private void OnDisable()
    {
        EnemyBehavior.AbsorbDoneAndRichochet -= AbsorbDoneAndRichochet;
        GameManager.GameStarted -= GameStarted;
        EnemyBehavior.AbsorbDone -= AbsorbDone;
        GameManager.GameOverConfirmed -= GameOverConfirmed;
        LevelGenerator.NextLvlGenerated -= NextLvlGenerated;
        LevelGenerator.TransitionDone -= TransitionDone;
    }

    private void Update()
    {
        if (gameRunning)
        {
            targets[0].animator.SetBool("InUse", targets[0].inUse);
            targets[1].animator.SetBool("InUse", targets[1].inUse);
        }

        if (growShrink1)
        {
            if (gameRunning)
            {
                targets[0].transform.localScale = new Vector3(((defaultTargetSize.x-smallestTargestSize)/2 + smallestTargestSize) + (Mathf.Sin(Time.time * growShrinkSpeed) 
                    * ((defaultTargetSize.x-smallestTargestSize) / 2)), ((defaultTargetSize.x - smallestTargestSize) / 2 + smallestTargestSize) 
                    + (Mathf.Sin(Time.time * growShrinkSpeed) * ((defaultTargetSize.x - smallestTargestSize) / 2)), 
                    0.575f + Mathf.Sin(Time.time * growShrinkSpeed) * ((1 - 0.15f)/2)); //float values in z scale make particle system particle look normal when they shrink
            }
        }

        if (growShrink2)
        {
            if (gameRunning)
            {
                targets[1].transform.localScale = new Vector3(((defaultTargetSize.x - smallestTargestSize) / 2 + smallestTargestSize) + (Mathf.Sin(Time.time * growShrinkSpeed) 
                    * ((defaultTargetSize.x - smallestTargestSize) / 2)), ((defaultTargetSize.x - smallestTargestSize) / 2 + smallestTargestSize) 
                    + (Mathf.Sin(Time.time * growShrinkSpeed) * ((defaultTargetSize.x - smallestTargestSize) / 2)), 
                    0.575f + Mathf.Sin(Time.time * growShrinkSpeed) * ((1 - 0.15f) / 2)); //float values in z scale make particle system particle look normal when they shrink
            }
        }

        if (target1Hit)
        {
            growShrink1 = false;
            target1Travel = false;
            if (gameRunning)
            {
                if (targets[0].transform.position.y < nextLvl.position.y - 5) //5.2 is about half the height of a lvl
                {
                    targets[0].StopUsing();
                }
            }
        }
        if (target2Hit)
        {
            growShrink2 = false;
            target2Travel = false;
            if (gameRunning)
            {
                if (targets[1].transform.position.y < nextLvl.position.y - 5) //5.2 is about half the height of a lvl
                {
                    targets[1].StopUsing();
                }
            }
        }

        if (target1Travel)
        {
            nextPoint1 = PointOnPath(travelPath1, pointCounter1);
            targets[0].transform.localPosition = Vector2.MoveTowards(targets[0].transform.localPosition, nextPoint1, Time.deltaTime * travelSpeed);
            if (targets[0].transform.localPosition == nextPoint1)
            {
                pointCounter1 += 1;
                if (pointCounter1 > travelPath1.Length - 1)
                {
                    pointCounter1 = 0;
                }
            }
        }

        if (target2Travel)
        {
            nextPoint2 = PointOnPath(travelPath2, pointCounter2);
            targets[1].transform.localPosition = Vector2.MoveTowards(targets[1].transform.localPosition, nextPoint2, Time.deltaTime * travelSpeed);
            if (targets[1].transform.localPosition == nextPoint2)
            {
                pointCounter2 += 1;
                if (pointCounter2 > travelPath2.Length - 1)
                {
                    pointCounter2 = 0;
                }
            }
        }

    }

    void SelectTargetToTravel(Target target)
    {
        int aRandomNum = Random.Range(0, 10); //if greater than 4 will travel in normal, order if smaller than 5 will travel in reverse order

        if (target == targets[0])
        {
            target1Hit = false;
            target1Travel = true;

            if (aRandomNum > 4)
            {
                tempPath1 = nextObstaclePath;
                System.Array.Reverse(tempPath1);
                travelPath1 = tempPath1;
            }
            else
            {
                travelPath1 = nextObstaclePath;
            }

            int randPos = Random.Range(0, travelPath1.Length - 1);
            pointCounter1 = randPos + 1;
            targets[0].transform.localPosition = travelPath1[randPos];
        }

        if (target == targets[1])
        {
            target2Hit = false;
            target2Travel = true;

            if (aRandomNum > 4)
            {
                tempPath2 = nextObstaclePath;
                System.Array.Reverse(tempPath2);
                travelPath2 = tempPath2;
            }
            else
            {
                travelPath2 = nextObstaclePath;
            }
            
            int randPos = Random.Range(0, travelPath2.Length - 1);
            pointCounter2 = randPos + 1;
            targets[1].transform.localPosition = travelPath2[randPos];
        }
    }

    Vector3 PointOnPath(Vector3[] path, int iterator)
    { 
        return path[iterator];
    }

    void AbsorbDone()
    {
        TargetHit();
        for (int i = 0; i < targets.Length; i++)
        {
            if (targets[i].inUse)
            {
                if (ball.GetTargetHit == "Target0")
                {
                    target1Hit = true;
                }
                if (ball.GetTargetHit == "Target1")
                {
                    target2Hit = true;
                }
            }
        }

    }

    void AbsorbDoneAndRichochet()
    {
        TargetHitAndRichochet();

        for (int i = 0; i < targets.Length; i++)
        {
            if (targets[i].inUse)
            {
                if (ball.GetTargetHit == "Target0")
                {
                    target1Hit = true;
                }
                if (ball.GetTargetHit == "Target1")
                {
                    target2Hit = true;
                }
            }
        }

    }

    void TransitionDone()
    {
        for (int i = 0; i < targets.Length; i++)
        {
            if (targets[i].inUse)
            {
                currentTargetInUse = targets[i];

                if(targets[i] == targets[0] && target1Travel)
                {
                    target1Moving = true;
                }
                if (targets[i] == targets[1] && target2Travel)
                {
                    target2Moving = true;
                }
            }

            if (!targets[0].inUse)
            {
                target1Moving = false;
            }

            if (!targets[1].inUse)
            {
                target2Moving = false;
            }
        }

        target1Hit = false;
        target2Hit = false;
    }

    void SelectTargetToGrowShrink(Target t)
    {
        if(t == targets[0])
        {
            growShrink1 = true;
        }
        if (t == targets[1])
        {
            growShrink2 = true;
        }
    }

    void NextLvlGenerated()
    {
        nextLvl = LG.GetNextLvl;
        nextObstaclePath = LG.GetNextObstaclePath;

        for (int i = 0; i < targets.Length; i++)
        {
            if (!targets[i].inUse)
            {
                targets[i].Use();
                if (LG.GetNextLvlNumber == 1)
                {
                    targets[i].transform.parent = nextLvl;
                    targets[i].transform.localScale = troubleshootingSize;
                    targets[i].transform.localPosition = RandomPos();
                }
                
                if (LG.GetNextLvlNumber == 2)
                {
                    targets[i].transform.parent = nextLvl;
                    //SelectTargetToGrowShrink(targets[i]);
                    targets[i].transform.localScale = troubleshootingSize;
                    targets[i].transform.localPosition = RandomPos();
                }

                if(LG.GetNextLvlNumber == 3)
                {
                    targets[i].transform.parent = nextLvl;
                    targets[i].transform.localScale = troubleshootingSize;
                    targets[i].transform.localPosition = nextObstaclePath[Random.Range(0, nextObstaclePath.Length)];
                    SelectTargetToTravel(targets[i]);
                }
                
                if (LG.GetNextLvlNumber >= 4)
                {
                    targets[i].transform.parent = nextLvl;
                    //SelectTargetToGrowShrink(targets[i]);
                    targets[i].transform.localScale = troubleshootingSize;
                    targets[i].transform.localPosition = nextObstaclePath[Random.Range(0, nextObstaclePath.Length)];
                    SelectTargetToTravel(targets[i]);
                }
            }
        }
    }

    void GameOverConfirmed()
    {
        targets[0].transform.parent = null;
        targets[1].transform.parent = null;
    }

    public void ResetTargets()
    {

        targets[0].transform.position = Vector2.right * -1000;
        targets[1].transform.position = Vector2.right * -1000;

        target1Travel = false;
        target2Travel = false;
        gameRunning = false;
        target1Hit = false;
        target2Hit = false;
        growShrink1 = false;
        growShrink2 = false;

        targets[1].animator.ResetTrigger("GameStarted");
    }

    void GameStarted()
    {
        targets[0].StopUsing();
        targets[1].Use();
        targets[1].transform.parent = LG.GetCurrentLvl;
        targets[1].transform.localPosition = RandomPos();
        targets[1].animator.SetTrigger("GameStarted");
        currentTargetInUse = targets[1];

        targets[0].transform.localScale = defaultTargetSize;
        targets[1].transform.localScale = defaultTargetSize;
        gameRunning = true;
    }

    public Vector2 RandomPos()
    {
        return new Vector2(Random.Range(spawnAreaCorners[0].x + (targetSpawnOffset), spawnAreaCorners[3].x - (targetSpawnOffset)), Random.Range(spawnAreaCorners[0].y + (targetSpawnOffset), spawnAreaCorners[2].y - (targetSpawnOffset)));
    }

    public int RandomSpawnAreaXRange
    {
        get
        {
            return (int)Random.Range(spawnAreaCorners[0].x + (0.66f), spawnAreaCorners[3].x - (0.66f)); //float comes from measuring radius of ballspawner
        }
    }

    public Vector3 GetCurrentTargetPos
    {
        get
        {
            return currentTargetInUse.transform.position;
        }
    }

    public float getTravelSpeed
    {
        get
        {
            return travelSpeed;
        }
    }

    public bool IsMoving()
    {
        if(target1Moving || target2Moving){
            return true;
        }
        return false;
    }
}
