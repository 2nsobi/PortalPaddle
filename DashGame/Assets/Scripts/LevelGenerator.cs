using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour {

    #region Singleton
    public static LevelGenerator Instance;

    private void Awake()
    {
        Instance = this;
    }
    #endregion

    [System.Serializable]
    public class Pool
    {
        public string tag;
        public GameObject prefab1;
        public GameObject prefab2;
        public GameObject prefab3;
        public int size;
    }

    public Dictionary<string, Queue<GameObject>> LvlComponentDict;
    public List<Pool> pools;
    public GameObject StartLvl;
    GameObject StartLevel; //for code use
    GameManager game;
    public float transitionSpeed;
    Vector3 levelOffset = new Vector3(0, 10.8f,0); // used to offset a level when it is spawned so that is spawns above the active level
    GameObject NextLvl;
    GameObject CurrentLvl;
    GameObject TempLvl;
    bool currentlyTransitioning;
    bool playedOnce; // ateast one game session has been started since opening app, used to get rid of NullReferenceException when GameOverConfirmed() is called after app is opened

    public delegate void LevelDelegate();
    public static event LevelDelegate TransitionDone;
    public static event LevelDelegate NextLvlGenerated;

    private void OnEnable()
    {
        GameManager.GameStarted += GameStarted;
        GameManager.GameOverConfirmed += GameOverConfirmed;
        EnemyBehavior.AbsorbDone += AbsorbDone;
        EnemyBehavior.AbsorbDoneAndRichochet += AbsorbDone;
        GameManager.MoveToNextLvl += MoveToNextLvl;
    }

    private void OnDisable()
    {
        GameManager.GameStarted -= GameStarted;
        GameManager.GameOverConfirmed -= GameOverConfirmed;
        EnemyBehavior.AbsorbDone -= AbsorbDone;
        EnemyBehavior.AbsorbDoneAndRichochet -= AbsorbDone;
        GameManager.MoveToNextLvl -= MoveToNextLvl;
    }

    private void Start()
    {
        game = GameManager.Instance;

        currentlyTransitioning = false;

        StartLevel = Instantiate(StartLvl, transform);
        CurrentLvl = StartLevel;

        LvlComponentDict = new Dictionary<string, Queue<GameObject>>();

        foreach (Pool pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();
            
            for(int i = 0; i<pool.size; i++)
            {
                GameObject obj1 = Instantiate(pool.prefab1);
                obj1.SetActive(false);
                objectPool.Enqueue(obj1);

                GameObject obj2 = Instantiate(pool.prefab2);
                obj2.SetActive(false);
                objectPool.Enqueue(obj2);

                GameObject obj3 = Instantiate(pool.prefab3);
                obj3.SetActive(false);
                objectPool.Enqueue(obj3);
            }

            LvlComponentDict.Add(pool.tag,objectPool);
        }
    }

    void GameOverConfirmed()
    {
        if (playedOnce)
        {
            foreach (GameObject obj in LvlComponentDict["Level2"])
            {
                for (int i = 0; i < LvlComponentDict["Level2"].Count; i++)
                {
                    obj.SetActive(false);
                }
            }
            foreach (GameObject obj in LvlComponentDict["Level3"])
            {
                for (int i = 0; i < LvlComponentDict["Level3"].Count; i++)
                {
                    obj.SetActive(false);
                }
            }
            foreach (GameObject obj in LvlComponentDict["Level4"])
            {
                for (int i = 0; i < LvlComponentDict["Level4"].Count; i++)
                {
                    obj.SetActive(false);
                }
            }
            CurrentLvl = StartLevel;
            CurrentLvl.transform.position = Vector3.zero;
        }
    }

    public GameObject SpawnFromPool(string tag, Vector2 position, Quaternion rotation)
    {
        GameObject objectToSpawn = LvlComponentDict[tag].Dequeue();

        objectToSpawn.SetActive(true);
        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;

        LvlComponentDict[tag].Enqueue(objectToSpawn);

        return objectToSpawn;
    }

    void AbsorbDone()
    {
        currentlyTransitioning = true;
    }

    void MoveToNextLvl()
    {
        currentlyTransitioning = true;
    }

    private void FixedUpdate()
    {
        if (currentlyTransitioning)
        {
            CurrentLvl.transform.position = Vector2.MoveTowards(CurrentLvl.transform.position, this.transform.position - levelOffset, Time.deltaTime * transitionSpeed);
            NextLvl.transform.position = Vector2.MoveTowards(NextLvl.transform.position, Vector3.zero, Time.deltaTime * transitionSpeed);

            if (NextLvl.transform.position == Vector3.zero)
            {
                CurrentLvl = NextLvl;
                TransitionDone();
                GenerateNextLvl();
                currentlyTransitioning = false;
            }
        }
    }

    void GameStarted()
    {
        NextLvl = SpawnFromPool("Level2", transform.position + levelOffset, transform.rotation);
        NextLvlGenerated();
        playedOnce = true;
    }

    void GenerateNextLvl() //note that 1 score point is gained on teh starting level "level1"
    {
        if (game.GetScore < 3)
        {
            NextLvl = SpawnFromPool("Level2", transform.position + levelOffset, transform.rotation);
            NextLvlGenerated();
        }
        if (game.GetScore >= 3 && game.GetScore <6)
        {
            NextLvl = SpawnFromPool("Level3", transform.position + levelOffset, transform.rotation);
            NextLvlGenerated();
        }
        if(game.GetScore >= 6)
        {
            NextLvl = SpawnFromPool("Level4", transform.position + levelOffset, transform.rotation);
            NextLvlGenerated();
        }
    }

    public Transform GetNextLvl
    {
        get
        {
            return NextLvl.transform;
        }
    }

    public Transform GetCurrentLvl
    {
        get
        {
            return CurrentLvl.transform;
        }
    }
}
