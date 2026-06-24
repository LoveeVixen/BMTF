// LOVEEVIXEN
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class VersionDisplay : MonoBehaviour
    {
        private void Awake()
        {
            GetComponent<Text>().text = Application.productName + " " + Application.version;
        }
    }
}