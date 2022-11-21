using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OffscreenEnemyPointer : MonoBehaviour
{
    //should only be instantiated when an enemy is instantiated
    public GameObject enemyPointer;
    public GameObject target;
    Renderer renderer;

    // Start is called before the first frame update
    void Start()
    {
        renderer = GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!renderer.isVisible)
        {
            if(!enemyPointer.activeSelf)
            {
                enemyPointer.SetActive(true);
            }

            Vector2 direction = target.transform.position - transform.position;
            RaycastHit2D ray = Physics2D.Raycast(transform.position, direction);

            if(ray.collider != null)
            {
                enemyPointer.transform.position = ray.point;
            }
        }
        else
        {
            if (enemyPointer.activeSelf)
            {
                enemyPointer.SetActive(false);
            }

        }
        //show only if bound enemy is not being rendered by camera
        //update position relative to enemy and center of camera; orbit should not overlap with bars on the top-left
        //destroy with enemy
    }
}
