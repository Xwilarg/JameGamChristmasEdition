using UnityEngine;

namespace JameGam.WebGL
{
    public class EnableOnlyIfWebGL : MonoBehaviour
    {
        private void Awake()
        {
            if (Application.platform != RuntimePlatform.WebGLPlayer)
            {
                Destroy(gameObject);
            }
        }
    }
}
