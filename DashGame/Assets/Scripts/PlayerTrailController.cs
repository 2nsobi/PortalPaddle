using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTrailController : MonoBehaviour {
    public PhysicsMaterial2D trailMaterial;
    LineRenderer LR;
    DashController DC;
    PlayerTrailerDashController PTDC;
    BoxCollider2D trailCollider;
    //public float delay;
    float codeDelay;
    public float startAlpha;
    public float endAlpha;
    float codeStartAlpha;
    float codeEndAlpha;
    public float startFadeSpeed;
    public float endFadeSpeed;
    Color start;
    Color end;
    float LRWidth;
    float LRLength;
    Vector2 midPoint;
    float angle;
    public float additionalOffset;
    public float delay;
    public float delayBetweenColliders;

    //collider[] colliders;
    BoxCollider2D[] boxColliders;

    /*
    class collider
    {
        public Transform transform;

        public collider(Transform t)
        {
            transform = t;
        }
    }
    */

    private void Start()
    {
        codeStartAlpha = startAlpha;
        codeEndAlpha = endAlpha;
        //codeDelay = delay;
        PTDC = PlayerTrailerDashController.Instance;
        DC = DashController.Instance;
        Physics2D.IgnoreLayerCollision(8, 11);
        Physics2D.IgnoreLayerCollision(8, 9);
        LR = GetComponent<LineRenderer>();
        LRWidth = LR.endWidth;

        MakeColliders();
    }

    private void OnEnable()
    {
        DashController.FinishedDash += MoveColliders;
    }

    private void OnDisable()
    {
        DashController.FinishedDash -= MoveColliders;
    }

    void MakeColliders()
    {
        /*
        trailCollider = new GameObject("TrailCollider").AddComponent<BoxCollider2D>();
        trailCollider.gameObject.layer = 9;
        trailCollider.transform.parent = LR.transform;
        LRWidth = LR.endWidth;
        */

        boxColliders = new BoxCollider2D[DC.colliderCount];
        for (int i = 0; i < boxColliders.Length; i++)
        {
            BoxCollider2D trailCollider = new GameObject("TrailCollider").AddComponent<BoxCollider2D>();
            trailCollider.gameObject.layer = 11;
            boxColliders[i] = trailCollider;
            boxColliders[i].sharedMaterial = trailMaterial;
            trailCollider.gameObject.SetActive(false);
        }
    }
    
    void MoveColliders()
    {
        StopAllCoroutines();

        angle = Mathf.Atan2(Mathf.Abs(DC.transform.position.y - PTDC.transform.position.y), Mathf.Abs(DC.transform.position.x - PTDC.transform.position.x));
        if ((PTDC.transform.position.y < DC.transform.position.y && PTDC.transform.position.x > DC.transform.position.x) || (DC.transform.position.y < PTDC.transform.position.y && DC.transform.position.x > PTDC.transform.position.x))
        {
            angle *= -1;
        }
        /*
        trailCollider.transform.rotation = Quaternion.Euler(0, 0, angle * Mathf.Rad2Deg); // good method to convert quaternion(transform.rotation) to euler angles(degrees)

        trailCollider.offset = new Vector2(((DC.transform.position.x - PTDC.transform.position.x) / 2) - additionalOffset, 0);

        LRLength = Vector2.Distance(DC.transform.position, PTDC.transform.position);
        trailCollider.size = new Vector2(LRLength, LRWidth);
        midPoint = (PTDC.transform.position + DC.GetEndPosition) / 2;
        trailCollider.transform.position = PTDC.transform.position;
        */

        float length = Vector2.Distance(DC.GetStartPosition,DC.GetEndPosition)/(DC.colliderCount+1);

        int i = 0;
        foreach (Vector2 midPoint in DC.GetMidPoints)
        {
            if (DC.GetStartPosition.x < DC.GetEndPosition.x)
            {
                boxColliders[i].offset = new Vector2(-0.13f, 0);
            }else boxColliders[i].offset = new Vector2(0.13f, 0);
            boxColliders[i].transform.position = midPoint;
            boxColliders[i].transform.rotation = Quaternion.Euler(0, 0, angle * Mathf.Rad2Deg);
            boxColliders[i].size = new Vector2(length,LRWidth);
            boxColliders[i].gameObject.SetActive(true);
            i++;
        }

        StartCoroutine(Delay());
    }

    IEnumerator Delay() //used to deactivae colliders after a delay
    {
        yield return new WaitForSeconds(delay);
        for (int i = 0; i<DC.colliderCount; i++)
        {
            yield return new WaitForSeconds(delayBetweenColliders);
            if (boxColliders[i].gameObject.activeInHierarchy == true) {
                boxColliders[i].gameObject.SetActive(false);
            }
        }
        
    }

    void Update () {

        LR.SetPosition(0, DC.transform.position);
        LR.SetPosition(1, DC.GetStartPosition);
        
        if (DC.StartedDashing)
        {
            Fade();
            if (Input.GetMouseButtonDown(0))
            {
                Debug.Log(angle + " angle");
                codeStartAlpha = startAlpha;
                codeEndAlpha = endAlpha;
            }
        }
    }

    void Fade()
    {
        codeStartAlpha -= Time.deltaTime * startFadeSpeed;
        codeEndAlpha -= Time.deltaTime * endFadeSpeed;
        start = Color.red;
        end = Color.red;
        start.a = codeStartAlpha;
        end.a = codeEndAlpha;
        LR.endColor = end;
        LR.startColor = start;
    }
}

