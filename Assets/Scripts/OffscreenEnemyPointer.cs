using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OffscreenEnemyPointer : MonoBehaviour
{
    //should only be instantiated when an enemy is instantiated
    //Enemy field
    //Camera field

    // Start is called before the first frame update
    void Start()
    {
        //bind to enemy
            //change color to enemy's color
        //bind to camera
    }

    // Update is called once per frame
    void Update()
    {
        //show only if bound enemy is not being rendered by camera
        //update position relative to enemy and center of camera; orbit should not overlap with bars on the top-left
        //destroy with enemy
    }
}
