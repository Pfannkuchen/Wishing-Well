using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Leipzig;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using Canvas = Leipzig.Canvas;

public class DataProvider : MonoBehaviour
{
    private const int ImageResolution = 800;


    [SerializeField] private CoinThrower _thrower;
    private CoinThrower Thrower => _thrower;

    [SerializeField] private string _dataBaseURL = "";
    private string DataBaseURL => _dataBaseURL;

    private List<Manifest> _allManifests;
    private List<Manifest> AllManifests => _allManifests ??= new List<Manifest>();

    [SerializeField] private UnityEvent<List<Manifest>> _databaseLoaded;

    // Start is called before the first frame update
    private void Start()
    {
        LoadData();
    }

    private async void LoadData()
    {
        Debug.Log("LoadData started.");

        AllManifests.Clear();

        string json = await GetDataBaseJSON(DataBaseURL);
        Root databaseRoot = JsonConvert.DeserializeObject<Root>(json);

        foreach (Manifest m in databaseRoot.manifests)
        {
            AllManifests.Add(m);
        }

        Debug.Log($"Manifests in Database: " + AllManifests.Count);

        Thrower.CreateNewCoins();
    }

    /*
    [System.Obsolete("Deprecated default library consisting of collections.")]
    private async void LoadDataDefault()
    {
        Debug.Log("LoadData started.");
        
        AllManifests.Clear();

        string d = await GetDataBaseJSON(DataBaseURL);
        Root databaseRoot = JsonConvert.DeserializeObject<Root>(d);

        foreach (Collection c in databaseRoot.collections)//.GetRange(0, 2))
        {
            string cJSON = await GetDataBaseJSON(c.Id);
            CollectionContent cContent = JsonConvert.DeserializeObject<CollectionContent>(cJSON);

            foreach (Manifest m in cContent.manifests)
            {
                AllManifests.Add(m);
            }
            
            Debug.Log($"Manifests in CollectionContent: " + AllManifests.Count);
        }

        //Texture2D tex = await RequestLoadData();
        //_testCoin.SetCoinData(tex, tex, "");

        Debug.Log($"LoadData found {AllManifests.Count} Manifests.");
        
        Thrower.CreateNewCoins();
    }
    */

    public async Task<CoinData> GetRandomCoinData()
    {
        // choose random manifest
        Manifest m = AllManifests[UnityEngine.Random.Range(0, AllManifests.Count)];

        // deserialize manifest
        string mJSON = await GetDataBaseJSON(m.Id);
        ManifestDeserialized mDeserialized = JsonConvert.DeserializeObject<ManifestDeserialized>(mJSON);
        if (mDeserialized == null) return null;

        List<string> information = new List<string>();
        foreach (Metadata meta in mDeserialized.metadata.Where(x => !string.IsNullOrEmpty(x.value)))
        {
            information.Add(meta.value);
        }

        if (mDeserialized.sequences == null || mDeserialized.sequences.Count <= 0 || mDeserialized.sequences[0] == null) return null;
        if (mDeserialized.sequences[0].canvases == null || mDeserialized.sequences[0].canvases.Count <= 0) return null;

        List<Texture2D> images = new List<Texture2D>();
        foreach (Canvas cvs in mDeserialized.sequences[0].canvases)
        {
            if (cvs?.images == null || cvs.images.Count <= 0 || cvs.images[0] == null) continue;

            string imagePath = cvs.images[0].resource.service.Id + "/full/" + ImageResolution.ToString() + "," + ImageResolution.ToString() + "/0/default.jpg";

            images.Add(await GetDatabaseTexture2D(imagePath));
        }

        // no images
        if (images.Count <= 0) return null;

        // one image
        if (images.Count <= 1) return new CoinData(mDeserialized, images[0], images[0], information.ToArray());

        // two image
        return new CoinData(mDeserialized, images[0], images[1], information.ToArray());
    }

    public async Task<string> GetDataBaseJSON(string url)
    {
        UnityWebRequest request = UnityWebRequest.Get(url);
        
        Debug.unityLogger.logEnabled = false;
        request.SetRequestHeader("X-Accept-Encoding", "gzip");
        Debug.unityLogger.logEnabled = true;

        request.SendWebRequest();

        while (!request.isDone) await Task.Yield();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(request.error + ", " + request.result);
            return null;
        }
        
        Debug.Log($"Database downloaded: {request.downloadedBytes} bytes");
        return request.downloadHandler.text;
    }

    private async Task<Texture2D> GetDatabaseTexture2D(string url)
    {
        //Debug.Log($"GetDatabaseTexture2D({url})");
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
        request.SendWebRequest();

        while (!request.isDone) await Task.Yield();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(request.error + ", " + request.result);
            return null;
        }
    
        Debug.Log($"Image downloaded: {request.downloadedBytes} bytes");
        return DownloadHandlerTexture.GetContent(request);
    }
}