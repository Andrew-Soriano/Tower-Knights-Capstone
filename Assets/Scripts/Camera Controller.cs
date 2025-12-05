using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using UnityEngine.Windows;
using static Unity.Collections.AllocatorManager;
using static UnityEngine.GraphicsBuffer;

public class CameraController : MonoBehaviour
{
    [Header("References")]
    public Camera cam;
    public PlayerInput input;

    [Header("Movement")]
    public float moveSpeed = 20f;
    public float acceleration = 10f;
    public float dragPanSpeed = 0.5f;

    [Header("Zoom")]
    public float zoomSpeed = 5f;
    public float minZoom = 5f;
    public float maxZoom = 40f;
    private float targetZoom;
    private float zoomVelocity;
    public float zoomSmoothTime = 0.1f;

    [Header("Rotation")]
    public float rotateSpeed = 120f;
    public float snapRotateSpeed = 5f;
    private float targetYaw;
    private bool snapping = false;

    private Vector2 moveInput;
    private Vector2 lookInput;
    private float rotateInput;
    private float zoomInput;
    private Vector2 dragInput;
    private bool rmb;
    private bool mmb;
    private bool _ignoreNextClick = false;
    private bool _leftMouseDownThisClick = false;
    private int _frameLastClicked = -1;
    private int _frameInputStarted;

    Vector3 velocity;

    public static CameraController instance;

    public void OnMove(InputAction.CallbackContext ctx)
        => moveInput = ctx.ReadValue<Vector2>();

    public void OnLook(InputAction.CallbackContext ctx)
        => lookInput = ctx.ReadValue<Vector2>();

    public void OnRotate(InputAction.CallbackContext ctx)
        => rotateInput = ctx.ReadValue<float>();

    public void OnZoom(InputAction.CallbackContext ctx)
        => zoomInput = ctx.ReadValue<float>();

    public void OnDragMove(InputAction.CallbackContext ctx)
        => dragInput = ctx.ReadValue<Vector2>();

    public void OnRMB(InputAction.CallbackContext ctx)
        => rmb = ctx.ReadValueAsButton();

    public void OnMMB(InputAction.CallbackContext ctx)
        => mmb = ctx.ReadValueAsButton();
    
    private void OnLMBStarted(InputAction.CallbackContext ctx)
    {
        _leftMouseDownThisClick = true;
    }

    private void OnLMBPerformed(InputAction.CallbackContext ctx)
    {
        if (!_leftMouseDownThisClick)
            return;

        if (_ignoreNextClick)
        {
            _ignoreNextClick = false;
            _leftMouseDownThisClick = false;
            return;
        }

        if (IsPointerOverUI())
        {
            _leftMouseDownThisClick = false;
            return;
        }

        HandleClick();
        _leftMouseDownThisClick = false;
    }

    private void OnLMBCanceled(InputAction.CallbackContext ctx)
    {
        _leftMouseDownThisClick = false;
    }



