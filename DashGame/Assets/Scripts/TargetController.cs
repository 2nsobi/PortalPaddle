using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetController : MonoBehaviour
{
    public GameObject spawnArea;
    RectTransform spawnAreaRect;
    Vector3[] spawnAreaCorners = new Vector3[4];
    CircleCollider2D collider; //used to account for the offset needed for the radius of the target
    public GameObject TargetPrefab;
    BallController ballC;
    LevelGenerator LG;
    GameManager game;
    float randomSize;
    Vector3 defaultTargetSize = new Vector3(0.23f, 0.23f, 1);
    Vector3 troubleshootingSize = new Vector3(0.7f, 0.7f, 1);
    public float travelSpeed;
    float codeTravelSpeed;
    float tempSpeed;
    Transform nextLvl;
    public float growShrinkSpeed;
    float smallestTargestSize = 0.03f; //smallest a target will get when it grows and shrinks
    Vector3[] nextObstaclePath;
    float targetRadius = 2.53f; //based on 2d circle collider radius
    float targetSpawnOffset = 0.667f;
    static CryptoRandom rng = new CryptoRandom();
    List<Target> targets;
    GameObject targetP;
    Target targetComponent;
    int nextLvlNum;

    public static TargetController Instance;

    private void Awake()
    {
        Instance = this;

        codeTravelSpeed = travelSpeed;

        collider = TargetPrefab.GetComponent<CircleCollider2D>();

        spawnAreaRect = spawnArea.transform as RectTransform;
        spawnAreaRect.GetWorldCorners(spawnAreaCorners);

        targets = new List<Target>();

        SpawnTargets(3, Color.yellow);
    }

    private void Start()
    {
        LG = LevelGenerator.Instance;
        game = GameManager.Instance;
        ballC = BallController.Instance;
    }

    public void SpawnTargets(int size, Color color)
    {
        int i = targets.Count;
        while (targets.Count < size)
        {
            targetP = Instantiate(TargetPrefab);
            targetP.SetActive(false);
            targetP.name = "target" + i;
            i++;

            targetComponent = targetP.GetComponent<Target>();
            targetComponent.SetColor(color);

            targets.Add(targetComponent);
        }
    }

    private void OnEnable()
    {
        GameManager.GameStarted += GameStarted;
        LevelGenerator.NextLvlGenerated += NextLvlGenerated;
    }

    private void OnDisable()
    {
        GameManager.GameStarted -= GameStarted;
        LevelGenerator.NextLvlGenerated -= NextLvlGenerated;
    }

    public void IncreaseTravelSpeed(float speed) // initial travel speed started at 2
    {
        codeTravelSpeed = speed;
    }

    void NextLvlGenerated()
    {
        nextLvl = LG.GetNextLvl;
        nextObstaclePath = LG.GetNextObstaclePath;
        tempSpeed = codeTravelSpeed;
        nextLvlNum = LG.GetNextLvlNumber;

        for (int i = 0; i < targets.Count; i++)
        {
            if (!targets[i].InUse)
            {
                if (nextLvlNum == 1 || nextLvlNum == 2)
                {
                    if (nextObstaclePath != null)
                    {
                        targets[i].gameObject.SetActive(true);
                        targets[i].Spawn(nextLvl, RandomPos(), defaultTargetSize, true, false, nextObstaclePath, tempSpeed);
                    }
                    else
                    {
                        targets[i].gameObject.SetActive(true);
                        targets[i].Spawn(nextLvl, RandomPos(), defaultTargetSize, false, false);
                    }
                }

                if (nextLvlNum == 3)
                {
                    int randomNumber = Random.Range(1, 11);
                    if (randomNumber % 2 == 0)
                    {
                        if (nextObstaclePath != null)
                        {
                            targets[i].gameObject.SetActive(true);
                            targets[i].Spawn(nextLvl, RandomPos(), defaultTargetSize, true, true, nextObstaclePath, tempSpeed, growShrinkSpeed);
                        }
                        else
                        {
                            targets[i].gameObject.SetActive(true);
                            targets[i].Spawn(nextLvl, RandomPos(), defaultTargetSize, false, true, nextObstaclePath, tempSpeed, growShrinkSpeed);
                        }
                    }
                    else
                    {
                        if (nextObstaclePath != null)
                        {
                            targets[i].gameObject.SetActive(true);
                            targets[i].Spawn(nextLvl, RandomPos(), defaultTargetSize, true, false, nextObstaclePath, tempSpeed);
                        }
                        else
                        {
                            targets[i].gameObject.SetActive(true);
                            targets[i].Spawn(nextLvl, RandomPos(), defaultTargetSize, false, false, nextObstaclePath);
                        }
                    }
                }

                if (nextLvlNum >= 4)
                {
                    if (nextObstaclePath != null)
                    {
                        targets[i].gameObject.SetActive(true);
                        targets[i].Spawn(nextLvl, RandomPos(), defaultTargetSize, true, true, nextObstaclePath, tempSpeed, growShrinkSpeed);
                    }
                    else
                    {
                        targets[i].gameObject.SetActive(true);
                        targets[i].Spawn(nextLvl, RandomPos(), defaultTargetSize, false, true, nextObstaclePath, tempSpeed, growShrinkSpeed);
                    }
                }
                break;
            }
        }
    }

    public void SpawnTarget(Transform obTransform, bool travel, Vector3[] obPath = null) // used for other game modes mainly
    {
        tempSpeed = codeTravelSpeed;

        for (int i = 0; i < targets.Count; i++)
        {
            if (!targets[i].InUse)
            {
                targets[i].gameObject.SetActive(true);
                targets[i].Spawn(obTransform, RandomPos(), defaultTargetSize, travel, false, obPath, tempSpeed);
                break;
            }
        }
    }

    public void ResetTargets()
    {
        codeTravelSpeed = travelSpeed;

        for (int i = 0; i < targets.Count; i++)
        {
            if (targets[i].isActiveAndEnabled)
            {
                targets[i].StopUsing();
            }
        }
    }

    void GameStarted()
    {
        for (int i = 0; i < targets.Count; i++)
        {
            if (!targets[i].InUse)
            {
                targets[i].gameObject.SetActive(true);
                targets[i].Spawn(LG.GetCurrentLvl, RandomPos(), defaultTargetSize, false, false);
                break;
            }
        }
    }

    public Vector2 RandomPos()
    {
        return new Vector2(RandomFloat(spawnAreaCorners[0].x + (targetSpawnOffset), spawnAreaCorners[3].x - (targetSpawnOffset)), RandomFloat(spawnAreaCorners[0].y + (targetSpawnOffset), spawnAreaCorners[2].y - (targetSpawnOffset)));
    }

    public float RandomSpawnAreaXRange
    {
        get
        {
            return RandomFloat(spawnAreaCorners[0].x + (0.64f), spawnAreaCorners[3].x - (0.64f)); //float comes from measuring radius of ballspawner
        }
    }

    float RandomFloat(double min, double max)
    {
        return (float)(min + rng.NextDouble() * (max - min));
    }

    public Vector3 GetCurrentTargetPos(int targetIndex)
    {
        return targets[targetIndex].transform.position;
    }

    public float getTravelSpeed(int targetIndex)
    {
        return targets[targetIndex].TravelSpeed;
    }

    public bool IsMoving(int targetIndex)
    {
        return targets[targetIndex].Moving;
    }

    public void ShrinkTarget(int targetIndex)
    {
        targets[targetIndex].Shrink();
    }

    public void ShrinkTarget2(int targetIndex) //used for other game modes to make transitions less weird
    {
        targets[targetIndex].Shrink2();
    }
}
