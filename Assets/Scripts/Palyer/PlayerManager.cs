using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager current;

    [SerializeField] private Image startImage;
    [SerializeField] private GameObject CanvasButtons;
    [SerializeField] private AudioClip diamondFx;
    [SerializeField] private GameObject particleDiamond;
    [SerializeField] private float speedRuning;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private AnimationCurve jumpCurve;

    private bool started; //per tenere traccia se il gioco è già partito o no
    private bool moveFoward;
    private bool slide; //serve per sapere se già slide
    private bool jumping; //serve per sapere se già saltando
    
    private Rigidbody rb;
    private Animator animator;

    private float colHeight, colRadius, colCenterY, colCenterZ; //per il collider dello slide

    private float jumpTimer;
    private float yPos;

    private void Awake()
    {
        current = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        started = false;
        jumping = false;
        moveFoward = true;
        animator.SetBool("idle", true);
    }

    // Update is called once per frame
    void Update()
    {
        if (started)  //se il gioco non è partito
        {
            if (PlatformSpawnerScript.current.gameOver)
                return;
            if (BilocazioneManager.current.bilocazione)
                return;

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

            // Movimento tra corsiee
            float acceleration = Input.acceleration.x * Time.deltaTime * moveSpeed;
            transform.Translate(acceleration, 0, 0);

            // Muovo il player
            if (moveFoward)
                transform.position = Vector3.MoveTowards(transform.position, new Vector3(transform.position.x, yPos, transform.position.z) + transform.forward, Time.deltaTime * speedRuning);
        }
        
    }

    #region ACTIONS
    public void StartLevel()
    {
        started = true;

        //Faccio partire lo spawn delle piattaforme
        PlatformSpawnerScript.current.BeginToSpawn();

        animator.SetBool("idle", false);  //imposto la variabile idle a false, succede che poi si passa in RunForward

        startImage.enabled = false;
        CanvasButtons.SetActive(true);

        ScoreManagerScript.current.StartScore();
        AudioManageScript.current.PlayRunning();
    }

    public void Jump()
    {
        if (!BilocazioneManager.current.bilocazione)
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
    }

    public void TurnLeft()
    {
        if (!BilocazioneManager.current.bilocazione)
            rb.transform.Rotate(0.0f, -90.0f, 0.0f);
    }

    public void TurnRight()
    {
        if (!BilocazioneManager.current.bilocazione)
            rb.transform.Rotate(0.0f,90.0f,0.0f);
    }

    public void Slide()
    {
        if (!slide && !jumping && !BilocazioneManager.current.bilocazione)
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
    #endregion

    #region TRIGGERS
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "diamond")
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
        else if (other.gameObject.tag == "obstacle" && !PoteriManager.current.IsInvisible())
        {
            animator.SetTrigger("fall1");
            ScoreManagerScript.current.StopScore();
            PlatformSpawnerScript.current.gameOver = true;
            moveFoward = false;
            AudioManageScript.current.PlayGameOver();
        }
        else if (other.gameObject.tag == "fence")
        {
            animator.SetTrigger("fall2");
            ScoreManagerScript.current.StopScore();
            rb.velocity = -Vector3.up * speedRuning + transform.forward * speedRuning;
            PlatformSpawnerScript.current.gameOver = true;
            AudioManageScript.current.PlayGameOver();
        }
        else if (other.gameObject.tag == "fall")
        {
            animator.SetTrigger("fall2");
            ScoreManagerScript.current.StopScore();
            rb.velocity = -Vector3.up * speedRuning;
            PlatformSpawnerScript.current.gameOver = true;
            moveFoward = false;
            AudioManageScript.current.PlayGameOver();
        }
    }
    #endregion

    public void Replay()
    {
        //In questo modo vado a Ricare la Scena. Naturalmente posso mettere il numero oppure il nome della scena
        SceneManager.LoadScene(0);
    }
}