using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    #region COMPONENTS
	private Rigidbody2D rb;
	#endregion

    #region STATE PARAMETERS
    public float movementSpeed = 10;
    public float jumpForce = 7;
    private Vector2 direction;
    #endregion

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Movement();
    }

    private void Movement(){
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        direction = new Vector2(x, y);

        Walk();

        ImproveJump();
        if(Input.GetKeyDown(KeyCode.Space)){
            Jump();
        }
    }

    private void Walk(){
        rb.velocity = new Vector2(direction.x * movementSpeed, rb.velocity.y);
    }

    private void Jump(){
        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.velocity += Vector2.up * jumpForce;
    }

    private void ImproveJump(){
        if(rb.velocity.y < 0){//si estoy cayendo
            rb.velocity += Vector2.up * Physics2D.gravity.y * (2.5f - 1) * Time.deltaTime;
        }else if(rb.velocity.y > 0 && !Input.GetKey(KeyCode.Space)){//no esta saltando y la velocidad es mayor que 0
            rb.velocity += Vector2.up * Physics2D.gravity.y * (2.0f - 1) * Time.deltaTime;
        }
    }
}
