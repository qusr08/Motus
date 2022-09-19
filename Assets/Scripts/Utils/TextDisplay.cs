using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Don't forget this line

public class TextDisplay : MonoBehaviour
{
    public Text displayText;

    [SerializeField] PlayerController player;

    public void Update()
    {

        displayText.text = "    Player Health: " + player.Health.ToString();
    }
}
