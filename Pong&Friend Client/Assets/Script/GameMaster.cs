using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameMaster : MonoBehaviour
{
    bool gameend;

    public int p1Score = 0;
    public int p2Score = 0;

    public Text p1Text;
    public Text p2Text;
    public Text resultText;
    public GameObject ballObject;

    [Space]

    public int maxScore;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        p1Text.text = p1Score.ToString();
        p2Text.text = p2Score.ToString();

        if (p1Score >= maxScore)
        {

            resultText.text = "P1 Won";
        }

        if (p2Score >= maxScore)
        {
            resultText.text = "P2 Won";
        }

    }

    public void spawnNew ()
    {
        if (p1Score < maxScore && p2Score < maxScore)
        {
            Instantiate(ballObject);
        }  
    }
}
