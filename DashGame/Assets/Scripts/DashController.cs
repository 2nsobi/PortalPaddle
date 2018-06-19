using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class DashController : MonoBehaviour
{
    Rigidbody2D rigidbody;
    public float dashSpeed;
    public float dashTime; //dash time for inspector use
    float codeDashTime; //dash time for code use
    bool dashing;
    bool startedDash;
    bool stoppedDash;
    Vector2 direction;
    Vector3 startPosition;
    Vector3 endPosition;
    public LineRenderer PlayerTrailPrefab;
    float distance;
    List<Vector2> midPoints;
    List<Vector2> midPointsFinished; //final list of midpoints after for loop
    public int colliderCount;
    public static DashController Instance;

    public delegate void DashDelegate();
    public static event DashDelegate FinishedDash;

    private void Awake()
    {
        PlayerTrailEffect();
    }

    private void Start()
    {
        Instance = this;
        rigidbody = GetComponent<Rigidbody2D>();
        codeDashTime = dashTime;
        dashing = false;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            codeDashTime = dashTime;
            //need to subtract the current postion from the target postion
            direction = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - this.transform.localPosition);
            startPosition = this.transform.localPosition; // used to see the distance player dashes
            dashing = true;
            startedDash = true;
            if (stoppedDash)
            {
                midPoints.Clear();
                midPointsFinished.Clear();
                stoppedDash = false;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        rigidbody.velocity = Vector2.zero;
    }

    private void FixedUpdate()
    {
        if (dashing)
        {
            Dash();
        }
    }

    void Dash()
    {
        if (codeDashTime > 0)
        {
            rigidbody.velocity = direction.normalized * dashSpeed;
            codeDashTime -= Time.deltaTime;
        }
        else
        {
            //Debug.Log(rigidbody.velocity.magnitude);// used to see the velocity the player travels at
           // Debug.Log(direction.normalized.magnitude);
            rigidbody.velocity = Vector2.zero;
            endPosition = this.transform.localPosition; // used to see the distance player dashes
            distance = Vector2.Distance(startPosition, endPosition);// used to see the distance player dashes
           // Debug.Log(distance); // used to see the distance player dashes
            stoppedDash = true;

            CollectMidPoints();
            FinishedDash();

            dashing = false;
        }
    }

    void CollectMidPoints()
    {
        float xDiff = endPosition.x - startPosition.x;
        float yDiff = endPosition.y - startPosition.y;
        float xInterval = xDiff / (colliderCount + 1);
        float yInterval = yDiff / (colliderCount + 1);

        midPoints = new List<Vector2>();
        for (int i = 1; i <= colliderCount; i++)
        {
            midPoints.Add(new Vector2(startPosition.x + xInterval * i, startPosition.y + yInterval * i));
        }
        midPointsFinished = midPoints;
    }

    void PlayerTrailEffect()
    {
        Instantiate(PlayerTrailPrefab, this.transform.localPosition, this.transform.rotation);
    }

    public Vector2 GetDirection
    {
        get
        {
            return direction;
        }
    }

    public Vector3 GetStartPosition
    {
        get
        {
            return startPosition;
        }
    }
    public Vector3 GetEndPosition
    {
        get
        {
            return endPosition;
        }
    }

    public bool StartedDashing
    {
        get
        {
            return startedDash;
        }
    }

    public bool StoppedDashing
    {
        get
        {
            return stoppedDash;
        }
    }

    public Vector2 GetCurrentPostion
    {
        get
        {
            return rigidbody.position;
        }
    }

    public List<Vector2> GetMidPoints
    {
        get
        {
            return midPointsFinished;
        }
    }
}
