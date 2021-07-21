using UnityEngine;
using UnityEngine.InputSystem;

[DefaultExecutionOrder(-2)] //Run beafor all scripts
public class InputManager : Singleton<InputManager>
{
    //public static InputManager instance;

    #region Events
    public delegate void StartTouch(Vector2 position, float time);
    public event StartTouch OnStartTouch;
    public delegate void EndTouch(Vector2 position, float time);
    public event EndTouch OnEndTouch;
    #endregion

    private PlayerControls playerControls;
    private Camera mainCamera;
    private Swipe action;

    private void Awake()
    {
        //instance = this;
        playerControls = new PlayerControls();
        mainCamera = Camera.main;
    }

    private void OnEnable()
    {
        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }

    void Start()
    {
        playerControls.Touch.PrimaryContact.started += ctx => StartTouchPrimary(ctx);
        playerControls.Touch.PrimaryContact.canceled += ctx => EndTouchPrimary(ctx);
    }

    private void StartTouchPrimary(InputAction.CallbackContext context)
    {
        if (OnStartTouch != null)
            OnStartTouch(playerControls.Touch.PrimaryPosition.ReadValue<Vector2>(), (float)context.startTime);
    }

    private void EndTouchPrimary(InputAction.CallbackContext context)
    {
        if (OnEndTouch != null)
            OnEndTouch(playerControls.Touch.PrimaryPosition.ReadValue<Vector2>(), (float)context.time);
    }

    public Vector2 PrimaryPosition()
    {
        Vector3 position = playerControls.Touch.PrimaryPosition.ReadValue<Vector2>();
        position.z = mainCamera.nearClipPlane;
        return mainCamera.ScreenToWorldPoint(position);
    }

    public Swipe GetAction()
    {
        return action;
    }

    public void SetAction(Swipe action)
    {
        this.action = action;
    }

    
}
