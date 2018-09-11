using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleSpawner : MonoBehaviour {

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
    }

    public GameObject PlusOneLab;
    public GameObject DeadeyeLab;
    public GameObject ClairvoyanceLab;

    public Vector2 targetAspectRatio;
    public List<ObstacleTexture> obstacleTextures;
    public static List<ObstacleTexture> obstacletextures;
    public List<SinglePool> obstacles;
    static Dictionary<string, Queue<Obstacle>> ObstacleDict; //for obstacle use mainly

    public static ObstacleSpawner Instance;

    public delegate void ObSpawnerDelegate();
    public static event ObSpawnerDelegate ObstacleSet;

    GameObject plusOneLab;
    GameObject deadeyeLab;
    GameObject clairvoyanceLab;
    OtherGameModesManager.gameMode currentGameMode;
    GameMode_Plus1 PlusOneGameModeC;
    GameMode_Deadeye DeadeyeGameModeC;
    GameMode_Clairvoyance ClairvoyanceGameModeC;
    static CryptoRandom rng = new CryptoRandom();
    Obstacle currentObstacle;
    Obstacle nextObstacle;
    Vector2 levelOffset;
    TargetController targetC;

    int ezObstacleCount = 0;
    bool dontMoveWalls = false;
    float distanceDiff4Walls;
    float thisDeviceCameraRadius;
    bool moveInObstacle = false;
    bool moveOutObstacle = false;
    int tempNum = 0;
    int tempNum2 = 0;
    string tag4Obstacles;
    bool allPrefsInQInUse = true; // for spawnfrom obstacles method
    string currentObTexture;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }

        levelOffset = new Vector2(0, Camera.main.orthographicSize+5.8f); //5.8 will always make sure the obstacle is fully off screen no matter the camera height / aspect ratio

        PlusOneGameModeC = GetComponent<GameMode_Plus1>();
        DeadeyeGameModeC = GetComponent<GameMode_Deadeye>();
        ClairvoyanceGameModeC = GetComponent<GameMode_Clairvoyance>();

        ConfigureCamera(); //called here before the tap area rect is configured

        distanceDiff4Walls = GetDistanceDifferenceForWalls();

        obstacletextures = obstacleTextures;

        plusOneLab = Instantiate(PlusOneLab);
        plusOneLab.SetActive(false);
        Transform wallW3 = plusOneLab.transform.Find("wallW");
        Transform wallE3 = plusOneLab.transform.Find("wallE");
        wallW3.localPosition = new Vector3(-distanceDiff4Walls, wallW3.localPosition.y, 0);
        wallE3.localPosition = new Vector3(distanceDiff4Walls, wallE3.localPosition.y, 0);

        deadeyeLab = Instantiate(DeadeyeLab);
        deadeyeLab.SetActive(false);
        Transform wallW4 = deadeyeLab.transform.Find("wallW");
        Transform wallE4 = deadeyeLab.transform.Find("wallE");
        wallW4.localPosition = new Vector3(-distanceDiff4Walls, wallW4.localPosition.y, 0);
        wallE4.localPosition = new Vector3(distanceDiff4Walls, wallE4.localPosition.y, 0);

        clairvoyanceLab = Instantiate(ClairvoyanceLab);
        clairvoyanceLab.SetActive(false);
        Transform wallW5 = clairvoyanceLab.transform.Find("wallW");
        Transform wallE5 = clairvoyanceLab.transform.Find("wallE");
        wallW5.localPosition = new Vector3(-distanceDiff4Walls, wallW5.localPosition.y, 0);
        wallE5.localPosition = new Vector3(distanceDiff4Walls, wallE5.localPosition.y, 0);
    }

    private void Start()
    {
        targetC = TargetController.Instance;

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
    }

    public float GetDistanceDifferenceForWalls() //width of a wall is a bout 0.116524, and this gives the east wall an X pos of 3.700936 when the target aspect ratio is 9:16
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

                currentObTexture = "lab_gold";
                break;

            case OtherGameModesManager.gameMode.Deadeye:
                currentGameMode = OtherGameModesManager.gameMode.Deadeye;

                PlusOneGameModeC.enabled = false;
                DeadeyeGameModeC.enabled = true;
                ClairvoyanceGameModeC.enabled = false;

                currentObTexture = "lab_darkRed";
                break;

            case OtherGameModesManager.gameMode.Clairvoyance:
                currentGameMode = OtherGameModesManager.gameMode.Clairvoyance;

                PlusOneGameModeC.enabled = false;
                DeadeyeGameModeC.enabled = false;
                ClairvoyanceGameModeC.enabled = true;

                currentObTexture = "lab_poptart";
                break;

            case OtherGameModesManager.gameMode.None:
                currentGameMode = OtherGameModesManager.gameMode.None;

                PlusOneGameModeC.enabled = false;
                DeadeyeGameModeC.enabled = false;
                ClairvoyanceGameModeC.enabled = false;
                break;
        }
    }


    public void SetGameModeBackground()
    {
        print(currentGameMode);
        if(currentGameMode == OtherGameModesManager.gameMode.PlusOne)
        {
            plusOneLab.SetActive(true);
            plusOneLab.transform.position = Vector2.zero;
        }
        if (currentGameMode == OtherGameModesManager.gameMode.Deadeye)
        {
            deadeyeLab.SetActive(true);
            deadeyeLab.transform.position = Vector2.zero;
        }
        if (currentGameMode == OtherGameModesManager.gameMode.Clairvoyance)
        {
            clairvoyanceLab.SetActive(true);
            clairvoyanceLab.transform.position = Vector2.zero;
        }
    }

    public void SpawnObstacle()
    {
        nextObstacle = SpawnFromObstacles(1, obstacles.Count, -levelOffset, Quaternion.identity, currentObTexture);

        nextObstacle.gameObject.SetActive(true);

        moveInObstacle = true;
    }

    public void DespawnObstacle()
    {
        moveOutObstacle = true;
    }

    private void FixedUpdate()
    {
        if (moveInObstacle)
        {
            if (nextObstacle.transform.position.y < -0.8f)
            {

                nextObstacle.transform.position = Vector2.Lerp(nextObstacle.transform.position, Vector2.zero, Time.deltaTime * 4);
            }
            else
            {
                nextObstacle.transform.position = Vector2.MoveTowards(nextObstacle.transform.position, Vector2.zero, Time.deltaTime * 2);

                if (nextObstacle.transform.position.y == 0)
                {
                    moveInObstacle = false;

                    currentObstacle = nextObstacle;

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

                SpawnObstacle();
            }
        }
    }

    public Obstacle SpawnFromObstacles(int minObstacle, int maxObstacle, Vector2 position, Quaternion rotation, string texture, bool easy = false)
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

                            targetC.SpawnTarget(obstacleToSpawn.transform,true,obstacleToSpawn.path);

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

            targetC.SpawnTarget(obstacleToSpawn.transform, true, obstacleToSpawn.path);

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

                            targetC.SpawnTarget(obstacleToSpawn.transform, true, obstacleToSpawn.path);

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

            targetC.SpawnTarget(obstacleToSpawn.transform, true, obstacleToSpawn.path);

            obstacleToSpawn.SetObstacleTextures(texture);

            obstacleToSpawn.transform.position = position;
            obstacleToSpawn.transform.rotation = rotation;

            return obstacleToSpawn;
        }

    }
}
