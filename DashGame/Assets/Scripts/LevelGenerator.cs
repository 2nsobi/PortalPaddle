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
        public List<GameObject> prefabs;
        public int sizePerPrefab;
        public GameObject filter; //artwork filter for level
    }

    [System.Serializable]
    public class SinglePool
    {
        public GameObject prefab;
        public int size;
    }

    public class Obstacle
    {
        public GameObject gameObject;
        public Transform transform;
        public Vector3[] path;
        public string obstacleTexture;
        SpriteRenderer[] obstacleSprites;

        public Obstacle(GameObject go, string obstacleTexture)
        {
            this.transform = go.transform;
            this.gameObject = go;
            this.obstacleTexture = obstacleTexture;
            Transform t = go.transform.Find("TargetTravelPath");
            this.path = new Vector3[t.childCount];
            for (int i = 0; i < t.childCount; i++)
            {
                this.path[i] = t.GetChild(i).localPosition;
            }

            obstacleSprites = go.GetComponentsInChildren<SpriteRenderer>();
            foreach(SpriteRenderer sr in obstacleSprites)
            {
                for (int i = 0; i < obstacletextures.Count; i++)
                {
                    if (sr.name == obstacletextures[i].shapes[i].tag && obstacleTexture == obstacletextures[i].tag)
                    {
                        sr.sprite = obstacletextures[i].shapes[i].shape;
                    }
                }
            }
        }

        public void SetObstacleTextures(string obstacleTexture)
        {
            foreach (SpriteRenderer sr in obstacleSprites)
            {
                for (int i = 0; i < obstacletextures.Count; i++)
                {
                    if (sr.name == obstacletextures[i].shapes[i].tag && obstacleTexture == obstacletextures[i].tag)
                    {
                        sr.sprite = obstacletextures[i].shapes[i].shape;
                    }
                }
            }
        }
    }

    [System.Serializable]
    public class ObstacleTexture
    {
        [System.Serializable]
        public class Shape
        {
            public string tag;
            public Sprite shape;
        }
        public string tag;
        public Shape[] shapes;
    }

    public Vector2 TargetAspectRatio;
    public float transitionSpeed;
    Dictionary<string, List<Queue<GameObject>>> LvlComponentDict; //for background use mainly
    Dictionary<string, Queue<Obstacle>> ObstacleDict; //for obstacle use mainly
    public List<ObstacleTexture> obstacleTextures;
    public static List<ObstacleTexture> obstacletextures;
    public List<MultiPool> levels;
    public List<SinglePool> obstacles;
    public GameObject StartLvl;
    GameObject StartLevel; //for code use
    GameManager game;
    Vector3 levelOffset = new Vector3(0, 10.8f, 0); // used to offset a level when it is spawned so that is spawns above the active level
    GameObject NextLvl;
    GameObject CurrentLvl;
    GameObject PreviousLvl;
    Obstacle NextObstacle;
    Obstacle CurrentObstacle;
    Obstacle PreviousObstacle;
    bool currentlyTransitioning;
    bool playedOnce; // ateast one game session has been started since opening app, used to get rid of NullReferenceException when GameOverConfirmed() is called after app is opened
    Vector3[] travelPath1;
    bool obstacleDespawned;
    int obstacleSpawnCounter;
    int levelSpawnCounter;
    float targetAspectRatio;
    PaddleController paddle;
    bool switchColors;
    Material material;
    static System.Random rng = new System.Random();
    Queue<GameObject> tempQ;

    List<Obstacle> AllObstacles;

    public delegate void LevelDelegate();
    public static event LevelDelegate TransitionDone;
    public static event LevelDelegate NextLvlGenerated;

    private void OnEnable()
    {
        GameManager.GameStarted += GameStarted;
        GameManager.GameOverConfirmed += GameOverConfirmed;
        EnemyBehavior.AbsorbDone += AbsorbDone;
        EnemyBehavior.AbsorbDoneAndRichochet += AbsorbDone;
        //GameManager.Revive += MoveToNextLvl;
        GameManager.GoToSettingsPage += GoToSettingsPage;
        GameManager.ComeBackFromSettingsPage += ComeBackFromSettingsPage;
    }

    private void OnDisable()
    {
        GameManager.GameStarted -= GameStarted;
        GameManager.GameOverConfirmed -= GameOverConfirmed;
        EnemyBehavior.AbsorbDone -= AbsorbDone;
        EnemyBehavior.AbsorbDoneAndRichochet -= AbsorbDone;
        //GameManager.Revive -= MoveToNextLvl;
        GameManager.GoToSettingsPage -= GoToSettingsPage;
        GameManager.ComeBackFromSettingsPage -= ComeBackFromSettingsPage;
    }

    private void Start()
    {
        obstacletextures = obstacleTextures;

        switchColors = true;
        targetAspectRatio = TargetAspectRatio.x / TargetAspectRatio.y;

        levelSpawnCounter = 0;
        obstacleSpawnCounter = 0;
        obstacleDespawned = false;

        game = GameManager.Instance;
        paddle = PaddleController.Instance;

        currentlyTransitioning = false;

        StartLevel = Instantiate(StartLvl, transform);
        material = StartLevel.GetComponentInChildren<Renderer>().sharedMaterial;
        Transform wallW = StartLevel.transform.GetChild(0);
        Transform wallE = StartLevel.transform.GetChild(1);
        wallW.localPosition = new Vector3(wallW.localPosition.x + paddle.GetDistanceDifferenceForWalls(), wallW.localPosition.y, 0);
        wallE.localPosition = new Vector3(wallE.localPosition.x - paddle.GetDistanceDifferenceForWalls(), wallE.localPosition.y, 0);
        CurrentLvl = StartLevel;

        LvlComponentDict = new Dictionary<string, List<Queue<GameObject>>>();
        ObstacleDict = new Dictionary<string, Queue<Obstacle>>();

        foreach (MultiPool level in levels)
        {
            List<Queue<GameObject>> lvlPrefabs = new List<Queue<GameObject>>();
            foreach (GameObject prefab in level.prefabs)
            {
                Queue<GameObject> objectPool = new Queue<GameObject>(); //objectPool is a queue of duplications of the same prefab

                for (int i = 0; i < level.sizePerPrefab; i++)
                {
                    GameObject obj1 = Instantiate(prefab);
                    Transform wallW0 = obj1.transform.GetChild(0);
                    Transform wallE0 = obj1.transform.GetChild(1);
                    wallW0.localPosition = new Vector3(wallW0.localPosition.x + paddle.GetDistanceDifferenceForWalls(), wallW0.localPosition.y, 0);
                    wallE0.localPosition = new Vector3(wallE0.localPosition.x - paddle.GetDistanceDifferenceForWalls(), wallE0.localPosition.y, 0);
                    obj1.SetActive(false);
                    objectPool.Enqueue(obj1);
                }
                lvlPrefabs.Add(objectPool);
            }
            LvlComponentDict.Add(level.tag, lvlPrefabs);
        }

        AllObstacles = new List<Obstacle>();

        foreach (SinglePool pool in obstacles)
        {
            Queue<Obstacle> obstaclePool = new Queue<Obstacle>();

            for (int i = 0; i < pool.size; i++)
            {
                Obstacle obstacle = new Obstacle(Instantiate(pool.prefab));

                obstacle.gameObject.SetActive(false);
                obstaclePool.Enqueue(obstacle);

                AllObstacles.Add(obstacle);
            }

            ObstacleDict.Add(pool.prefab.name, obstaclePool);
        }

    }

    public void ShufflePrefabsInLevels()
    {
        foreach (List<Queue<GameObject>> level in LvlComponentDict.Values)
        {
            int n = level.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                Queue<GameObject> tempQ = level[k];
                level[k] = level[n];
                level[n] = tempQ;
            }
        }
    }

    void GameOverConfirmed()
    {
        obstacleSpawnCounter = 0;
        levelSpawnCounter = 0;
        obstacleDespawned = false;

        if (playedOnce)
        {
            #region Disactivates All Level Prefabs
            foreach (List<Queue<GameObject>> level in LvlComponentDict.Values)
            {
                foreach(Queue<GameObject> prefabs in level)
                {
                    foreach(GameObject obj in prefabs)
                    {
                        obj.SetActive(false);
                    }
                }
            }
            //foreach (GameObject obj in LvlComponentDict["Level2"])
            //{
            //    obj.SetActive(false);
            //}
            //foreach (GameObject obj in LvlComponentDict["Level3"])
            //{
            //    obj.SetActive(false);
            //}
            //foreach (GameObject obj in LvlComponentDict["Level4"])
            //{
            //    obj.SetActive(false);
            //}
            //foreach (GameObject obj in LvlComponentDict["Level5"])
            //{
            //    obj.SetActive(false);
            //}
            #endregion

            #region Disactivates All Obstacle Prefabs
            foreach (Obstacle ob in ObstacleDict["Obstacle1_Lvl2"])
            {
                ob.transform.parent = null;
                ob.gameObject.SetActive(false);
            }
            foreach (Obstacle ob in ObstacleDict["Obstacle2_Lvl2"])
            {
                ob.transform.parent = null;
                ob.gameObject.SetActive(false);
            }
            foreach (Obstacle ob in ObstacleDict["Obstacle3_Lvl2"])
            {
                ob.transform.parent = null;
                ob.gameObject.SetActive(false);
            }
            foreach (Obstacle ob in ObstacleDict["Obstacle4_Lvl2"])
            {
                ob.transform.parent = null;
                ob.gameObject.SetActive(false);
            }
            foreach (Obstacle ob in ObstacleDict["Obstacle5_Lvl2"])
            {
                ob.transform.parent = null;
                ob.gameObject.SetActive(false);
            }
            foreach (Obstacle ob in ObstacleDict["Obstacle6_Lvl2"])
            {
                ob.transform.parent = null;
                ob.gameObject.SetActive(false);
            }
            foreach (Obstacle ob in ObstacleDict["Obstacle7_Lvl2"])
            {
                ob.transform.parent = null;
                ob.gameObject.SetActive(false);
            }
            foreach (Obstacle ob in ObstacleDict["Obstacle8_Lvl2"])
            {
                ob.transform.parent = null;
                ob.gameObject.SetActive(false);
            }
            foreach (Obstacle ob in ObstacleDict["Obstacle9_Lvl2"])
            {
                ob.transform.parent = null;
                ob.gameObject.SetActive(false);
            }
            #endregion

            NextObstacle = null;
            PreviousObstacle = null;

            CurrentLvl = StartLevel;
            CurrentLvl.transform.position = Vector3.zero;
        }
    }

    public GameObject SpawnFromPool(string tag, Vector2 position, Quaternion rotation)
    {
        levelSpawnCounter++;

        GameObject objectToSpawn = LvlComponentDict[tag][0].Dequeue();

        objectToSpawn.SetActive(true);
        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;

        LvlComponentDict[tag][0].Enqueue(objectToSpawn);

        tempQ = LvlComponentDict[tag][0];
        LvlComponentDict[tag][0] = LvlComponentDict[tag][LvlComponentDict[tag].Count - 1];
        LvlComponentDict[tag][LvlComponentDict[tag].Count - 1] = tempQ;

        return objectToSpawn;
    }

    public Obstacle SpawnFromObstacles(string tag, Vector2 position, Quaternion rotation)
    {
        obstacleSpawnCounter++;

        Obstacle obstacleToSpawn = ObstacleDict[tag].Dequeue();

        obstacleToSpawn.gameObject.SetActive(true);
        obstacleToSpawn.gameObject.transform.position = position;
        obstacleToSpawn.gameObject.transform.rotation = rotation;

        ObstacleDict[tag].Enqueue(obstacleToSpawn);

        return obstacleToSpawn;
    }

    void AbsorbDone()
    {
        currentlyTransitioning = true;
    }

    //void MoveToNextLvl()
    //{
    //    currentlyTransitioning = true;
    //}

    public void InvertColors()
    {
        switchColors = !switchColors;
        if (!switchColors)
        {
            material.SetFloat("_InvertColors", 1);
        }
        else
        {
            material.SetFloat("_InvertColors", 0);
        }
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

                if (obstacleSpawnCounter == 1)
                {
                    CurrentObstacle = NextObstacle;
                }

                if (obstacleSpawnCounter == 3)
                {
                    obstacleDespawned = true;
                }

                if (obstacleDespawned)
                {
                    PreviousObstacle.transform.parent = null;
                    PreviousObstacle.gameObject.SetActive(false);
                }
            }
        }

        if (game.IsGameRunning)
        {
            if (CurrentObstacle != null)
            {
                if (CurrentObstacle.transform.position.y < -5.2)
                {
                    PreviousObstacle = CurrentObstacle;
                    CurrentObstacle = NextObstacle;
                }
            }
        }

    }

    void GameStarted()
    {
        ShufflePrefabsInLevels();
        GenerateNextLvl();
        playedOnce = true;
    }

    void GenerateNextLvl()
    {
        if (game.GetScore >= 0 && game.GetScore < 1)
        {
            NextLvl = SpawnFromPool("Level2", transform.position + levelOffset, transform.rotation);
        }
        /*
        if (game.GetScore >= 2 && game.GetScore < 4)
        {
            NextLvl = SpawnFromPool("Level3", transform.position + levelOffset, transform.rotation);
        }
        if (game.GetScore >= 4 && game.GetScore < 6)
        {
            NextLvl = SpawnFromPool("Level4", transform.position + levelOffset, transform.rotation);
            NextObstacle = SpawnFromObstacles("Obstacle" + Random.Range(1, 1) + "_Lvl4", transform.position + levelOffset, transform.rotation);
            NextObstacle.go.transform.parent = NextLvl.transform;
        }
        */
        if (game.GetScore >= 1)
        {
            NextLvl = SpawnFromPool("Level2", transform.position + levelOffset, transform.rotation);
            NextObstacle = SpawnFromObstacles("Obstacle" + Random.Range(1, 10) + "_Lvl2", transform.position + levelOffset, transform.rotation);
            NextObstacle.gameObject.transform.parent = NextLvl.transform;
        }
        NextLvlGenerated();
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

    public void GoToSettingsPage()
    {

    }

    public void ComeBackFromSettingsPage()
    {

    }
}
