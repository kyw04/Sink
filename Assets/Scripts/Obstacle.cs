using System;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public Vector2 direction;
    public float power;

    private void OnCollisionStay2D(Collision2D other)
    {
        other.gameObject.GetComponent<Rigidbody2D>().AddForce(direction * power, ForceMode2D.Impulse);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("asdasd");
        }
    }
}
