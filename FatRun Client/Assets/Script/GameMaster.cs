using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameMaster : MonoBehaviour
{
    bool gameend;

    public Text midText;
    public GameObject ResetButton;
    public FatHead player;


    // Start is called before the first frame update
    void Start()
    {
        ResetButton.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void GameEnd(bool Fwin)
    {
        if (Fwin == true)
        {
            midText.text = "Team F Win";
            ResetButton.SetActive(true);
        } else
        {
            midText.text = "Team J Win";
            ResetButton.SetActive(true);
        }
    }

    public void Reset()
    {
        midText.text = "";
        ResetButton.SetActive(false);
        player.gameEnded = false;
    }
}
