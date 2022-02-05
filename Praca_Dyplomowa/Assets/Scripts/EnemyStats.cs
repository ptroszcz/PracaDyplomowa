using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Random = UnityEngine.Random;

[Serializable]
public class EnemyStats : EntityStats
{
    public int power = 0;

    public int experienceGain = 1;

    public void calculateStats()
    {
        this.healthPoints = this.maxHealth;
        for (int i = 0; i < power; ++i)
        {
            switch (Random.Range(0, 5))
            {
                case 0:
                    this.attack += Random.Range(1, 4);
                    break;
                case 1:
                    this.defence += Random.Range(1, 4);
                    break;
                case 2:
                    this.speed += Random.Range(1, 4);
                    break;
                case 3:
                    this.accuracy += Random.Range(1, 4);
                    break;
                case 4:
                    this.agility += Random.Range(1, 4);
                    break;
            }
        }
    }

    public int CalculateExperience()
    {
        return experienceGain +
        ((int)(Mathf.Pow(power + 1, 2)) - (power + 1)) * experienceGain / 2;
    }
}
