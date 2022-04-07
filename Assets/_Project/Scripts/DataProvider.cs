using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Leipzig;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using Canvas = Leipzig.Canvas;
using Debug = UnityEngine.Debug;

public class DataProvider : MonoBehaviour
{
    public static readonly CancellationTokenSource ApplicationQuit = new CancellationTokenSource();
    
    
    [SerializeField] private DatabaseSettings _settings;
    private DatabaseSettings Settings => _settings;
    
    [SerializeField] private UnityEvent<List<Manifest>> _databaseLoaded;
    

    private List<Manifest> _allManifests;
    private List<Manifest> AllManifests => _allManifests ??= new List<Manifest>();


    // Start is called before the first frame update
    private void Start()
    {
        AdditionalInfoManager.Parameter.Sessions.AddValue(1);
        LoadData();
    }

    private void OnApplicationQuit()
    {
        ApplicationQuit.Cancel();
    }

    private async void LoadData()
    {
        Stopwatch stopwatch = Stopwatch.StartNew(); 
        
        Debug.Log("DataProvider -> LoadData()");

        if (Settings == null || Settings.RequestSettings == null)
        {
            Debug.LogError("Settings is null or incomplete!");
            return;
        }

        AllManifests.Clear();

        UnityWebRequest request = await GetDataBaseJson(Settings.RequestSettings.URL);
        if (request == null)
        {
            Debug.LogError($"Request is null!");
            return;
        }

        string json = request.downloadHandler.text;
        Root databaseRoot = JsonConvert.DeserializeObject<Root>(json);

        if (databaseRoot == null)
        {
            Debug.LogError("Found no Root in database!");
            return;
        }

        foreach (Manifest m in databaseRoot.manifests)
        {
            if (ApplicationQuit.IsCancellationRequested) return;
            AllManifests.Add(m);
        }

        // report size
        Debug.Log($"Database downloaded: {request.downloadedBytes} bytes");
        AdditionalInfoManager.Parameter.DatabaseSize.SetValue((int)request.downloadedBytes);
        
        // report load time
        stopwatch.Stop();
        AdditionalInfoManager.Parameter.DatabaseSize.SetValue((int)stopwatch.ElapsedMilliseconds);
        
        // report manifests
        Debug.Log($"DataProvider -> Found {AllManifests.Count} coin manifests.");
        AdditionalInfoManager.Parameter.CoinManifests.SetValue(AllManifests.Count);
    
        _databaseLoaded.Invoke(AllManifests);
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
        Manifest m = AllManifests[Random.Range(0, AllManifests.Count)];

        // deserialize manifest
        UnityWebRequest request = await GetDataBaseJson(m.Id);
        if (request == null)
        {
            Debug.LogError("Request is null!");
            return null;
        }
        
        string json = request.downloadHandler.text;
        ManifestDeserialized mDeserialized = JsonConvert.DeserializeObject<ManifestDeserialized>(json);
        if (mDeserialized == null)
        {
            Debug.LogError($"JSON could not be deserialized: {json}");
            return null;
        }

        List<string> information = new List<string>();
        foreach (Metadata meta in mDeserialized.metadata.Where(x => !string.IsNullOrEmpty(x.value)))
        {
            information.Add(meta.value);
        }

        if (mDeserialized.sequences == null || mDeserialized.sequences.Count <= 0 || mDeserialized.sequences[0] == null) return null;
        if (mDeserialized.sequences[0].canvases == null || mDeserialized.sequences[0].canvases.Count <= 0) return null;

        string imageRes = Settings.ImageResolution.ToString();

        List<Texture2D> images = new List<Texture2D>();
        foreach (Canvas cvs in mDeserialized.sequences[0].canvases)
        {
            if (cvs?.images == null || cvs.images.Count <= 0 || cvs.images[0] == null) continue;

            //string imagePath = cvs.images[0].resource.service.Id + "/full/" + imageRes + "," + imageRes + "/0/default.jpg";
            string imagePath = $"{cvs.images[0].resource.service.Id}/full/{imageRes},{imageRes}/0/default.jpg";

            images.Add(await GetDatabaseTexture2D(imagePath));
        }

        // no images
        if (images.Count <= 0) return null;

        // one image
        if (images.Count <= 1) return new CoinData(mDeserialized, images[0], images[0], information.ToArray());

        // two image
        return new CoinData(mDeserialized, images[0], images[1], information.ToArray());
    }

    public static async Task<UnityWebRequest> GetDataBaseJson(string url)
    {
        UnityWebRequest request = UnityWebRequest.Get(url);
        
        //request.SetRequestHeader("Accept-Encoding", "gzip");
        request.SendWebRequest();

        while (!request.isDone)
        {
            if (ApplicationQuit.IsCancellationRequested)
            {
                Debug.Log("Database loading process was stopped by the OnApplicationQuit Event!");
                return null;
            }
            
            await Task.Yield();
        }

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(request.error + ", " + request.result);
            return null;
        }

        return request;
        //return request.downloadHandler.text;
    }

    private async Task<Texture2D> GetDatabaseTexture2D(string url, bool log = false)
    {
        //Debug.Log($"GetDatabaseTexture2D({url})");
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
        request.SendWebRequest();

        while (!request.isDone)
        {
            if (ApplicationQuit.IsCancellationRequested) return null;
            await Task.Yield();
        }

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(request.error + ", " + request.result);
            return null;
        }

        if(log) Debug.Log($"Image downloaded: {request.downloadedBytes} bytes");
        return DownloadHandlerTexture.GetContent(request);
    }
}