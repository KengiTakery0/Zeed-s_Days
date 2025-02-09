using Cinemachine;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(InputController))]
public class PlayerController : MonoBehaviour
{
    [Header("Params")]
    [SerializeField] float _walkSpeed;
    [SerializeField] float _sprintSpeed;
    [SerializeField] float _jumpForce;
    [Range(0.0f, 0.3f)]
    float _rotationSmoothTime = 0.12f;

    [Header("Camera")]
    [SerializeField] LayerMask AimLayerMask;
    [SerializeField] GameObject _folowCamera;
    [SerializeField] float _clampX;
    [SerializeField] float _clampY;
    [SerializeField] float normalLookSensativity;
    [SerializeField] float aimLookSensativity;
    [SerializeField] CinemachineVirtualCamera aimVirtualCamera;
    public float LookSensativity { get; set; }

    [Header("Components")]
    [SerializeField] Animator animator;
    Rigidbody rb;
    InputController input;

    [Header("Settings")]
    [SerializeField] LayerMask GroundMask;
    [SerializeField] LayerMask InteractionMask;

    /*[SerializeField] UnityEvent FindedInteractionIvent;
      [SerializeField] UnityEvent LostInteractionIvent;*/
    [SerializeField] Transform InteractTransform;


    private bool isDead { get; set; }
    private float speed;
    private float targetRotation = 0.0f;
    private float rotationVelocity;
    private float cameraYaw = 0;
    private float cameraPitch = 0;
    private float animationBlend;
    private bool OnFoot { get; set; }
    public bool RotateOnMove { get; private set; }
    public bool IsOnTransport { get; private set; }
    public bool CameraLocked { get; private set; }

    const float speedOffset = 0.01f;

    private int _animIDSpeed;
    private int _animIDGrounded;
    private int _animIDJump;
    private int _animIDFreeFall;
    private int _animIDMotionSpeed;

    IInteractable currentInteratableObj = null;

    Vector3 mouseWorldPos = Vector3.zero;

    [SerializeField] Weapon w;
    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;

