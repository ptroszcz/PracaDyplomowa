using System.Collections;
using System.Text;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

[Serializable]
public class Item : MonoBehaviour
{
    public int power;
    public ItemType type;
    public string itemName;

    public int attackModifier=0;
    public int defenceModifier=0;
    public int speedModifier=0;
    public int agilityModifier=0;
    public int accuracyModifier=0;
    
    public Sprite sprite;

    public override string ToString(){
        StringBuilder sb = new StringBuilder();
        sb.Append(type+" "+itemName+":");
        if (attackModifier!=0){
            sb.Append(" "+attackModifier.ToString("+0;-#")+" Attack");
        }
        if (defenceModifier!=0){
            sb.Append(" "+defenceModifier.ToString("+0;-#")+" Defence");
        }
        if (speedModifier!=0){
            sb.Append(" "+speedModifier.ToString("+0;-#")+" Speed");
        }
        if (agilityModifier!=0){
            sb.Append(" "+agilityModifier.ToString("+0;-#")+" Agility");
        }
        if (accuracyModifier!=0){
            sb.Append(" "+accuracyModifier.ToString("+0;-#")+" Accuracy");
        }
        return sb.ToString();
    }
   
}

public enum ItemType{
    Helmet, Weapon, Shield, Chestplate
}

