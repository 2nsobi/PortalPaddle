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
        public bool hasObstacle;

        public LvlPrefab(GameObject go)
        {
            this.gameObject = go;
            hasObstacle = false;
        }
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
    public float finishTransitionSpeed;
    public float nextLvlThreshold; //number used to determine when the next lvl becomes teh current lvl
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
    LvlPrefab NextLvl, NextLvl4Transition;
    LvlPrefab CurrentLvl;
    LvlPrefab PreviousLvl;
    Obstacle NextObstacle;
    Obstacle CurrentObstacle;
    Obstacle PreviousObstacle;
    bool currentlyTransitioning;
    bool playedOnce = false; // ateast one game session has been started since opening app, used to get rid of NullReferenceException when GameOverConfirmed() is called after app is opened
    Vector3[] travelPath1;
    bool obstaclesShouldDespawn;
    int obstacleSpawnCounter;
    int levelSpawnCounter;
    float targetAspectRatio;
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
    static Queue<Obstacle> obstacleSpawnQ = new Queue<Obstacle>();
    int specialLvlCount = 0;
    static LvlPrefab activeLvl;
    LvlPrefab transitionLvl;
    LvlPrefab specialLvl;
    LvlPrefab defaultLvl;
    LvlPrefab gradientDespawner;
    static Obstacle activeObstacle;
    static string activeObstacleTexture;
    bool activeObstacleDifficulty;
    int currentLvlNumber;
    bool gradientSpawned;
    bool finishTransitioning;
    LvlPrefab tempCurrLvl;
    LvlPrefab tempNextLvl;
    static string activeLvlName;
    bool allPrefsInQHaveObstacle;//for spawnfrompool method
    bool dothis; //for spawnfrompool method
    public GameObject filters;
    Animator filtersAnimC;
    bool caves2SkyFilter, cavesFilter, removeCaves2SkyFilter, disableFilters;
    bool obstaclesShouldBSpawning =false;
    bool go2Settings = false;
    bool comeBackFromSettings;
    GameObject settingsLevel;
    Vector3 offset2 = new Vector3(0, 10.6f);

    public delegate void LevelDelegate();
    public static event LevelDelegate TransitionDone;
    public static event LevelDelegate NextLvlGenerated;

    private void OnEnable()
    {
        GameManager.GameStarted += GameStarted;
        EnemyBehavior.AbsorbDone += AbsorbDone;
        EnemyBehavior.AbsorbDoneAndRichochet += AbsorbDone;
    }

    private void OnDisable()
    {
        GameManager.GameStarted -= GameStarted;
        EnemyBehavior.AbsorbDone -= AbsorbDone;
        EnemyBehavior.AbsorbDoneAndRichochet -= AbsorbDone;
    }

    private void Start()
    {
        settingsLevel = Instantiate(SettingsLvl);
        settingsLevel.SetActive(false);

        disableFilters = true;
        removeCaves2SkyFilter = true;
        caves2SkyFilter = true;
        cavesFilter = true;
        filtersAnimC = filters.GetComponent<Animator>();
        obstacletextures = obstacleTextures;

        gradientSpawned = false;
        switchColors = true;
        targetAspectRatio = TargetAspectRatio.x / TargetAspectRatio.y;

        levelSpawnCounter = 0;
        obstacleSpawnCounter = 0;
        obstaclesShouldDespawn = false;

        game = GameManager.Instance;
        paddle = PaddleController.Instance;

        currentlyTransitioning = false;

        StartLevel = new LvlPrefab(Instantiate(StartLvl, transform));
        material = StartLevel.gameObject.GetComponentInChildren<Renderer>().sharedMaterial;
        material.SetFloat("_InvertColors", 0);
        Transform wallW = StartLevel.gameObject.transform.GetChild(0);
        Transform wallE = StartLevel.gameObject.transform.GetChild(1);
        wallW.localPosition = new Vector3(wallW.localPosition.x + paddle.GetDistanceDifferenceForWalls(), wallW.localPosition.y, 0);
        wallE.localPosition = new Vector3(wallE.localPosition.x - paddle.GetDistanceDifferenceForWalls(), wallE.localPosition.y, 0);
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
                    wallW0.localPosition = new Vector3(wallW0.localPosition.x + paddle.GetDistanceDifferenceForWalls(), wallW0.localPosition.y, 0);
                    wallE0.localPosition = new Vector3(wallE0.localPosition.x - paddle.GetDistanceDifferenceForWalls(), wallE0.localPosition.y, 0);
                    obj1.gameObject.SetActive(false);

                    objectPool.Enqueue(obj1);
                }
                lvlPrefabs.Add(objectPool);
            }
            LvlComponentDict.Add(level.tag, lvlPrefabs);

            transitionLvls[k] = new LvlPrefab(Instantiate(level.transitionLvl));
            Transform wallW1 = transitionLvls[k].gameObject.transform.GetChild(0);
            Transform wallE1 = transitionLvls[k].gameObject.transform.GetChild(1);
            wallW1.localPosition = new Vector3(wallW1.localPosition.x + paddle.GetDistanceDifferenceForWalls(), wallW1.localPosition.y, 0);
            wallE1.localPosition = new Vector3(wallE1.localPosition.x - paddle.GetDistanceDifferenceForWalls(), wallE1.localPosition.y, 0);
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
                    wallW0.localPosition = new Vector3(wallW0.localPosition.x + paddle.GetDistanceDifferenceForWalls(), wallW0.localPosition.y, 0);
                    wallE0.localPosition = new Vector3(wallE0.localPosition.x - paddle.GetDistanceDifferenceForWalls(), wallE0.localPosition.y, 0);
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
        game.SetPageState(GameManager.pageState.StartPage);
        obstacleSpawnCounter = 0;
        levelSpawnCounter = 0;
        obstaclesShouldDespawn = false;
        activeObstacle = null;
        activeLvl = null;
        activeLvlName = null;
        currentlySpawnedGradient = null;
        gradientDespawner = null;
        activeObstacleTexture = null;
        gradientSpawned = false;
        obstaclesShouldBSpawning = false;
        NextLvl4Transition = null;
        NextLvl = null;
        NextObstacle = null;

        if (playedOnce)
        {
            #region Disactivates All Level Prefabs

            lvlSpawnQ.Clear();
            obstacleSpawnQ.Clear();

            foreach (List<Queue<LvlPrefab>> level in LvlComponentDict.Values)
            {
                foreach (Queue<LvlPrefab> prefabs in level)
                {
                    foreach (LvlPrefab obj in prefabs)
                    {
                        obj.hasObstacle = false;
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

            NextObstacle = null;
            PreviousObstacle = null;

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
    }

    public LvlPrefab SpawnFromPool(string tag)
    {
        LvlPrefab lvlPrefab2Spawn = LvlComponentDict[tag][0].Dequeue();

        allPrefsInQHaveObstacle = true;
        dothis = true;
        if (lvlPrefab2Spawn.hasObstacle)
        {
            LvlComponentDict[tag][0].Enqueue(lvlPrefab2Spawn);
            while (dothis)
            {
                while (allPrefsInQHaveObstacle)
                {
                    for (int i = 0; i < LvlComponentDict[tag][0].Count; i++)
                    {
                        lvlPrefab2Spawn = LvlComponentDict[tag][0].Dequeue();
                        if (!lvlPrefab2Spawn.hasObstacle)
                        {
                            allPrefsInQHaveObstacle = false;
                            dothis = false;
                        }
                        LvlComponentDict[tag][0].Enqueue(lvlPrefab2Spawn);
                    }
                    if (!allPrefsInQHaveObstacle)
                    {
                        break;
                    }

                    FIFOList(LvlComponentDict[tag]);
                }
            }
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

    public Obstacle SpawnFromObstacles(string tag, Vector2 position, Quaternion rotation, LvlPrefab lvlPrefab, string texture, bool easy = false)
    {
        if (easy)
        {
            int num = rng.Next(1, ezObstacleCount + 1);

            Obstacle obstacleToSpawn = ObstacleDict["EasyObstacle" + num].Dequeue();

            obstacleToSpawn.gameObject.transform.position = position;
            obstacleToSpawn.gameObject.transform.rotation = rotation;
            obstacleToSpawn.SetObstacleTextures(texture);
            obstacleToSpawn.gameObject.SetActive(true);
            obstacleToSpawn.transform.parent = lvlPrefab.gameObject.transform;

            lvlPrefab.hasObstacle = true;

            ObstacleDict["EasyObstacle" + num].Enqueue(obstacleToSpawn);

            return obstacleToSpawn;

        }
        else
        {
            Obstacle obstacleToSpawn = ObstacleDict[tag].Dequeue();

            obstacleToSpawn.gameObject.transform.position = position;
            obstacleToSpawn.gameObject.transform.rotation = rotation;
            obstacleToSpawn.SetObstacleTextures(texture);
            obstacleToSpawn.gameObject.SetActive(true);
            obstacleToSpawn.transform.parent = lvlPrefab.gameObject.transform;

            lvlPrefab.hasObstacle = true;

            ObstacleDict[tag].Enqueue(obstacleToSpawn);

            return obstacleToSpawn;
        }

    }

    void AbsorbDone()
    {
        NextLvl4Transition = NextLvl;
        StartCoroutine("transitionDelay");
    }

    IEnumerator transitionDelay()
    {
        yield return new WaitForSeconds(0.5f);
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
                CurrentLvl.gameObject.transform.position = Vector2.Lerp(CurrentLvl.gameObject.transform.position, this.transform.position + levelOffset, 4 * Time.deltaTime);
                settingsLevel.transform.position = Vector2.Lerp(settingsLevel.transform.position, Vector3.zero, 4 * Time.deltaTime);
            }
            else
            {
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
                CurrentLvl.gameObject.transform.position = Vector2.Lerp(CurrentLvl.gameObject.transform.position, Vector3.zero, 4 * Time.deltaTime);
                settingsLevel.transform.position = Vector2.Lerp(settingsLevel.transform.position, this.transform.position - offset2, 4 * Time.deltaTime);
            }
            else
            {
                CurrentLvl.gameObject.transform.position = Vector2.MoveTowards(CurrentLvl.gameObject.transform.position, Vector3.zero, 2 *  Time.deltaTime);
                settingsLevel.transform.position = Vector2.MoveTowards(settingsLevel.transform.position, this.transform.position - offset2, 2 * Time.deltaTime);
                if (CurrentLvl.gameObject.transform.position.y == 0)
                {
                    comeBackFromSettings = false;
                    game.SetPageState(GameManager.pageState.StartPage);
                }
            }
        }

        if (currentlyTransitioning)
        {
            CurrentLvl.gameObject.transform.position = Vector2.Lerp(CurrentLvl.gameObject.transform.position, this.transform.position - levelOffset, transitionSpeed * Time.deltaTime);
            NextLvl4Transition.gameObject.transform.position = Vector2.Lerp(NextLvl4Transition.gameObject.transform.position, Vector3.zero, transitionSpeed * Time.deltaTime);

            if (NextLvl4Transition.gameObject.transform.position.y <= nextLvlThreshold)
            {
                tempCurrLvl = CurrentLvl;
                tempNextLvl = NextLvl4Transition;

                CurrentLvl = NextLvl4Transition;
                TransitionDone();
                GenerateNextLvl();
                currentlyTransitioning = false;
                finishTransitioning = true;

                if (obstacleSpawnCounter == 2)
                {
                    obstaclesShouldDespawn = true;
                }

                if (obstacleSpawnCounter >= 1 && !obstaclesShouldDespawn)
                {
                    if (NextObstacle.transform.position.y <= 10.4)
                    {
                        CurrentObstacle = NextObstacle;
                    }
                }

                if (obstaclesShouldDespawn)
                {
                    PreviousObstacle = CurrentObstacle;
                    if (NextObstacle.transform.position.y <= 10.4)
                    {
                        CurrentObstacle = NextObstacle;
                    }
                }

                if (PreviousObstacle != null)
                {
                    if (PreviousObstacle.transform.position.y <= -10.4)
                    {
                        PreviousObstacle.transform.parent = null;
                        PreviousObstacle.gameObject.SetActive(false);
                    }
                }
            }
        }

        if (game.IsGameRunning)
        {
            if (finishTransitioning)
            {
                tempCurrLvl.gameObject.transform.position = Vector2.MoveTowards(tempCurrLvl.gameObject.transform.position, this.transform.position - levelOffset, finishTransitionSpeed * Time.deltaTime);
                tempNextLvl.gameObject.transform.position = Vector2.MoveTowards(tempNextLvl.gameObject.transform.position, Vector3.zero, finishTransitionSpeed * Time.deltaTime);

                if (tempNextLvl.gameObject.transform.position.y == 0)
                {
                    if (gradientDespawner != null)
                    {
                        if (NextLvl == gradientDespawner)
                        {
                            currentlySpawnedGradient.transform.parent = CurrentLvl.gameObject.transform;
                            StartCoroutine("SetGradient2Null");
                        }
                    }
                    tempCurrLvl.gameObject.SetActive(false);
                    tempCurrLvl.hasObstacle = false;
                    finishTransitioning = false;
                }
            }
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

    IEnumerator SetGradient2Null()
    {
        yield return new WaitForSeconds(3.5f);
        currentlySpawnedGradient = null;
        gradientDespawner = null;
        gradientSpawned = false;
    }

    void GameStarted()
    {
        ShufflePrefabsInLevels();
        GenerateNextLvl();

        playedOnce = true;
        disableFilters = true;
        removeCaves2SkyFilter = true;
        caves2SkyFilter = true;
        cavesFilter = true;

        filtersAnimC.ResetTrigger("gameOver");
    }

    void GenerateNextLvl()
    {
        bool threshold = true;

        while (threshold)
        {
            if (game.GetScore >= 0)
            {
                activeLvlName = "level1";
            }
            else
            {
                break;
            }

            if (game.GetScore >= 1)
            {
                if (levels[1].about2Spawn)
                {
                    transitionLvl = SpawnFromPool(0);
                    lvlSpawnQ.Enqueue(transitionLvl);

                    defaultLvl = SpawnFromPool("level2");
                    lvlSpawnQ.Enqueue(defaultLvl);

                    currentlySpawnedGradient = SpawnFromGradients(0);
                    currentlySpawnedGradient.transform.position = Vector3.zero;
                    currentlySpawnedGradient.SetActive(true);
                    currentlySpawnedGradient.transform.localPosition = Vector3.zero;
                    currentlySpawnedGradient.transform.parent = defaultLvl.gameObject.transform;

                    levels[1].about2Spawn = false;
                }
                activeLvlName = "level2";
            }
            else
            {
                break;
            }

            if (game.GetScore >= 2)
            {
                if (levels[2].about2Spawn)
                {
                    if (levels[2].comeBack2)
                    {
                        gradientDespawner = SpawnFromPool(1); //gradientdespawner is considered to be the transitionlvl in this case
                        lvlSpawnQ.Enqueue(gradientDespawner);

                        defaultLvl = SpawnFromPool("level3");
                        obstacleSpawnQ.Enqueue(SpawnFromObstacles("Obstacle" + rng.Next(1, 10), defaultLvl.gameObject.transform.position, defaultLvl.gameObject.transform.rotation, defaultLvl, levels[2].obstacleTexture));
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

            if (game.GetScore >= 3)
            {
                if (levels[3].about2Spawn)
                {
                    specialLvl = specialLvls[0];
                    obstacleSpawnQ.Enqueue(SpawnFromObstacles("Obstacle" + rng.Next(1, 10), specialLvl.gameObject.transform.position, specialLvl.gameObject.transform.rotation, specialLvl, levels[2].obstacleTexture));
                    lvlSpawnQ.Enqueue(specialLvl);

                    transitionLvl = SpawnFromPool(2);
                    obstacleSpawnQ.Enqueue(SpawnFromObstacles("Obstacle" + rng.Next(1, 10), transitionLvl.gameObject.transform.position, transitionLvl.gameObject.transform.rotation, transitionLvl, levels[3].obstacleTexture));
                    lvlSpawnQ.Enqueue(transitionLvl);

                    defaultLvl = SpawnFromPool("level4");
                    obstacleSpawnQ.Enqueue(SpawnFromObstacles("Obstacle" + rng.Next(1, 10), defaultLvl.gameObject.transform.position, defaultLvl.gameObject.transform.rotation, defaultLvl, levels[3].obstacleTexture));
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

            if (game.GetScore >= 4)
            {
                if (levels[2].about2Spawn)
                {
                    transitionLvl = SpawnFromPool(3);
                    obstacleSpawnQ.Enqueue(SpawnFromObstacles("Obstacle" + rng.Next(1, 10), transitionLvl.gameObject.transform.position, transitionLvl.gameObject.transform.rotation, transitionLvl, levels[3].obstacleTexture, true));
                    lvlSpawnQ.Enqueue(transitionLvl);

                    defaultLvl = SpawnFromPool("level3");
                    obstacleSpawnQ.Enqueue(SpawnFromObstacles("Obstacle" + rng.Next(1, 10), defaultLvl.gameObject.transform.position, defaultLvl.gameObject.transform.rotation, defaultLvl, levels[2].obstacleTexture));
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
        if (activeObstacleTexture != null)
        {
            activeObstacle = SpawnFromObstacles("Obstacle" + rng.Next(1, 10), activeLvl.gameObject.transform.position, activeLvl.gameObject.transform.rotation, activeLvl, activeObstacleTexture, activeObstacleDifficulty);
            obstacleSpawnQ.Enqueue(activeObstacle);
        }
        lvlSpawnQ.Enqueue(activeLvl);

        levelSpawnCounter++;
        NextLvl = lvlSpawnQ.Dequeue();

        if (NextLvl.gameObject.tag == "level1")
        {
            currentLvlNumber = 1;
        }
        else if (NextLvl.gameObject.tag == "level2")
        {
            currentLvlNumber = 2;
        }
        else if (NextLvl.gameObject.tag == "level3")
        {
            currentLvlNumber = 3;
        }
        else if (NextLvl.gameObject.tag == "level4")
        {
            currentLvlNumber = 4;
        }
        NextLvl.gameObject.SetActive(true);
        NextLvl.gameObject.transform.position = transform.position + levelOffset;
        NextLvl.gameObject.transform.rotation = transform.rotation;

        if (currentlySpawnedGradient != null)
        {
            if (currentlySpawnedGradient.transform.IsChildOf(NextLvl.gameObject.transform))
            {
                gradientSpawned = true;
            }
        }

        for (int i = 0; i < NextLvl.gameObject.transform.childCount; i++)
        {
            if (NextLvl.gameObject.transform.GetChild(i).tag == "obstacle")
            {
                obstacleSpawnCounter++;
                NextObstacle = obstacleSpawnQ.Dequeue();
                NextObstacle.gameObject.SetActive(true);
                break;
            }
        }

        if (NextLvl.hasObstacle)
        {
            obstaclesShouldBSpawning = true;
        }
        if (obstaclesShouldBSpawning && !NextLvl.hasObstacle)
        {
            Debug.LogError(NextLvl.gameObject.name + "does not have an obstacle!!!");
        }

        NextLvlGenerated();
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
            if (NextObstacle != null)
            { 
                return NextObstacle.path;
            }
            return null;
        }
    }

    public Vector3[] GetCurrentObstaclePath
    {
        get
        {
            return CurrentObstacle.path;
        }
    }

    public int GetCurrentLvlNumber
    {
        get
        {
            return currentLvlNumber;
        }
    }

    public void GoToSettingsPage()
    {
        settingsLevel.SetActive(true);
        settingsLevel.transform.position = new Vector2(0,-10.6f);
        go2Settings = true;
    }

    public void ComeBackFromSettingsPage()
    {
        comeBackFromSettings = true;
    }

    public bool PlayedOnce
    {
        get
        {
            return playedOnce;
        }
    }

}
