using UnityEngine;

public class DualCharacterController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 6f;
    public float gravity = -20f;

    [Header("Look")]
    public float mouseSensitivity = 2f;
    public float maxLookAngle = 80f;

    [Header("Flip")]
    public float flipCooldown = 0.3f;
    public GameObject frontPivot;
    public GameObject backPivot;
    public Camera frontCamera;
    public Camera backCamera;

    [Header("Flip Feedback")]
    public float flipFOVPunch = 95f;
    public float normalFOV = 80f;
    public float fovReturnSpeed = 8f;

    [Header("State")]
    public bool isFacingFront = true;

    private CharacterController _cc;
    private float _verticalVelocity;
    private float _flipTimer;
    private float _cameraPitch;
    private float _currentFOV;
    private bool _isReturningFOV = false;

    [HideInInspector] public bool inputLocked = false;

    void Start()
    {
        float savedFOV = PlayerPrefs.GetFloat("FOV", 80f);
        normalFOV = savedFOV;
        frontCamera.fieldOfView = savedFOV;
        backCamera.fieldOfView = savedFOV;
        _cc = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        isFacingFront = true;
        frontCamera.enabled = true;
        backCamera.enabled = false;

        _currentFOV = normalFOV;
    }

    void Update()
    {
        if (!inputLocked)
        {
            HandleMovement();
            HandleLook();
        }
        HandleFlip();
        HandleFOVReturn();
    }

    void HandleMovement()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        float facingMultiplier = isFacingFront ? 1f : -1f;
        Vector3 move = transform.forward * v * facingMultiplier
                     + transform.right * h * facingMultiplier;
        move.y = 0f;

        if (_cc.isGrounded)
            _verticalVelocity = -2f;
        else
            _verticalVelocity += gravity * Time.deltaTime;

        move.y = _verticalVelocity;
        _cc.Move(move * moveSpeed * Time.deltaTime);
    }

    void HandleLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        transform.Rotate(Vector3.up * mouseX);

        _cameraPitch -= mouseY;
        _cameraPitch = Mathf.Clamp(_cameraPitch, -maxLookAngle, maxLookAngle);

        frontPivot.transform.localEulerAngles = new Vector3(_cameraPitch, 0f, 0f);
        backPivot.transform.localEulerAngles = new Vector3(_cameraPitch, 180f, 0f);
    }

    void HandleFlip()
    {
        _flipTimer -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.Space) && _flipTimer <= 0f)
        {
            isFacingFront = !isFacingFront;
            frontCamera.enabled = isFacingFront;
            backCamera.enabled = !isFacingFront;
            _flipTimer = flipCooldown;
            AudioManager.Instance?.PlayFlip();

            TriggerFlipFeedback();
            UpgradeManager.Instance?.OnFlip();
        }
    }

    void TriggerFlipFeedback()
    {
        _currentFOV = flipFOVPunch;
        frontCamera.fieldOfView = _currentFOV;
        backCamera.fieldOfView = _currentFOV;
        _isReturningFOV = true;
    }

    void HandleFOVReturn()
    {
        if (!_isReturningFOV) return;

        _currentFOV = Mathf.Lerp(_currentFOV, normalFOV, Time.deltaTime * fovReturnSpeed);
        frontCamera.fieldOfView = _currentFOV;
        backCamera.fieldOfView = _currentFOV;

        if (Mathf.Abs(_currentFOV - normalFOV) < 0.1f)
        {
            _currentFOV = normalFOV;
            _isReturningFOV = false;
        }
    }

    public Camera GetActiveCamera() => isFacingFront ? frontCamera : backCamera;
}