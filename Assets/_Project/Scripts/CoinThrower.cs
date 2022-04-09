using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Leipzig;
using UnityEngine;
using UnityEngine.Rendering.UI;
using Random = UnityEngine.Random;

public class CoinThrower : MonoBehaviour
{
    [SerializeField] private ApplicationSettings _settings;
    private ApplicationSettings Settings => _settings;

    [SerializeField] private DataProvider _provider;
    private DataProvider Provider => _provider;

    [SerializeField] private Coin _coinPrefab;
    private Coin CoinPrefab => _coinPrefab;

    [SerializeField] private TextDisplay _textDisplayPrefab;
    private TextDisplay TextDisplayPrefab => _textDisplayPrefab;

    [SerializeField] private Vector2 _coinPositionRangeX = new Vector2(-2f, 2f);
    private Vector2 CoinPositionRangeX => _coinPositionRangeX;

    [SerializeField] private Transform _throwTarget;
    private Transform ThrowTarget => _throwTarget;

    [SerializeField] private Vector2 _throwTargetOffset;
    private Vector2 ThrowTargetOffset => _throwTargetOffset;

    [SerializeField] private float _waterHeight;
    private float WaterHeight => _waterHeight;

    [SerializeField] private Vector2 _textDisplayPositionRangeX = new Vector2(-2f, 2f);
    private Vector2 TextDisplayPositionRangeX => _textDisplayPositionRangeX;

    [SerializeField] private Vector2 _textDisplayPositionRangeY = new Vector2(0f, 3f);
    private Vector2 TextDisplayPositionRangeY => _textDisplayPositionRangeY;


    private Queue<Coin> _preloadedCoins;
    private Queue<Coin> PreloadedCoins => _preloadedCoins ??= new Queue<Coin>();

    private List<Coin> _visibleCoins;
    private List<Coin> VisibleCoins => _visibleCoins ??= new List<Coin>();

    private List<TextDisplay> _loadedTextDisplays;
    private List<TextDisplay> LoadedTextDisplays => _loadedTextDisplays ??= new List<TextDisplay>();

    private bool _allowUserInteraction;
    private int _coinIndex;


    public bool CreateCoins { get; set; }


    private void Start()
    {
        for (int i = 0; i < Settings.MaxMetadataDisplay * 2; i++)
        {
            TextDisplay display = Instantiate(TextDisplayPrefab);
            display.Hide(0f);
            LoadedTextDisplays.Add(display);
        }
    }

    private bool UserInteractionIsAllowed()
    {
        foreach(Coin coin in VisibleCoins)
        {
            if (coin == null) continue;

            if (!coin.PreloadFinished) return false;
        }

        return true;
    }

    public void ThrowCoin(Coin coin)
    {
        if (!UserInteractionIsAllowed()) return;

        Debug.Log($"Throwing Coin: {coin.gameObject.name}");

        _allowUserInteraction = false;

        foreach (Coin visibleCoin in VisibleCoins.Where(x => x != null))
        {
            if (visibleCoin == coin)
            {
                AdditionalInfoManager.Parameter.CoinsThrown.AddValue(1);
                PlayThrowAnimation(visibleCoin);
            }
            else
            {
                PlayDestroyAnimation(visibleCoin);
            }
        }
        
        VisibleCoins.Clear();

        Resources.UnloadUnusedAssets();

        //CreateNewCoins();
    }
    
    
    #region MonoBehaviour
    

    private void Update()
    {
        if (!CreateCoins) return;
        
        int coinPreloadCount = Settings.CoinSelectionCount * 2;
        
        //Debug.Log($"coinPreloadCount = {coinPreloadCount}, PreloadedCoins.Count = {PreloadedCoins.Count}, VisibleCoins.Count = {VisibleCoins.Count}");
        
        // more coins need to be preloaded
        if (PreloadedCoins.Count < coinPreloadCount)
        {
            for (int i = PreloadedCoins.Count; i < coinPreloadCount; i++)
            {
                Coin preloadingCoin = InstantiateCoin();
                preloadingCoin.Loader.Hide();
                Task t = PreloadCoin(preloadingCoin);
                PreloadedCoins.Enqueue(preloadingCoin);
                
                Debug.Log($"Preload {preloadingCoin}");
            }
        }
        
        // there are enough preloaded coins, and preloaded is needed
        if (PreloadedCoins.Count >= Settings.CoinSelectionCount && VisibleCoins.Count <= 0)
        {
            for (int i = 0; i < Settings.CoinSelectionCount; i++)
            {
                Coin coin = PreloadedCoins.Dequeue();
                
                float coinPosX = Mathf.Lerp(CoinPositionRangeX.x, CoinPositionRangeX.y, (1f / (Settings.CoinSelectionCount - 1f)) * i);
                Vector3 coinPosComposite = new Vector3(coinPosX, 0f, 0f);
                coin.transform.position = coinPosComposite;

                coin.Show();
                VisibleCoins.Add(coin);
            }
        }
    }

