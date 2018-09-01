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

    public static ObstacleSpawner Instance;

    GameObject plusOneLab;
    GameObject deadeyeLab;
    GameObject clairvoyanceLab;
    OtherGameModesManager.gameMode currentGameMode;
    GameMode_Plus1 PlusOneGameModeC;
    GameMode_Deadeye DeadeyeGameModeC;
    GameMode_Clairvoyance ClairvoyanceGameModeC;

    bool dontMoveWalls = false;
    float distanceDiff4Walls;
    float thisDeviceCameraRadius;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }


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

    void Start () {
		
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
                break;

            case OtherGameModesManager.gameMode.Deadeye:
                currentGameMode = OtherGameModesManager.gameMode.Deadeye;

                PlusOneGameModeC.enabled = false;
                DeadeyeGameModeC.enabled = true;
                ClairvoyanceGameModeC.enabled = false;
                break;

            case OtherGameModesManager.gameMode.Clairvoyance:
                currentGameMode = OtherGameModesManager.gameMode.Clairvoyance;

                PlusOneGameModeC.enabled = false;
                DeadeyeGameModeC.enabled = false;
                ClairvoyanceGameModeC.enabled = true;
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
}
