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
        public string obstacleTexture;
        public List<GameObject> prefabs;
        public GameObject[] specialLvlPrefabs;
        public int sizePerPrefab;
        public GameObject gradient; //for artwork gradients and tings for lvl
        public GameObject transitionLvl;
    }

    [System.Serializable]
    public class SinglePool
    {
        public GameObject prefab;
        public int size;
        public bool easy; //is the obstacle easy or not
    }

    public class Obstacle
    {
        public GameObject gameObject;
        public Transform transform;
        public Vector3[] path;
        public string obstacleTexture;
        SpriteRenderer[] obstacleSprites;

        public Obstacle(GameObject go)
        {
            this.transform = go.transform;
            this.gameObject = go;
            Transform t = go.transform.Find("TargetTravelPath");
            if (t != null)
            {
                this.path = new Vector3[t.childCount];
                for (int i = 0; i < t.childCount; i++)
                {
                    this.path[i] = t.GetChild(i).localPosition;
                }
            }

            obstacleSprites = go.GetComponentsInChildren<SpriteRenderer>();
        }

        public void SetObstacleTextures(string obstacleTexture)
        {
            this.obstacleTexture = obstacleTexture;
            foreach (SpriteRenderer sr in obstacleSprites)
            {
                for (int i = 0; i < obstacletextures.Count; i++)
                {
                    if (obstacleTexture.Equals(obstacletextures[i].tag))
                    {
                        for (int j = 0; j < obstacletextures[i].shapes.Length; j++)
                        {
                            if (sr.name.Equals(obstacletextures[i].shapes[j].tag))
                            {
                                sr.sprite = obstacletextures[i].shapes[j].shape;
                            }
                        }
                        break;
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
    public float nextLvlThreshold; //number used to determine when the next lvl becomes teh current lvl
    Dictionary<string, List<Queue<GameObject>>> LvlComponentDict; //for background use mainly
    Dictionary<string, Queue<Obstacle>> ObstacleDict; //for obstacle use mainly
    Dictionary<string, Queue<GameObject>> CustomLvlQsDict;
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
    GameObject[] transitionLvls;
    GameObject[] specialLvls;
    GameObject[] lvlGradients;
    GameObject currentlySpawnedGradient;
    GameObject previouslySpawnedGradient;
    int gradientCounter; //how mand lvls have art gradients
    int ezObstacleCount = 0;
    Queue<GameObject> lvlSpawnQ = new Queue<GameObject>();
    Queue<Obstacle> obstacleSpawnQ = new Queue<Obstacle>();
    int specialLvlCount = 0;
    GameObject activeLvl;
    GameObject gradientDespawner;
    Obstacle activeObstacle;
    int currentLvlNumber;

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
        CustomLvlQsDict = new Dictionary<string, Queue<GameObject>>();

        transitionLvls = new GameObject[levels.Count];

        int k = 0;
        gradientCounter = 0;
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

            transitionLvls[k] = Instantiate(level.transitionLvl);
            Transform wallW1 = transitionLvls[k].transform.GetChild(0);
            Transform wallE1 = transitionLvls[k].transform.GetChild(1);
            wallW1.localPosition = new Vector3(wallW1.localPosition.x + paddle.GetDistanceDifferenceForWalls(), wallW1.localPosition.y, 0);
            wallE1.localPosition = new Vector3(wallE1.localPosition.x - paddle.GetDistanceDifferenceForWalls(), wallE1.localPosition.y, 0);
            transitionLvls[k].SetActive(false);

            if(level.specialLvlPrefabs.Length > 0)
            {
                specialLvlCount++;
            }

            if (level.gradient != null)
            {
                gradientCounter++;
            }

            k++;
        }
        k = 0;

        lvlGradients = new GameObject[gradientCounter];
        foreach (MultiPool level in levels)
        {
            if (level.gradient != null)
            {
                lvlGradients[k] = level.gradient;
            }
        }
        k = 0;

        specialLvls = new GameObject[specialLvlCount];
        foreach (MultiPool level in levels)
        {
            if (level.specialLvlPrefabs.Length > 0)
            {
                foreach(GameObject go in level.specialLvlPrefabs)
                {
                    GameObject obj1 = Instantiate(go);
                    Transform wallW0 = obj1.transform.GetChild(0);
                    Transform wallE0 = obj1.transform.GetChild(1);
                    wallW0.localPosition = new Vector3(wallW0.localPosition.x + paddle.GetDistanceDifferenceForWalls(), wallW0.localPosition.y, 0);
                    wallE0.localPosition = new Vector3(wallE0.localPosition.x - paddle.GetDistanceDifferenceForWalls(), wallE0.localPosition.y, 0);
                    obj1.SetActive(false);
                    specialLvls[k] = obj1;
                    k++;
                }
            }
        }
        k = 0;

        foreach (SinglePool obstacleType in obstacles)
        {
            Queue<Obstacle> obstaclePool = new Queue<Obstacle>();
            Queue<Obstacle> easyObstaclePool = new Queue<Obstacle>();

            for (int i = 0; i < obstacleType.size; i++)
            {
                Obstacle ob = new Obstacle(Instantiate(obstacleType.prefab));

                ob.gameObject.SetActive(false);
                obstaclePool.Enqueue(ob);
            }

            if (obstacleType.easy)
            {
                for (int i = 0; i < obstacleType.size; i++)
                {
                    Obstacle ob1 = new Obstacle(Instantiate(obstacleType.prefab));

                    ob1.gameObject.SetActive(false);
                    easyObstaclePool.Enqueue(ob1);
                }
                ezObstacleCount++;
                ObstacleDict.Add("EasyObstacle" + ezObstacleCount, easyObstaclePool);
            }

            ObstacleDict.Add(obstacleType.prefab.name, obstaclePool);
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
                foreach (Queue<GameObject> prefabs in level)
                {
                    foreach (GameObject obj in prefabs)
                    {
                        obj.SetActive(false);
                    }
                }
            }

            foreach (GameObject transitionLvl in transitionLvls)
            {
                transitionLvl.SetActive(false);
            }
            #endregion

            #region Disactivates All Obstacle Prefabs
            foreach (Queue<Obstacle> q in ObstacleDict.Values)
            {
                foreach (Obstacle ob in q)
                {
                    ob.transform.parent = null;
                    ob.gameObject.SetActive(false);
                }
            }
            #endregion

            NextObstacle = null;
            PreviousObstacle = null;

            CurrentLvl = StartLevel;
            CurrentLvl.transform.position = Vector3.zero;
        }
    }

    public GameObject SpawnFromPool(string tag)
    {
        levelSpawnCounter++;

        GameObject objectToSpawn = LvlComponentDict[tag][0].Dequeue();

        LvlComponentDict[tag][0].Enqueue(objectToSpawn);

        tempQ = LvlComponentDict[tag][0];
        LvlComponentDict[tag][0] = LvlComponentDict[tag][LvlComponentDict[tag].Count - 1];
        LvlComponentDict[tag][LvlComponentDict[tag].Count - 1] = tempQ;

        return objectToSpawn;
    }

    public GameObject SpawnFromPool(int transitionLvl)
    {
        levelSpawnCounter++;

        GameObject objectToSpawn = transitionLvls[transitionLvl];

        return objectToSpawn;
    }

    public Obstacle SpawnFromObstacles(string tag, Vector2 position, Quaternion rotation, Transform parent, string texture, bool easy = false)
    {
        if (easy)
        {
            obstacleSpawnCounter++;

            int num = rng.Next(1, ezObstacleCount + 1);
            Obstacle obstacleToSpawn = ObstacleDict["EasyObstacle" + 1].Dequeue();

            obstacleToSpawn.gameObject.transform.position = position;
            obstacleToSpawn.gameObject.transform.rotation = rotation;
            obstacleToSpawn.transform.parent = parent;
            obstacleToSpawn.SetObstacleTextures(texture);

            ObstacleDict["EasyObstacle" + 1].Enqueue(obstacleToSpawn);

            return obstacleToSpawn;
        }
        else
        {
            obstacleSpawnCounter++;

            Obstacle obstacleToSpawn = ObstacleDict[tag].Dequeue();

            obstacleToSpawn.gameObject.transform.position = position;
            obstacleToSpawn.gameObject.transform.rotation = rotation;
            obstacleToSpawn.transform.parent = parent;
            obstacleToSpawn.SetObstacleTextures(texture);

            ObstacleDict[tag].Enqueue(obstacleToSpawn);

            return obstacleToSpawn;
        }

    }

    void AbsorbDone()
    {
        StartCoroutine("transitionDelay");
    }

    IEnumerator transitionDelay()
    {
        yield return new WaitForSeconds(0.5f);
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
            //CurrentLvl.transform.position = Vector2.Lerp(CurrentLvl.transform.position, this.transform.position - levelOffset,  transitionSpeed);
            //NextLvl.transform.position = Vector2.Lerp(NextLvl.transform.position, Vector3.zero,  transitionSpeed);
            CurrentLvl.transform.position = Vector2.Lerp(CurrentLvl.transform.position, this.transform.position - levelOffset, transitionSpeed * Time.deltaTime);
            NextLvl.transform.position = Vector2.Lerp(NextLvl.transform.position, Vector3.zero, transitionSpeed * Time.deltaTime);

            if (NextLvl.transform.position.y <= nextLvlThreshold)
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

                if (NextLvl.transform.position.y == 0)
                {
                    currentlySpawnedGradient.transform.parent = null;
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

    void AddLvl2SpawnQ(string lvl)
    {

    }

    void GenerateNextLvl()
    {
        if (game.GetScore < 1)
        {
            activeLvl = SpawnFromPool("level1");
        }
        else if (game.GetScore == 1)
        {
            activeLvl = SpawnFromPool(0);
        }
        else if (game.GetScore >= 2 && game.GetScore < 3)
        {
            activeLvl = SpawnFromPool("level2");
            currentlySpawnedGradient = Instantiate(lvlGradients[0],Vector3.zero,activeLvl.transform.rotation, NextLvl.transform);
            currentlySpawnedGradient.transform.parent = activeLvl.transform;
        }
        else if (game.GetScore == 3)
        {
            gradientDespawner = SpawnFromPool(1);
            activeLvl = gradientDespawner;
        }
        else if (game.GetScore >= 4 && game.GetScore < 5)
        {
            activeLvl = SpawnFromPool("level3");
            activeObstacle = SpawnFromObstacles("Obstacle" + rng.Next(1, 10), activeLvl.transform.position, activeLvl.transform.rotation, activeLvl.transform, levels[2].obstacleTexture);
        }
        else if(game.GetScore == 5)
        {
            activeLvl = specialLvls[0];
            activeObstacle = SpawnFromObstacles("Obstacle" + rng.Next(1, 10), activeLvl.transform.position, activeLvl.transform.rotation, activeLvl.transform, levels[2].obstacleTexture);
        }
        else if (game.GetScore == 6)
        {
            activeLvl = SpawnFromPool(2);
            activeObstacle = SpawnFromObstacles("Obstacle" + rng.Next(1, 10), activeLvl.transform.position, activeLvl.transform.rotation, activeLvl.transform, levels[3].obstacleTexture);
        }
        else if (game.GetScore >= 7 && game.GetScore < 8)
        {
            activeLvl = SpawnFromPool("level4");
            activeObstacle = SpawnFromObstacles("Obstacle" + rng.Next(1, 10), activeLvl.transform.position, activeLvl.transform.rotation, activeLvl.transform, levels[3].obstacleTexture);
        }
        else if (game.GetScore == 8)
        {
            activeLvl = SpawnFromPool(3);
            activeObstacle = SpawnFromObstacles("Obstacle" + rng.Next(1, 10), activeLvl.transform.position, activeLvl.transform.rotation, activeLvl.transform, levels[3].obstacleTexture,true);
        }
        else
        {
            activeLvl = SpawnFromPool("level3");
            activeObstacle = SpawnFromObstacles("Obstacle" + rng.Next(1, 10), activeLvl.transform.position, activeLvl.transform.rotation, activeLvl.transform, levels[2].obstacleTexture);
        }

        lvlSpawnQ.Enqueue(activeLvl);

        NextLvl = lvlSpawnQ.Dequeue();

        if(NextLvl.tag == "level1")
        {
            currentLvlNumber = 1;
        }
        else if (NextLvl.tag == "level2")
        {
            currentLvlNumber = 2;
        }
        else if (NextLvl.tag == "level3")
        {
            currentLvlNumber = 3;
        }
        else
        {
            currentLvlNumber = 4;
        }

        if (NextLvl == gradientDespawner)
        {
            currentlySpawnedGradient.transform.parent = CurrentLvl.transform;
        }

        NextLvl.SetActive(true);
        NextLvl.transform.position = transform.position + levelOffset;
        NextLvl.transform.rotation = transform.rotation;

        if (activeObstacle != null)
        {
            obstacleSpawnQ.Enqueue(activeObstacle);
            if (activeObstacle.transform.IsChildOf(NextLvl.transform))
            {
                NextObstacle = obstacleSpawnQ.Dequeue();
                NextObstacle.gameObject.SetActive(true);
            }
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

    public int GetCurrentLvlNumber
    {
        get
        {
            return currentLvlNumber;
        }
    }
}
