using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Image startImage;
    [SerializeField] private GameObject CanvasButtons;
    bool started; //per tenere traccia se il gioco è già partito o no
    bool jumping; //serve per sapere se già saltando
    bool slide; ////serve per sapere se già slide
    Rigidbody rb;  //private ?
    Animator animator;  //private ?

    public AudioClip diamondFx;
    public GameObject particleDiamond;

    float colHeight, colRadius, colCenterY, colCenterZ;
    
    [SerializeField]
    float speedRuning;  //private ?
    [SerializeField]
    float jump;  //private ?

    int turn;// mi serve per sapere da che parte è girato il player

    [SerializeField] private Transform movePoint; //Per muovere il personaggio dentro la corsia
    [SerializeField] private LayerMask whatStopsMovement; //Per non andare oltre
    public float moveSpeed = 5f;


    /*
     * VERSINE 2
     */
    public AnimationCurve jumpCurve;
    private float jumpTimer;
    private float yPos;
    private bool moveFoward;

    public float[] xPos;
    private int xPosIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        
        started = false;
        jumping = false;
        moveFoward = true;
        animator.SetBool("idle", true);

        //movePoint.parent = null;
    }

    // Update is called once per frame
    void Update()
    {
        if (!started)  //se il gioco non è partito
        {
            if (Input.GetMouseButtonDown(0))  //se c'è un click dell'utente sullo schermo, facciamo partire il personaggio
            {
                started = true;
                turn = 1; //Up
                //Faccio partire lo spawn delle piattaforme
                PlatformSpawnerScript.current.BeginToSpawn();

                animator.SetBool("idle", false);  //imposto la variabile idle a false, succede che poi si passa in RunForward

                startImage.enabled = false;
                CanvasButtons.SetActive(true);

                ScoreManagerScript.current.StartScore();

                PlatformManager.current.gameOver = false;
            }
        }
        else
        {
            if (PlatformManager.current.gameOver)
                return;

            if (SwipeManager.IsSwipingUp())  //se c'è lo swipe verso sopra, salto
                Jump();
            else if (SwipeManager.IsSwipingLeft())
                TurnLeft();
            else if (SwipeManager.IsSwipingRight())
                TurnRight();
            else if (SwipeManager.IsSwipingDown())
                Slide();

            if (jumping)
            {
                yPos = jumpCurve.Evaluate(jumpTimer);
                jumpTimer += Time.deltaTime;

                if (jumpTimer > 1f)
                {
                    jumpTimer = 0f;
                    jumping = false;
                    animator.SetBool("saltar", jumping);
                }
            }

            // Muovo il player
            //transform.position = Vector3.MoveTowards(transform.position, transform.position + Vector3.forward, Time.deltaTime * speed);
            //transform.position = Vector3.MoveTowards(transform.position, new Vector3(xPos[xPosIndex], yPos, transform.position.z), Time.deltaTime * speed);
            if (moveFoward)
                transform.position = Vector3.MoveTowards(transform.position, new Vector3(transform.position.x, yPos, transform.position.z) + transform.forward, Time.deltaTime * speedRuning);
        }
        
    }

    #region ACTIONS
    void Jump()
    {
        if (!jumping)
        {
            jumping = true; //Se non sta saltando lo imposto a true
            //animator.SetTrigger("jump");
            animator.SetBool("saltar", jumping);
        }
        else
        {
            //se c'è lo swipe verso sopra di nuovo, divento invisibile
            PoteriManager.current.TurnInvisible();
        }
    }

    void TurnLeft()
    {
        rb.transform.Rotate(0.0f, -90.0f, 0.0f);
    } 

    void TurnRight()
    {
        rb.transform.Rotate(0.0f,90.0f,0.0f);
    } 

    void Slide()
    {
        if (!slide && !jumping)
        {
            slide = true;
            animator.SetTrigger("slide");
            CapsuleCollider coll = gameObject.GetComponent<CapsuleCollider>();

            //salvo i valori del collaider per poterli ripristinare più avanti
            colHeight = coll.height;
            colRadius = coll.radius;
            colCenterY = coll.center.y;
            colCenterZ = coll.center.z;

            //vado a modificare il capsule collaider
            coll.height = 1f;
            coll.radius = 0.7f;
            coll.center = new Vector3 (0, 0.72f, 0.34f);

            Invoke("ExitSlide", 1f);
        }
    }

    void ExitSlide()
    {
        CapsuleCollider coll = gameObject.GetComponent<CapsuleCollider>();
        //vado a ripristinare il capsule collaider
        coll.height = colHeight;
        coll.radius = colRadius;
        coll.center = new Vector3(0, colCenterY, colCenterZ);

        slide = false;
    }

    void MoveLeft()
    {
        Debug.Log("Move Left");

        //if (Physics2D.OverlapCircle(movePoint.position + new Vector3(-1, 0f, 0f), 0.2f, whatStopsMovement))
        //{
        //    movePoint.position += new Vector3(-1, 0f, 0f);
        //}

        //transform.position = Vector3.MoveTowards(transform.position, new Vector3(-1, 0f, 0f), moveSpeed * Time.deltaTime);

        transform.position += new Vector3(-0.05f, 0f, 0f);
    }

    void MoveRigth()
    {
        Debug.Log("Move Rigth");

        //if (Physics2D.OverlapCircle(movePoint.position + new Vector3(-1, 0f, 0f), 0.2f, whatStopsMovement))
        //{
        //    movePoint.position += new Vector3(1, 0f, 0f);
        //}

        //transform.position = Vector3.MoveTowards(transform.position, new Vector3(1, 0f, 0f), moveSpeed * Time.deltaTime);
        if (Input.GetMouseButtonDown(0))
            transform.position += new Vector3(1f * Time.deltaTime, 0f, 0f);
    }
    #endregion

    #region TRIGGERS
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "obstacle" && !PoteriManager.current.IsInvisible())
        {
            animator.SetTrigger("fall1");
            ScoreManagerScript.current.StopScore();
            PlatformSpawnerScript.current.gameOver = true;
            moveFoward = false;
        }
        else if (other.gameObject.tag == "fence")
        {
            animator.SetTrigger("fall2");
            ScoreManagerScript.current.StopScore();
            rb.velocity = -Vector3.up * speedRuning + transform.forward * speedRuning;
            PlatformManager.current.gameOver = true;
        }
        else if (other.gameObject.tag == "fall")
        {
            animator.SetTrigger("fall2");
            ScoreManagerScript.current.StopScore();
            rb.velocity = -Vector3.up * speedRuning;
            PlatformManager.current.gameOver = true;
            moveFoward = false;
        }
        else if (other.gameObject.tag == "diamond")
        {
            if (!PlatformSpawnerScript.current.gameOver)
            {
                Destroy(other.gameObject);
                ScoreManagerScript.current.DiamondScore();
                AudioManageScript.current.PlaySound(diamondFx);

                // Lo facciamo in questo modo perchè lo instaziamo momentaneamente, in quanto dopo lo andiamo a distruggere
                GameObject part = Instantiate(particleDiamond, new Vector3(other.gameObject.transform.position.x, other.gameObject.transform.position.y, other.gameObject.transform.position.z + 4f), Quaternion.identity) as GameObject;
                Destroy(part, 2f);
            }
        }
    }
    #endregion

    public void Replay()
    {
        //In questo modo vado a Ricare la Scena. Naturalmente posso mettere il numero oppure il nome della scena
        SceneManager.LoadScene(0);
    }
}
