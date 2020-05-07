using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIO;

[RequireComponent(typeof(SocketIOComponent))]
public class FatHead : MonoBehaviour
{
    static SocketIOComponent socket;
    public float speed = 30;
    public bool gameEnded;
    public GameMaster gm;

    // Start is called before the first frame update
    void Start()
    {
        gm = FindObjectOfType<GameMaster>();
        socket = GetComponent<SocketIOComponent>();

        socket.On("JPressed", JPressed);
        socket.On("FPressed", FPressed);
        socket.On("Fwon", Fwon);
        socket.On("Jwon", Jwon);
        socket.On("Reset", Reset);
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.x >= 50)
        {
            gameEnded = true;
            gm.GameEnd(false);
            socket.Emit("Jwin");
        }
        if (transform.position.x <= -50)
        {
            gameEnded = true;
            gm.GameEnd(true);
            socket.Emit("Fwin");
        }

        if (Input.GetKeyDown(KeyCode.F) && gameEnded == false)
        {
            socket.Emit("PressF");
        }

        if (Input.GetKeyDown(KeyCode.J) && gameEnded == false)
        {
            socket.Emit("PressJ");
        }
    }

    void FPressed (SocketIOEvent obj)
    {
        JSONObject posx = obj.data;
        transform.position = new Vector2(float.Parse(posx["position"].ToString()), 0);
        Debug.Log(posx["position"]);
    }

    void JPressed(SocketIOEvent obj)
    {
        JSONObject posx = obj.data;
        transform.position = new Vector2(float.Parse(posx["position"].ToString()), 0);
        Debug.Log(posx["position"]);
    }

    void Fwon(SocketIOEvent obj)
    {
        gameEnded = true;
        gm.GameEnd(true);
    }

    void Jwon(SocketIOEvent obj)
    {
        gameEnded = true;
        gm.GameEnd(false);
    }

    void Reset(SocketIOEvent obj)
    {
        transform.position = Vector2.zero;
        gm.Reset();
    }

    public void ResetPressed()
    {
        transform.position = Vector2.zero;
        socket.Emit("Reset");
        gm.Reset();
        gameEnded = false;
    }
}
