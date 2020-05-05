using UnityEngine;
using System.Collections;

public class MoveRacket : MonoBehaviour {

    public bool canControl = false;
    public float speed = 30;
    public bool gameEnded;
    bool walked;

    void Update () {

        //if (!canControl)
        //    return;

        if (Input.GetKeyDown(KeyCode.F) && gameEnded == false)
        {
            //Debug.Log("Bruh");
            transform.position += Vector3.right * speed * Time.deltaTime;
            //walked = true;
        }

        if (Input.GetKeyDown(KeyCode.J) && gameEnded == false)
        {
            //Debug.Log("Bruh");
            transform.position += Vector3.right * speed * Time.deltaTime;
            //walked = true;
        }
    }
}
