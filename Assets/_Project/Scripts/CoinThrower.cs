using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
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


    private List<Coin> _loadedCoins;
    private List<Coin> LoadedCoins => _loadedCoins ??= new List<Coin>();

    private List<TextDisplay> _loadedTextDisplays;
    private List<TextDisplay> LoadedTextDisplays => _loadedTextDisplays ??= new List<TextDisplay>();

    private bool _allowUserInteraction;


    private void Start()
    {
        for (int i = 0; i < Settings.MaxMetadataDisplay * 2; i++)
        {
            TextDisplay display = Instantiate(TextDisplayPrefab);
            display.Hide(0f);
            LoadedTextDisplays.Add(display);
        }
    }

    public void ThrowCoin(Coin coin)
    {
        if (!_allowUserInteraction) return;

        Debug.Log($"Throwing Coin: {coin.gameObject.name}");

        _allowUserInteraction = false;

        foreach (Coin loadedCoin in LoadedCoins.Where(x => x != null))
        {
            if (loadedCoin == coin)
            {
                PlayThrowAnimation(loadedCoin);
            }
            else
            {
                PlayDestroyAnimation(loadedCoin);
            }
        }

        Resources.UnloadUnusedAssets();

        CreateNewCoins();

        // instead ShowPreloadedCoins();
        // and when that is done (once preloading was done) do another PreloadCoins();
    }

    public async void CreateNewCoins()
    {
        LoadedCoins.Clear();

        if (Settings == null || CoinPrefab == null)
        {
            Debug.LogError("Settings is null or incomplete!");
            return;
        }

        for (int i = 0; i < Settings.CoinSelectionCount; i++)
        {
            int attempts = 100;
            CoinData randomCoinData = null;
            while (randomCoinData == null && attempts > 0)
            {
                if (DataProvider.ApplicationQuit.Token.IsCancellationRequested) return;
                randomCoinData = await Provider.GetRandomCoinData();
                attempts--;
            }

            if (randomCoinData == null) continue;
            if (DataProvider.ApplicationQuit.Token.IsCancellationRequested) return;

            float coinPosX = Mathf.Lerp(CoinPositionRangeX.x, CoinPositionRangeX.y, (1f / (Settings.CoinSelectionCount - 1f)) * i);
            Vector3 coinPosComposite = new Vector3(coinPosX, 0f, 0f);

            Coin newCoin = Instantiate(CoinPrefab, coinPosComposite, Quaternion.identity);
            newCoin.SetScale(Settings.CoinScale);
            newCoin.SetCoinConnection(this, i);
            newCoin.SetCoinData(randomCoinData);
            newCoin.gameObject.name = $"Coin_{i}_{randomCoinData.information?[0]}";
            LoadedCoins.Add(newCoin);
        }

        _allowUserInteraction = true;
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
        for (int i = 0; i < coin.Information.Length; i++)
        {
            allMetadata.Add(coin.Information[i]);
        }

        // shuffle to randomize order
        allMetadata.Shuffle();

        int count = allMetadata.Count < metadataCount ? allMetadata.Count : metadataCount;
        //Debug.Log($"allMetadata.Count = {allMetadata.Count}, metadataCount = {metadataCount}, count = {count}");

        // create metadata display with random max count or capped by number of metadata information
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