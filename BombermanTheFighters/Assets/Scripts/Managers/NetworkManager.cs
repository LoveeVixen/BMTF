// LOVEEVIXEN
using UnityEngine;
using System.Threading.Tasks;
using Fusion;

public class NetworkManager : MonoBehaviour
{
    private static NetworkManager instance;
    
    [SerializeField] NetworkRunner networkRunnerPrefab;
    private NetworkRunner networkRunner;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            // Setup network runner.
            NetworkRunner nr = Instantiate(networkRunnerPrefab);
            networkRunner = nr;
            DontDestroyOnLoad(nr.gameObject);
        }
        else if (instance != this)
            Destroy(gameObject);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartGame(OfflineGame());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Begin a new game, whether offline or online.
    public Task<StartGameResult> StartGame(StartGameArgs args)
    {
        Task<StartGameResult> result = networkRunner.StartGame(args);
        if (result.Result.Ok)
            Debug.Log("Started session successfully!");
        else
            Debug.Log("Session start failure." + result.Result.ShutdownReason);

        return result;
    }

    // Preparation settings for an offline game session.
    public StartGameArgs OfflineGame()
    {
        StartGameArgs args = new StartGameArgs();
        args.GameMode = GameMode.Single;
        args.SessionName = Application.productName;
        args.SceneManager = networkRunner.GetComponent<NetworkSceneManagerDefault>();
        return args;
    }

    // Preparation settings for an online game session.
    public StartGameArgs OnlineGame()
    {
        StartGameArgs args = new StartGameArgs();
        args.GameMode = GameMode.Host;
        args.SessionName = Application.productName;
        args.SceneManager = networkRunner.GetComponent<NetworkSceneManagerDefault>();
        return args;
    }
}
