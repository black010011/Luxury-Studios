using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 3.5f;
    public float sprintSpeed = 6.5f;
    public float crouchSpeed = 1.8f;
    public float rotationSpeed = 12f;
    public float gravity = -9.81f;
    public float acceleration = 8f;   // how fast currentSpeed ramps up
    public float deceleration = 10f;  // how fast currentSpeed ramps down

    [Header("Jump")]
    public float jumpHeight = 1.2f;

    [Header("Crouch")]
    public float crouchHeight = 1f;
    public float crouchTransitionSpeed = 8f;

    [Header("References")]
    public Transform cameraTransform;
    public Transform upperBodyBone;

    private CharacterController controller;
    private Animator animator;
    private HashSet<string> animatorParams = new HashSet<string>();

    private Vector3 moveDirection;
    private Vector3 velocity;

    private float currentSpeed;  // smoothed, actual speed applied this frame
    private float targetSpeed;

    private bool isSprinting = false;
    private bool isCrouching = false;
    public bool isAiming = false;

    private float standHeight;
    private Vector3 standCenter;
    private Vector3 crouchCenter;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        if (animator != null)
        {
            foreach (AnimatorControllerParameter param in animator.parameters)
            {
                animatorParams.Add(param.name);
            }
        }

        standHeight = controller.height;
        standCenter = controller.center;
        crouchCenter = new Vector3(standCenter.x, standCenter.y - (standHeight - crouchHeight) / 2f, standCenter.z);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        HandleAiming();
        HandleCrouch();
        HandleMovement();
        HandleJump();
        HandleGravity();
        HandleRotation();
        HandleAnimations();
        HandleMovementInput();
        UpdateAimLayer();
    }

    void LateUpdate()
    {
        HandleUpperBodyAim();
    }

    void HandleMovement()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;

        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        moveDirection = (forward * vertical + right * horizontal).normalized;

        isSprinting =
            Input.GetKey(KeyCode.LeftShift) &&
            vertical > 0.1f &&
            !isAiming &&
            !isCrouching;

        float maxSpeed = isCrouching ? crouchSpeed : (isSprinting ? sprintSpeed : walkSpeed);
        targetSpeed = moveDirection.sqrMagnitude > 0.01f ? maxSpeed : 0f;

        // Ease toward the target speed instead of snapping, so starting/stopping/sprinting feels smooth
        float rate = targetSpeed > currentSpeed ? acceleration : deceleration;
        currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, rate * Time.deltaTime);

        controller.Move(moveDirection * currentSpeed * Time.deltaTime);
    }

    void HandleJump()
    {
        if (controller.isGrounded && !isCrouching && Input.GetButtonDown("Jump"))
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            SetAnimatorTrigger("Jump");
        }
    }

    void HandleGravity()
    {
        if (controller.isGrounded && velocity.y < 0f)
        {
            velocity.y = -2f;
        }

        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);
    }

    void HandleCrouch()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl) && controller.isGrounded)
        {
            // Only stand back up if there is room above the player's head
            if (isCrouching && !CanStandUp())
            {
                // stay crouched, ceiling in the way
            }
            else
            {
                isCrouching = !isCrouching;
            }
        }

        float targetHeight = isCrouching ? crouchHeight : standHeight;
        Vector3 targetCenter = isCrouching ? crouchCenter : standCenter;

        controller.height = Mathf.Lerp(controller.height, targetHeight, crouchTransitionSpeed * Time.deltaTime);
        controller.center = Vector3.Lerp(controller.center, targetCenter, crouchTransitionSpeed * Time.deltaTime);

        SetAnimatorBool("IsCrouching", isCrouching);
    }

    bool CanStandUp()
    {
        float clearance = standHeight - controller.height;
        Vector3 origin = transform.position + Vector3.up * controller.height;
        return !Physics.Raycast(origin, Vector3.up, clearance, ~0, QueryTriggerInteraction.Ignore);
    }

    void HandleRotation()
    {
        if (isAiming)
        {
            Vector3 lookDirection = cameraTransform.forward;

            lookDirection.y = 0f;

            if (lookDirection != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(lookDirection);

                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    targetRotation,
                    rotationSpeed * Time.deltaTime
                );
            }
        }
        else if (moveDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }
    }

    void HandleAnimations()
    {
        if (animator == null)
            return;

        // Drive the blend tree from the actual smoothed speed (0 = idle, 1 = full sprint)
        // instead of a hardcoded step, so walk/run transitions match real movement.
        float normalizedSpeed = sprintSpeed > 0f ? currentSpeed / sprintSpeed : 0f;

        animator.SetFloat(
            "Speed",
            normalizedSpeed,
            0.1f,
            Time.deltaTime
        );
    }

    void HandleMovementInput()
    {
        if (animator == null)
            return;

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        animator.SetFloat(
            "Horizontal",
            horizontal,
            0.1f,
            Time.deltaTime
        );

        animator.SetFloat(
            "Vertical",
            vertical,
            0.1f,
            Time.deltaTime
        );
    }

    void HandleAiming()
    {
        isAiming = Input.GetMouseButton(1);

        animator.SetBool("IsAiming", isAiming);
    }

    void HandleUpperBodyAim()
    {
        if (!isAiming)
            return;

        if (upperBodyBone == null)
            return;

        float pitch = cameraTransform.eulerAngles.x;

        if (pitch > 180f)
            pitch -= 360f;

        Quaternion targetRotation = Quaternion.Euler(
            pitch,
            0f,
            0f
        );

        upperBodyBone.localRotation = Quaternion.Lerp(
            upperBodyBone.localRotation,
            targetRotation,
            Time.deltaTime * 10f
        );
    }

    void UpdateAimLayer()
    {
        if (animator == null)
            return;

        int layerIndex = animator.GetLayerIndex("AimingLayer");

        if (layerIndex == -1)
            return;

        float targetWeight = isAiming ? 1f : 0f;

        float currentWeight = animator.GetLayerWeight(layerIndex);

        animator.SetLayerWeight(
            layerIndex,
            Mathf.Lerp(
                currentWeight,
                targetWeight,
                Time.deltaTime * 8f
            )
        );
    }

    void SetAnimatorBool(string name, bool value)
    {
        if (animator != null && animatorParams.Contains(name))
        {
            animator.SetBool(name, value);
        }
    }

    void SetAnimatorTrigger(string name)
    {
        if (animator != null && animatorParams.Contains(name))
        {
            animator.SetTrigger(name);
        }
    }
}
