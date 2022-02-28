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
    [SerializeField]
    private Transform cubePrefab;

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
        client.On("addStatic", OnAddStaticObject);

        await client.ConnectAsync();
        Debug.Log("SocketIO::Connected");

        return true;
    }

    private void OnInstantiate(SocketIOResponse response)
    {
        actionQueue.Enqueue(() =>
        {
            if (response.GetValue<string>(0) == "player")
            {
                SendMessage("InstantiatePlayer", response);
            }
        });
    }

    public void InstantiatePlayer(SocketIOResponse response)
    {
        Debug.Log(response);
        GameObject playerGO = Instantiate(playerPrefab, response.GetValue<Vector3>(2), response.GetValue<Quaternion>(3));
        playerGO.GetComponent<CannonId>().id = response.GetValue<int>(1);
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

    private void OnAddStaticObject(SocketIOResponse response)
    {
        actionQueue.Enqueue(() =>
        {
            SendMessage("AddStaticObject", response);
        });
    }

    public void AddStaticObject(SocketIOResponse response)
    {
        //gameObject.shape, (0)
        //gameObject.id, (1)
        //gameObject.position, (2)
        //gameObject.quaternion, (3)
        //gameObject.scale, (4)

        Debug.Log(response);

        Vector3 position = response.GetValue<Vector3>(2);
        Quaternion rotation = response.GetValue<Quaternion>(3);
        Vector3 scale = response.GetValue<Vector3>(4);

        Transform cubeObject = Instantiate(cubePrefab);
        cubeObject.position = position;
        cubeObject.rotation = rotation;
        cubeObject.localScale = scale;
    }

    private void OnAnyHandler(string eventName, SocketIOResponse response)
    {
        Debug.Log(eventName);
    }

    public async Task<bool> Disconnect()
    {
        client.Off("instantiate");
        client.Off("update");
        client.Off("addStatic");

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
