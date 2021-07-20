using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerManagerBilocazione : MonoBehaviour
{
    [SerializeField] private Transform spawn;
    [SerializeField] private AudioClip diamondFx;
    [SerializeField] private GameObject particleDiamond;
    [SerializeField] private float speedRuning;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private AnimationCurve jumpCurve;

    private bool moveFoward;
    private bool jumping; //serve per sapere se già saltando
    
    private Rigidbody rb;
    private Animator animator;

    private float yPos;
    private float altezza;

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

        // Movimento tra corsiee
        float acceleration = Input.acceleration.x * Time.deltaTime * moveSpeed;
        transform.Translate(acceleration, 0, 0);

        // Muovo il player
        if (moveFoward)
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(transform.position.x, yPos, transform.position.z) + transform.forward, Time.deltaTime * speedRuning);
    }

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
