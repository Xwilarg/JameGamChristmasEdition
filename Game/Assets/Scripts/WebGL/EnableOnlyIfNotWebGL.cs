using UnityEngine;

namespace JameGam.WebGL
{
    public class EnableOnlyIfNotWebGL : MonoBehaviour
    {
        private void Awake()
        {
            if (Application.platform == RuntimePlatform.WebGLPlayer)
            {
                Destroy(gameObject);
            }
        }
    }
}
