using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{   
    private Rigidbody2D body;
    [SerializeField] private float speed;
    [SerializeField] private float jumpPower;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask wallLayer;
    private Animator anim;
    private BoxCollider2D boxCollider;
    private float wallJumpCooldown;
    private float horizontalInput;
    private float slip;

    private void Awake()
    {   
        //References for Rigidbody and Animator from Object
        body = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {   
        horizontalInput = Input.GetAxis("Horizontal");
        body.velocity = new Vector2(Input.GetAxis("Horizontal") * speed,body.velocity.y);

        //Flips character when moving
        if(horizontalInput > 0.01f)
        {
            transform.localScale = Vector3.one;
        }else if (horizontalInput < -0.01)
        {
            transform.localScale = new Vector3(-1,1,1);
        }

        if(Input.GetKey(KeyCode.Space) && isGrounded())
        {
            Jump();
        }

        //Set animator param
        anim.SetBool("Run", horizontalInput != 0);
        anim.SetBool("Grounded", isGrounded());

        //Wall Jumping
        if(wallJumpCooldown > 0.0f)
        {
        
            body.velocity = new Vector2(horizontalInput * speed, body.velocity.y);
            //slip = 2 * Time.deltaTime;
            slip = 5;
            if(onWall() && !isGrounded()){
                body.velocity = Vector2.zero;
                //body.gravityScale += 2 * Time.deltaTime;
                //body.gravityScale += slip;
                //slip *= slip * Time.deltaTime;
                body.gravityScale += slip * Time.deltaTime;
                slip *= slip * Time.deltaTime;
                
            }
            else
            {
                body.gravityScale = 3;
            }

            if(Input.GetKey(KeyCode.Space))
            {
            Jump();
            }
        }
        else
        {
            wallJumpCooldown += Time.deltaTime;
        }
        slip = 2 * Time.deltaTime;

    }

    private void Jump()
    {   
        if(isGrounded())
        {
            body.velocity = new Vector2(body.velocity.x, jumpPower);
            anim.SetTrigger("Jump"); 
        }
        else if(onWall() && !isGrounded())
        {
            
            if(horizontalInput > 0)
            {
                body.velocity = new Vector2(-Mathf.Sign(transform.localScale.x) * 10, 15);
                transform.localScale = new Vector3(-Mathf.Sign(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
            
            wallJumpCooldown = 0;
        } 

    }

    private bool isGrounded()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, Vector2.down, 0.1f, groundLayer);
        return raycastHit.collider != null;
    }

    private bool onWall()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, new Vector2(transform.localScale.x, 0), 0.1f, wallLayer);
        return raycastHit.collider != null;
    }

}
