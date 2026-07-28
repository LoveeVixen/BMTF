// LOVEEVIXEN
using UnityEngine;
using Photon.Pun;
using UI;
using System.Collections;
using UnityEngine.UI;

public class ConnectingPrompt : MonoBehaviourPunCallbacks
{
    private Menu menu;
    [SerializeField] Text connectingPromptText;
    [SerializeField] Menu openMenuOnConnect;

    private void Awake()
    {
        menu = GetComponent<Menu>();
        StartCoroutine(IAnimateText());
    }

    private void Start()
    {
        if (!NetworkManager.instance.OpenLobbyOnLoadOnlineScene)
        {
            // Connect client to Photon Network.
            if (PhotonNetwork.IsConnected)
                openMenuOnConnect.Open();
            else
                PhotonNetwork.ConnectUsingSettings();
        }
        else
        {
            // Go straight to lobby.
            NetworkManager.instance.OpenLobbyOnLoadOnlineScene = false;
            NetworkSessionLobby lobby = FindFirstObjectByType<NetworkSessionLobby>();
            lobby.GetMenu().Open();
            lobby.UpdateSessionNameText();
        }
    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        openMenuOnConnect.Open();
    }

    IEnumerator IAnimateText()
    {
        float animationSpeed = 0.2f;
        if (menu.IsOpen())
        {
            connectingPromptText.text = "Connecting to Server.";
            yield return new WaitForSeconds(animationSpeed);
            connectingPromptText.text = "Connecting to Server..";
            yield return new WaitForSeconds(animationSpeed);
            connectingPromptText.text = "Connecting to Server...";
            yield return new WaitForSeconds(animationSpeed);

            StartCoroutine(IAnimateText());
        }
    }
}