        rb = GetComponent<Rigidbody>();
        input = GetComponent<InputController>();
        InitAnimsationsID();
    }
    private void Start()
    {
        StartCoroutine(Motion());
    }

    IEnumerator Motion()
    {
        while (!isDead)
        {
            Move();
            InterationCheck();
            Look();

            yield return new WaitForSeconds(0.0001f);
        }
    }

    private void InterationCheck()
    {
        if (IsOnTransport) return;
        Collider[] p = Physics.OverlapSphere(InteractTransform.position, 0.5f, InteractionMask);
        if (p.Length > 0)
        {
            currentInteratableObj = p[0].GetComponent<IInteractable>();
            currentInteratableObj.EnterInteract();
            if (input.isInteract)
            {
                currentInteratableObj.Interact(gameObject);
                currentInteratableObj.LeaveInteract();
            }
        }
        else
        {
            if (currentInteratableObj != null)
            {
                currentInteratableObj.LeaveInteract();
                currentInteratableObj = null;
            }
        }
    }
    public void ReloadWeapon()
    {
        w.Reload();
    }

    public void SeatOnTransport(GameObject transport)
    {
        input.SetupInput();
        IsOnTransport = true;
        input.OnTransport = true;
        CameraLocked = true;
        animator.SetLayerWeight(2, 1);
        TransportInteraction t = transport.GetComponent<TransportInteraction>();
        t.controller = input;
        t.canMove = true;
        transform.SetParent(t.PlayerSit);
        transform.position = Vector3.zero;
        t.StartCoroutine(t.Move());
    }
    private void Look()
    {

        if (input.LookDirection.sqrMagnitude >= 0.1f && !CameraLocked)
        {
            cameraYaw += input.LookDirection.x * LookSensativity * Time.deltaTime;
            cameraPitch += input.LookDirection.y * LookSensativity * Time.deltaTime;
        }

        cameraPitch = ClampAngle(cameraPitch, _clampX);
        cameraYaw = ClampAngle(cameraYaw, _clampY);

        _folowCamera.transform.rotation = Quaternion.Euler(cameraPitch, cameraYaw, 0);

        mouseWorldPos = Vector3.zero;
        Vector2 _screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Ray _ray = Camera.main.ScreenPointToRay(_screenCenterPoint);
        if (Physics.Raycast(_ray, out RaycastHit _hit, 999f, AimLayerMask))
        {
            mouseWorldPos = _hit.point;
        }
        if (input.isAim)
        {

            Vector3 worldAimTargetPos = mouseWorldPos;
            worldAimTargetPos.y = transform.position.y;
            Vector3 aimDir = (worldAimTargetPos - transform.position).normalized;
            transform.forward = Vector3.Lerp(transform.forward, aimDir, Time.deltaTime * 10f);
            animator.SetLayerWeight(1, Mathf.Lerp(animator.GetLayerWeight(1), 1, Time.deltaTime * 10));
        }
        else animator.SetLayerWeight(1, Mathf.Lerp(animator.GetLayerWeight(1), 0, Time.deltaTime * 10));

    }
    private void Move()
    {
        if (!IsOnTransport)
        {
            float targetSpeed = input.isSprint ? _sprintSpeed : _walkSpeed;

            if (input.MoveDirection == Vector2.zero) targetSpeed = 0f;
            float currentHorizontalSpeed = new Vector3(rb.velocity.x, 0, rb.velocity.z).magnitude;

            if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
            {
                speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed, Time.deltaTime * 10);
                speed = Mathf.Round(speed * 1000f) / 1000f;
            }
            else speed = targetSpeed;

            animationBlend = Mathf.Lerp(animationBlend, targetSpeed, Time.deltaTime * 10);
            if (animationBlend < 0.01f) animationBlend = 0f;

            Vector3 inputDir = new Vector3(input.MoveDirection.x, 0, input.MoveDirection.y).normalized;

            if (input.MoveDirection != Vector2.zero)
            {
                targetRotation = Mathf.Atan2(inputDir.x, inputDir.z) * Mathf.Rad2Deg + Camera.main.transform.eulerAngles.y;
                float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref rotationVelocity, _rotationSmoothTime);
                if (RotateOnMove) transform.rotation = Quaternion.Euler(0, rotation, 0);
            }
            Vector3 targetDirection = Quaternion.Euler(0, targetRotation, 0) * Vector3.forward;
            rb.velocity = targetDirection * speed;
            animator.SetFloat(_animIDSpeed, animationBlend);
        }

    }
    public void OnShoot()
    {
        if (!input.isAim || IsOnTransport) return;
        Vector3 aimDir = (mouseWorldPos - w.shootHole.position).normalized;
        w.Shoot(aimDir);
        input.isShoot = false;
    }
    public void OnAim()
    {
        if (IsOnTransport) return;
        if (input.isAim)
        {
            aimVirtualCamera.gameObject.SetActive(true);
            LookSensativity = aimLookSensativity;
            RotateOnMove = false;
        }
        else
        {
            aimVirtualCamera.gameObject.SetActive(false);
            LookSensativity = normalLookSensativity;
            RotateOnMove = true;
        }
    }

    private float ClampAngle(float Angle, float Max)
    {
        float Min = -Max;
        if (Angle < -360f) Angle += 360f;
        if (Angle > 360f) Angle -= 360f;
        return Mathf.Clamp(Angle, Min, Max);
    }
    private void InitAnimsationsID()
    {
        _animIDSpeed = Animator.StringToHash("Speed");
        _animIDGrounded = Animator.StringToHash("Grounded");
        _animIDJump = Animator.StringToHash("Jump");
        _animIDFreeFall = Animator.StringToHash("FreeFall");
        _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
    }
}
