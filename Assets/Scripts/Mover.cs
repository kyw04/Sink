using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Mover : MonoBehaviour
{
    private Rigidbody2D rigi;
    private SpriteRenderer sprite;
    private PlayerInput playerInput;
    private InputAction moveAction;

    private Vector2 currentMove;

    public float speed;
    public float dashPower;
    
    private void Awake()
    {
        rigi = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.currentActionMap.FindAction("Move");
    }

    private void Update()
    {
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        Vector2 value = moveAction.ReadValue<Vector2>();
        sprite.flipX = value.x == 0 ? sprite.flipX : value.x > 0 ? true : false;
        Vector2.SmoothDamp(currentMove, new Vector2(value.x, 0), ref currentMove, 2.0f);
        Debug.Log(currentMove);
        transform.Translate(transform.right * currentMove * speed * Time.deltaTime);
    }
    
    private void OnDash()
    {
        Vector2 direction = sprite.flipX ? Vector2.right : -Vector2.right;
        rigi.linearVelocity = Vector2.zero;
        rigi.AddForce(direction * dashPower, ForceMode2D.Force);
    }
}
