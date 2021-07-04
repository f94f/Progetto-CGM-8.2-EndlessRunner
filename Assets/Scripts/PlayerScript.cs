using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class PlayerScript : MonoBehaviour
{
    public Image startImage;
    bool started; //per tenere traccia se il gioco è già partito o no
    bool jumping; //serve per sapere se già saltando
    Rigidbody rb;  //private ?
    Animator animator;  //private ?

    public AudioClip diamondFx;
    public GameObject particleDiamond;

    float colHeight, colRadius, colCenterY, colCenterZ;
    
    [SerializeField]
    float speed;  //private ?
    [SerializeField]
    float jump;  //private ?

    int turn;// mi serve per sapere da che parte è girato il player
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        
        started = false;
        jumping = false;
        animator.SetBool("idle", true);
        

    }

    // Update is called once per frame
    void Update()
    {
        if (!started)  //se il gioco non è partito
        {
            if (Input.GetMouseButtonDown(0))  //se c'è un click dell'utente sullo schermo, facciamo partire il personaggio
            {
                rb.velocity = Vector3.forward * speed;  //faccio muovere il personaggio

                started = true;
                turn = 1; //Up
                //Faccio partire lo spawn delle piattaforme
                PlatformSpawnerScript.current.BeginToSpawn() ;
                animator.SetBool("idle", false);  //imposto la variabile idle a false, succede che poi si passa in RunForward

                startImage.enabled = false;

                ScoreManagerScript.current.StartScore();



            }
            
        }
        else
        {

            if (!PlatformSpawnerScript.current.gameOver)
            {
                //provo con la riga seguente per farlo saltare 
                //if (Input.GetMouseButtonDown(0))
                //{
                //    animator.SetTrigger("jump");
                //    rb.AddForce(Vector3.up * jump, ForceMode.Impulse);
                //}
                //fine aggiunta mia per prova. da cancellare per prova solo

                if (SwipeManager.IsSwipingUp())  //se c'è lo swipe verso sopra, salto
                {
                    Jump();
                }
                else if (SwipeManager.IsSwipingLeft())
                {
                    Debug.Log("Left");
                    TurnLeft();
                }
                else if (SwipeManager.IsSwipingRight())
                {
                    Debug.Log("Right");
                    TurnRight();
                }
                else if (SwipeManager.IsSwipingDown())
                {
                    Debug.Log("Slide");
                    Slide();
                }
            }

        }
        
    }

    void Jump()
    {
        if (!jumping)
        {
            jumping = true; //Se non salta lo imposto a true
            animator.SetTrigger("jump");
            rb.AddForce(Vector3.up * jump, ForceMode.Impulse);
        }
        
    }

    void TurnLeft()
    {
        rb.transform.Rotate(0.0f, -90.0f, 0.0f);
        //a seconda della posizione in cui si trova lo faccio muovere

        if (turn == 1)
        {
            rb.velocity = Vector3.left * speed;
            turn = 4; //right
        }
        else if (turn == 2)
        {
            rb.velocity = Vector3.back * speed;
            turn = 3; //down
        }
        else if (turn == 3)
        {
            rb.velocity = Vector3.right * speed;
            turn = 2; //right
        }
        else if (turn == 2)
        {
            rb.velocity = Vector3.forward * speed;
            turn = 1; //up
        }
    } 

    void TurnRight()
    {
        rb.transform.Rotate(0.0f,90.0f,0.0f);
        //a seconda della posizione in cui si trova lo faccio muovere

        if (turn == 1)
        {
            rb.velocity = Vector3.right * speed;
            turn = 2; //right
        }
        else if (turn==2)
        {
            rb.velocity = Vector3.back * speed;
            turn = 3; //down
        }
        else if (turn == 3)
        {
            rb.velocity = Vector3.left * speed;
            turn = 4; //left
        }
        else if (turn == 4)
        {
            rb.velocity = Vector3.forward * speed;
            turn = 1; //up
        }
    } 

    void Slide()
    {
        animator.SetTrigger("slide");
        CapsuleCollider coll = gameObject.GetComponent<CapsuleCollider>();

        //salvo i valori del collaider per poterli ripristinare più avanti
        colHeight = coll.height;
        colRadius = coll.radius;
        colCenterY = coll.center.y;
        colCenterZ = coll.center.z;

        //vado a modificare il capsule collaider
        coll.height = 1.69f;
        coll.radius = 0.85f;
        coll.center = new Vector3 (0, 0.43f, 0.34f);

        Invoke("ExitSlide",2f);
    }

    private void OnCollisionEnter(Collision collision) //void che si attiva ogni volta che il player entra in collsione con qualcosa
                                                    //collsion rappresenta l'oggetto con cui il playrer va in collsione
    {
        if (collision.gameObject.tag == "terrain")  //Se l'oggetto con cui va in collsione è terrain
        {
            jumping = false;                       //jumping diventa falsa e quindi poi puo saltare
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "obstacle")
        {
            animator.SetTrigger("fall1");

            PlatformSpawnerScript.current.gameOver=true;

            ScoreManagerScript.current.StopScore();
        }
        else if (other.gameObject.tag == "fence")
        {
            animator.SetTrigger("fall2");
            ScoreManagerScript.current.StopScore();
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

    void ExitSlide()
    {
        CapsuleCollider coll = gameObject.GetComponent<CapsuleCollider>();
        //vado a ripristinare il capsule collaider
        coll.height = colHeight;
        coll.radius = colRadius;
        coll.center = new Vector3(0, colCenterY, colCenterZ);
    }

    public void Replay()
    {
        //In questo modo vado a Ricare la Scena. Naturalmente posso mettere il numero oppure il nome della scena
        SceneManager.LoadScene(0);
    }
}
