using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LvlMaker : MonoBehaviour {
    public GameObject pathCirlce;
    public GameObject square;
    public GameObject circle;
    public GameObject triangle;
    GameObject activeObject;
    GameObject renameObstacle;
    GameObject TargetTravelPath;
    public GameObject blankCanvas;
    SpriteRenderer sr;
    Vector3 point;
    Vector3 point2;

    private void Start()
    {
        activeObject = null;
        renameObstacle = new GameObject("renameObstacle");
        renameObstacle.transform.parent = this.transform;
        TargetTravelPath = new GameObject("TargetTravelPath");
        TargetTravelPath.transform.parent = renameObstacle.transform;
        blankCanvas.GetComponent<SpriteRenderer>().sortingLayerName = "Game";
        blankCanvas.GetComponent<SpriteRenderer>().sortingOrder = 0;
        Instantiate(blankCanvas,Vector3.zero,Quaternion.identity,this.transform);
    }

    void Update () {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            activeObject = pathCirlce;
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

        if (Input.GetMouseButtonDown(0))
        {
            point = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            point2 = new Vector3(point.x, point.y, 0);
            if (activeObject == pathCirlce)
            {
                Instantiate(activeObject, point2, Quaternion.identity, TargetTravelPath.transform);
                sr = activeObject.GetComponent<SpriteRenderer>();
                sr.sortingLayerName = "Game";
                sr.sortingOrder = 1;
            }
            else
            {
                Instantiate(activeObject,point2,Quaternion.identity,renameObstacle.transform);
                activeObject.GetComponent<SpriteRenderer>().sortingLayerName = "Game";
                sr = activeObject.GetComponent<SpriteRenderer>();
                sr.sortingLayerName = "Game";
                sr.sortingOrder = 2;
            }
        }
	}
}
