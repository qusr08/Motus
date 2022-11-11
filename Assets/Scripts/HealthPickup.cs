using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Editors:              Matthew Meyrowitz
//Date created:         11/7/2022
//Date Last Editted:    11/7/2022
public class HealthPickup : ObjectController
{

    [Space]
    [SerializeField] public bool IsInitialized;
    [SerializeField] public bool DidCollide = false;

    //detect if health pickup collided with player
    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject collisionGameObject = collision.gameObject;


        //check what the health pickup collided with
        PlayerController playerCollision = collisionGameObject.GetComponent<PlayerController>();

        //check to see if the collider is a circle collider
        bool IsCircleCollider = collisionGameObject.GetComponent<CircleCollider2D>();

        //check for collision
        if (!DidCollide)
        {
            if(playerCollision != null)
            {
                playerCollision.Heal(1);
                DidCollide = true;
            }
            //destroy health pickup
            Destroy(gameObject);
        }
    }
}
