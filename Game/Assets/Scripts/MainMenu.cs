using TMPro;
using UnityEngine;

namespace JameGam.Scripts
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField]
        private GameObject _menu;

        [SerializeField]
        private TMP_InputField _ip, _port, _name;

        private string GetName()
        {
            string[] names = new[]
            {
                "Joey", "Cassandra", "Benjamin", "Thierry", "Abram",
                "Carole", "Michael", "William", "Amy", "Daellia"
            };
            return names[Random.Range(0, names.Length)];
        }

        private void Awake()
        {
            _name.text = GetName();
        }

        public void QuickConnect()
        {
            if (GameManager.Instance.Connect("51.159.6.4", 9999, string.IsNullOrWhiteSpace(_name.text) ? GetName() : _name.text))
            {
                _menu.SetActive(false);
            }
        }

        public void Connect()
        {
            if (GameManager.Instance.Connect(_ip.text, int.Parse(_port.text), string.IsNullOrWhiteSpace(_name.text) ? GetName() : _name.text))
            {
                _menu.SetActive(false);
            }
        }
    }
}
