using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PlayerScript : MonoBehaviour
{
    public Image startImage;
    bool started; //per tenere traccia se il gioco è già partito o no
    bool jumping; //serve per sapere se già saltando
    Rigidbody rb;
    Animator animator;
    
    [SerializeField]
    float speed;
    [SerializeField]
    float jump;
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
                
                animator.SetBool("idle", false);  //imposto la variabile idle a false, succede che poi si passa in RunForward

                startImage.enabled = false;



            }
            
        }
        else
        {
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
        
    } 

    void TurnRight()
    {
        
    } 

    void Slide()
    {
        
    }

    private void OnCollisionEnter(Collision collision) //void che si attiva ogni volta che il player entra in collsione con qualcosa
                                                    //collsion rappresenta l'oggetto con cui il playrer va in collsione
    {
        if (collision.gameObject.tag == "terrain")  //Se l'oggetto con cui va in collsione è terrain
        {
            jumping = false;                       //jumping diventa falsa e quindi poi puo saltare
        }
    }
}
