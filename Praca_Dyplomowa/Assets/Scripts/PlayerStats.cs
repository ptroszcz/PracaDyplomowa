using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerStats : EntityStats
{
    public int baseMovement = 10;

    public int movement;

    public int neededExperience = 3;

    public int experience = 0;

    public int level = 1;

    public int baseAttack = 5;

    public int baseDefence = 5;

    public int baseSpeed = 5;

    public int baseAgility = 5;

    public int baseAccuracy = 5;

    public Item weapon;

    public Item shield;

    public Item chestplate;

    public Item helmet;

    public PlayerStats()
    {
        this.healthPoints = maxHealth;
        this.movement = baseMovement;
    }

    public void equipItem(Item item)
    {
        switch (item.type)
        {
            case ItemType.Weapon:
                this.weapon = item;
                break;
            case ItemType.Shield:
                this.shield = item;
                break;
            case ItemType.Chestplate:
                this.chestplate = item;
                break;
            case ItemType.Helmet:
                this.helmet = item;
                break;
        }
        calculateActualStats();
    }

    public void calculateActualStats()
    {
        this.attack =
            baseAttack +
            weapon.attackModifier +
            shield.attackModifier +
            helmet.attackModifier +
            chestplate.attackModifier;
        this.defence =
            baseDefence +
            weapon.defenceModifier +
            shield.defenceModifier +
            helmet.defenceModifier +
            chestplate.defenceModifier;
        this.speed =
            baseSpeed +
            weapon.speedModifier +
            shield.speedModifier +
            helmet.speedModifier +
            chestplate.speedModifier;
        this.agility =
            baseAgility +
            weapon.agilityModifier +
            shield.agilityModifier +
            helmet.agilityModifier +
            chestplate.agilityModifier;
        this.accuracy =
            baseAccuracy +
            weapon.accuracyModifier +
            shield.accuracyModifier +
            helmet.accuracyModifier +
            chestplate.accuracyModifier;
    }

    public void Heal(int healedPoints)
    {
        healthPoints =
            (
            (healthPoints + healedPoints) > maxHealth
                ? maxHealth
                : healthPoints + healedPoints
            );
    }

    public void copyTo(PlayerStats stats)
    {
        stats.baseAccuracy = this.baseAccuracy;
        stats.baseAgility = this.baseAgility;
        stats.baseAttack = this.baseAttack;
        stats.baseDefence = this.baseDefence;
        stats.baseSpeed = this.baseSpeed;
        stats.baseMovement = this.baseMovement;
        stats.weapon = this.weapon;
        stats.shield = this.shield;
        stats.helmet = this.helmet;
        stats.chestplate = this.chestplate;
        stats.maxHealth = this.maxHealth;
        stats.healthPoints = this.healthPoints;
        stats.level = this.level;
        stats.neededExperience = this.neededExperience;
        stats.experience = this.experience;
    }

    public void LevelUp(int choice)
    {
        switch (choice)
        {
            case 1:
                baseAttack++;
                break;
            case 2:
                baseDefence++;
                break;
            case 3:
                baseSpeed++;
                break;
            case 4:
                baseAgility++;
                break;
            case 5:
                baseAccuracy++;
                break;
            default:
                return;
        }
        level++;
        experience = experience - neededExperience;
        neededExperience = 4*level*(level-1);
        baseMovement += (GameManager.instance.level == 0 ? 2 : 1);
        maxHealth += 2;
        healthPoints = maxHealth;
    }
}
