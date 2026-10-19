// LOVEEVIXEN
using Audio;
using EntitySystem;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(fileName="Stage", menuName="Stage")]
[System.Serializable]
public class Stage : ScriptableObject
{
    public string stageName = "New Stage";
    public Material floor;
    public Material skybox;
    public string music = "Stage Music";
    public enum Weather { normal, rain, snow, storm };
    public Weather weather = Weather.normal;
    public bool isDark = false;
    public bool selectableAtRandom = true;

    private static List<Stage> stages = new List<Stage>();
    private static string stagesResourcePath = "Stages";

    // Stage planes.
    private static GameObject[] stagePlanes = new GameObject[9];
    private static GameObject centerPlane;
    private static GameObject northWestPlane;
    private static GameObject northPlane;
    private static GameObject northEastPlane;
    private static GameObject eastPlane;
    private static GameObject southEastPlane;
    private static GameObject southPlane;
    private static GameObject southWestPlane;
    private static GameObject westPlane;

    // Additional stage setup.
    private static GameObject weatherParticles;
    private static List<DarknessSpotlight> darknessSpotlights = new List<DarknessSpotlight>();
    private static Light directionalLight;
    private static AudioObject rainSoundObj;
    private static StormSystem stormSystem;

    // Load and return all stage files from resources folder.
    public static Stage[] LoadStagesFromResources()
    {
        object[] stagesFromResources = Resources.LoadAll(stagesResourcePath, typeof(Stage));
        List<Stage> loadedStages = new List<Stage>();
        foreach (var stage in stagesFromResources)
            loadedStages.Add((Stage)stage);

        stages = loadedStages;
        return loadedStages.ToArray();
    }

    // Load stage data into currently loaded scene.
    public static void LoadStageIntoScene(Stage getStage)
    {
        bool enableDirectionalLight = true;
        bool enableFog = false;

        // Clear up previously loaded stage remains if necessary.
        if(weatherParticles != null) Destroy(weatherParticles);
        foreach (DarknessSpotlight spotlight in darknessSpotlights)
        {
            if (spotlight != null) Destroy(spotlight.gameObject);
        }

        darknessSpotlights.RemoveRange(0, darknessSpotlights.Count);
        if(rainSoundObj != null) Destroy(rainSoundObj.gameObject);
        if(stormSystem != null) Destroy(stormSystem.gameObject);

        // Setup directional light reference.
        if (directionalLight == null)
        {
            Light[] lightSources = FindObjectsByType<Light>(FindObjectsSortMode.None);
            for (int i = 0; i < lightSources.Length; i++)
            {
                if (lightSources[i].type == LightType.Directional)
                {
                    directionalLight = lightSources[i];
                    break;
                }
            }
        }

        // Setup stage plane and insert floor texture.
        for (int i = 0; i < stagePlanes.Length; i++)
        {
            if (stagePlanes[i] == null)
            {
                stagePlanes[i] = (GameObject)Instantiate(Resources.Load("StagePlane"));
                stagePlanes[i].name = "Plane" + i;
            }
        }

        for (int i = 0; i < stagePlanes.Length; i++)
            stagePlanes[i].GetComponent<MeshRenderer>().material = getStage.floor;

        // Setup neighbor references to get them easier
        northWestPlane = stagePlanes[0];
        northPlane = stagePlanes[1];
        northEastPlane = stagePlanes[2];
        westPlane = stagePlanes[3];
        centerPlane = stagePlanes[4];
        eastPlane = stagePlanes[5];
        southWestPlane = stagePlanes[6];
        southPlane = stagePlanes[7];
        southEastPlane = stagePlanes[8];

        MovePlaneTo(Vector3.zero);

        // Insert skybox into scene.
        RenderSettings.skybox = getStage.skybox;

        // Setup weather.
        GameObject particles = null;
        if (getStage.weather == Weather.rain)
        {
            // Rain particles.
            particles = Instantiate(GameManager.instance.GetRainParticlesPrefab());
            weatherParticles = particles;

            // Play rain sound.
            SoundProperties prop = new SoundProperties();
            prop.loop = true;
            rainSoundObj = AudioManager.instance.PlayNonDiegeticSound("Rain", prop);
        }
        else if (getStage.weather == Weather.snow)
        {
            // Snow particles.
            particles = Instantiate(GameManager.instance.GetSnowParticlesPrefab());
            weatherParticles = particles;
        }
        else if(getStage.weather == Weather.storm)
        {
            // Rain particles
            particles = Instantiate(GameManager.instance.GetRainParticlesPrefab());
            weatherParticles = particles;

            // Play rain sound.
            SoundProperties prop = new SoundProperties();
            prop.loop = true;
            rainSoundObj = AudioManager.instance.PlayNonDiegeticSound("Rain", prop);

            // Add storm system.
            GameObject stormSystemObj = Instantiate(GameManager.instance.GetStormSystemPrefab());
            stormSystem = stormSystemObj.GetComponent<StormSystem>();
        }

        // Setup darkness.
        if(getStage.isDark)
        {
            enableFog = true;
            enableDirectionalLight = false;
            RenderSettings.fogDensity = 0.06f;
            RenderSettings.fogColor = Color.black;
            RenderSettings.ambientMode = AmbientMode.Flat;
            RenderSettings.ambientSkyColor = new Color(0.3f, 0.3f, 0.3f);

            // Setup darkness spotlights for entities that need it.
            Entity[] entities = FindObjectsByType<Entity>(FindObjectsSortMode.None);
            for(int i = 0; i < entities.Length; i++)
            {
                Player player = entities[i] as Player;
                if(player != null)
                {
                    GameObject spotlightObj = Instantiate(GameManager.instance.GetDarknessSpotlightPrefab());
                    DarknessSpotlight spotlight = spotlightObj.GetComponent<DarknessSpotlight>();
                    spotlight.FollowEntity(player);
                    darknessSpotlights.Add(spotlight);
                }
            }
        }

        directionalLight.gameObject.SetActive(enableDirectionalLight);
        RenderSettings.fog = enableFog;
    }

