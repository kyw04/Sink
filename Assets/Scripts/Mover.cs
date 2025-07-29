using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class Mover : MonoBehaviour
{
    private Rigidbody2D rigi;
    private SpriteRenderer sprite;
    private PlayerInput playerInput;
    private InputAction moveAction;

    private Vector2 currentMove;
    private int jumpCount;
    private int dashCount;

    public GameObject doubleJumpEffect;
    public GameObject dashEffect;
    
    public Vector2 footSize = new Vector2(0.5f, 0.25f);
    public float footPos = 0.5f;
    
    public float speed = 15.0f;
    public float jumpPower = 250.0f;
    public float dashPower = 150.0f;
    
    private void Awake()
    {
        rigi = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.currentActionMap.FindAction("Move");
    }

    private void FixedUpdate()
    {
        if (OnGround())
        {
            jumpCount = 0;
            dashCount = 0;
        }
        
        Move();
    }

    private void Move()
    {
        Vector2 value = moveAction.ReadValue<Vector2>();
        sprite.flipX = value.x == 0 ? sprite.flipX : value.x > 0 ? true : false;
        Vector2.SmoothDamp(currentMove, new Vector2(value.x, 0), ref currentMove, 2.0f);
        transform.Translate(transform.right * currentMove * speed * Time.deltaTime);
    }

    private bool OnGround()
    {
        return Physics2D.BoxCast(transform.position, footSize, 0, Vector2.down, footPos, LayerMask.GetMask("Floor"));
    }

    private void FreezePositionY()
    {
        rigi.linearVelocity = Vector2.zero;
        rigi.constraints = RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
    }

    private void MeltPosition()
    {
        rigi.constraints = ~RigidbodyConstraints2D.FreezePosition | RigidbodyConstraints2D.FreezeRotation;
    }
    
    private void OnJump()
    {
        if (jumpCount >= 2)
            return;

        jumpCount++;
        if (!OnGround())
        {
            jumpCount++;
            Instantiate(doubleJumpEffect, transform.position, quaternion.identity);
        }
        
        CancelInvoke(nameof(MeltPosition));
        MeltPosition();
        rigi.linearVelocity = Vector2.zero;
        rigi.AddForce(Vector2.up * jumpPower);
    }
    
    private void OnDash()
    {
        if (dashCount >= 2)
            return;
        dashCount++;
        
        Vector2 direction = sprite.flipX ? Vector2.right : -Vector2.right;
        ParticleSystemRenderer deshRenderer = Instantiate(dashEffect, transform.position, quaternion.identity).GetComponent<ParticleSystemRenderer>();
        deshRenderer.flip = direction;
        
        FreezePositionY();
        Invoke(nameof(MeltPosition), 0.15f);
        rigi.AddForce(direction * dashPower, ForceMode2D.Force);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = OnGround() ? Color.green : Color.red;
        Gizmos.DrawWireCube(transform.position + Vector3.down * footPos, footSize);
    }
}
