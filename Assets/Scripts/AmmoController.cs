using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoController : MonoBehaviour
{
    [SerializeField] GameObject player;
    Sprite currSprite;
    [SerializeField] Sprite empty;
    [SerializeField] Sprite lit;
    [SerializeField] Sprite blinking;

    void Start()
    {
        currSprite = empty;
    }

    // Update is called once per frame
    void Update()
    {
        /*switch (player.getBulletCount())
        {
            case 0:
                currSprite = empty;
                break;

            case (>0 && <player.getBulletCap()):
                currSprite = lit;
                break;

            case player.getBulletCap():
                currSprite = blinking;
                break;

            default:
                currSprite = empty;
                break;
                
        }*/
        
    }
}
