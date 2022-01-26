using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UserInput : MonoBehaviour
{
    [SerializeField]
    private PlayerInput input;
    [SerializeField]
    private NetworkingManager networkManager;

    public void InputMove(InputAction.CallbackContext context)
    {
        Dictionary<string, float[]> data = new Dictionary<string, float[]>();
        Vector2 value = context.ReadValue<Vector2>();
        data.Add(context.action.name.ToLower(), new float[] { value.x, value.y });
        networkManager.Emit("input", data);
    }

    public void InputLook(InputAction.CallbackContext context)
    {
        Dictionary<string, float[]> data = new Dictionary<string, float[]>();
        Vector2 value = context.ReadValue<Vector2>();
        data.Add(context.action.name.ToLower(), new float[] { value.x, value.y });
        networkManager.Emit("input", data);
    }

    public void InputFire(InputAction.CallbackContext context)
    {
        //if (context.phase == InputActionPhase.Started) return;

        //Dictionary<string, bool> data = new Dictionary<string, bool>();
        //data.Add(context.action.name.ToLower(), context.ReadValueAsButton());
        //networkManager.Emit("input", data);
    }

    public void InputJump(InputAction.CallbackContext context)
    {
        if (context.phase != InputActionPhase.Performed && context.phase != InputActionPhase.Canceled) return;

        Dictionary<string, bool> data = new Dictionary<string, bool>();
        data.Add(context.action.name.ToLower(), context.ReadValueAsButton());
        networkManager.Emit("input", data);
    }
}
