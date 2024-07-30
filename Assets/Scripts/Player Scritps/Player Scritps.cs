using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScritps : MonoBehaviour
{
    private Vector3 PlayerMovementInput;
    private Vector2 PlayerMouseInput;
    private float xRot;
    [SerializeField] private LayerMask GroundMask;
    [SerializeField] private Transform FeetTransform;
    [SerializeField] private Transform PlayerCamera;
    [SerializeField] private Rigidbody PlayerBody;
    [Space]
    [SerializeField] private float Speed;
    [Range(0, 360)]
    [SerializeField] private float Sensivity;
    [SerializeField] private float Jumpfore;

    void Update()
    {
        InputPlayer();
        CameraMove();
        Movement();
    }
    void InputPlayer()
    {
        PlayerMovementInput = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
        PlayerMouseInput = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxis("Mouse Y"));
    }
    void CameraMove()
    {
        xRot -= PlayerMouseInput.y * Sensivity;
        xRot = Mathf.Clamp(xRot, -90f, 90f); // Giới hạn xoay trục x (pitch) từ -90 đến 90 độ

        transform.Rotate(0f, PlayerMouseInput.x * Sensivity, 0f);

        PlayerCamera.transform.localRotation = Quaternion.Euler(xRot, 0f, 0f);

        // Giới hạn xoay trục y (yaw) của người chơi
        transform.localRotation = Quaternion.Euler(0f, transform.localRotation.eulerAngles.y, 0f);
    }
    void Movement()
    {
        Vector3 Move = transform.TransformDirection(PlayerMovementInput) * Speed;
        PlayerBody.velocity = new Vector3(Move.x, PlayerBody.velocity.y, Move.z);
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (Physics.CheckSphere(FeetTransform.position, 0.1f, GroundMask))
            {
                PlayerBody.AddForce(Vector3.up * Jumpfore, ForceMode.Impulse);
            }
        }
    }
}
