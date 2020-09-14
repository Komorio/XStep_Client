﻿using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(fileName = "AchieveData", menuName = "Scriptable Object/Achieve Data", order = 0)]
public class AchieveData : ScriptableObject {
    [SerializeField]
    private Achieve[] achieves;
    public Achieve[] Achieves => achieves;

    public void Unlock(string achieveName, int amount) {
        var findAchieves = new List<Achieve>();
        
        foreach (var achieve in achieves) {
            if (achieve.AchieveName.Equals(achieveName)) {
                findAchieves.Add(achieve);
            }
        }

        foreach (var findAchieve in findAchieves) {
            if (findAchieve.IsUnlock == false) {
                if (amount < findAchieve.Amount) {
                    return;
                }
                
                findAchieve.IsUnlock = true;
                GameManager.instance.PlayerSetting.currentExp += findAchieve.RewardXp;
                GameManager.instance.PlayerSetting.TitleData.UnLockTitle(findAchieve.OpenTitleName);
            }
        }
    }

    [Button("Reset")]
    public void Reset() {
        foreach (var achieve in achieves) {
            achieve.IsUnlock = false;
        }
    }
}