    public static void MovePlaneTo(Vector3 setPos)
    {
        float offset = 200f;

        // Make sure no stage planes are missing before attempting to move stage.
        foreach (GameObject plane in stagePlanes)
            if (plane == null) { Debug.Log("Scene is missing a stage plane."); return; }

        stagePlanes[0].transform.position = new Vector3(setPos.x - offset, setPos.y, setPos.z + offset);
        stagePlanes[1].transform.position = new Vector3(setPos.x, setPos.y, setPos.z + offset);
        stagePlanes[2].transform.position = new Vector3(setPos.x + offset, setPos.y, setPos.z + offset);
        stagePlanes[3].transform.position = new Vector3(setPos.x - offset, setPos.y, setPos.z);
        stagePlanes[4].transform.position = new Vector3(setPos.x, setPos.y, setPos.z);
        stagePlanes[5].transform.position = new Vector3(setPos.x + offset, setPos.y, setPos.z);
        stagePlanes[6].transform.position = new Vector3(setPos.x - offset, setPos.y, setPos.z - offset);
        stagePlanes[7].transform.position = new Vector3(setPos.x, setPos.y, setPos.z - offset);
        stagePlanes[8].transform.position = new Vector3(setPos.x + offset, setPos.y, setPos.z - offset);
    }

    // Find a stage by it's name.
    public static Stage Find(string getStageName)
    {
        foreach(Stage stage in stages)
        {
            if(stage.stageName == getStageName)
                return stage;
        }

        Debug.Log("Could not find stage with name: " + getStageName);
        return null;
    }

    // Find a stage by it's asset file.
    public static Stage Find(Stage getStage)
    {
        foreach (Stage stage in stages)
        {
            if (stage == getStage)
                return stage;
        }

        Debug.Log("The stage " + getStage.stageName + " could not be found in loaded list. Make sure stage file is in project directory Resources/" + stagesResourcePath + ".");
        return null;
    }

    // Get stage plane gameobject references.
    #region
    public static GameObject GetCenterPlane() { return centerPlane; }
    public static GameObject GetNorthWestPlane() { return northWestPlane; }
    public static GameObject GetNorthPlane() { return northPlane; }
    public static GameObject GetNorthEastPlane() { return northEastPlane; }
    public static GameObject GetEastPlane() { return eastPlane; }
    public static GameObject GetSouthEastPlane() { return southEastPlane; }
    public static GameObject GetSouthPlane() { return southPlane; }
    public static GameObject GetSouthWestPlane() { return southWestPlane; }
    public static GameObject GetWestPlane() { return westPlane; }

    #endregion
}
