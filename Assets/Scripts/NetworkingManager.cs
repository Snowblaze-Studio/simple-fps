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
    private GameObject enemyPrefab;
    [SerializeField]
    private Transform cubePrefab;

    private Queue<Action> actionQueue = new Queue<Action>();

    private Dictionary<int, GameObject> dynamicObjectsDictionary = new Dictionary<int, GameObject>();

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
        client.On("update", OnUpdateDynamic);
        client.On("addStatic", OnAddStaticObject);
        client.On("addDynamic", OnAddDynamicObject);
        client.On("destroyDynamic", OnDestroyDynamicObject);

        await client.ConnectAsync();

        Debug.Log(client.Id);
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
        //'player', (0)
        //gameObject.id, (1)
        //gameObject.position, (2)
        //gameObject.quaternion, (3)

        //Debug.Log(response);

        int id = response.GetValue<int>(1);
        Vector3 position = response.GetValue<Vector3>(2);
        Quaternion rotation = response.GetValue<Quaternion>(3);

        GameObject playerGO = Instantiate(playerPrefab, position, rotation);
        playerGO.GetComponent<CannonId>().id = id;

        dynamicObjectsDictionary.Add(id, playerGO);
    }

    private void OnUpdateDynamic(SocketIOResponse response)
    {
        actionQueue.Enqueue(() =>
        {
            SendMessage("UpdateDynamic", response);
        });
    }

    public void UpdateDynamic(SocketIOResponse response)
    {
        //gameObject.id, (0)
        //gameObject.position, (1)
        //gameObject.quaternion, (2)
        //gameObject.velocity, (3)

        //Debug.Log(response);

        int id = response.GetValue<int>(0);

        if (!dynamicObjectsDictionary.ContainsKey(id)) return;

        Vector3 position = response.GetValue<Vector3>(1);
        Quaternion rotation = response.GetValue<Quaternion>(2);

        GameObject gameObject = dynamicObjectsDictionary[id];
        Transform playerTransform = gameObject.GetComponent<Transform>();
        playerTransform.position = position;
        playerTransform.rotation = rotation;
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

        //Debug.Log(response);

        Vector3 position = response.GetValue<Vector3>(2);
        Quaternion rotation = response.GetValue<Quaternion>(3);
        Vector3 scale = response.GetValue<Vector3>(4);

        Transform cubeObject = Instantiate(cubePrefab);
        cubeObject.position = position;
        cubeObject.rotation = rotation;
        cubeObject.localScale = scale;
    }

    private void OnAddDynamicObject(SocketIOResponse response)
    {
        actionQueue.Enqueue(() =>
        {
            SendMessage("AddDynamicObject", response);
        });
    }

    public void AddDynamicObject(SocketIOResponse response)
    {
        //gameObject.shape, (0)
        //gameObject.id, (1)
        //gameObject.position, (2)
        //gameObject.quaternion, (3)
        //gameObject.velocity, (4)

        //Debug.Log(response);

        int id = response.GetValue<int>(1);
        Vector3 position = response.GetValue<Vector3>(2);
        Quaternion rotation = response.GetValue<Quaternion>(3);
        Vector3 velocity = response.GetValue<Vector3>(4);

        GameObject enemyGameObject = Instantiate(enemyPrefab, position, rotation);
        enemyGameObject.GetComponent<CannonId>().id = id;

        dynamicObjectsDictionary.Add(id, enemyGameObject);
    }

    private void OnDestroyDynamicObject(SocketIOResponse response)
    {
        actionQueue.Enqueue(() =>
        {
            SendMessage("DestroyDynamicObject", response);
        });
    }

    public void DestroyDynamicObject(SocketIOResponse response)
    {
        //gameObject.id, (0)

        //Debug.Log(response);

        int id = response.GetValue<int>(0);

        if (!dynamicObjectsDictionary.ContainsKey(id)) return;

        GameObject gameObject = dynamicObjectsDictionary[id];

        Destroy(gameObject);
        dynamicObjectsDictionary.Remove(id);
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
        client.Off("addDynamic");
        client.Off("destoryDynamic");

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
