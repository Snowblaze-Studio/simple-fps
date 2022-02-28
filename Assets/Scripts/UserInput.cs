using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using SocketIOClient.JsonSerializer;

public class UserInput : MonoBehaviour
{
    [SerializeField]
    private PlayerInput input;
    [SerializeField]
    private NetworkingManager networkManager;

    private PlayerInputData inputData; 

    private void Awake()
    {
        inputData = new PlayerInputData();
    }

    public void InputMove(InputAction.CallbackContext context)
    {
        Vector2 value = context.ReadValue<Vector2>();
        inputData.move = new Move(value.x, value.y);
        SendInput();
    }

    public void InputLook(InputAction.CallbackContext context)
    {
        Vector2 value = context.ReadValue<Vector2>();
        inputData.look = new Look(value.x, value.y);
        SendInput();
    }

    public void InputFire(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started) return;

        inputData.fire = context.ReadValueAsButton();
        SendInput();
    }

    public void InputJump(InputAction.CallbackContext context)
    {
        if (context.phase != InputActionPhase.Performed && context.phase != InputActionPhase.Canceled) return;

        inputData.jump = context.ReadValueAsButton();
        SendInput();
    }

    public void SendInput()
    {
        networkManager.Emit("input", inputData);
    }
}
