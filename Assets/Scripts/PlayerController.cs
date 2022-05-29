using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    [Header("Components")]
	private Rigidbody2D rb;
    private Vector2 direction;

    [Header("Stats")]
    public float movementSpeed = 10;
    public float jumpForce = 7;

    [Header("Collisions")]
    public LayerMask layerFloor;
    public Vector2 floor;
    public float radioCollider;

    [Header("Booleans")]
    public bool canMove = true;
    public bool onTheFloor = true;

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
        RestrictJump();
    }

    private void Movement(){
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        direction = new Vector2(x, y);

        Walk();

        ImproveJump();
        if(Input.GetKeyDown(KeyCode.Space) && onTheFloor){
            Jump();
        }
    }

    private void Walk(){
        if(canMove){
            rb.velocity = new Vector2(direction.x * movementSpeed, rb.velocity.y);//añade velocidad de movimiento a la direccion horizontal y mantiene la direccion vertical

            if(direction.x < 0 && transform.localScale.x > 0){//si el jugador se esta desplazando para la izquierda(x < 0) y en la escala indica que esta mirando a la derecha(localScale.x > 0)
                transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);//cambiamos la escala de x a valor negativo
            }else if(direction.x > 0 && transform.localScale.x < 0){
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);//necesitamos cambiar la escala de x a su valor absoluto
            }
        }
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

    private void RestrictJump(){
        onTheFloor = Physics2D.OverlapCircle((Vector2)transform.position + floor, radioCollider, layerFloor);
    }
}
