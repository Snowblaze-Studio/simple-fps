using System.Threading.Tasks;
using UnityEngine;
using System;
using SocketIOClient;
using System.Collections.Generic;

public class NetworkingManager : MonoBehaviour
{
    private SocketIO client;
    private Uri uri;

    [SerializeField]
    private GameObject playerPrefab;

    private Queue<Action> actionQueue = new Queue<Action>();

    private void Awake()
    {
        uri = new Uri("http://localhost:3000/");
        client = new SocketIO(uri);
        client.OnConnected += async (sender, e) =>
        {
            // Emit a string
            await client.EmitAsync("message", "socket.io");
        };
    }

    private void Update()
    {
        if (actionQueue.Count > 0)
        {
            actionQueue.Dequeue()();
        }
    }

    public async Task<bool> Connect()
    {
        client.On("instantiate", OnInstantiate);
        client.On("update", OnUpdate);

        await client.ConnectAsync();
        Debug.Log("SocketIO::Connected");

        return true;
    }

    private void OnInstantiate(SocketIOResponse response)
    {
        actionQueue.Enqueue(() =>
        {
            SendMessage("InstantiatePlayer", response);
        });
    }

    public void InstantiatePlayer(SocketIOResponse response)
    {
        Debug.Log(response);
        GameObject playerGO = Instantiate(playerPrefab, response.GetValue<Vector3>(1), response.GetValue<Quaternion>(2));
        playerGO.GetComponent<CannonId>().id = response.GetValue<int>(0);
    }

    private void OnUpdate(SocketIOResponse response)
    {
        actionQueue.Enqueue(() =>
        {
            SendMessage("UpdatePlayer", response);
        });
    }

    public void UpdatePlayer(SocketIOResponse response)
    {
        CannonId cannonId = FindObjectOfType<CannonId>();

        if (!cannonId) return;

        Transform playerTransform = FindObjectOfType<CannonId>().transform;
        playerTransform.position = response.GetValue<Vector3>(1);
        playerTransform.rotation = response.GetValue<Quaternion>(2);
    }

    private void OnAnyHandler(string eventName, SocketIOResponse response)
    {
        Debug.Log(eventName);
    }

    public async Task<bool> Disconnect()
    {
        client.Off("instantiate");
        client.Off("update");

        await client.DisconnectAsync();
        client.Dispose();
        Debug.Log("SocketIO::Disconnected");

        return true;
    }

    public void Emit(string eventName, object data)
    {
        client.EmitAsync(eventName, data);
    }
}
