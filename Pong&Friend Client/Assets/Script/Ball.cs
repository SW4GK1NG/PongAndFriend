using UnityEngine;
using System.Collections;

public class Ball : MonoBehaviour {

    public float speed = 30;
    public GameMaster gm;

    void Start() {
        Invoke("BallStart", 2f);
        gm = FindObjectOfType<GameMaster>();
    }
    
    float hitFactor(Vector2 ballPos, Vector2 racketPos, float racketHeight) {
        return (ballPos.y - racketPos.y) / racketHeight;
    }

    void OnCollisionEnter2D(Collision2D col) {

        if (col.gameObject.name == "RacketLeft") {

            float y = hitFactor(transform.position, col.transform.position, col.collider.bounds.size.y);
            Vector2 dir = new Vector2(1, y).normalized;
            GetComponent<Rigidbody2D>().velocity = dir * speed;

        }

        if (col.gameObject.name == "RacketRight") {

            float y = hitFactor(transform.position, col.transform.position, col.collider.bounds.size.y);
            Vector2 dir = new Vector2(-1, y).normalized;
            GetComponent<Rigidbody2D>().velocity = dir * speed;

        }

        if (col.gameObject.tag == "Goal")
        {
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            if (rb.velocity.x > 0)
            {
                gm.p2Score++;
            } else {
                gm.p1Score++;
            }
            gm.spawnNew();
            Destroy(gameObject);
        }
    }

    void BallStart()
    {
        int[] dir = {-1, 1};
        int temp = Random.Range(0, 2);
        GetComponent<Rigidbody2D>().velocity = new Vector2(dir[temp] * speed, 0);
    }
}
