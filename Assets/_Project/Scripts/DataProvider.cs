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
    public static CancellationTokenSource ApplicationQuit;
    
    
    [SerializeField] private DatabaseSettings _settings;
    private DatabaseSettings Settings => _settings;
    
    [SerializeField] private GameObject _messageLoadingDatabase;
    private GameObject MessageLoadingDatabase => _messageLoadingDatabase;
    
    [SerializeField] private GameObject _messageCouldNotLoadDatabase;
    private GameObject MessageCouldNotLoadDatabase => _messageCouldNotLoadDatabase;
    
    [SerializeField] private GameObject _messageCouldNotDeserializeDatabase;
    private GameObject MessageCouldNotDeserializeDatabase => _messageCouldNotDeserializeDatabase;
    
    [SerializeField] private UnityEvent<List<Manifest>> _databaseLoaded;
    
    private List<Manifest> _allManifests;
    private List<Manifest> AllManifests => _allManifests ??= new List<Manifest>();


    // Start is called before the first frame update
    private void Start()
    {
        ApplicationQuit = new CancellationTokenSource();

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
        
        MessageLoadingDatabase.SetActive(true);

        if (Settings == null || Settings.RequestSettings == null)
        {
            Debug.LogError("Settings is null or incomplete!");
            return;
        }

        AllManifests.Clear();

        UnityWebRequest request = await GetDataBaseJson(Settings.RequestSettings.URL);
        MessageLoadingDatabase.SetActive(false);
        
        if (request == null)
        {
            if (DataProvider.ApplicationQuit.Token.IsCancellationRequested) return;
            MessageCouldNotLoadDatabase.SetActive(true);
            Debug.LogError($"Could not load database root json!");
            return;
        }

        string json = request.downloadHandler.text;
        Root databaseRoot = JsonConvert.DeserializeObject<Root>(json);
        
        if (databaseRoot == null)
        {
            Debug.LogError("Found no Root in database!");
            MessageCouldNotDeserializeDatabase.SetActive(true);
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
        AdditionalInfoManager.Parameter.DatabaseLoadTime.SetValue((int)stopwatch.ElapsedMilliseconds);
        
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

    public Manifest GetRandomManifest()
    {
        return AllManifests[Random.Range(0, AllManifests.Count)];
    }

    public async Task<CoinData> GetCoinData(Manifest m, LoadingIcon loader)
    {
        // deserialize OriginalManifest
        UnityWebRequest request = await GetDataBaseJson(m.Id, loader, new Vector2(0f, 0.2f));
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
        
        CoinData newCoinData = new CoinData();
        newCoinData.SetOriginalManifest(mDeserialized);
        newCoinData.SetMetadata(mDeserialized.metadata.ToArray());

        string imageRes = Settings.ImageResolution.ToString();
        if (mDeserialized.sequences == null || mDeserialized.sequences.Count <= 0 || mDeserialized.sequences[0] == null) return null;
        if (mDeserialized.sequences[0].canvases == null || mDeserialized.sequences[0].canvases.Count <= 0) return null;
        List<Texture2D> images = new List<Texture2D>();
        for (int i = 0; i < mDeserialized.sequences[0].canvases.Count && i < 2; i++)
            //foreach (Canvas cvs in mDeserialized.sequences[0].canvases)
        {
            Canvas cvs = mDeserialized.sequences[0].canvases[i];
            
            if (cvs?.images == null || cvs.images.Count <= 0 || cvs.images[0] == null) continue;

            //string imagePath = cvs.images[0].resource.service.Id + "/full/" + imageRes + "," + imageRes + "/0/default.jpg";
            string imagePath = $"{cvs.images[0].resource.service.Id}/full/{imageRes},{imageRes}/0/default.jpg";

            images.Add(await GetDatabaseTexture2D(imagePath, loader, i == 0 ? new Vector2(0.2f, 0.6f) : new Vector2(0.6f, 1f)));
        }

        // no images
        if (images.Count <= 0) return null;

        // one image
        else if (images.Count <= 1) newCoinData.SetTextures(images[0], images[0]);

        // two image
        else newCoinData.SetTextures(images[0], images[1]);

        return newCoinData;
    }

    public static async Task<UnityWebRequest> GetDataBaseJson(string url, LoadingIcon loader = null, Vector2 progressRange = new Vector2())
    {
        UnityWebRequest request = UnityWebRequest.Get(url);
        
        request.SetRequestHeader("Accept-Encoding", "gzip");
        request.SendWebRequest();

        while (!request.isDone)
        {
            if (ApplicationQuit.IsCancellationRequested)
            {
                Debug.Log("GetDataBaseJson stopped: ApplicationQuit.Token has been cancelled!");
                return null;
            }
            
            if(loader != null) loader.SetProgress(Mathf.Lerp(progressRange.x, progressRange.y, request.downloadProgress));
            
            await Task.Yield();
        }

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(request.error + ", " + request.result + ", " + url);
            return null;
        }

        return request;
        //return request.downloadHandler.text;
    }

    private async Task<Texture2D> GetDatabaseTexture2D(string url, LoadingIcon loader = null, Vector2 progressRange = new Vector2())
    {
        //Debug.Log($"GetDatabaseTexture2D({url})");
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
        request.SendWebRequest();

        while (!request.isDone)
        {
            if (ApplicationQuit.IsCancellationRequested)
            {
                Debug.Log("Database loading process was stopped by the OnApplicationQuit Event!");
                return null;
            }

            if (loader != null) loader.SetProgress(Mathf.Lerp(progressRange.x, progressRange.y, request.downloadProgress));
            
            await Task.Yield();
        }

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(request.error + ", " + request.result);
            return null;
        }
        
        return DownloadHandlerTexture.GetContent(request);
    }
}