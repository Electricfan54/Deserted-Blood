using System;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UIElements;
[CreateAssetMenu]
public  class Ability : ScriptableObject
{
    enum AbilityType
    {
        fire,lightning,ice,shockwave,tp,sheild,sonicboom
    }

   [SerializeField] AbilityType type;
    public int maxCharge;
    public int currentcharge;
    public GameObject abilityPrefab;



}
