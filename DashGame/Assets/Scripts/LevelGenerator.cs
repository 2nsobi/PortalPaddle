using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{

    #region Singleton
    public static LevelGenerator Instance;

    private void Awake()
    {
        Instance = this;
    }
    #endregion

    [System.Serializable]
    public class MultiPool
    {
        public string tag;
        public GameObject prefab1;
        public GameObject prefab2;
        public GameObject prefab3;
        public int size;
    }

    [System.Serializable]
    public class SinglePool
    {
        public string tag;
        public GameObject prefab;
        public int size;
    }

    public Dictionary<string, Queue<GameObject>> LvlComponentDict; //for background use mainly
    Dictionary<string, Queue<Obstacle>> ObstacleList; //for obstacle use mainly
    public List<MultiPool> levels;
    public List<SinglePool> obstacles;
    public GameObject StartLvl;
    GameObject StartLevel; //for code use
    GameManager game;
    public float transitionSpeed;
    Vector3 levelOffset = new Vector3(0, 10.8f, 0); // used to offset a level when it is spawned so that is spawns above the active level
    GameObject NextLvl;
    GameObject CurrentLvl;
    Obstacle NextObstacle;
    Obstacle CurrentObstacle;
    bool currentlyTransitioning;
    bool playedOnce; // ateast one game session has been started since opening app, used to get rid of NullReferenceException when GameOverConfirmed() is called after app is opened
    Vector3[] travelPath1;
    Obstacle[] obstaclesLvl2;
    Obstacle[] obstaclesLvl3;
    Obstacle[] obstaclesLvl4;

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

    public class Obstacle
    {
        public GameObject go;
        public Transform transform;
        public Vector3[] path;

        public Obstacle(GameObject go)
        {
            this.transform = go.transform.Find("TargetTravelPath");
            this.go = go;
            this.path = new Vector3[transform.childCount];
            for (int i = 0; i < transform.childCount; i++)
            {
                this.path[i] = transform.GetChild(i).localPosition;
            }
        }
    }

    private void Start()
    {
        game = GameManager.Instance;

        currentlyTransitioning = false;

        StartLevel = Instantiate(StartLvl, transform);
        CurrentLvl = StartLevel;

        LvlComponentDict = new Dictionary<string, Queue<GameObject>>();
        ObstacleList = new Dictionary<string, Queue<Obstacle>>();

        foreach (MultiPool pool in levels)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < pool.size; i++)
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

            LvlComponentDict.Add(pool.tag, objectPool);
        }

        foreach (SinglePool pool in obstacles)
        {
            Queue<Obstacle> obstaclePool = new Queue<Obstacle>();

            for (int i = 0; i < pool.size; i++)
            {
                Obstacle obstacle = new Obstacle(Instantiate(pool.prefab)); 
                obstacle.go.SetActive(false);
                obstaclePool.Enqueue(obstacle);
            }

            ObstacleList.Add(pool.tag, obstaclePool);
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

    public Obstacle SpawnFromObstacles(string tag, Vector2 position, Quaternion rotation)
    {
        Obstacle obstacleToSpawn = ObstacleList[tag].Dequeue();

        obstacleToSpawn.go.SetActive(true);
        obstacleToSpawn.go.transform.position = position;
        obstacleToSpawn.go.transform.rotation = rotation;

        ObstacleList[tag].Enqueue(obstacleToSpawn);

        return obstacleToSpawn;
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
                CurrentObstacle = NextObstacle;
                TransitionDone();
                GenerateNextLvl();
                currentlyTransitioning = false;
            }
        }
    }

    void GameStarted()
    {
        NextLvl = SpawnFromPool("Level2", transform.position + levelOffset, transform.rotation);
        NextObstacle = SpawnFromObstacles("Obstacle" + Random.Range(1, 1) + "_Lvl2", transform.position + levelOffset, transform.rotation);
        NextObstacle.go.transform.parent = NextLvl.transform;
        NextLvlGenerated();
        playedOnce = true;
    }

    void GenerateNextLvl() //note that 1 score point is gained on teh starting level "level1"
    {
        /*
        if (game.GetScore < -1)
        {
            NextLvl = SpawnFromPool("Level2", transform.position + levelOffset, transform.rotation);
            NextLvlGenerated();
        }
        if (game.GetScore >= -1 && game.GetScore < -1)
        {
            NextLvl = SpawnFromPool("Level3", transform.position + levelOffset, transform.rotation);
            NextLvlGenerated();
        }
        */
        //if (game.GetScore > 0)
        {
            NextLvl = SpawnFromPool("Level2", transform.position + levelOffset, transform.rotation);
            NextObstacle = SpawnFromObstacles("Obstacle" + Random.Range(1, 1) + "_Lvl2", transform.position + levelOffset, transform.rotation);
            NextObstacle.go.transform.parent = NextLvl.transform;
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

    public Vector3[] GetNextObstaclePath
    {
        get
        {
            return NextObstacle.path;
        }
    }

    public Vector3[] GetCurrentObstaclePath
    {
        get
        {
            return CurrentObstacle.path;
        }
    }
}
