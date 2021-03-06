﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

[CreateAssetMenu(fileName = "ProfileTitleData", menuName = "Scriptable Object/ProfileTitleData", order = 0)]
public class ProfileTitleData : ScriptableObject {
    [SerializeField]
    private Title[] titleResources;
    public Title[] TitleResources => titleResources;

    public void UnLockTitle(string key) {
        foreach (var title in titleResources) {
            if (title.title.Equals(key)) {
                title.isUnLock = true;
                break;
            }
        }
    }
    
    [Button("Reset")]
    public void DataReset() {
        for (int i = 1; i < titleResources.Length; i++) {
            titleResources[i].isUnLock = false;
        }
    }
}