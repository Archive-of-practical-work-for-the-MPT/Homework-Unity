using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float MovmentSpeed = 2.0f;

    public float SpringSpeed = 4.0f;

    public float JumpForce = 5.0f;

    public float RotationSmoothing = 20f;

    public GameObject HandMeshes;

    public GameObject[] WeaponInventory;

    public GameObject[] WeaponsMeshes;

    private int SelectedWeaponId = 0;

    private Weapons _Weapon;

    public GameManager _GameManager;

    private float pitch, yaw;

    public float Sensative = 5f;

    private Vector2 _rotation;

    private Rigidbody _Rigidbody;

    private bool IsGrounded;

    public float DistationToGround = 0.1f;

    private float lastDamageTime = 0;

    private Vector3 startPosition;

    private Quaternion startRotation;

    private AnimationManager _AnimationManager;

    private bool IsSprinting = false;

    private void Jump()
    {
        _Rigidbody.AddForce(Vector3.up * JumpForce, ForceMode.Impulse);
    }

    private void GroundedCheck()
    {
        IsGrounded = Physics.Raycast(transform.position, Vector3.down, DistationToGround);
    }

    private Vector3 CalculateMovment()
    {
        IsSprinting = false;

        float HorizontalDirection = Input.GetAxis("Horizontal");
        float VerticalDirection = Input.GetAxis("Vertical");

        Vector3 Move = transform.right * HorizontalDirection + transform.forward * VerticalDirection;

        return _Rigidbody.transform.position + Move * Time.fixedDeltaTime * MovmentSpeed;
    }

    private Vector3 CalculateSprint()
    {
        IsSprinting = true;

        float HorizontalDirection = Input.GetAxis("Horizontal");
        float VerticalDirection = Input.GetAxis("Vertical");

        Vector3 Move = transform.right * HorizontalDirection + transform.forward * VerticalDirection;

        return _Rigidbody.transform.position + Move * Time.fixedDeltaTime * SpringSpeed;
    }

    private void Start()
    {
        _Rigidbody = GetComponent<Rigidbody>();
        _GameManager = FindObjectOfType<GameManager>();
        
        _Weapon = WeaponInventory[SelectedWeaponId].GetComponent<Weapons>();
        WeaponsMeshes[SelectedWeaponId].SetActive(true);

        startPosition = transform.position;
        startRotation = transform.rotation;

        _AnimationManager = WeaponsMeshes[SelectedWeaponId].GetComponent<AnimationManager>();
    }

    private void FixedUpdate()
    {
        GroundedCheck();

        if (Input.GetKey(KeyCode.Space) && IsGrounded) Jump();

        if (Input.GetKey(KeyCode.Mouse0))
        {
            if (_Weapon.Fire()) { _AnimationManager.SetAnimationFire(); };
        }

        if (Input.GetKey(KeyCode.R))
        {
            if (_Weapon.Reload()) { _AnimationManager.SetAnimationReload(); };
        }

        if (Input.GetAxis("Mouse ScrollWheel") > 0) SelectedNextWeapon();
        else if (Input.GetAxis("Mouse ScrollWheel") < 0) SelectedPrevWeapon();


        if (Input.GetKey(KeyCode.LeftShift) && !_GameManager.IsStaminaRestoring)
        {
            _GameManager.SpendStamina();
            _Rigidbody.MovePosition(CalculateSprint());
        }
        else { _Rigidbody.MovePosition(CalculateMovment()); }

        SetRotation();

        SetAnimation();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + (Vector3.down * DistationToGround));    
    }

    public void SetRotation()
    {
        yaw += Input.GetAxis("Mouse X") * Sensative;
        pitch -= Input.GetAxis("Mouse Y") * Sensative;

        _rotation.x = Mathf.Repeat(_rotation.x * pitch, 360);

        pitch = Mathf.Clamp(pitch, -60, 90);

        Quaternion SmoothRotation = Quaternion.Euler(pitch, yaw, 0);

        HandMeshes.transform.rotation = Quaternion.Slerp(HandMeshes.transform.rotation,
            SmoothRotation, RotationSmoothing * Time.fixedDeltaTime );

        SmoothRotation = Quaternion.Euler(0, yaw, 0);

        transform.rotation = Quaternion.Slerp(transform.rotation, SmoothRotation,
            RotationSmoothing * Time.fixedDeltaTime);
    }

    public void Respawn()
    {
        transform.position = startPosition;
        transform.rotation = startRotation;

        _Rigidbody.velocity = Vector3.zero;
        _Rigidbody.angularVelocity = Vector3.zero;

        _GameManager.RespawnPlayer();
    }

    private void SelectedPrevWeapon()
    {
        if (SelectedWeaponId != 0)
        {
            WeaponsMeshes[SelectedWeaponId].SetActive(false);
            SelectedWeaponId -= 1;
            _Weapon = WeaponInventory[SelectedWeaponId].GetComponent<Weapons>();
            WeaponsMeshes[SelectedWeaponId].SetActive(true);
            _AnimationManager = WeaponsMeshes[SelectedWeaponId].GetComponent<AnimationManager>(); //!

            Debug.LogWarning("Оружие " + _Weapon.WeaponType);
        }
    }

    private void SelectedNextWeapon()
    {
        if (WeaponInventory.Length > SelectedWeaponId + 1)
        {
            WeaponsMeshes[SelectedWeaponId].SetActive(false);
            SelectedWeaponId++;
            _Weapon = WeaponInventory[SelectedWeaponId].GetComponent<Weapons>();
            WeaponsMeshes[SelectedWeaponId].SetActive(true);
            _AnimationManager = WeaponsMeshes[SelectedWeaponId].GetComponent<AnimationManager>(); //!

            Debug.LogWarning("Оружие " + _Weapon.WeaponType);
        }
    }

    public void AddToInventory(GameObject weaponType, GameObject weaponPrefab)
    {
        System.Array.Resize(ref WeaponInventory, WeaponInventory.Length + 1);
        System.Array.Resize(ref WeaponsMeshes, WeaponsMeshes.Length + 1);

        WeaponsMeshes[WeaponInventory.Length - 1] = weaponPrefab;
        WeaponInventory[WeaponInventory.Length - 1] = weaponType;
        WeaponsMeshes[WeaponInventory.Length - 1].SetActive(false);
    }

    private bool IsMoving()
    {
        return Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0;
    }

    private void SetAnimation()
    {
        if (IsMoving())
        {
            if (IsSprinting) _AnimationManager.SetAnimationRun();
            else _AnimationManager.SetAnimationWalk();
        }
        else _AnimationManager.SetAnimationIdle();
    }

}
