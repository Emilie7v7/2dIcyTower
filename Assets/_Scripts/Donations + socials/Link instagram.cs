using UnityEngine;

public class InstagramLink : MonoBehaviour
{
    private string instagram = "https://www.instagram.com/gamedev_emilie/";

    public void OpenInstagram()
    {
        Application.OpenURL(instagram);
    }
}
