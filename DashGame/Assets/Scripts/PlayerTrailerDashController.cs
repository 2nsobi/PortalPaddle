using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTrailerDashController : MonoBehaviour {

    public static PlayerTrailerDashController Instance;

    Rigidbody2D rigidbody;
    public float dashSpeed;
    public float dashTime; //dash time for inspector use
    public float delay; // for delay the PlayerTrailer before dashing
    float codeDelay;
    float codeDashTime; //dash time for code use
    bool dashing;
    public Transform PlayerTransform;
    Vector2 direction;
    public DashController DC;
   
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
            codeDelay = delay;
            this.transform.position = PlayerTransform.position;
            dashing = true;
        }
    }

    private void FixedUpdate()
    {
        direction = (PlayerTransform.position - this.transform.position); //direction follows that of the player's transform
        if (dashing)
        {
            codeDelay -= Time.deltaTime;
            if (DC.StartedDashing)
            {
                rigidbody.velocity = Vector2.zero;
            }
            if (codeDelay <= 0)
            {
                Dash();
            }
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
            rigidbody.velocity = Vector2.zero;
            dashing = false;
        }
    }
}
