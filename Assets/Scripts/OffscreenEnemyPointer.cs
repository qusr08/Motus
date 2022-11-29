using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OffscreenEnemyPointer : MonoBehaviour
{
    //should only be instantiated when an enemy is instantiated
    public GameObject enemy;
    public SpriteRenderer pointer;
    public PlayerController target;
    new Renderer renderer;
    public LayerMask layermask;

    // Start is called before the first frame update
    void Start()
    {
        renderer = enemy.GetComponent<Renderer>();
        target = FindObjectOfType<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!renderer.isVisible)
        {   
            if(!pointer.enabled)
            {
                pointer.enabled = true;
            }

            Vector2 direction = target.transform.position - enemy.transform.position; 
            RaycastHit2D ray = Physics2D.Raycast(enemy.transform.position, direction, 1000, layermask);

            if(ray.collider != null)
            {
                transform.position = ray.point;
                Quaternion toRotation = Quaternion.LookRotation(Vector3.forward, - direction);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, 720);
            }
        }
        else
        {
            if (pointer.enabled)
            {
                pointer.enabled = false;
            }

        }
        //show only if bound enemy is not being rendered by camera
        //update position relative to enemy and center of camera; orbit should not overlap with bars on the top-left
        //destroy with enemy
    }
}
