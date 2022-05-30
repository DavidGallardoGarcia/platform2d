using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    [Header("Components")]
	private Rigidbody2D rb;
    private Vector2 direction;
    private Animator animator;

    [Header("Stats")]
    public float movementSpeed = 10;
    public float jumpForce = 7;
    public float dashVelocity = 20;

    [Header("Collisions")]
    public LayerMask layerFloor;
    public Vector2 floor;
    public float radioCollider;

    [Header("Booleans")]
    public bool canMove = true;
    public bool onTheFloor = true;
    public bool canDash;
    public bool isDash;
    public bool touchFloor;

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
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
        float x = Input.GetAxis("Horizontal");//GetAxis puede devolver valores con DECIMALES comprendidos entre -1 y 1
        float y = Input.GetAxis("Vertical");

        float xRaw = Input.GetAxisRaw("Horizontal");//GetAxisRaw solo puede devolver -1, 0 y 1
        float yRaw = Input.GetAxisRaw("Vertical");

        direction = new Vector2(x, y);

        Walk();

        ImproveJump();
        if(Input.GetKeyDown(KeyCode.Space) && onTheFloor){
            animator.SetBool("jump", true);
            Jump();
        }

        if(Input.GetKeyDown(KeyCode.X) && !isDash){
            if(xRaw != 0 || yRaw != 0)//izquierda, derecha, arriba y abajo
                Dash(xRaw, yRaw);
        }

        if(onTheFloor && !touchFloor){
            ValidateTouchFloor();
            touchFloor = true;
        }

        if(!onTheFloor && touchFloor){
            touchFloor = true;
        }

        float velocity;
            if(rb.velocity.y > 0)//si tiene velocidad vertical positiva esta saltando
                velocity = 1;
            else
                velocity = -1;

        if(!onTheFloor){//si no esta tocando el suelo(jump or fall)
            animator.SetFloat("verticalSpeed", velocity);
        }else{
            if(velocity == -1)
                EndJump();
        }
    }

    private void Walk(){
        if(canMove && !isDash){
            rb.velocity = new Vector2(direction.x * movementSpeed, rb.velocity.y);//añade velocidad de movimiento a la direccion horizontal y mantiene la direccion vertical

            if(direction != Vector2.zero){//si no esta quieto(!x0y0)
                animator.SetBool("walk", true);
                
                if(direction.x < 0 && transform.localScale.x > 0){//si el jugador se esta desplazando para la izquierda(x < 0) y en la escala indica que esta mirando a la derecha(localScale.x > 0)
                    transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);//cambiamos la escala de x a valor negativo
                }else if(direction.x > 0 && transform.localScale.x < 0){
                    transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);//necesitamos cambiar la escala de x a su valor absoluto
                }
            }else{//si esta quieto animacion walk false
                animator.SetBool("walk", false);
            }
        }
    }

    private void Jump(){
        rb.velocity += Vector2.up * jumpForce;//incrementa la velocidad del rb sumando (actual Y * jumpForce), no se incrementa nada a X
    }

    private void ImproveJump(){//INVESTIGAR
        if(rb.velocity.y < 0){
            rb.velocity += Vector2.up * Physics2D.gravity.y * (2.5f) * Time.deltaTime;//velocidad de caida
        }else if(rb.velocity.y > 0 && !Input.GetKey(KeyCode.Space)){//si esta manteniendo el salto
            rb.velocity += Vector2.up * Physics2D.gravity.y * (2.0f) * Time.deltaTime;//altura del salto sin mantener
        }
    }

    private void RestrictJump(){
        //crearemos un collider2d al que le pasaremos(posicion central del player + posicion del suelo, radio de la hitbox, layer sobre la que actuar)
        onTheFloor = Physics2D.OverlapCircle((Vector2)transform.position + floor, radioCollider, layerFloor);
    }

    public void EndJump(){
        animator.SetBool("jump", false);
    }

    private void Dash(float x, float y){
        animator.SetBool("dash", true);
        Vector3 playerPosition = Camera.main.WorldToViewportPoint(transform.position);
        Camera.main.GetComponent<RippleEffect>().Emit(playerPosition);//obtenemos la posicion del jugador con respecto con respecto la camara

        canDash = true;
        rb.velocity = Vector2.zero;//transforma la velocidad a 0
        rb.velocity += new Vector2(x, y).normalized * dashVelocity;
        StartCoroutine(prepareDash());
    }

    private IEnumerator prepareDash(){
        StartCoroutine(dashInFloor());

        rb.gravityScale = 0;//para que no afecte la gravedad al jugar
        isDash = true;

        yield return new WaitForSeconds(0.3f);//en estos segundos la gravedad no afectara al jugador

        rb.gravityScale = 1;//reasignamos la gravedad al jugador
        isDash = false;
        endDash();
    }

    private IEnumerator dashInFloor(){
        yield return new WaitForSeconds(0.15f);
        if(onTheFloor)
            isDash = false;
    }

    public void endDash(){
        animator.SetBool("dash", false);
    }

    private void ValidateTouchFloor(){
        canDash = false;
        isDash = false;
        animator.SetBool("jump", false);
    }
}
