using UnityEngine;
using System.Collections;

public class MoveRacket : MonoBehaviour {

    public bool canControl = false;
    public float speed = 30;

    void FixedUpdate () {

        if (!canControl)
            return;

        float v = Input.GetAxisRaw("Vertical");
        GetComponent<Rigidbody2D>().velocity = new Vector2(0, v) * speed;
    }
}
