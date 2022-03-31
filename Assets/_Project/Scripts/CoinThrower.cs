using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;
using Random = System.Random;

public class CoinThrower : MonoBehaviour
{
    [SerializeField] private DataProvider _provider;
    private DataProvider Provider => _provider;
    
    [SerializeField] private Coin _coinPrefab;
    private Coin CoinPrefab => _coinPrefab;

    [SerializeField] private Transform[] _coinPositions;
    private Transform[] CoinPositions => _coinPositions;

    [SerializeField] private Transform _throwTarget;
    private Transform ThrowTarget => _throwTarget;

    [SerializeField] private Vector2 _throwTargetOffset;
    private Vector2 ThrowTargetOffset => _throwTargetOffset;

    [SerializeField] private float _waterHeight;
    private float WaterHeight => _waterHeight;

    [SerializeField] private float _underwaterGravity;
    private float UnderwaterGravity => _underwaterGravity;

    private List<Coin> _loadedCoins;
    private List<Coin> LoadedCoins => _loadedCoins ??= new List<Coin>();

    private bool _allowUserInteraction;

    CancellationTokenSource tokenSource = new CancellationTokenSource();

    private void OnApplicationQuit()
    {
        tokenSource.Cancel();
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
    }

    public async void CreateNewCoins()
    {
        LoadedCoins.Clear();

        for (int i = 0; i < CoinPositions.Length; i++)
        {
            int attempts = 100;
            CoinData randomCoinData = null;
            while (randomCoinData == null && attempts > 0)
            {
                if (tokenSource.Token.IsCancellationRequested) return;
                randomCoinData = await Provider.GetRandomCoinData();
                attempts--;
            }

            if (randomCoinData == null) continue;
            if (tokenSource.Token.IsCancellationRequested) return;
            
            Coin newCoin = Instantiate(CoinPrefab, CoinPositions[i].position, Quaternion.identity);
            newCoin.SetCoinConnection(this, i);
            newCoin.SetCoinData(randomCoinData);
            newCoin.gameObject.name = "Coin_" + i;
            LoadedCoins.Add(newCoin);
        }

        _allowUserInteraction = true;
    }

    private async void PlayThrowAnimation(Coin coin)
    {
        //Debug.Log($"PlayThrowAnimation({coin?.gameObject.name})");

        if (coin == null) return;
        
        coin.Deactivate();

        const float gravity = 1f;

        Vector3 startPos = coin.transform.position;
        Vector3 endPosOffset = new Vector3(UnityEngine.Random.Range(-ThrowTargetOffset.x, ThrowTargetOffset.x), 0f, UnityEngine.Random.Range(-ThrowTargetOffset.y, ThrowTargetOffset.y));
        Vector3 endPos = ThrowTarget.position + endPosOffset;

        int f = 6;

        Vector3 startRot = coin.transform.rotation.eulerAngles;
        Vector3 endRot = new Vector3(90f, 90f, 90f);
        endRot += new Vector3(UnityEngine.Random.Range(-f, f) * 180f, UnityEngine.Random.Range(-f, f) * 360f, UnityEngine.Random.Range(-f, f) * 360f);

        float lerp = 0f;
        bool hasBeenSetUnderwater = false;

        while (lerp < 1f)
        {
            bool isUnderwater = coin.transform.position.y < WaterHeight;
            
            lerp = Mathf.Clamp(lerp + Time.deltaTime * (isUnderwater ? UnderwaterGravity : gravity), 0f, 1f);
            
            Vector3 pos = Vector3.Lerp(startPos, endPos, lerp);
            pos.y = Mathf.Lerp(startPos.y, endPos.y, Easing.EaseInQuad(lerp));
            coin.transform.position = pos;
            
            Vector3 rot = Vector3.Lerp(startRot, endRot, Easing.EaseOutQuad(lerp));
            coin.transform.rotation = Quaternion.Euler(rot);
            
            if (!hasBeenSetUnderwater && isUnderwater)
            {
                coin.SetUnderwater(true);
                hasBeenSetUnderwater = true;
            }

            await Task.Yield();
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