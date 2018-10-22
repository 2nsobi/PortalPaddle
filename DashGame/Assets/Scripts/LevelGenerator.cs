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
        public bool hasWalls;
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

    public Vector2 targetAspectRatio;
    public float transitionSpeed;
    public float finishTransitionSpeed;
    public float finishTransitionThreshold; //number used to determine when the next lvl becomes teh current lvl
    static Dictionary<string, List<Queue<LvlPrefab>>> LvlComponentDict; //for background use mainly
    static Dictionary<string, Queue<Obstacle>> ObstacleDict; //for obstacle use mainly
    public List<ObstacleTexture> obstacleTextures;
    public static List<ObstacleTexture> obstacletextures;
    public List<MultiPool> levels;
    public List<SinglePool> obstacles;
    public GameObject StartLvl;
    public GameObject SettingsLvl;
    public Transform GameOverZoneN;
    public Transform GameOverZoneS;
    public GameObject settingsPage;
    LvlPrefab StartLevel; //for code use
    GameManager game;
    AudioManager audioManager;
    FilterController filterController;
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
    static CryptoRandom rng = new CryptoRandom();
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
    BallController ballC;
    TargetController target;
    float thisDeviceCameraRadius;
    bool dontMoveWalls = false;
    int tempNum = 0;
    int tempNum2 = 0;
    bool[] filterBools = new bool[3];
    bool moonLvlPassed = false;
    bool earthLvlPassed = false;
    bool soundOnDeck;
    string nextLvlSound2Play;
    bool musicOnDeck;
    string nextMusic2Play;
    AchievementsAndLeaderboards rankings;
    bool interstellar = false;
    bool lunarKing = false;

    public delegate void LevelDelegate();
    public static event LevelDelegate TransitionDone;
    public static event LevelDelegate NextLvlGenerated;

    public static LevelGenerator Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        settingsLevel = Instantiate(SettingsLvl, levelOffset * -1, Quaternion.identity); //should be instantiated here so that all the awake methods in the shop scripts are done

        ConfigureCamera(); //called here before the tap area rect is configured

        distanceDiff4Walls = GetDistanceDifferenceForWalls();

        earthLvlPassed = PlayerPrefsX.GetBool("earthLvlPassed");
        moonLvlPassed = PlayerPrefsX.GetBool("moonLvlPassed");
    }

    /*********************************************
    * In unity "The size value for orthographic camera basically decides the Height of the camera while the Aspect 
    * Ratio of your game decides the width of the camera. When increasing the "height" size on the camera the width 
    * is also increased to keep with the current aspect."
    * 
    * Also the [aspect ratio (width/height)] * [camera size] = camera width; 
    ************************************************/
    public void ConfigureCamera()
    {
        thisDeviceCameraRadius = (Camera.main.aspect * Camera.main.orthographicSize);
        float desiredCameraWidth = (targetAspectRatio.x / targetAspectRatio.y) * Camera.main.orthographicSize;

        if (thisDeviceCameraRadius < desiredCameraWidth - 0.001f) //for some reason (targetAspectRatio.x / targetAspectRatio.y) * Camera.main.orthographicSize does not equal what is should exactly
        {
            Camera.main.orthographicSize = desiredCameraWidth / Camera.main.aspect;
            dontMoveWalls = true;
        }

        GameOverZoneN.position = new Vector2(0, Camera.main.orthographicSize + 1.725946f);
        GameOverZoneS.position = new Vector2(0, -(Camera.main.orthographicSize + 1.725946f));
    }

    public float GetDistanceDifferenceForWalls() //width of a wall is a bout 0.116524, and this gives the east wall an X pos of 3.700936 when the target aspect ratio is 9:16
    {
        if (dontMoveWalls)
        {
            return 3.700936f; // x pos of wall at aspect ratio of 3.700936
        }
        return thisDeviceCameraRadius + 0.888436f; // 0.888436 is the diff between the x pos of a wall at x pos 3.700936 and the camera width of a 9:16 aspect ratio
    }

    private void OnEnable()
    {
        GameManager.GameStarted += GameStarted;
        Ball.AbsorbDone += AbsorbDone;
        Ball.AbsorbDoneAndRichochet += AbsorbDone;
    }

    private void OnDisable()
    {
        GameManager.GameStarted -= GameStarted;
        Ball.AbsorbDone -= AbsorbDone;
        Ball.AbsorbDoneAndRichochet -= AbsorbDone;

        PlayerPrefsX.SetBool("earthLvlPassed", earthLvlPassed);
        PlayerPrefsX.SetBool("moonLvlPassed", moonLvlPassed);

        if (interstellar)
        {
            rankings.UnlockAchievement(GPGSIds.achievement_interstellar);
        }

        if (lunarKing)
        {
            rankings.UnlockAchievement(GPGSIds.achievement_lunar_king);
        }
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
        GameObject dummyGO = new GameObject("dummyGO");
        dummyGO.SetActive(false);
        dummyLvlPref = new LvlPrefab(dummyGO);
        PreviousLvl = dummyLvlPref;

        shop = settingsLevel.transform.Find("ShopCanvas").gameObject;
        settingsPage = settingsLevel.transform.Find("SettingsCanvas").gameObject;

        obstacletextures = obstacleTextures;

        gradientSpawned = false;
        switchColors = true;

        levelSpawnCounter = 0;
        obstacleSpawnCounter = 0;

        game = GameManager.Instance;
        paddle = PaddleController.Instance;
        shopC = ShopController.Instance; //singleton needs to be accessed here because the awake function atached to the shopcontroller script is not called until the object it is attached to is instantiated
        ballC = BallController.Instance;
        target = TargetController.Instance;
        filterController = FilterController.Instance;
        audioManager = AudioManager.Instance;
        rankings = AchievementsAndLeaderboards.Instance;

        currentlyTransitioning = false;

        StartLevel = new LvlPrefab(Instantiate(StartLvl, transform));
        labMonitorsAnimC = StartLevel.gameObject.transform.Find("labBackground1Monitors2_0").GetComponent<Animator>();

        playButtonGlow = StartLevel.gameObject.transform.Find("playButtonGlow").GetComponent<ParticleSystem>();
        playButtonGlowMainMod = playButtonGlow.main;
        if (earthLvlPassed)
        {
            playButtonGlowMainMod.startColor = new Color(0, 1, 0.9901032f, 0.9176471f); // turquoise
        }
        if (moonLvlPassed)
        {
            playButtonGlowMainMod.startColor = Color.yellow;
        }

        material = StartLevel.gameObject.GetComponentInChildren<Renderer>().sharedMaterial;
        material.SetFloat("_InvertColors", 0);
        Transform wallW = StartLevel.gameObject.transform.GetChild(0);
        Transform wallE = StartLevel.gameObject.transform.GetChild(1);
        wallW.localPosition = new Vector3(-distanceDiff4Walls, wallW.localPosition.y, 0);
        wallE.localPosition = new Vector3(distanceDiff4Walls, wallE.localPosition.y, 0);
        CurrentLvl = StartLevel;

        LvlComponentDict = new Dictionary<string, List<Queue<LvlPrefab>>>();
        ObstacleDict = new Dictionary<string, Queue<Obstacle>>();

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
                    if (level.hasWalls)
                    {
                        Transform wallW0 = obj1.gameObject.transform.GetChild(0);
                        Transform wallE0 = obj1.gameObject.transform.GetChild(1);
                        wallW0.localPosition = new Vector3(-distanceDiff4Walls, wallW0.localPosition.y, 0);
                        wallE0.localPosition = new Vector3(distanceDiff4Walls, wallE0.localPosition.y, 0);
                    }
                    obj1.gameObject.SetActive(false);

                    objectPool.Enqueue(obj1);
                }
                lvlPrefabs.Add(objectPool);
            }
            LvlComponentDict.Add(level.tag, lvlPrefabs);

            transitionLvls[k] = new LvlPrefab(Instantiate(level.transitionLvl));
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

            if (obstacleType.easy)
            {
                Queue<Obstacle> easyObstaclePool = new Queue<Obstacle>();

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

            for (int i = 0; i < obstacleType.size; i++)
            {
                Obstacle ob = new Obstacle(Instantiate(obstacleType.prefab));

                ob.gameObject.SetActive(false);
                obstaclePool.Enqueue(ob);
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

        audioManager.StopMusic();
        audioManager.PlayLvlSound("ambientLab");

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
                transitionLvl.RemoveObstacle();
                transitionLvl.gameObject.transform.localPosition = Vector3.zero;
                transitionLvl.gameObject.SetActive(false);
            }

            foreach (LvlPrefab specialLvl in specialLvls)
            {
                specialLvl.RemoveObstacle();
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

            filterController.gameObject.SetActive(false);
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
                for (int i = 0; i < LvlComponentDict[tag][0].Count; i++)
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

        while (number == tempNum)
        {
            number = rng.Next(minObstacle, maxObstacle + 1);
        }

        tempNum = number;

        tag4Obstacles = "Obstacle" + number;

        if (easy)
        {
            int num = rng.Next(1, ezObstacleCount + 1);

            while (num == tempNum2)
            {
                num = rng.Next(1, ezObstacleCount + 1);
            }

            tempNum2 = num;

            tag4Obstacles = "EasyObstacle" + num;

            Obstacle obstacleToSpawn = ObstacleDict[tag4Obstacles].Dequeue();

            if (obstacleToSpawn.attached2Lvl)
            {
                ObstacleDict[tag4Obstacles].Enqueue(obstacleToSpawn);

                while (allPrefsInQAttached2Lvl)
                {
                    for (int i = 0; i < ObstacleDict[tag4Obstacles].Count; i++)
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
                    for (int i = 0; i < ObstacleDict[tag4Obstacles].Count; i++)
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
        yield return new WaitForSeconds(0.5f);

        if (NextLvl.gameObject.tag == "level1")
        {
            if (filterBools[0])
            {
                filterController.Fade2Filter("CavesBlack2TransparentGradient");
                filterBools[0] = false;
            }

            if (nextLvlSound2Play != "ambientCaves")
            {
                nextLvlSound2Play = "ambientCaves";
                soundOnDeck = true;
            }
        }
        if (NextLvl.gameObject.tag == "caves2Sky")
        {
            if (filterBools[1])
            {
                filterController.Fade2Filter("caves2SkyBackgroundGradient");
                filterBools[1] = false;
            }

            nextMusic2Play = "skyMusic";
            musicOnDeck = true;

            nextLvlSound2Play = "caves2Sky";
            soundOnDeck = true;
        }
        if(nextLvlNumber == 2)
        {
            if (filterBools[2])
            {
                filterController.ClearFilters();
                filterBools[2] = false;
            }

            if(nextLvlSound2Play != "ambientSky")
            {
                nextLvlSound2Play = "ambientSky";
                soundOnDeck = true;
            }
        }

        if(NextLvl == transitionLvls[1])
        {
            nextLvlSound2Play = null;
            soundOnDeck = true;

            nextMusic2Play = "spaceMusic";
            musicOnDeck = true;

            interstellar = true;
        }

        if (NextLvl == transitionLvls[2])
        {
            musicOnDeck = true;
            nextMusic2Play = "moonMusic";
        }

        if (NextLvl == transitionLvls[3])
        {
            musicOnDeck = true;
            nextMusic2Play = "spaceMusic";

            lunarKing = true;
        }

        if (soundOnDeck)
        {
            if (nextLvlSound2Play == null)
            {
                audioManager.ClearLvlSounds();
            }
            else
            {
                audioManager.Fade2LvlSound(nextLvlSound2Play);
            }
            soundOnDeck = false;
        }

        if (musicOnDeck)
        {
            if(nextMusic2Play != null)
            {
                audioManager.Fade2Music(nextMusic2Play);
            }
            musicOnDeck = false;
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
                    settingsPage.SetActive(false);
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

                    CurrentLvl = NextLvl; //which is the level currently on screen

                    if (!earthLvlPassed)
                    {
                        if (CurrentLvl == transitionLvls[1])
                        {
                            playButtonGlowMainMod.startColor = new Color(0, 1, 0.9901032f, 0.9176471f); // turquoise
                            earthLvlPassed = true;
                        }
                    }
                    if (!moonLvlPassed)
                    {
                        if (CurrentLvl == transitionLvls[3])
                        {
                            playButtonGlowMainMod.startColor = Color.yellow;
                            moonLvlPassed = true;
                        }
                    }

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
        audioManager.PlayMusic("cavesMusic");

        for (int i = 0; i < filterBools.Length; i++)
        {
            filterBools[i] = true;
        }

        ShufflePrefabsInLevels();

        NextLvlGenerated(); //need to call this here outside of GenerateNextLvl() since two levels are always loaded above before game starts

        playedOnce = true;

        filterController.gameObject.SetActive(true);
    }

    public void TurnOffLab()
    {
        labMonitorsAnimC.SetBool("gameRunning", true);

        playButtonGlowMainMod.simulationSpeed = 6;
        playButtonGlow.Stop();
    }

    void GenerateNextLvl()
    {
        while (generateNextLvlSequence)
        {
            if (game.GetScore >= 0)
            {
                activeLvlName = "level1";
                activeObstacleTexture = levels[0].obstacleTexture;
                activeObstacleDifficulty = true; // true as in easy
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
                    SpawnFromObstacles(1, obstacles.Count, Vector2.zero, transitionLvl.gameObject.transform.rotation, transitionLvl, levels[1].obstacleTexture, true);
                    lvlSpawnQ.Enqueue(transitionLvl);

                    defaultLvl = SpawnFromPool("level2");
                    SpawnFromObstacles(1, obstacles.Count, Vector2.zero, defaultLvl.gameObject.transform.rotation, defaultLvl, levels[1].obstacleTexture);
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
                activeObstacleDifficulty = false; // true as in easy
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
                        SpawnFromObstacles(1, obstacles.Count, Vector2.zero, gradientDespawner.gameObject.transform.rotation, gradientDespawner, levels[1].obstacleTexture, true);
                        lvlSpawnQ.Enqueue(gradientDespawner);

                        defaultLvl = SpawnFromPool("level3");
                        SpawnFromObstacles(1, obstacles.Count, Vector2.zero, defaultLvl.gameObject.transform.rotation, defaultLvl, levels[2].obstacleTexture);
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
                    SpawnFromObstacles(1, obstacles.Count, Vector2.zero, specialLvl.gameObject.transform.rotation, specialLvl, levels[2].obstacleTexture);
                    lvlSpawnQ.Enqueue(specialLvl);

                    transitionLvl = SpawnFromPool(2);
                    SpawnFromObstacles(1, obstacles.Count, Vector2.zero, transitionLvl.gameObject.transform.rotation, transitionLvl, levels[3].obstacleTexture);
                    lvlSpawnQ.Enqueue(transitionLvl);

                    defaultLvl = SpawnFromPool("level4");
                    SpawnFromObstacles(1, obstacles.Count, Vector2.zero, defaultLvl.gameObject.transform.rotation, defaultLvl, levels[3].obstacleTexture);
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
                    SpawnFromObstacles(1, obstacles.Count, Vector2.zero, transitionLvl.gameObject.transform.rotation, transitionLvl, levels[3].obstacleTexture, true);
                    lvlSpawnQ.Enqueue(transitionLvl);

                    defaultLvl = SpawnFromPool("level3");
                    SpawnFromObstacles(1, obstacles.Count, Vector2.zero, defaultLvl.gameObject.transform.rotation, defaultLvl, levels[2].obstacleTexture);
                    lvlSpawnQ.Enqueue(defaultLvl);

                    levels[2].about2Spawn = false;
                }
                activeLvlName = "level3";
                activeObstacleTexture = levels[2].obstacleTexture;
                activeObstacleDifficulty = false;
            }
            break;
        }

        if (lvlSpawnQ.Count <= 5) //limit the amount of lvls being queued to prevent obstacle theft errors
        {
            activeLvl = SpawnFromPool(activeLvlName);
            if (activeLvl != null)
            {
                if (activeObstacleTexture != null)
                {
                    SpawnFromObstacles(1, obstacles.Count, Vector2.zero, activeLvl.gameObject.transform.rotation, activeLvl, activeObstacleTexture, activeObstacleDifficulty);
                }

                lvlSpawnQ.Enqueue(activeLvl);

                if (startOfGame)
                {
                    LvlPrefab tempLvl = SpawnFromPool(activeLvlName);
                    SpawnFromObstacles(1, obstacles.Count, Vector2.zero, tempLvl.gameObject.transform.rotation, tempLvl, activeObstacleTexture, activeObstacleDifficulty);
                    lvlSpawnQ.Enqueue(tempLvl);
                }
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
            ballC.IncreaseDropSpeed(4.75f, 18.25f);
            target.IncreaseTravelSpeed(3);
        }
        else if (NextLvl.gameObject.tag == "level3")
        {
            if (nextLvlNumber >= 4)
            {
                nextLvlNumber = 5;
                ballC.IncreaseDropSpeed(10, 25);
                target.IncreaseTravelSpeed(6);
            }
            else
            {
                nextLvlNumber = 3;
                ballC.IncreaseDropSpeed(6.5f, 20.5f);
                target.IncreaseTravelSpeed(4);
            }
        }
        else if (NextLvl.gameObject.tag == "level4")
        {
            nextLvlNumber = 4;
            ballC.IncreaseDropSpeed(8.25f, 22.75f);
            target.IncreaseTravelSpeed(5);
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
        audioManager.Go2LabBasement();

        settingsLevel.transform.position = new Vector2(0, -10.8f);
        go2Settings = true;
        shop.SetActive(false);
    }

    public void GoToShop()
    {
        audioManager.Go2LabBasement();

        settingsPage.SetActive(false);
        settingsLevel.transform.position = new Vector2(0, -10.8f);
        go2Settings = true;
        shopC.Go2Shop();
    }

    public void ComeBackFromSettingsPage()
    {
        audioManager.ComeBackFromBasement();

        go2Settings = false;
        comeBackFromSettings = true;
    }

    public void ComeBackFromShop()
    {
        audioManager.ComeBackFromBasement();

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
