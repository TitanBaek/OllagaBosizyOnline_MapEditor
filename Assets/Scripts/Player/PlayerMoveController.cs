using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;

public class PlayerMoveController : MonoBehaviour
{
    private Rigidbody2D rb;
    private bool isGround;
    private Vector2 moveDir;
    [SerializeField] private LayerMask platformLayer;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float maximumSpeed;
    [SerializeField] private float jumpPower;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        isGround = false;
    }

    private void Update()
    {
        DoMove();
    }

    private void FixedUpdate()
    {
        GroundCheck();
    }

    private void DoMove()
    {
        // 최고 속력일 경우 힘을 가해도 속력이 빨라지지 않음
        if (moveDir.x < 0 && rb.velocity.x > -maximumSpeed)
        {
            Debug.Log("왼쪽");
            //gfx.rotation = Quaternion.Euler(0, -90, 0);
            rb.AddForce(Vector2.right * moveDir.x * moveSpeed * Time.deltaTime, ForceMode2D.Force);
        }
        else if (moveDir.x > 0 && rb.velocity.x < maximumSpeed)
        {
            Debug.Log("오른쪽");
            //gfx.rotation = Quaternion.Euler(0, 90, 0);
            rb.AddForce(Vector2.right * moveDir.x * moveSpeed * Time.deltaTime, ForceMode2D.Force);
        }
    }

    private void DoJump()
    {
        rb.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
    }

    public void OnMove(InputValue value)
    {
        Debug.Log("이동");
        // 이동 구현
        moveDir = value.Get<Vector2>();
    }

    public void OnJump(InputValue value)
    {
        if (value.isPressed && isGround)
        {
            Debug.Log("점프");
            DoJump();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Goal 여부 확인
    }

    private void GroundCheck()
    {
        Debug.DrawRay(transform.position, Vector2.down * 1f, Color.red);
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 1f, platformLayer);
        if (hit.collider != null)
        {
            Debug.Log("땅임");
            isGround = true;
        }
        else
        {
            Debug.Log("공중임");
            isGround = false;
        }
    }

}
