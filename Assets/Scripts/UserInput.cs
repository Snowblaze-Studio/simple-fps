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
    private SystemTextJsonSerializer serializer;

    private void Awake()
    {
        inputData = new PlayerInputData();
        serializer = new SystemTextJsonSerializer();
    }

    public void InputMove(InputAction.CallbackContext context)
    {
        Dictionary<string, float[]> data = new Dictionary<string, float[]>();
        Vector2 value = context.ReadValue<Vector2>();
        data.Add(context.action.name.ToLower(), new float[] { value.x, value.y });
        SendInput();

        //networkManager.Emit("input", data);
    }

    public void InputLook(InputAction.CallbackContext context)
    {
        //Dictionary<string, float[]> data = new Dictionary<string, float[]>();
        //Vector2 value = context.ReadValue<Vector2>();
        //data.Add(context.action.name.ToLower(), new float[] { value.x, value.y });
        //SendInput();

        ////networkManager.Emit("input", data);
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
        SendInput();

        //networkManager.Emit("input", data);
    }

    public void SendInput()
    {
        Debug.Log(serializer.Serialize(new object[] { "sdasd", 12312, "asdo" }));
        //networkManager.Emit("input", serializer.Serialize(inputData));
    }
}
