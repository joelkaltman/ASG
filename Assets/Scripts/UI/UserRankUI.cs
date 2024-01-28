using UnityEngine;
using UnityEngine.UI;

public class UserRankUI : MonoBehaviour
{
    public Text usernameText;
    public Text killsText;

    public void SetUser(AuthManager.UserRank userRank)
    {
        usernameText.text = userRank.username;
        killsText.text = userRank.maxKills.ToString();
    }
}
