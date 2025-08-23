using UnityEngine;

namespace _Scripts.DonationsSocials
{
    public class LinkManager : MonoBehaviour
    {
        private string instagram = "https://www.instagram.com/gamedev_emilie/";
        private string koFiUrl = "https://ko-fi.com/Z8Z61FBMFO";

        public void OpenInstagram()
        {
            Application.OpenURL(instagram);
        }

        public void OpenKoFi()
        {
            Application.OpenURL(koFiUrl);
        }
    }
}
