using UnityEngine;
using UnityEngine.InputSystem;

public class GatherInput : MonoBehaviour
{
    private Controls myControl;
    public float valueX;

    // เพิ่มตัวนี้สำหรับรับค่าการกระโดด
    public bool jumpInput;

    public void Awake()
    {
        myControl = new Controls();
    }

    private void OnEnable()
    {
        myControl.Player.Move.performed += StartMove;
        myControl.Player.Move.canceled += StopMove;

        // เพิ่ม 2 บรรทัดนี้สำหรับ Jump
        myControl.Player.Jump.performed += JumpStart;
        myControl.Player.Jump.canceled += JumpStop;

        myControl.Player.Enable();
    }

    private void OnDisable()
    {
        myControl.Player.Move.performed -= StartMove;
        myControl.Player.Move.canceled -= StopMove;

        // เพิ่ม 2 บรรทัดนี้สำหรับ Jump
        myControl.Player.Jump.performed -= JumpStart;
        myControl.Player.Jump.canceled -= JumpStop;

        myControl.Player.Disable();
        //myControl.Disable();
    }

    private void StartMove(InputAction.CallbackContext ctx)
    {
        valueX = ctx.ReadValue<float>();
    }

    private void StopMove(InputAction.CallbackContext ctx)
    {
        valueX = 0;
    }

    // เพิ่มฟังก์ชันนี้สำหรับเริ่มกระโดด
    private void JumpStart(InputAction.CallbackContext ctx)
    {
        jumpInput = true;
    }

    // เพิ่มฟังก์ชันนี้สำหรับหยุดกระโดด
    private void JumpStop(InputAction.CallbackContext ctx)
    {
        jumpInput = false;
    }
}