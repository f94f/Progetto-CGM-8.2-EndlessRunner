using System.Collections.Generic;
using UnityEngine;

public class SwipeDetection : MonoBehaviour
{
    [SerializeField] private float minimumDistance = 0.03f;
    [SerializeField] private float maximumtime = 1f;
    [SerializeField, Range(0, 1)] private float directionThreshold = 0.9f;

    private InputManager inputManager;

    private Vector2 startPosition;
    private float startTime;
    private Vector2 endPosition;
    private float endTime;

    private bool startLevel;


    private void Awake()
    {
        inputManager = InputManager.Instance;
    }

    private void OnEnable()
    {
        inputManager.OnStartTouch += SwipeStart;
        inputManager.OnEndTouch += SwipeEnd;
    }

    private void OnDisable()
    {
        inputManager.OnStartTouch -= SwipeStart;
        inputManager.OnEndTouch -= SwipeEnd;
    }

    private void SwipeStart(Vector2 position, float time)
    {
        startPosition = position;
        startTime = time;
    }

    private void SwipeEnd(Vector2 position, float time)
    {
        endPosition = position;
        endTime = time;
        DetectSwipe();
    }

    private void DetectSwipe()
    {
        //Debug.Log("DetectSwipe");
        if ((endTime - startTime) <= maximumtime) //Nel tempo stabilito
        {
            if (!startLevel && Vector3.Distance(startPosition, endPosition) < minimumDistance) //è un tocco
            {
                startLevel = true;
                PlayerManager.current.StartLevel();
            }
            else if (Vector3.Distance(startPosition, endPosition) >= minimumDistance) //è uno swipe
            {
                //Debug.Log("Swipe Detected");
                //Debug.DrawLine(startPosition, endPosition, Color.red, 5f);

                Vector3 direction = endPosition - startPosition;
                Vector2 direction2D = new Vector2(direction.x, direction.y).normalized;
                SwipeDirection(direction2D);
            }
        }
    }

    private void SwipeDirection(Vector2 direction)
    {
        if (Vector2.Dot(Vector2.up, direction) > directionThreshold)
        {
            //Debug.Log("Swipe Up");
            PlayerManager.current.Jump();
        }
        else if (Vector2.Dot(Vector2.down, direction) > directionThreshold)
        {
            //Debug.Log("Swipe Down");
            PlayerManager.current.Slide();
        }
        else if (Vector2.Dot(Vector2.left, direction) > directionThreshold)
        {
            //Debug.Log("Swipe Left");
            PlayerManager.current.TurnLeft();
            Debug.Log(Time.time);
        }
        else if (Vector2.Dot(Vector2.right, direction) > directionThreshold)
        {
            //Debug.Log("Swipe Right");
            PlayerManager.current.TurnRight();
            Debug.Log(Time.time);
        }
    }
}