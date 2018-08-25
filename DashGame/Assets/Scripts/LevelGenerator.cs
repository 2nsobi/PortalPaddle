using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
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
        public bool about2Spawn;
        public bool comeBack2; // can this lvl type spawn again?
    }

    [System.Serializable]
    public class SinglePool
    {
        public GameObject prefab;
        public int size;
        public bool easy; //is the obstacle easy or not
    }

    public class LvlPrefab
    {
        public GameObject gameObject;
        public Obstacle obstacle;
        public bool hasObstacle;
        public int obstacleCount;

        public LvlPrefab(GameObject go)
        {
            this.gameObject = go;
            obstacle = null;
            hasObstacle = false;
            obstacleCount = 0;
        }

        public void AttachObstacle(Obstacle ob, Vector2 position, Quaternion rotation, string texture)
        {
            //obstacleCount++;
            if (!hasObstacle && obstacleCount == 0)
            {
                obstacleCount++;

                hasObstacle = true;
                this.obstacle = ob;

                ob.attached2Lvl = true;
                ob.transform.parent = this.gameObject.transform;
                ob.transform.localPosition = position;
                ob.transform.rotation = rotation;
                ob.SetObstacleTextures(texture);
                ob.gameObject.SetActive(true);
            }
        }

        public void RemoveObstacle()
        {
            if (hasObstacle)
            {
                obstacle.attached2Lvl = false;
                obstacle.gameObject.SetActive(false);
                obstacle.transform.parent = null;

                hasObstacle = false;
                obstacle = null;

                obstacleCount--;
            }
        }
    }

    public class Obstacle
    {
        public GameObject gameObject;
        public Transform transform;
        public Vector3[] path;
        public string obstacleTexture;
        SpriteRenderer[] obstacleSprites;
        public bool attached2Lvl;

        public Obstacle(GameObject go)
        {
            attached2Lvl = false;
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

    public float transitionSpeed;
    public float finishTransitionSpeed;
    public float finishTransitionThreshold; //number used to determine when the next lvl becomes teh current lvl
    static Dictionary<string, List<Queue<LvlPrefab>>> LvlComponentDict; //for background use mainly
    static Dictionary<string, Queue<Obstacle>> ObstacleDict; //for obstacle use mainly
    static Dictionary<string, Queue<GameObject>> CustomLvlQsDict;
    public List<ObstacleTexture> obstacleTextures;
    public static List<ObstacleTexture> obstacletextures;
    public List<MultiPool> levels;
    public List<SinglePool> obstacles;
    public GameObject StartLvl;
    public GameObject SettingsLvl;
    LvlPrefab StartLevel; //for code use
    GameManager game;
    Vector3 levelOffset = new Vector3(0, 10.8f, 0); // used to offset a level when it is spawned so that is spawns above the active level
    LvlPrefab NextLvl;
    LvlPrefab CurrentLvl;
    LvlPrefab PreviousLvl;
    bool currentlyTransitioning;
    bool playedOnce = false; // ateast one game session has been started since opening app, used to get rid of NullReferenceException when GameOverConfirmed() is called after app is opened
    Vector3[] travelPath1;
    int obstacleSpawnCounter;
    int levelSpawnCounter;
    PaddleController paddle;
    bool switchColors;
    Material material;
    static System.Random rng = new System.Random();
    Queue<LvlPrefab> tempQ;
    LvlPrefab[] transitionLvls;
    LvlPrefab[] specialLvls;
    GameObject[] lvlGradients;
    GameObject currentlySpawnedGradient;
    GameObject previouslySpawnedGradient;
    int gradientCounter; //how mand lvls have art gradients
    int ezObstacleCount = 0;
    static Queue<LvlPrefab> lvlSpawnQ = new Queue<LvlPrefab>();
    int specialLvlCount = 0;
    static LvlPrefab activeLvl;
    LvlPrefab transitionLvl;
    LvlPrefab specialLvl;
    LvlPrefab defaultLvl;
    LvlPrefab gradientDespawner;
    static string activeObstacleTexture;
    bool activeObstacleDifficulty;
    int nextLvlNumber;
    bool gradientSpawned;
    bool finishTransitioning;
    static string activeLvlName;
    bool allPrefsInQAttached2Lvl = true; // for spawnfrom obstacles method
    public GameObject filters;
    Animator filtersAnimC;
    bool caves2SkyFilter, cavesFilter, removeCaves2SkyFilter, disableFilters;
    bool go2Settings = false;
    bool comeBackFromSettings;
    GameObject settingsLevel;
    Vector3 offset2 = new Vector3(0, 10.6f);
    string tag4Obstacles;
    bool pauseAllCoroutines = false;
    bool generateNextLvlSequence = true;
    float distanceDiff4Walls;
    bool comeBackFromShop = false;
    public GameObject shop;
    ShopController shopC;
    ParticleSystem playButtonGlow;
    ParticleSystem.MainModule playButtonGlowMainMod;
    LvlPrefab LvlOnDeck;
    LvlPrefab dummyLvlPref;
    bool startOfGame = true;
    bool moveSettings = true;
    bool previousLvlActive = false;
    Animator labMonitorsAnimC;

    public delegate void LevelDelegate();
    public static event LevelDelegate TransitionDone;
    public static event LevelDelegate NextLvlGenerated;

    public static LevelGenerator Instance;

    private void Awake()
    {
        Instance = this;
        settingsLevel = Instantiate(SettingsLvl, levelOffset * -1, Quaternion.identity); //should be instantiated here so that all the awake methods in the shop scripts are done
    }

    private void OnEnable()
    {
        GameManager.GameStarted += GameStarted;
        BallController.AbsorbDone += AbsorbDone;
        BallController.AbsorbDoneAndRichochet += AbsorbDone;
    }

    private void OnDisable()
    {
        GameManager.GameStarted -= GameStarted;
        BallController.AbsorbDone -= AbsorbDone;
        BallController.AbsorbDoneAndRichochet -= AbsorbDone;
    }

    private void OnApplicationFocus(bool focus)
    {
        if (!focus)
        {
            pauseAllCoroutines = true;
        }
        else
        {
            pauseAllCoroutines = false;
        }
    }

    private void Start()
    {
        GameObject dummyGO = Instantiate(new GameObject("dummyGO"));
        dummyGO.SetActive(false);
        dummyLvlPref = new LvlPrefab(dummyGO);
        PreviousLvl = dummyLvlPref;

        shop = settingsLevel.transform.Find("ShopCanvas").gameObject;

        disableFilters = true;
        removeCaves2SkyFilter = true;
        caves2SkyFilter = true;
        cavesFilter = true;
        filtersAnimC = filters.GetComponent<Animator>();
        obstacletextures = obstacleTextures;

        gradientSpawned = false;
        switchColors = true;

        levelSpawnCounter = 0;
        obstacleSpawnCounter = 0;

        game = GameManager.Instance;
        paddle = PaddleController.Instance;
        shopC = ShopController.Instance; //singleton needs to be accessed here because the awake function atached to the shopcontroller script is not called until the object it is attached to is instantiated

        currentlyTransitioning = false;

        distanceDiff4Walls = game.GetDistanceDifferenceForWalls();

        StartLevel = new LvlPrefab(Instantiate(StartLvl, transform));

        labMonitorsAnimC = StartLevel.gameObject.transform.Find("labBackground1Monitors2_0").GetComponent<Animator>();

        playButtonGlow = StartLevel.gameObject.transform.Find("playButtonGlow").GetComponent<ParticleSystem>();
        playButtonGlowMainMod = playButtonGlow.main;

        material = StartLevel.gameObject.GetComponentInChildren<Renderer>().sharedMaterial;
        material.SetFloat("_InvertColors", 0);
        Transform wallW = StartLevel.gameObject.transform.GetChild(0);
        Transform wallE = StartLevel.gameObject.transform.GetChild(1);
        wallW.localPosition = new Vector3(-distanceDiff4Walls, wallW.localPosition.y, 0);
        wallE.localPosition = new Vector3(distanceDiff4Walls, wallE.localPosition.y, 0);
        CurrentLvl = StartLevel;

        LvlComponentDict = new Dictionary<string, List<Queue<LvlPrefab>>>();
        ObstacleDict = new Dictionary<string, Queue<Obstacle>>();
        CustomLvlQsDict = new Dictionary<string, Queue<GameObject>>();

        transitionLvls = new LvlPrefab[levels.Count];

        int k = 0;
        gradientCounter = 0;
        foreach (MultiPool level in levels)
        {
            List<Queue<LvlPrefab>> lvlPrefabs = new List<Queue<LvlPrefab>>();
            level.about2Spawn = true;
            level.comeBack2 = true;

            foreach (GameObject prefab in level.prefabs)
            {
                Queue<LvlPrefab> objectPool = new Queue<LvlPrefab>(); //objectPool is a queue of duplications of the same prefab

                for (int i = 0; i < level.sizePerPrefab; i++)
                {
                    LvlPrefab obj1 = new LvlPrefab(Instantiate(prefab));
                    Transform wallW0 = obj1.gameObject.transform.GetChild(0);
                    Transform wallE0 = obj1.gameObject.transform.GetChild(1);
                    wallW0.localPosition = new Vector3(-distanceDiff4Walls, wallW0.localPosition.y, 0);
                    wallE0.localPosition = new Vector3(distanceDiff4Walls, wallE0.localPosition.y, 0);
                    obj1.gameObject.SetActive(false);

                    objectPool.Enqueue(obj1);
                }
                lvlPrefabs.Add(objectPool);
            }
            LvlComponentDict.Add(level.tag, lvlPrefabs);

            transitionLvls[k] = new LvlPrefab(Instantiate(level.transitionLvl));
            Transform wallW1 = transitionLvls[k].gameObject.transform.GetChild(0);
            Transform wallE1 = transitionLvls[k].gameObject.transform.GetChild(1);
            wallW1.localPosition = new Vector3(-distanceDiff4Walls, wallW1.localPosition.y, 0);
            wallE1.localPosition = new Vector3(distanceDiff4Walls, wallE1.localPosition.y, 0);
            transitionLvls[k].gameObject.SetActive(false);

            if (level.specialLvlPrefabs.Length > 0)
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
                lvlGradients[k] = Instantiate(level.gradient);
                lvlGradients[k].SetActive(false);
            }
        }
        k = 0;

        specialLvls = new LvlPrefab[specialLvlCount];
        foreach (MultiPool level in levels)
        {
            if (level.specialLvlPrefabs.Length > 0)
            {
                foreach (GameObject go in level.specialLvlPrefabs)
                {
                    LvlPrefab obj1 = new LvlPrefab(Instantiate(go));
                    Transform wallW0 = obj1.gameObject.transform.GetChild(0);
                    Transform wallE0 = obj1.gameObject.transform.GetChild(1);
                    wallW0.localPosition = new Vector3(-distanceDiff4Walls, wallW0.localPosition.y, 0);
                    wallE0.localPosition = new Vector3(distanceDiff4Walls, wallE0.localPosition.y, 0);
                    obj1.gameObject.SetActive(false);
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
                    GameObject go = Instantiate(obstacleType.prefab);
                    go.name = obstacleType.prefab.name + "_easy";
                    Obstacle ob1 = new Obstacle(go);

                    ob1.gameObject.SetActive(false);
                    easyObstaclePool.Enqueue(ob1);
                }
                ezObstacleCount++;
                ObstacleDict.Add("EasyObstacle" + ezObstacleCount, easyObstaclePool);
            }

            ObstacleDict.Add(obstacleType.prefab.name, obstaclePool);
        }

        GenerateNextLvl();
    }

    public void ShufflePrefabsInLevels()
    {
        foreach (List<Queue<LvlPrefab>> level in LvlComponentDict.Values)
        {
            int n = level.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                Queue<LvlPrefab> tempQ = level[k];
                level[k] = level[n];
                level[n] = tempQ;
            }
        }
    }

    public void GoBack2StartLvl()
    {
        playButtonGlowMainMod.simulationSpeed = 1;
        playButtonGlow.Play();

        labMonitorsAnimC.SetBool("gameRunning", false);

        game.SetPageState(GameManager.pageState.StartPage);
        obstacleSpawnCounter = 0;
        levelSpawnCounter = 0;
        activeLvl = null;
        activeLvlName = null;
        currentlySpawnedGradient = null;
        gradientDespawner = null;
        activeObstacleTexture = null;
        gradientSpawned = false;
        NextLvl = null;
        LvlOnDeck = null;
        PreviousLvl = dummyLvlPref;

        if (playedOnce)
        {
            #region Disactivates All Level Prefabs

            foreach (LvlPrefab prefab in lvlSpawnQ)
            {
                prefab.RemoveObstacle();
            }
            lvlSpawnQ.Clear();

            foreach (List<Queue<LvlPrefab>> level in LvlComponentDict.Values)
            {
                foreach (Queue<LvlPrefab> prefabs in level)
                {
                    foreach (LvlPrefab obj in prefabs)
                    {
                        obj.RemoveObstacle();
                        obj.gameObject.transform.localPosition = Vector3.zero;
                        obj.gameObject.SetActive(false);
                    }
                }
            }

            foreach (LvlPrefab transitionLvl in transitionLvls)
            {
                transitionLvl.gameObject.transform.localPosition = Vector3.zero;
                transitionLvl.gameObject.SetActive(false);
            }

            foreach (LvlPrefab specialLvl in specialLvls)
            {
                specialLvl.gameObject.transform.localPosition = Vector3.zero;
                specialLvl.gameObject.SetActive(false);
            }

            foreach (GameObject lvlGradient in lvlGradients)
            {
                lvlGradient.transform.localPosition = Vector3.zero;
                lvlGradient.SetActive(false);
            }
            #endregion

            #region Disactivates All Obstacle Prefabs
            foreach (Queue<Obstacle> q in ObstacleDict.Values)
            {
                foreach (Obstacle ob in q)
                {
                    ob.transform.localPosition = Vector3.zero;
                    ob.transform.parent = null;
                    ob.gameObject.SetActive(false);
                }
            }
            #endregion

            CurrentLvl = StartLevel;
            CurrentLvl.gameObject.SetActive(true);
            CurrentLvl.gameObject.transform.position = Vector3.zero;

            foreach (MultiPool level in levels)
            {
                level.about2Spawn = true;
                level.comeBack2 = true;
            }

            filtersAnimC.SetTrigger("gameOver");
        }

        startOfGame = true;
        previousLvlActive = false;

        settingsLevel.SetActive(true);
        settingsLevel.transform.position = levelOffset * -1;
        moveSettings = true;

        GenerateNextLvl();
    }

    public LvlPrefab SpawnFromPool(string tag)
    {
        LvlPrefab lvlPrefab2Spawn = LvlComponentDict[tag][0].Dequeue();

        if (lvlPrefab2Spawn.hasObstacle)
        {
            LvlComponentDict[tag][0].Enqueue(lvlPrefab2Spawn);
            for (int j = 0; j < LvlComponentDict[tag].Count; j++)
            {
                for (int i = 0; i < LvlComponentDict[tag][0].Count - 1; i++) // -1 cause u already know that one prefab in the q has an obstacle
                {
                    lvlPrefab2Spawn = LvlComponentDict[tag][0].Dequeue();
                    if (!lvlPrefab2Spawn.hasObstacle)
                    {
                        LvlComponentDict[tag][0].Enqueue(lvlPrefab2Spawn);
                        FIFOList(LvlComponentDict[tag]);
                        return lvlPrefab2Spawn;
                    }
                    LvlComponentDict[tag][0].Enqueue(lvlPrefab2Spawn);
                }
                FIFOList(LvlComponentDict[tag]);
            }
            return null;
        }
        else
        {
            LvlComponentDict[tag][0].Enqueue(lvlPrefab2Spawn);
        }
        FIFOList(LvlComponentDict[tag]);

        return lvlPrefab2Spawn;
    }

    public void FIFOList<T>(List<T> list, int numberOfShifts = 1)
    {
        var temp = list[0];
        for (int i = numberOfShifts; i < list.Count; i++)
        {
            list[i - numberOfShifts] = list[i];
        }
        list[list.Count - 1] = temp;
    }

    public LvlPrefab SpawnFromPool(int transitionLvl)
    {
        LvlPrefab objectToSpawn = transitionLvls[transitionLvl];
        return objectToSpawn;
    }

    public GameObject SpawnFromGradients(int lvlGradient)
    {
        GameObject objectToSpawn = lvlGradients[lvlGradient];
        return objectToSpawn;
    }

    public Obstacle SpawnFromObstacles(int minObstacle, int maxObstacle, Vector2 position, Quaternion rotation, LvlPrefab lvlPrefab, string texture, bool easy = false)
    {
        int number = rng.Next(minObstacle, maxObstacle + 1);
        tag4Obstacles = "Obstacle" + number;

        if (easy)
        {
            int num = rng.Next(1, ezObstacleCount + 1);
            tag4Obstacles = "EasyObstacle" + num;

            Obstacle obstacleToSpawn = ObstacleDict[tag4Obstacles].Dequeue();

            if (obstacleToSpawn.attached2Lvl)
            {
                ObstacleDict[tag4Obstacles].Enqueue(obstacleToSpawn);

                while (allPrefsInQAttached2Lvl)
                {
                    for (int i = 0; i < ObstacleDict[tag4Obstacles].Count - 1; i++) // -1 cause u already know that one prefab in the q has an obstacle
                    {
                        obstacleToSpawn = ObstacleDict[tag4Obstacles].Dequeue();
                        if (!obstacleToSpawn.attached2Lvl)
                        {
                            lvlPrefab.AttachObstacle(obstacleToSpawn, position, rotation, texture);

                            ObstacleDict[tag4Obstacles].Enqueue(obstacleToSpawn);
                            return obstacleToSpawn;

                        }
                        ObstacleDict[tag4Obstacles].Enqueue(obstacleToSpawn);
                    }

                    int num2 = rng.Next(1, ezObstacleCount + 1);
                    while (num2 == num)
                    {
                        num2 = rng.Next(1, ezObstacleCount + 1);
                    }
                    num = num2;
                    tag4Obstacles = "EasyObstacle" + num2;
                }

            }
            else
            {
                lvlPrefab.AttachObstacle(obstacleToSpawn, position, rotation, texture);

                ObstacleDict[tag4Obstacles].Enqueue(obstacleToSpawn);
            }

            return obstacleToSpawn;

        }
        else
        {
            Obstacle obstacleToSpawn = ObstacleDict[tag4Obstacles].Dequeue();

            if (obstacleToSpawn.attached2Lvl)
            {
                ObstacleDict[tag4Obstacles].Enqueue(obstacleToSpawn);

                while (allPrefsInQAttached2Lvl)
                {
                    for (int i = 0; i < ObstacleDict[tag4Obstacles].Count - 1; i++) // -1 cause u already know that one prefab in the q has an obstacle
                    {
                        obstacleToSpawn = ObstacleDict[tag4Obstacles].Dequeue();
                        if (!obstacleToSpawn.attached2Lvl)
                        {
                            lvlPrefab.AttachObstacle(obstacleToSpawn, position, rotation, texture);

                            ObstacleDict[tag4Obstacles].Enqueue(obstacleToSpawn);
                            return obstacleToSpawn;
                        }
                        ObstacleDict[tag4Obstacles].Enqueue(obstacleToSpawn);
                    }

                    int number2 = rng.Next(minObstacle, maxObstacle + 1);
                    while (number2 == number)
                    {
                        number2 = rng.Next(minObstacle, maxObstacle + 1);
                    }
                    number = number2;
                    tag4Obstacles = "Obstacle" + number2;
                }

            }
            else
            {
                lvlPrefab.AttachObstacle(obstacleToSpawn, position, rotation, texture);

                ObstacleDict[tag4Obstacles].Enqueue(obstacleToSpawn);
            }

            return obstacleToSpawn;
        }

    }

    void AbsorbDone()
    {
        StartCoroutine(transitionDelay());
    }

    IEnumerator transitionDelay()
    {
        for (float i = 0.0f; i < 0.5f; i += 0.1f)
        {
            yield return new WaitForSeconds(0.1f);
            while (pauseAllCoroutines || game.Paused)
            {
                yield return null;
            }
        }
        Debug.Log("should be transitioning");

        if (cavesFilter)
        {
            if (NextLvl.gameObject.tag == "level1")
            {
                filtersAnimC.SetTrigger("fade2Caves");
                cavesFilter = false;
            }
        }
        if (caves2SkyFilter)
        {
            if (NextLvl.gameObject.tag == "caves2Sky")
            {
                filtersAnimC.SetTrigger("fade2Caves2Sky");
                caves2SkyFilter = false;
            }
        }
        else
        {
            if (removeCaves2SkyFilter)
            {
                filtersAnimC.SetTrigger("remove");
                removeCaves2SkyFilter = false;
            }

            if (disableFilters)
            {
                filtersAnimC.SetTrigger("disableFilters");
                disableFilters = false;
            }
        }
        currentlyTransitioning = true;
    }

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
        if (go2Settings)
        {
            if (settingsLevel.transform.position.y < -0.8f)
            {
                NextLvl.gameObject.transform.position = Vector2.Lerp(NextLvl.gameObject.transform.position, this.transform.position + levelOffset * 2, 4 * Time.deltaTime);
                CurrentLvl.gameObject.transform.position = Vector2.Lerp(CurrentLvl.gameObject.transform.position, this.transform.position + levelOffset, 4 * Time.deltaTime);
                settingsLevel.transform.position = Vector2.Lerp(settingsLevel.transform.position, Vector3.zero, 4 * Time.deltaTime);
            }
            else
            {
                NextLvl.gameObject.transform.position = Vector2.MoveTowards(NextLvl.gameObject.transform.position, this.transform.position + levelOffset * 2, 2 * Time.deltaTime);
                CurrentLvl.gameObject.transform.position = Vector2.MoveTowards(CurrentLvl.gameObject.transform.position, this.transform.position + levelOffset, 2 * Time.deltaTime);
                settingsLevel.transform.position = Vector2.MoveTowards(settingsLevel.transform.position, Vector3.zero, 2 * Time.deltaTime);
                if (settingsLevel.transform.position.y == 0)
                {
                    go2Settings = false;
                }
            }
        }

        if (comeBackFromSettings)
        {
            if (CurrentLvl.gameObject.transform.position.y > 0.8f)
            {
                NextLvl.gameObject.transform.position = Vector2.Lerp(NextLvl.gameObject.transform.position, this.transform.position + levelOffset, 4 * Time.deltaTime);
                CurrentLvl.gameObject.transform.position = Vector2.Lerp(CurrentLvl.gameObject.transform.position, Vector3.zero, 4 * Time.deltaTime);
                settingsLevel.transform.position = Vector2.Lerp(settingsLevel.transform.position, this.transform.position - offset2, 4 * Time.deltaTime);
            }
            else
            {
                NextLvl.gameObject.transform.position = Vector2.MoveTowards(NextLvl.gameObject.transform.position, this.transform.position + levelOffset, 2 * Time.deltaTime);
                CurrentLvl.gameObject.transform.position = Vector2.MoveTowards(CurrentLvl.gameObject.transform.position, Vector3.zero, 2 * Time.deltaTime);
                settingsLevel.transform.position = Vector2.MoveTowards(settingsLevel.transform.position, this.transform.position - offset2, 2 * Time.deltaTime);
                if (CurrentLvl.gameObject.transform.position.y == 0)
                {
                    comeBackFromSettings = false;
                    game.SetPageState(GameManager.pageState.StartPage);
                }
            }
        }

        if (comeBackFromShop)
        {
            if (CurrentLvl.gameObject.transform.position.y > 0.8f)
            {
                NextLvl.gameObject.transform.position = Vector2.Lerp(NextLvl.gameObject.transform.position, this.transform.position + levelOffset, 4 * Time.deltaTime);
                CurrentLvl.gameObject.transform.position = Vector2.Lerp(CurrentLvl.gameObject.transform.position, Vector3.zero, 4 * Time.deltaTime);
                settingsLevel.transform.position = Vector2.Lerp(settingsLevel.transform.position, this.transform.position - offset2, 4 * Time.deltaTime);
            }
            else
            {
                NextLvl.gameObject.transform.position = Vector2.MoveTowards(NextLvl.gameObject.transform.position, this.transform.position + levelOffset, 2 * Time.deltaTime);
                CurrentLvl.gameObject.transform.position = Vector2.MoveTowards(CurrentLvl.gameObject.transform.position, Vector3.zero, 2 * Time.deltaTime);
                settingsLevel.transform.position = Vector2.MoveTowards(settingsLevel.transform.position, this.transform.position - offset2, 2 * Time.deltaTime);
                if (CurrentLvl.gameObject.transform.position.y == 0)
                {
                    comeBackFromShop = false;
                    shop.SetActive(false);
                    game.SetPageState(GameManager.pageState.StartPage);
                }
            }
        }

        if (currentlyTransitioning)
        {
            if (NextLvl.gameObject.transform.position.y > finishTransitionThreshold)
            {
                if (moveSettings)
                {
                    settingsLevel.gameObject.transform.position = Vector2.Lerp(settingsLevel.gameObject.transform.position, this.transform.position - levelOffset * 2, transitionSpeed * Time.deltaTime);
                }
                PreviousLvl.gameObject.transform.position = Vector2.Lerp(PreviousLvl.gameObject.transform.position, this.transform.position - levelOffset * 2, transitionSpeed * Time.deltaTime);
                CurrentLvl.gameObject.transform.position = Vector2.Lerp(CurrentLvl.gameObject.transform.position, this.transform.position - levelOffset, transitionSpeed * Time.deltaTime);
                NextLvl.gameObject.transform.position = Vector2.Lerp(NextLvl.gameObject.transform.position, Vector3.zero, transitionSpeed * Time.deltaTime);
                LvlOnDeck.gameObject.transform.position = Vector2.Lerp(LvlOnDeck.gameObject.transform.position, levelOffset, transitionSpeed * Time.deltaTime);
            }
            else
            {
                if (moveSettings)
                {
                    settingsLevel.SetActive(false);
                    moveSettings = false;
                }
                PreviousLvl.gameObject.transform.position = Vector2.MoveTowards(PreviousLvl.gameObject.transform.position, this.transform.position - levelOffset * 2, finishTransitionSpeed * Time.deltaTime);
                CurrentLvl.gameObject.transform.position = Vector2.MoveTowards(CurrentLvl.gameObject.transform.position, this.transform.position - levelOffset, finishTransitionSpeed * Time.deltaTime);
                NextLvl.gameObject.transform.position = Vector2.MoveTowards(NextLvl.gameObject.transform.position, Vector3.zero, finishTransitionSpeed * Time.deltaTime);
                LvlOnDeck.gameObject.transform.position = Vector2.MoveTowards(LvlOnDeck.gameObject.transform.position, levelOffset, finishTransitionSpeed * Time.deltaTime);
                if (NextLvl.gameObject.transform.position.y == 0)
                {
                    if (PreviousLvl.hasObstacle)
                    {
                        PreviousLvl.RemoveObstacle();
                    }

                    PreviousLvl.gameObject.SetActive(false);

                    PreviousLvl = CurrentLvl;

                    CurrentLvl = NextLvl; //technically the next lvl at this point
                    NextLvl = LvlOnDeck;

                    TransitionDone();
                    GenerateNextLvl();
                    currentlyTransitioning = false;

                    if (gradientDespawner != null)
                    {
                        if (NextLvl == gradientDespawner)
                        {
                            currentlySpawnedGradient.transform.parent = CurrentLvl.gameObject.transform;
                            currentlySpawnedGradient = null;
                            gradientDespawner = null;
                            gradientSpawned = false;
                        }
                    }
                }
            }

        }

        if (game.IsGameRunning)
        {
            if (gradientSpawned)
            {
                if (currentlySpawnedGradient.transform.position.y == 0 && NextLvl != gradientDespawner)
                {
                    currentlySpawnedGradient.transform.parent = null;
                    gradientSpawned = false;
                }
            }

        }

    }

    void GameStarted()
    {
        labMonitorsAnimC.SetBool("gameRunning", true);

        playButtonGlowMainMod.simulationSpeed = 6;
        playButtonGlow.Stop();

        ShufflePrefabsInLevels();
        NextLvlGenerated(); //need to call this here outside of GenerateNextLvl() since two levels are always loaded above before game starts

        playedOnce = true;
        disableFilters = true;
        removeCaves2SkyFilter = true;
        caves2SkyFilter = true;
        cavesFilter = true;

        filtersAnimC.ResetTrigger("gameOver");
    }

    void GenerateNextLvl()
    {
        while (generateNextLvlSequence)
        {
            if (game.GetScore >= 0)
            {
                activeLvlName = "level1";
            }
            else
            {
                break;
            }

            if (game.GetScore >= 2)
            {
                if (levels[1].about2Spawn)
                {
                    transitionLvl = SpawnFromPool(0);
                    lvlSpawnQ.Enqueue(transitionLvl);

                    defaultLvl = SpawnFromPool("level2");
                    SpawnFromObstacles(1, 9, Vector2.zero, defaultLvl.gameObject.transform.rotation, defaultLvl, levels[1].obstacleTexture, true);
                    lvlSpawnQ.Enqueue(defaultLvl);

                    currentlySpawnedGradient = SpawnFromGradients(0);
                    currentlySpawnedGradient.transform.position = Vector3.zero;
                    currentlySpawnedGradient.SetActive(true);
                    currentlySpawnedGradient.transform.localPosition = Vector3.zero;
                    currentlySpawnedGradient.transform.parent = defaultLvl.gameObject.transform;

                    levels[1].about2Spawn = false;
                }
                activeLvlName = "level2";
                activeObstacleTexture = levels[1].obstacleTexture;
                activeObstacleDifficulty = true; // true as in easy
            }
            else
            {
                break;
            }

            if (game.GetScore >= 4)
            {
                if (levels[2].about2Spawn)
                {
                    if (levels[2].comeBack2)
                    {
                        gradientDespawner = SpawnFromPool(1); //gradientdespawner is considered to be the transitionlvl in this case
                        SpawnFromObstacles(1, 9, Vector2.zero, gradientDespawner.gameObject.transform.rotation, gradientDespawner, levels[1].obstacleTexture, true);
                        lvlSpawnQ.Enqueue(gradientDespawner);
                        Debug.Log("gradientDespawner has been enqueed with obstacle");

                        defaultLvl = SpawnFromPool("level3");
                        SpawnFromObstacles(1, 9, Vector2.zero, defaultLvl.gameObject.transform.rotation, defaultLvl, levels[2].obstacleTexture);
                        lvlSpawnQ.Enqueue(defaultLvl);

                        levels[2].about2Spawn = false;
                    }
                }
                activeLvlName = "level3";
                activeObstacleTexture = levels[2].obstacleTexture;
                activeObstacleDifficulty = false;
            }
            else
            {
                break;
            }

            if (game.GetScore >= 6)
            {
                if (levels[3].about2Spawn)
                {
                    specialLvl = specialLvls[0];
                    SpawnFromObstacles(1, 9, Vector2.zero, specialLvl.gameObject.transform.rotation, specialLvl, levels[2].obstacleTexture);
                    lvlSpawnQ.Enqueue(specialLvl);

                    transitionLvl = SpawnFromPool(2);
                    SpawnFromObstacles(1, 9, Vector2.zero, transitionLvl.gameObject.transform.rotation, transitionLvl, levels[3].obstacleTexture);
                    lvlSpawnQ.Enqueue(transitionLvl);

                    defaultLvl = SpawnFromPool("level4");
                    SpawnFromObstacles(1, 9, Vector2.zero, defaultLvl.gameObject.transform.rotation, defaultLvl, levels[3].obstacleTexture);
                    lvlSpawnQ.Enqueue(defaultLvl);

                    levels[3].about2Spawn = false;
                    levels[2].about2Spawn = true;
                    levels[2].comeBack2 = false;
                }
                activeLvlName = "level4";
                activeObstacleTexture = levels[3].obstacleTexture;
                activeObstacleDifficulty = false;
            }
            else
            {
                break;
            }

            if (game.GetScore >= 8)
            {
                if (levels[2].about2Spawn)
                {
                    transitionLvl = SpawnFromPool(3);
                    SpawnFromObstacles(1, 9, Vector2.zero, transitionLvl.gameObject.transform.rotation, transitionLvl, levels[3].obstacleTexture, true);
                    lvlSpawnQ.Enqueue(transitionLvl);

                    defaultLvl = SpawnFromPool("level3");
                    SpawnFromObstacles(1, 9, Vector2.zero, defaultLvl.gameObject.transform.rotation, defaultLvl, levels[2].obstacleTexture);
                    lvlSpawnQ.Enqueue(defaultLvl);

                    levels[2].about2Spawn = false;
                }
                activeLvlName = "level3";
                activeObstacleTexture = levels[2].obstacleTexture;
                activeObstacleDifficulty = false;
            }
            break;
        }

        activeLvl = SpawnFromPool(activeLvlName);
        if (activeLvl != null)
        {
            if (activeObstacleTexture != null)
            {
                SpawnFromObstacles(1, 9, Vector2.zero, activeLvl.gameObject.transform.rotation, activeLvl, activeObstacleTexture, activeObstacleDifficulty);
            }

            lvlSpawnQ.Enqueue(activeLvl);

            if (startOfGame)
            {
                LvlPrefab tempLvl = SpawnFromPool(activeLvlName);
                lvlSpawnQ.Enqueue(tempLvl);
            }
        }

        if (startOfGame)
        {
            levelSpawnCounter++;
            NextLvl = lvlSpawnQ.Dequeue();
        }
        levelSpawnCounter++;
        LvlOnDeck = lvlSpawnQ.Dequeue();

        if (NextLvl.gameObject.tag == "level1")
        {
            nextLvlNumber = 1;
        }
        else if (NextLvl.gameObject.tag == "level2")
        {
            nextLvlNumber = 2;
        }
        else if (NextLvl.gameObject.tag == "level3")
        {
            nextLvlNumber = 3;
        }
        else if (NextLvl.gameObject.tag == "level4")
        {
            nextLvlNumber = 4;
        }
        NextLvl.gameObject.SetActive(true);
        NextLvl.gameObject.transform.position = transform.position + levelOffset;
        NextLvl.gameObject.transform.rotation = transform.rotation;

        LvlOnDeck.gameObject.SetActive(true);
        LvlOnDeck.gameObject.transform.position = transform.position + levelOffset * 2;
        LvlOnDeck.gameObject.transform.rotation = transform.rotation;

        if (currentlySpawnedGradient != null)
        {
            if (currentlySpawnedGradient.transform.IsChildOf(NextLvl.gameObject.transform))
            {
                gradientSpawned = true;
            }
        }

        if (NextLvl.hasObstacle)
        {
            obstacleSpawnCounter++;
        }

        if (!startOfGame)
        {
            NextLvlGenerated();
        }
        else
        {
            startOfGame = false;
        }
    }

    public Transform GetNextLvl
    {
        get
        {
            return NextLvl.gameObject.transform;
        }
    }

    public Transform GetCurrentLvl
    {
        get
        {
            return CurrentLvl.gameObject.transform;
        }
    }

    public Vector3[] GetNextObstaclePath
    {
        get
        {
            if (NextLvl.hasObstacle)
            {
                return NextLvl.obstacle.path;
            }
            return null;
        }
    }

    public Vector3[] GetCurrentObstaclePath
    {
        get
        {
            return CurrentLvl.obstacle.path;
        }
    }

    public int GetNextLvlNumber
    {
        get
        {
            return nextLvlNumber;
        }
    }

    public void GoToSettingsPage()
    {
        settingsLevel.transform.position = new Vector2(0, -10.8f);
        go2Settings = true;
        shop.SetActive(false);
    }

    public void GoToShop()
    {
        settingsLevel.transform.position = new Vector2(0, -10.8f);
        go2Settings = true;
        shopC.Go2Shop();
    }

    public void ComeBackFromSettingsPage()
    {
        go2Settings = false;
        comeBackFromSettings = true;
    }

    public void ComeBackFromShop()
    {
        go2Settings = false;
        comeBackFromShop = true;
    }

    public bool PlayedOnce
    {
        get
        {
            return playedOnce;
        }
    }

}
