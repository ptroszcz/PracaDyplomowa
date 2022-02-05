using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loader : MonoBehaviour
{
    public GameObject gameManager;

    public GameObject fightManager;
    void Awake()
    {
        if (GameManager.instance==null){
            Instantiate(gameManager);
        }
        if (FightManager.instance==null){
            Instantiate(fightManager);
        }
    }

}