    private void Update()
    {
        HandleMovement();
        HandleRotation();
        HandleZoom();
    }

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        
        // Initialize target zoom to current camera zoom
        targetZoom = cam.orthographic ? cam.orthographicSize : cam.transform.localPosition.y;
        targetYaw = transform.eulerAngles.y;
    }

    private void Start()
    {
        _frameInputStarted = Time.frameCount;
        input.SwitchCurrentActionMap("Camera");
    }

    void OnEnable()
    {
        input.actions["LMB"].started += OnLMBStarted;
        input.actions["LMB"].performed += OnLMBPerformed;
        input.actions["LMB"].canceled += OnLMBCanceled;
    }

    void OnDisable()
    {
        input.actions["LMB"].started -= OnLMBStarted;
        input.actions["LMB"].performed -= OnLMBPerformed;
        input.actions["LMB"].canceled -= OnLMBCanceled;
    }

    void HandleMovement()
    {
        Vector3 inputDir = Vector3.zero;
        Vector3 forward = cam.transform.forward;
        Vector3 right = cam.transform.right;
        
        forward.y = 0;
        forward.Normalize();
        right.y = 0;
        right.Normalize();

        // WASD movement
        if (moveInput.sqrMagnitude > 0.01f)
        {
            inputDir = forward * moveInput.y + right * moveInput.x;
        }

        // Middle-mouse drag panning (Only if not using WASD movement
        if (mmb && dragInput.sqrMagnitude > 0.01f)
        {
            transform.position += (-right * dragInput.x - forward * dragInput.y) * dragPanSpeed * Time.deltaTime;
        }

        // Smooth acceleration
        Vector3 targetVel = inputDir * moveSpeed;
        velocity = Vector3.MoveTowards(velocity, targetVel, acceleration * Time.deltaTime);

        transform.position += velocity * Time.deltaTime;
    }

    void HandleRotation()
    {
        // Q/E rotation
        if (Mathf.Abs(rotateInput) > 0.01f)
        {
            if (!snapping)
            {
                // Determine new target in 90° increments
                float currentYaw = transform.eulerAngles.y;
                float direction = Mathf.Sign(rotateInput);
                targetYaw = Mathf.Round(currentYaw / 90f) * 90f + direction * 90f;
                snapping = true;
            }
        }

        // RIGHT MOUSE DRAG rotate
        else if (rmb)
        {
            float rot = lookInput.x * rotateSpeed * 0.02f;
            transform.Rotate(Vector3.up, rot, Space.World);

            // Cancel snapping while dragging
            targetYaw = transform.eulerAngles.y;
            snapping = false;
        }

        if (snapping)
        {
            // Smoothly interpolate rotation toward targetYaw
            float newYaw = Mathf.LerpAngle(transform.eulerAngles.y, targetYaw, snapRotateSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Euler(0f, newYaw, 0f);

            // Stop snapping when close enough
            if (Mathf.Abs(Mathf.DeltaAngle(transform.eulerAngles.y, targetYaw)) < 0.1f)
                snapping = false;
        }
    }

    void HandleZoom()
    {
        if (Mathf.Abs(zoomInput) > 0.01f)
        {
            // Update target zoom based on scroll input
            targetZoom -= zoomInput * zoomSpeed;
            targetZoom = Mathf.Clamp(targetZoom, minZoom, maxZoom);
        }

        if (cam.orthographic)
        {
            // Smoothly interpolate orthographic size
            cam.orthographicSize = Mathf.SmoothDamp(
                cam.orthographicSize,
                targetZoom,
                ref zoomVelocity,
                zoomSmoothTime
            );
        }
        else
        {
            // Smoothly interpolate perspective camera height
            Vector3 pos = cam.transform.localPosition;
            pos.y = Mathf.SmoothDamp(pos.y, targetZoom, ref zoomVelocity, zoomSmoothTime);
            cam.transform.localPosition = pos;
        }
    }

    public void HandleClick()
    {
        int currentFrame = Time.frameCount;

        if (_ignoreNextClick || IsPointerOverUI())
        {
            return;
        }
        _frameLastClicked = currentFrame;

        if (UnityEngine.EventSystems.EventSystem.current != null &&
            UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
            return;

        Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hit))
        {

            if (SelectionManager.instance.GetCurrent() is TowerCatapult catapult)
            {
                catapult.SetTarget(hit.collider.transform.position);
                SelectionManager.instance.Deselect();
                return;
            }

            if (hit.collider.TryGetComponent<IClickable>(out var clickable))
            {
                clickable.OnClicked();
            }

        }
    }

    public void IgnoreNextClick()
    {
        _ignoreNextClick = true;
        StartCoroutine(ResetIgnoreClick());
    }

    private IEnumerator ResetIgnoreClick()
    {
        yield return null; // wait 1 frame
        _ignoreNextClick = false;
    }

    private bool IsPointerOverUI()
    {
        var menu = UIManager.instance.currentMenu;
        if (menu == null || menu.panel == null)
            return false;

        Vector2 pos = Mouse.current.position.ReadValue();
        return menu.panel.Pick(pos) != null;
    }
}
