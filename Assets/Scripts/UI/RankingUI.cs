using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RankingUI : MonoBehaviour
{
    public int maxEntries;
    public GameObject userRankObject;
    
    private List<GameObject> instances = new ();

    private void OnDestroy()
    {
        DestroyRanking();
    }

    public async void LoadRanking()
    {
        var ranking = await AuthManager.GetScoreboard();
        
        for (int i = 0; i < maxEntries && i < ranking.Count; i++)
        {
            var rankObject = Instantiate(userRankObject, transform);
            var rankUi = rankObject.GetComponent<UserRankUI>();
            rankUi.SetUser(ranking[i]);
            instances.Add(rankObject);
        }
    }

    public void DestroyRanking()
    {
        foreach (var rankInstance in instances)
        {
            Destroy(rankInstance);
        }
    }
}