    #endregion
    
    [System.Obsolete]
    public async void CreateNewCoins()
    {
        if (Settings == null || CoinPrefab == null)
        {
            Debug.LogError("Settings is null or incomplete!");
            return;
        }

        for (int i = 0; i < Settings.CoinSelectionCount; i++)
        {
            Coin preloadingCoin = InstantiateCoin();
            PreloadCoin(preloadingCoin);
            
            /*
            float coinPosX = Mathf.Lerp(CoinPositionRangeX.x, CoinPositionRangeX.y, (1f / (Settings.CoinSelectionCount - 1f)) * i);
            Vector3 coinPosComposite = new Vector3(coinPosX, 0f, 0f);
            Coin newCoin = Instantiate(CoinPrefab, coinPosComposite, Quaternion.identity);
            
            int attempts = 0;
            CoinData randomCoinData = null;
            
            while (randomCoinData == null && attempts < 100)
            {
                Debug.Log($"{i}: CoinData loading attempt {attempts}.");
                
                if (DataProvider.ApplicationQuit.Token.IsCancellationRequested) return;
                randomCoinData = await Provider.GetRandomCoinData(newCoin.Loader);
                attempts++;
            }

            if (randomCoinData == null) continue;
            if (DataProvider.ApplicationQuit.Token.IsCancellationRequested) return;
            
            newCoin.SetScale(Settings.CoinScale * randomCoinData.Diameter);
            newCoin.SetReferences(this, i);
            newCoin.SetCoinData(randomCoinData);
            newCoin.gameObject.name = $"Coin_{i}_{randomCoinData.Information?[0]}";
            newCoin.Loader.SetProgress(1f);
            newCoin.Show();
            */
        }

        _allowUserInteraction = true;
    }

    private Coin InstantiateCoin()
    {
        Coin newCoin = Instantiate(CoinPrefab);
        newCoin.gameObject.name = "Coin_" + _coinIndex;
        _coinIndex++;
        newCoin.HideImmediately();

        return newCoin;
    }

    private async Task PreloadCoin(Coin newCoin)
    {
        //float coinPosX = Mathf.Lerp(CoinPositionRangeX.x, CoinPositionRangeX.y, (1f / (Settings.CoinSelectionCount - 1f)) * i);
        //Vector3 coinPosComposite = new Vector3(coinPosX, 0f, 0f);
        //Coin newCoin = Instantiate(CoinPrefab, coinPosComposite, Quaternion.identity);

        int attempts = 0;
        CoinData randomCoinData = null;

        while (randomCoinData == null && attempts < 100)
        {
            if (DataProvider.ApplicationQuit.Token.IsCancellationRequested)
            {
                Debug.Log("PreloadCoin stopped: ApplicationQuit.Token has been cancelled!");
                return;
            }

            // choose random OriginalManifest
            Manifest m = Provider.GetRandomManifest();
            if (m == null)
            {
                Debug.LogError($"Manifest is null!");
            }

            randomCoinData = await Provider.GetCoinData(m, newCoin.Loader);

            if (randomCoinData == null)
            {
                Debug.LogWarning($"Provider.GetCoinData() for {newCoin.gameObject.name} failed ({attempts}): {m.Id}");
            }
            else
            {
                Debug.Log($"Provider.GetCoinData() for {newCoin.gameObject.name} was successful ({attempts}): {m.Id}");
            }

            attempts++;
        }

        if (randomCoinData == null) return;
        if (DataProvider.ApplicationQuit.Token.IsCancellationRequested) return;
            
        newCoin.SetScale(Settings.CoinScale * randomCoinData.Diameter);
        newCoin.SetReferences(this);
        newCoin.SetCoinData(randomCoinData);
        newCoin.gameObject.name = $"Coin_{randomCoinData.Information?[0]}";
        newCoin.Loader.SetProgress(1f);

        AdditionalInfoManager.Parameter.CoinsLoaded.AddValue(1);
    }

