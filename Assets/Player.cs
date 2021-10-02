using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    Rigidbody2D rb;

    public LayerMask groundLayer;

    float speed = 20f;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Jump") && IsGrounded()) {
            Jump();
        }
    }

    void FixedUpdate()
    {
        Move(Input.GetAxis("Horizontal") * speed);
    }

    void Move(float move)
    {
        rb.velocity = new Vector2(move, rb.velocity.y);
    }

    void Jump()
    {
        rb.AddForce(new Vector2(0, 25f), ForceMode2D.Impulse);
    }

    bool IsGrounded() {
        Vector2 position = transform.position;
        Vector2 direction = Vector2.down;
        float distance = 1.1f;

        Debug.DrawRay(position, direction * distance, Color.green);
        RaycastHit2D hit = Physics2D.CircleCast(position, 1f, direction, distance, groundLayer);
        if (hit.collider != null) {
            return true;
        }

        return false;
    }
}
