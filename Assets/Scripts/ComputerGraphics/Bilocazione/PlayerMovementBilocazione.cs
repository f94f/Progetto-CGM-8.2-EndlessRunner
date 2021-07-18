using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class PlayerMovementBilocazione : MonoBehaviour
{
    [SerializeField] private Transform spawn;
    [SerializeField] private AudioClip diamondFx;
    [SerializeField] private GameObject particleDiamond;
    [SerializeField] private float speedRuning;
    [SerializeField] private float moveSpeed;

    private bool jumping; //serve per sapere se già saltando
    private bool slide; ////serve per sapere se già slide
    private Rigidbody rb;  //private ?
    private Animator animator;  //private ?
    private float colHeight, colRadius, colCenterY, colCenterZ;

    [SerializeField] private AnimationCurve jumpCurve;
    private float jumpTimer;
    private float yPos;
    private float altezza;
    private bool moveFoward;

    // Start is called before the first frame update
    void Start()
    {
        ResetPosition();
    }

    // Update is called once per frame
    void Update()
    {
        if (!BilocazioneManager.current.bilocazione)
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
            yPos = altezza + jumpCurve.Evaluate(jumpTimer);
            jumpTimer += Time.deltaTime;

            if (jumpTimer > 1f)
            {
                jumpTimer = 0f;
                jumping = false;
                animator.SetBool("saltar", jumping);
            }
        }

        float acceleration = Input.acceleration.x * Time.deltaTime * moveSpeed;
        transform.Translate(acceleration, 0, 0);

        // Muovo il player
        if (moveFoward)
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(transform.position.x, yPos, transform.position.z) + transform.forward, Time.deltaTime * speedRuning);
    }

    #region ACTIONS
    void Jump()
    {
        if (!jumping)
        {
            jumping = true; //Se non sta saltando lo imposto a true
            animator.SetBool("saltar", jumping);
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
        if (other.gameObject.tag == "fence")
        {
            moveFoward = false;
            animator.SetBool("idle", false);
            BilocazioneManager.current.EndBilocazione();
        }
        else if (other.gameObject.tag == "fall")
        {
            moveFoward = false;
            animator.SetBool("idle", false);
            BilocazioneManager.current.EndBilocazione();
        }
        else if (other.gameObject.tag == "diamond")
        {
            //if (!PlatformSpawnerScript.current.gameOver)
            if (true)
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

    private void ResetPosition()
    {
        moveFoward = true;
        jumping = false;

        altezza = spawn.transform.position.y;
        yPos = spawn.transform.position.y;
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        animator.SetBool("idle", false);
    }
}