    private async void PlayThrowAnimation(Coin coin)
    {
        if (coin == null) return;

        if (Settings == null)
        {
            Debug.LogError("Settings is null or incomplete!");
            return;
        }

        coin.Deactivate();

        const float gravity = 1f;
        float underwaterGravity = gravity * Settings.UnderwaterGravity;
        int f = Settings.MaxFlipRotations;

        Vector3 startPos = coin.transform.position;
        Vector3 endPosOffset = new Vector3(Random.Range(-ThrowTargetOffset.x, ThrowTargetOffset.x), 0f, Random.Range(-ThrowTargetOffset.y, ThrowTargetOffset.y));
        Vector3 endPos = ThrowTarget.position + endPosOffset;

        Vector3 startRot = coin.transform.rotation.eulerAngles;
        Vector3 endRot = new Vector3(90f, 90f, 90f);
        endRot += new Vector3(Random.Range(-f, f) * 180f, Random.Range(-f, f) * 360f, Random.Range(-f, f) * 360f);

        float lerp = 0f;
        bool hasBeenSetUnderwater = false;

        HideVisibleCoinMetadata();

        while (lerp < 1f)
        {
            if (DataProvider.ApplicationQuit.Token.IsCancellationRequested) return;

            bool isUnderwater = coin.transform.position.y < WaterHeight;

            lerp = Mathf.Clamp(lerp + Time.deltaTime * (isUnderwater ? underwaterGravity : gravity), 0f, 1f);

            Vector3 pos = Vector3.Lerp(startPos, endPos, lerp);
            pos.y = Mathf.Lerp(startPos.y, endPos.y, Easing.EaseInQuad(lerp));
            coin.transform.position = pos;

            Vector3 rot = Vector3.Lerp(startRot, endRot, Easing.EaseOutQuad(lerp));
            coin.transform.rotation = Quaternion.Euler(rot);

            if (!hasBeenSetUnderwater && isUnderwater)
            {
                coin.SetUnderwater(true);
                hasBeenSetUnderwater = true;

                ShowCoinMetadata(coin);
            }

            await Task.Yield();
        }
    }

    private void HideVisibleCoinMetadata()
    {
        foreach (TextDisplay display in LoadedTextDisplays)
        {
            if (!display.IsVisible) continue;
            
            float fadeDuration = Random.Range(Settings.MetadataFadeOutRange.x, Settings.MetadataFadeOutRange.y);
            display.Hide(fadeDuration);
        }
    }

    private void ShowCoinMetadata(Coin coin)
    {
        List<string> allMetadata = new List<string>();
        int metadataCount = Random.Range(Settings.MinMetadataDisplay, Settings.MaxMetadataDisplay + 1);

        // get all metadata
        for (int i = 0; i < coin.Data.Information.Length; i++)
        {
            allMetadata.Add(coin.Data.Information[i]);
        }

        // shuffle to randomize order
        allMetadata.Shuffle();

        int count = allMetadata.Count < metadataCount ? allMetadata.Count : metadataCount;
        //Debug.Log($"allMetadata.Count = {allMetadata.Count}, metadataCount = {metadataCount}, count = {count}");

        // create metadata display with random max count or capped by number of metadata Information
        for (int i = 0; i < count; i++)
        {
            TextDisplay hiddenDisplay = LoadedTextDisplays.Where(x => !x.IsVisible).First();
            if (hiddenDisplay != null)
            {
                float displayPosX = Random.Range(TextDisplayPositionRangeX.x, TextDisplayPositionRangeX.y);
                float displayPosZ = Mathf.Lerp(TextDisplayPositionRangeY.x, TextDisplayPositionRangeY.y, count < 2 ? 0f : (1f / (count - 1f)) * i);
                hiddenDisplay.SetPosition(new Vector3(displayPosX, WaterHeight, displayPosZ));
                float fadeDuration = Random.Range(Settings.MetadataFadeInRange.x, Settings.MetadataFadeInRange.y);
                hiddenDisplay.Show(fadeDuration, allMetadata[i]);
            }
        }
    }

    private async void PlayDestroyAnimation(Coin coin)
    {
        //Debug.Log($"PlayDestroyAnimation({coin?.gameObject.name})");

        if (coin == null) return;

        const float destroySpeed = 3f;

        Vector3 startPos = coin.transform.position;
        Vector3 endPos = startPos + new Vector3(0f, 0f, -2f);
        float lerp = 0f;

        while (lerp < 1f)
        {
            lerp = Mathf.Clamp(lerp + Time.deltaTime * destroySpeed, 0f, 1f);
            coin.transform.position = Vector3.Lerp(startPos, endPos, Easing.EaseInQuart(lerp));

            await Task.Yield();
        }

        Destroy(coin.gameObject);
    }
}