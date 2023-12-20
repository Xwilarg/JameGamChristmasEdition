using TMPro;
using UnityEngine;

namespace JameGam.Scripts
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField]
        private GameObject _menu;

        [SerializeField]
        private TMP_InputField _ip, _port;

        public void QuickConnect()
        {
            if (GameManager.Instance.Connect("51.159.6.4", 9999))
            {
                _menu.SetActive(false);
            }
        }

        public void Connect()
        {
            if (GameManager.Instance.Connect(_ip.text, int.Parse(_port.text)))
            {
                _menu.SetActive(false);
            }
        }
    }
}
