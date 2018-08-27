using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostLink : MonoBehaviour
{

    public Ball host;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag != "Wall")
        {
            host.OnTriggerEnter2D(collision);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag != "Wall")
        {
            host.OnCollisionEnter2D(collision);
        }
    }
}
