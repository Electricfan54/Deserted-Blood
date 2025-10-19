using System;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UIElements;
using NUnit.Framework.Constraints;
[CreateAssetMenu]
public  class Ability : ScriptableObject
{
  public  enum AbilityType
    {
        fire,lightning,ice,shockwave,tp,sheild,sonicboom
    }

  public AbilityType type;
    public int damage;
   
    public int maxCharge;
    public int currentcharge;
    public GameObject abilityPrefab;
 public GameObject abilityPrefab2
        ;public Sprite abilityIcon;

    public AudioClip abilitySound;
}
