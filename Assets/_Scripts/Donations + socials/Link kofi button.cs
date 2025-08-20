using UnityEngine;

public class KoFiLink : MonoBehaviour
{
    private string koFiUrl = "https://ko-fi.com/Z8Z61FBMFO";

    public void OpenKoFi()
    {
        Application.OpenURL(koFiUrl);
    }
}
