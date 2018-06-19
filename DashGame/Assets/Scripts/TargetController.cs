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
    public EnemyBehavior ball;
    LevelGenerator LG;
    GameManager game;
    float randomSize;
    Vector3 defaultTargetSize = new Vector3(0.2636f, 0.2636f,1);

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

        public Target(Transform t, Animator anim)
        {
            transform = t;
            animator = anim;
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

        targets = new Target[2];

        for (int i = 0; i < targets.Length; i++)
        {
            GameObject go = Instantiate(TargetPrefab, Vector2.right * -1000, Quaternion.identity);
            go.name = "Target" + i;
            Animator anim = go.GetComponent<Animator>();
            targets[i] = new Target(go.transform, anim);
        }
        targets[0].StopUsing();
        targets[1].Use();
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
        GameManager.MoveToNextLvl += MoveToNextLvl;
    }

    private void OnDisable()
    {
        EnemyBehavior.AbsorbDoneAndRichochet -= AbsorbDoneAndRichochet;
        GameManager.GameStarted -= GameStarted;
        EnemyBehavior.AbsorbDone -= AbsorbDone;
        GameManager.GameOverConfirmed -= GameOverConfirmed;
        LevelGenerator.NextLvlGenerated -= NextLvlGenerated;
        GameManager.MoveToNextLvl -= MoveToNextLvl;
    }

    private void Update()
    {
        targets[0].animator.SetBool("InUse", targets[0].inUse);
        targets[1].animator.SetBool("InUse", targets[1].inUse);
    }

    void MoveToNextLvl()
    {
        for (int i = 0; i < targets.Length; i++)
        {
            if (targets[i].inUse)
            {
                targets[i].StopUsing();
            }
            else
            {
                targets[i].Use();
            }
        }
    }

    void AbsorbDone()
    {
        TargetHit();

        for (int i = 0; i < targets.Length; i++)
        {
            if (targets[i].inUse)
            {
                targets[i].StopUsing();
            }
            else
            {
                targets[i].Use();
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
                targets[i].StopUsing();
            }
            else
            {
                targets[i].Use();
            }
        }
    }

    void NextLvlGenerated()
    {
        for (int i = 0; i < targets.Length; i++)
        {
            if (!targets[i].inUse)
            {
                if (game.GetScore < 3)
                {
                    targets[i].transform.parent = LG.GetNextLvl;
                    targets[i].transform.localScale = defaultTargetSize;
                    targets[i].transform.localPosition = RandomPos();
                }
                
                if (game.GetScore >= 3)
                {
                    targets[i].transform.parent = LG.GetNextLvl;
                    randomSize = Random.Range(.06f, defaultTargetSize.x);
                    Debug.Log(randomSize);
                    targets[i].transform.localScale = new Vector3(randomSize, randomSize, 1);
                    targets[i].transform.localPosition = RandomPos();
                }
                
            }
        }
    }

    void GameOverConfirmed()
    {
        targets[0].transform.position = Vector2.right * -1000;
        targets[1].transform.position = Vector2.right * -1000;
    }

    void GameStarted()
    {
        targets[0].StopUsing();
        targets[1].Use();
        targets[1].transform.parent = LG.GetCurrentLvl;
        targets[1].transform.localPosition = RandomPos();
        targets[1].animator.SetTrigger("GameStarted");
    }

    public Vector2 RandomPos()
    {
        return new Vector2(Random.Range(spawnAreaCorners[0].x + (collider.radius * TargetPrefab.transform.localScale.x), spawnAreaCorners[3].x - (collider.radius * TargetPrefab.transform.localScale.x)), Random.Range(spawnAreaCorners[0].y + (collider.radius * TargetPrefab.transform.localScale.x), spawnAreaCorners[2].y - (collider.radius * TargetPrefab.transform.localScale.x)));
    }

    public int RandomSpawnAreaXRange
    {
        get
        {
            return (int) Random.Range(spawnAreaCorners[0].x + (collider.radius * TargetPrefab.transform.localScale.x), spawnAreaCorners[3].x - (collider.radius * TargetPrefab.transform.localScale.x));
        }
    }
}
