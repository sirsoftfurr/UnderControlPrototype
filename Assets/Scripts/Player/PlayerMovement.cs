using System;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D body;

    public float speed;
    public float jumpForce = 10f;

    public Transform groundCheck;
    public LayerMask groundMask;
    public Vector2 groundCheckSize = new Vector2(0.2f, 0.2f);

    void Start()
    {
        body = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        body.linearVelocity = new Vector2(
            Input.GetAxis("Horizontal") * speed,
            body.linearVelocity.y
        );

        if (IsGrounded())
        {
            if (Input.GetButtonDown("Jump"))
                body.linearVelocity = new Vector2(
                    body.linearVelocity.x,
                    jumpForce
                );
        }
    }

    bool IsGrounded()
    {
        if (Physics2D.OverlapBox(groundCheck.position, groundCheckSize, 0, groundMask))
        {
            return true;
        }

        return false;
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;

        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(groundCheck.position, groundCheckSize);
    }
}


