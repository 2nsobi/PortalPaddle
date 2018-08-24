using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LvlMaker : MonoBehaviour
{
    public GameObject pathCircle;
    public GameObject square;
    public GameObject circle;
    public GameObject triangle;
    public GameObject wall;
    public GameObject customSprite;
    public GameObject ReferenceTarget;
    GameObject refTarg;
    GameObject circlePath;
    GameObject activeObject;
    GameObject renameObstacle;
    GameObject TargetTravelPath;
    public GameObject blankCanvas;
    SpriteRenderer sr, sr2;
    Vector3 point;
    Vector3 point2;
    public float circlePathRadius;
    public int NumPointsOnCirclePath;
    bool currentlySettingCirclePath;
    GameObject cp;
    public float TestTravelSpeed;
    bool testPath;
    Vector3[] path4Travel;
    Vector3 nextPoint;
    int pointCounter;

    private void Start()
    {
        activeObject = null;
        renameObstacle = new GameObject("renameObstacle");
        renameObstacle.transform.parent = this.transform;
        TargetTravelPath = new GameObject("TargetTravelPath");
        TargetTravelPath.transform.parent = renameObstacle.transform;
        blankCanvas.GetComponent<SpriteRenderer>().sortingLayerName = "Game";
        blankCanvas.GetComponent<SpriteRenderer>().sortingOrder = 1;
        Instantiate(blankCanvas, Vector3.zero, Quaternion.identity, this.transform);
        refTarg = Instantiate(ReferenceTarget, this.transform);
        circlePath = new GameObject();
        currentlySettingCirclePath = false;
        testPath = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            activeObject = pathCircle;
            Debug.Log("activeObject = " + activeObject.name);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            activeObject = square;
            Debug.Log("activeObject = " + activeObject.name);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            activeObject = circle;
            Debug.Log("activeObject = " + activeObject.name);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            activeObject = triangle;
            Debug.Log("activeObject = " + activeObject.name);
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            activeObject = circlePath;
            Debug.Log("activeObject = Circle Path\nClick again after spawning circle path to set it in its current pos");
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            activeObject = wall;
            Debug.Log("activeObject = " + activeObject.name);
        }
        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            activeObject = customSprite;
            try
            {
                Debug.Log("activeObject = " + activeObject.name);
            }
            catch (UnassignedReferenceException e)
            {
                Debug.LogError("You have not to set a custom sprite to use");
                activeObject = null;
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            testPath = !testPath;
            Debug.Log("Your path " + (testPath ? "is " : " has stopped ") + "being tested.");
            pointCounter = 1;

            try
            {
                path4Travel = getPath();
                refTarg.transform.localPosition = path4Travel[0];
            }
            catch (System.IndexOutOfRangeException e)
            {
                Debug.LogError("Nevermind. You have not made a path to test. " + e.Message);
                testPath = false;
            }
            catch (System.NullReferenceException e)
            {
                testPath = false;
                Debug.Log("Nevermind. You have to set the circle path before you test it. Click again to set path.");
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            ReverseLinearPath();
        }
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            activeObject = refTarg;
            Debug.Log("activeObject = " + activeObject.name);
        }

        if (testPath)
        {
            TestPath();
        }

        if (Input.GetMouseButtonDown(0))
        {
            point = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            point2 = new Vector3(point.x, point.y, 0);
            if (activeObject == pathCircle)
            {
                Instantiate(activeObject, point2, Quaternion.identity, TargetTravelPath.transform);
                sr = activeObject.GetComponent<SpriteRenderer>();
                sr.sortingLayerName = "Game";
                sr.sortingOrder = 3;

                refTarg.transform.localPosition = point2;
            }

            if (activeObject == circlePath)
            {
                currentlySettingCirclePath = !currentlySettingCirclePath;

                if (currentlySettingCirclePath)
                {
                    Destroy(GameObject.Find("TargetTravelPath"));
                    cp = MakeCirclePath();
                    cp.transform.localPosition = point2;
                }
                else
                {
                    Transform[] children = cp.GetComponentsInChildren<Transform>();

                    foreach (Transform child in children)
                    {
                        child.parent = this.transform;
                    }

                    cp.transform.parent = renameObstacle.transform;
                    cp.transform.localPosition = Vector3.zero;

                    foreach (Transform child in children)
                    {
                        child.parent = cp.transform;
                    }

                }
            }

            if (activeObject == refTarg)
            {
                refTarg.transform.localPosition = point2;
            }

            if (activeObject != pathCircle && activeObject != circlePath && activeObject != refTarg)
            {
                try
                {
                    Instantiate(activeObject, point2, Quaternion.identity, renameObstacle.transform);
                }
                catch (System.ArgumentException e)
                {
                    Debug.LogError(e.Message + " Please select a sprite to spawn with the number keys.");
                    return;
                }
                catch (UnassignedReferenceException e)
                {
                    Debug.LogError(e.Message + " Please select a sprite to spawn with the number keys.");
                    return;
                }
                sr = activeObject.GetComponent<SpriteRenderer>();
                sr.sortingLayerName = "Game";
                sr.sortingOrder = 2;
            }
        }
    }

    GameObject MakeCirclePath()
    {
        GameObject circlePath = new GameObject("TargetTravelPath");
        GameObject circle;

        float angle = 360 / NumPointsOnCirclePath;

        Vector2[] pointsOnCircle = new Vector2[NumPointsOnCirclePath];

        for (int i = 0; i < NumPointsOnCirclePath; i++)
        {
            pointsOnCircle[i] = new Vector2(circlePathRadius * Mathf.Cos(Mathf.Deg2Rad * angle * i), circlePathRadius * Mathf.Sin(Mathf.Deg2Rad * angle * i));
        }

        for (int i = 0; i < NumPointsOnCirclePath; i++)
        {
            circle = Instantiate(pathCircle, pointsOnCircle[i], Quaternion.identity, circlePath.transform);
            sr2 = circle.GetComponent<SpriteRenderer>();
            sr2.sortingLayerName = "Game";
            sr2.sortingOrder = 3;
        }

        return circlePath;
    }

    void TestPath()
    {
        nextPoint = PointOnPath(path4Travel, pointCounter);
        refTarg.transform.localPosition = Vector2.MoveTowards(refTarg.transform.localPosition, nextPoint, Time.deltaTime * TestTravelSpeed);
        if (refTarg.transform.localPosition == nextPoint)
        {
            pointCounter += 1;
            if (pointCounter > path4Travel.Length - 1)
            {
                pointCounter = 0;
            }
        }
    }

    Vector3[] getPath()
    {

        Transform t = renameObstacle.transform.Find("TargetTravelPath");
        Vector3[] path = new Vector3[t.childCount];
        for (int i = 0; i < t.childCount; i++)
        {
            path[i] = t.GetChild(i).localPosition;
        }

        return path;
    }

    Vector3 PointOnPath(Vector3[] path, int iterator)
    {
        try
        {
            return path[iterator];
        }
        catch (System.IndexOutOfRangeException)
        {
            testPath = false;
            Debug.Log("Nevermind. You have to set the circle path before you test it. Click again to set path.");
        }
        catch (System.NullReferenceException)
        {
            testPath = false;
            Debug.Log("Nevermind. You have to set the circle path before you test it. Click again to set path.");
        }
        return Vector3.zero;
    }

    void ReverseLinearPath()
    {
        Transform t = renameObstacle.transform.Find("TargetTravelPath");
        for (int i = t.childCount - 1 ; i >= 0 ; i--)
        {
            Instantiate(t.GetChild(i),t.transform);
        }
    }
}
