using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{

    public class Obstacle
    {
        public GameObject gameObject;
        public Transform transform;
        public Vector3[] path;
        public string obstacleTexture;
        SpriteRenderer[] obstacleSprites;
        public bool inUse;

        public Obstacle(GameObject go)
        {
            inUse = false;
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

    [System.Serializable]
    public class SinglePool
    {
        public GameObject prefab;
        public int size;
        public bool easy; //is the obstacle easy or not
        public bool onlyMotion; //obstacles with no actual colliders only moves target
    }

    public GameObject PlusOneLab;
    public GameObject DeadeyeLab;
    public GameObject ClairvoyanceLab;
    public Transform GameOverZoneN;
    public Transform GameOverZoneS;

    public Vector2 targetAspectRatio;
    public List<ObstacleTexture> obstacleTextures;
    public static List<ObstacleTexture> obstacletextures;
    public List<SinglePool> obstacles;
    static Dictionary<string, Queue<Obstacle>> ObstacleDict; //for obstacle use mainly

    public delegate void ObSpawnerDelegate();
    public static event ObSpawnerDelegate ObstacleSet;
    public static event ObSpawnerDelegate ObstacleGone;

    GameObject plusOneLab;
    GameObject deadeyeLab;
    GameObject clairvoyanceLab;
    OtherGameModesManager.gameMode currentGameMode;
    GameMode_Plus1 PlusOneGameModeC;
    GameMode_Deadeye DeadeyeGameModeC;
    GameMode_Clairvoyance ClairvoyanceGameModeC;
    AudioManager audioManager;
    static CryptoRandom rng = new CryptoRandom();
    Obstacle currentObstacle;
    Obstacle nextObstacle;
    Vector2 levelOffset;
    TargetController targetC;
    GameObject currentLab;
    Vector2 wallOffScreenPosW;
    Vector2 wallOffScreenPosE;
    Vector2 wallOnScreenPosW;
    Vector2 wallOnScreenPosE;
    Transform currentWallW;
    Transform currentWallE;
    GameObject deadeyeBRBackground;
    GameObject deadeyeBWBackground;

    Transform wallW1;
    Transform wallE1;
    Transform wallW2;
    Transform wallE2;
    Transform wallW3;
    Transform wallE3;

    int ezObstacleCount = 0;
    int onlyMotionObCount = 0;
    bool dontMoveWalls = false;
    float distanceDiff4Walls;
    float thisDeviceCameraRadius;
    bool moveInObstacle = false;
    bool moveOutObstacle = false;
    int tempNum = 0;
    int tempNum2 = 0;
    int tempNum3 = 0;
    string tag4Obstacles;
    bool allPrefsInQInUse = true; // for spawnfrom obstacles method
    string currentObTexture;
    bool excludeObstacles;
    bool targetsGrowShrink = false;
    bool targetsAlwaysGrowShrink = false;
    bool continuous = false; //should obstacles be continuously spawned
    bool moveWalls = false;
    bool moveWallsOut = false;

    public static ObstacleSpawner Instance;

    private void Awake()
    {
        if (Instance == null)
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
        }

        PlusOneGameModeC = GetComponent<GameMode_Plus1>();
        DeadeyeGameModeC = GetComponent<GameMode_Deadeye>();
        ClairvoyanceGameModeC = GetComponent<GameMode_Clairvoyance>();

        ConfigureCamera(); //called here before the tap area rect is configured

        distanceDiff4Walls = GetDistanceDifferenceForWalls();

        obstacletextures = obstacleTextures;

        plusOneLab = Instantiate(PlusOneLab);
        plusOneLab.SetActive(false);
        wallW1 = plusOneLab.transform.Find("wallW");
        wallE1 = plusOneLab.transform.Find("wallE");
        wallW1.localPosition = new Vector3(-distanceDiff4Walls, 0, 0);
        wallE1.localPosition = new Vector3(distanceDiff4Walls, 0, 0);

        deadeyeLab = Instantiate(DeadeyeLab);
        deadeyeBRBackground = deadeyeLab.transform.Find("LabWallBR").gameObject;
        deadeyeBWBackground = deadeyeLab.transform.Find("LabWallBW").gameObject;
        deadeyeLab.SetActive(false);
        wallW2 = deadeyeLab.transform.Find("wallW");
        wallE2 = deadeyeLab.transform.Find("wallE");
        wallW2.localPosition = new Vector3(-distanceDiff4Walls, 0, 0);
        wallE2.localPosition = new Vector3(distanceDiff4Walls, 0, 0);

        clairvoyanceLab = Instantiate(ClairvoyanceLab);
        clairvoyanceLab.SetActive(false);
        wallW3 = clairvoyanceLab.transform.Find("wallW");
        wallE3 = clairvoyanceLab.transform.Find("wallE");
        wallW3.localPosition = new Vector3(-distanceDiff4Walls, 0, 0);
        wallE3.localPosition = new Vector3(distanceDiff4Walls, 0, 0);

        wallOnScreenPosW = wallW3.localPosition;
        wallOnScreenPosE = wallE3.localPosition;

        wallOffScreenPosW = new Vector2(-distanceDiff4Walls, GameOverZoneS.position.y - 11.757f);
        wallOffScreenPosE = new Vector2(distanceDiff4Walls, GameOverZoneS.position.y - 11.757f);
    }

    private void Start()
    {
        targetC = TargetController.Instance;
        audioManager = AudioManager.Instance;

        ObstacleDict = new Dictionary<string, Queue<Obstacle>>();

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

            if (obstacleType.onlyMotion)
            {
                Queue<Obstacle> onlyMotionObstaclePool = new Queue<Obstacle>();

                for (int i = 0; i < obstacleType.size; i++)
                {
                    GameObject go = Instantiate(obstacleType.prefab);
                    go.name = obstacleType.prefab.name + "_easy";

                    Obstacle ob1 = new Obstacle(go);

                    ob1.gameObject.SetActive(false);
                    onlyMotionObstaclePool.Enqueue(ob1);
                }
                onlyMotionObCount++;
                ObstacleDict.Add("OnlyMotionObstacle" + ezObstacleCount, onlyMotionObstaclePool);
            }

            for (int i = 0; i < obstacleType.size; i++)
            {
                Obstacle ob = new Obstacle(Instantiate(obstacleType.prefab));

                ob.gameObject.SetActive(false);
                obstaclePool.Enqueue(ob);
            }

            ObstacleDict.Add(obstacleType.prefab.name, obstaclePool);
        }
    }

    public void ConfigureCamera()
    {
        thisDeviceCameraRadius = (Camera.main.aspect * Camera.main.orthographicSize);
        float desiredCameraWidth = (targetAspectRatio.x / targetAspectRatio.y) * Camera.main.orthographicSize;

        if (thisDeviceCameraRadius < desiredCameraWidth - 0.001f) //for some reason (targetAspectRatio.x / targetAspectRatio.y) * Camera.main.orthographicSize does not equal what is should exactly
        {
            Camera.main.orthographicSize = desiredCameraWidth / Camera.main.aspect;
            dontMoveWalls = true;
        }

        levelOffset = new Vector2(0, Camera.main.orthographicSize + 5.8f + 0.5f); //5.8 will always make sure the obstacle is fully off screen no matter the camera height / aspect ratio

        GameOverZoneN.position = new Vector2(0, Camera.main.orthographicSize + 1.725946f);
        GameOverZoneS.position = new Vector2(0, -(Camera.main.orthographicSize + 1.725946f));
    }

    public float GetDistanceDifferenceForWalls() //width of a wall on screen is a bout 0.116524, and this gives the east wall an X pos of 3.700936 when the target aspect ratio is 9:16
    {
        if (dontMoveWalls)
        {
            return 3.700936f; // x pos of wall at aspect ratio of 3.700936
        }
        return thisDeviceCameraRadius + 0.888436f; // 0.888436 is the diff between the x pos of a wall at x pos 3.700936 and the camera width of a 9:16 aspect ratio
    }

    public void SetGameMode(OtherGameModesManager.gameMode gameMode)
    {
        switch (gameMode)
        {
            case OtherGameModesManager.gameMode.PlusOne:
                currentGameMode = OtherGameModesManager.gameMode.PlusOne;

                PlusOneGameModeC.enabled = true;
                DeadeyeGameModeC.enabled = false;
                ClairvoyanceGameModeC.enabled = false;

                currentWallW = wallW1;
                currentWallE = wallE1;
                currentObTexture = "lab_gold";
                currentLab = plusOneLab;
                excludeObstacles = true;
                break;

            case OtherGameModesManager.gameMode.Deadeye:
                currentGameMode = OtherGameModesManager.gameMode.Deadeye;

                PlusOneGameModeC.enabled = false;
                DeadeyeGameModeC.enabled = true;
                ClairvoyanceGameModeC.enabled = false;

                currentWallW = wallW2;
                currentWallE = wallE2;
                currentObTexture = "lab_darkRed";
                currentLab = deadeyeLab;
                excludeObstacles = false;
                break;

            case OtherGameModesManager.gameMode.Clairvoyance:
                currentGameMode = OtherGameModesManager.gameMode.Clairvoyance;

                PlusOneGameModeC.enabled = false;
                DeadeyeGameModeC.enabled = false;
                ClairvoyanceGameModeC.enabled = true;

                currentWallW = wallW3;
                currentWallE = wallE3;
                currentObTexture = "lab_poptart";
                currentLab = clairvoyanceLab;
                excludeObstacles = false;
                break;

            case OtherGameModesManager.gameMode.None:
                currentGameMode = OtherGameModesManager.gameMode.None;

                PlusOneGameModeC.enabled = false;
                DeadeyeGameModeC.enabled = false;
                ClairvoyanceGameModeC.enabled = false;

                targetsGrowShrink = false;
                targetsAlwaysGrowShrink = false;
                break;
        }
    }


    public void SetGameModeBackground()
    {
        currentLab.SetActive(true);
        currentLab.transform.position = Vector2.zero;

        if (currentGameMode == OtherGameModesManager.gameMode.None)
        {
            currentLab.SetActive(false);
        }
    }

    public void SpawnObstacle()
    {
        if (nextObstacle == null)
        {
            nextObstacle = SpawnFromObstacles(1, obstacles.Count, -levelOffset, Quaternion.identity, currentObTexture, targetsGrowShrink);
            nextObstacle.gameObject.SetActive(true);
        }

        moveInObstacle = true;
    }

    public void DespawnObstacle(bool continuous)
    {
        this.continuous = continuous;

        moveOutObstacle = true;
    }

    public void EndGame()
    {
        if (nextObstacle != null)
        {
            nextObstacle.gameObject.SetActive(false);
            nextObstacle = null;
        }

        if (currentObstacle != null)
        {
            currentObstacle.gameObject.SetActive(false);
            currentObstacle = null;
        }

        currentWallW.localPosition = wallOnScreenPosW;
        currentWallE.localPosition = wallOnScreenPosE;
    }

    private void FixedUpdate()
    {
        if (moveInObstacle)
        {
            if (nextObstacle.transform.position.y < -0.8f)
            {

                nextObstacle.transform.position = Vector2.Lerp(nextObstacle.transform.position, Vector2.zero, Time.deltaTime * 2);
            }
            else
            {
                nextObstacle.transform.position = Vector2.MoveTowards(nextObstacle.transform.position, Vector2.zero, Time.deltaTime * 2);

                if (nextObstacle.transform.position.y == 0)
                {
                    moveInObstacle = false;

                    currentObstacle = nextObstacle;

                    if (targetsGrowShrink)
                    {
                        if (targetsAlwaysGrowShrink)
                        {
                            nextObstacle = SpawnFromObstacles(1, obstacles.Count, -levelOffset, Quaternion.identity, currentObTexture, true);
                            nextObstacle.gameObject.SetActive(true);
                        }
                        else
                        {
                            int randomNumber = Random.Range(1, 11);
                            if (randomNumber % 2 == 0)
                            {
                                nextObstacle = SpawnFromObstacles(1, obstacles.Count, -levelOffset, Quaternion.identity, currentObTexture, true);
                                nextObstacle.gameObject.SetActive(true);

                            }
                            else
                            {
                                nextObstacle = SpawnFromObstacles(1, obstacles.Count, -levelOffset, Quaternion.identity, currentObTexture, false);
                                nextObstacle.gameObject.SetActive(true);
                            }
                        }
                    }
                    else
                    {
                        nextObstacle = SpawnFromObstacles(1, obstacles.Count, -levelOffset, Quaternion.identity, currentObTexture, false);
                        nextObstacle.gameObject.SetActive(true);                       
                    }

                    ObstacleSet();
                }
            }
        }

        if (moveOutObstacle)
        {
            currentObstacle.transform.position = Vector2.MoveTowards(currentObstacle.transform.position, -levelOffset, Time.deltaTime * 9);

            if (currentObstacle.transform.position.y == -(levelOffset.y))
            {
                moveOutObstacle = false;

                currentObstacle.gameObject.SetActive(false);

                if (continuous)
                {
                    SpawnObstacle();
                }
                else
                {
                    ObstacleGone();
                }
            }
        }

        if (moveWalls)
        {
            if (moveWallsOut)
            {
                currentWallW.localPosition = Vector2.MoveTowards(currentWallW.localPosition, wallOffScreenPosW, 5 * Time.deltaTime);
                currentWallE.localPosition = Vector2.MoveTowards(currentWallE.localPosition, wallOffScreenPosE, 5 * Time.deltaTime);

                if(currentWallW.localPosition.y == wallOffScreenPosW.y)
                {
                    moveWalls = false;
                }
            }
            else
            {
                currentWallW.localPosition = Vector2.MoveTowards(currentWallW.localPosition, wallOnScreenPosW, 5 * Time.deltaTime);
                currentWallE.localPosition = Vector2.MoveTowards(currentWallE.localPosition, wallOnScreenPosE, 5 * Time.deltaTime);

                if (currentWallW.localPosition.y == wallOnScreenPosW.y)
                {
                    moveWalls = false;
                }
            }
        }
    }

    public Obstacle SpawnFromObstacles(int minObstacle, int maxObstacle, Vector2 position, Quaternion rotation, string texture, 
        bool targetGrowShrink, bool easy = false, bool onlyMotion = false)
    {
        int number = rng.Next(minObstacle, maxObstacle + 1);

        if (excludeObstacles)
        {
            while (number == tempNum || number == 9 || (number == tempNum && number == 9))
            {
                number = rng.Next(minObstacle, maxObstacle + 1);
            }

            tempNum = number;
        }
        else
        {
            while (number == tempNum)
            {
                number = rng.Next(minObstacle, maxObstacle + 1);
            }

            tempNum = number;
        }

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

            if (obstacleToSpawn.inUse)
            {
                ObstacleDict[tag4Obstacles].Enqueue(obstacleToSpawn);

                while (allPrefsInQInUse)
                {
                    for (int i = 0; i < ObstacleDict[tag4Obstacles].Count; i++)
                    {
                        obstacleToSpawn = ObstacleDict[tag4Obstacles].Dequeue();
                        if (!obstacleToSpawn.inUse)
                        {
                            obstacleToSpawn.inUse = true;

                            targetC.SpawnTarget(obstacleToSpawn.transform, true, targetGrowShrink, obstacleToSpawn.path);

                            obstacleToSpawn.SetObstacleTextures(texture);

                            obstacleToSpawn.transform.position = position;
                            obstacleToSpawn.transform.rotation = rotation;

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
                ObstacleDict[tag4Obstacles].Enqueue(obstacleToSpawn);
            }

            obstacleToSpawn.inUse = true;

            targetC.SpawnTarget(obstacleToSpawn.transform, true, targetGrowShrink, obstacleToSpawn.path);

            obstacleToSpawn.SetObstacleTextures(texture);

            obstacleToSpawn.transform.position = position;
            obstacleToSpawn.transform.rotation = rotation;

            return obstacleToSpawn;

        }
        else if (onlyMotion)
        {
            int num = rng.Next(1, onlyMotionObCount + 1);

            while (num == tempNum3)
            {
                num = rng.Next(1, onlyMotionObCount + 1);
            }

            tempNum3 = num;

            tag4Obstacles = "OnlyMotionObstacle" + num;

            Obstacle obstacleToSpawn = ObstacleDict[tag4Obstacles].Dequeue();

            if (obstacleToSpawn.inUse)
            {
                ObstacleDict[tag4Obstacles].Enqueue(obstacleToSpawn);

                while (allPrefsInQInUse)
                {
                    for (int i = 0; i < ObstacleDict[tag4Obstacles].Count; i++)
                    {
                        obstacleToSpawn = ObstacleDict[tag4Obstacles].Dequeue();
                        if (!obstacleToSpawn.inUse)
                        {
                            obstacleToSpawn.inUse = true;

                            targetC.SpawnTarget(obstacleToSpawn.transform, true, targetGrowShrink, obstacleToSpawn.path);

                            obstacleToSpawn.SetObstacleTextures(texture);

                            obstacleToSpawn.transform.position = position;
                            obstacleToSpawn.transform.rotation = rotation;

                            ObstacleDict[tag4Obstacles].Enqueue(obstacleToSpawn);

                            return obstacleToSpawn;
                        }
                        ObstacleDict[tag4Obstacles].Enqueue(obstacleToSpawn);
                    }

                    int num2 = rng.Next(1, onlyMotionObCount + 1);
                    while (num2 == num)
                    {
                        num2 = rng.Next(1, onlyMotionObCount + 1);
                    }
                    num = num2;
                    tag4Obstacles = "OnlyMotionObstacle" + num2;
                }

            }
            else
            {
                ObstacleDict[tag4Obstacles].Enqueue(obstacleToSpawn);
            }

            obstacleToSpawn.inUse = true;

            targetC.SpawnTarget(obstacleToSpawn.transform, true, targetGrowShrink, obstacleToSpawn.path);

            obstacleToSpawn.SetObstacleTextures(texture);

            obstacleToSpawn.transform.position = position;
            obstacleToSpawn.transform.rotation = rotation;

            return obstacleToSpawn;
        }
        else
        {
            Obstacle obstacleToSpawn = ObstacleDict[tag4Obstacles].Dequeue();

            if (obstacleToSpawn.inUse)
            {
                ObstacleDict[tag4Obstacles].Enqueue(obstacleToSpawn);

                while (allPrefsInQInUse)
                {
                    for (int i = 0; i < ObstacleDict[tag4Obstacles].Count; i++)
                    {
                        obstacleToSpawn = ObstacleDict[tag4Obstacles].Dequeue();
                        if (!obstacleToSpawn.inUse)
                        {
                            obstacleToSpawn.inUse = true;

                            targetC.SpawnTarget(obstacleToSpawn.transform, true, targetGrowShrink, obstacleToSpawn.path);

                            obstacleToSpawn.SetObstacleTextures(texture);

                            obstacleToSpawn.transform.position = position;
                            obstacleToSpawn.transform.rotation = rotation;

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
                ObstacleDict[tag4Obstacles].Enqueue(obstacleToSpawn);
            }

            obstacleToSpawn.inUse = true;

            targetC.SpawnTarget(obstacleToSpawn.transform, true, targetGrowShrink, obstacleToSpawn.path);

            obstacleToSpawn.SetObstacleTextures(texture);

            obstacleToSpawn.transform.position = position;
            obstacleToSpawn.transform.rotation = rotation;

            return obstacleToSpawn;
        }

    }

    public void AllowTargets2GrowShrink(bool allow, bool always = false)
    {
        if (allow)
        {
            targetsGrowShrink = true;

            if (always)
            {
                targetsAlwaysGrowShrink = true;
            }
        }
        else
        {
            targetsGrowShrink = false;
            targetsAlwaysGrowShrink = false;
        }
    }

    public void MoveWalls(bool moveOut)
    {
        moveWalls = true;

        if (moveOut)
        {
            moveWallsOut = true;
        }
        else
        {
            moveWallsOut = false;
        }
    }

    public void InvertDeadeyeBackground(bool normal = false) //makes thebackground look more black and white when using black and white ball
    {
        if (normal)
        {
            deadeyeBRBackground.SetActive(true);
            deadeyeBWBackground.SetActive(false);
        }
        else
        {
            deadeyeBRBackground.SetActive(false);
            deadeyeBWBackground.SetActive(true);
        }
    }
}
