using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(CharacterController))]
public class Movement : MonoBehaviourPunCallbacks
{
    [SerializeField] private Camera playerCamera;

    public float walkSpeed = 6f;
    public float runSpeed = 12f;
    public float jumpPower = 7f;
    public float gravity = 10f;
    public float lookSpeed = 2f;
    public float lookXLimit = 45f;
    public float defaultHeight = 2f;
    public float crouchHeight = 1f;
    public float crouchSpeed = 3f;

    private Vector3 moveDirection = Vector3.zero;
    private float rotationX = 0;
    private CharacterController characterController;
    private LifeManager lm;

    private PhotonView PV;


    private bool canMove = true;

    private GameObject blindaje;

    void Start()
    {
        PV = GetComponent<PhotonView>();
        if (PV.IsMine && PhotonNetwork.IsConnected)
        {
            characterController = GetComponent<CharacterController>();
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            lm = gameObject.GetComponent<LifeManager>();
        }
        if (!PV.IsMine)
        {
            playerCamera.enabled = false;
        }
    }

    void Update()
    {
        if (PV.IsMine && PhotonNetwork.IsConnected)
        {
            Vector3 forward = transform.TransformDirection(Vector3.forward);
            Vector3 right = transform.TransformDirection(Vector3.right);

            bool isRunning = Input.GetKey(KeyCode.LeftShift);
            float curSpeedX = canMove ? (isRunning ? runSpeed : walkSpeed) * Input.GetAxis("Vertical") : 0;
            float curSpeedY = canMove ? (isRunning ? runSpeed : walkSpeed) * Input.GetAxis("Horizontal") : 0;
            float movementDirectionY = moveDirection.y;
            moveDirection = (forward * curSpeedX) + (right * curSpeedY);

            if (Input.GetButton("Jump") && canMove && characterController.isGrounded)
            {
                moveDirection.y = jumpPower;
            }
            else
            {
                moveDirection.y = movementDirectionY;
            }

            if (!characterController.isGrounded)
            {
                moveDirection.y -= gravity * Time.deltaTime;
            }

            if (Input.GetKey(KeyCode.R) && canMove)
            {
                characterController.height = crouchHeight;
                walkSpeed = crouchSpeed;
                runSpeed = crouchSpeed;

            }
            else
            {
                characterController.height = defaultHeight;
                walkSpeed = 6f;
                runSpeed = 12f;
            }

            characterController.Move(moveDirection * Time.deltaTime);

            if (canMove)
            {
                rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
                rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
                playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
                transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (PV.IsMine && PhotonNetwork.IsConnected)
        {
            if (other.tag == "Armor")
            {
                if (lm._currentLife <= 5)
                {
                    Debug.Log("gotArmor");
                    blindaje = other.gameObject;
                    RPC_GotArmor();
                    PhotonNetwork.Destroy(blindaje.gameObject);
                }
            }
        }
    }

    [PunRPC]
    public void RPC_GotArmor()
    {
        lm._currentLife = 6;
        Debug.LogError(lm._currentLife);
    }
}