using UnityEngine;
using UnityEngine.UI;

public class CapsCounter : MonoBehaviour
{
    public Text capText;
    
    void Start()
    {
        SetTextCaps();
        UserManager.Instance().OnCapCountChange += RefreshCaps;
    }

    private void SetTextCaps()
    {
        var user = UserManager.Instance()?.UserData;
        int count = user != null ? user.caps : 0;
        capText.text = count < 10 ? $"x0{count}" : $"x{count}";
    }
    
    private void RefreshCaps()
    {
        SetTextCaps();
        capText.GetComponent<BounceText>()?.Bounce(0.65f);
    }
}
