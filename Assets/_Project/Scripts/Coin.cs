using System.Threading.Tasks;
using UnityEngine;

public class Coin : MonoBehaviour, IPreloadable
{
    private const float ScaleDuration = 1f;
    
    
    [SerializeField] private UserSettings _settings;
    private UserSettings Settings => _settings;
    
    [SerializeField] private MeshRenderer _front;
    private MeshRenderer Front => _front;
    
    [SerializeField] private MeshRenderer _back;
    private MeshRenderer Back => _back;
    
    [SerializeField] private AudioClip[] _hoverClips;
    private AudioClip[] HoverClips => _hoverClips;
    
    [SerializeField] private LoadingIcon _loaderPrefab;
    private LoadingIcon LoaderPrefab => _loaderPrefab;

    private LoadingIcon _loader;
    public LoadingIcon Loader
    {
        get
        {
            if (_loader == null)
            {
                _loader = Instantiate(LoaderPrefab);
                _loader.gameObject.name = "Loader " + gameObject.name;
            }
            return _loader;
        }
    }

    private const float HoverSpeed = 12f;
    private float _idleSpeed = 1f;
    private static Vector2 IdleSpeedRange => new Vector2(0.8f, 2.3f);
    private const float IdleAngle = 15f;
    
    private bool _hovering;
    private CoinThrower _thrower;

    [SerializeField] private CoinData _data;
    public CoinData Data => _data;

    private bool _preloadFinished;
    public bool PreloadFinished => _preloadFinished;

    public void SetScale(float scale, float texturePixelScale)
    {
        Vector3 frontPixelScale = Data.FrontTex == null
            ? Vector3.one
            : new Vector3(Data.FrontTex.width, Data.FrontTex.height, 1f);
        Vector2 frontNormalizedScale = AspectHelper.GetNormalizedScale(texturePixelScale, frontPixelScale);
        Front.transform.localScale = frontNormalizedScale * scale;
        
        Vector3 backPixelScale = Data.BackTex == null
            ? Vector3.one
            : new Vector3(Data.BackTex.width, Data.BackTex.height, 1f);
        Vector2 backNormalizedScale = AspectHelper.GetNormalizedScale(texturePixelScale, backPixelScale);
        Back.transform.localScale = backNormalizedScale * scale;
    }

    public void SetReferences(CoinThrower thrower)
    {
        this._thrower = thrower;
    }

    public void SetCoinData(CoinData data)
    {
        if (data == null)
        {
            Debug.LogError($"{gameObject.name} -> Coin.SetCoinData({data}) -> data is null!");
            return;
        }
        _data = data;

        if(Data.FrontTex == Data.BackTex) FlipBack(true);

        if (Front != null)
        {
            Front.material.SetTexture("_BaseMap", Data.FrontTex);
            Front.gameObject.SetActive(true);
        }

        if (Back != null)
        {
            Back.material.SetTexture("_BaseMap", Data.BackTex);
            Back.gameObject.SetActive(true);
        }

        _idleSpeed = Random.Range(IdleSpeedRange.x, IdleSpeedRange.y);

        _preloadFinished = true;
    }

    private void FlipBack(bool flip)
    {
        if (Back == null) return;
            
        var scale = Back.transform.localScale;
        scale.x *= flip ? -1f : 1f;
        Back.transform.localScale = scale;
    }

    public void SetUnderwater(bool underwater)
    {
        Front.gameObject.layer = LayerMask.NameToLayer(underwater ? "Underwater" : "Default");
        Back.gameObject.layer = LayerMask.NameToLayer(underwater ? "Underwater" : "Default");
    }

    public void HideImmediately()
    {
        transform.localScale = Vector3.one * 0f;
    }

    public void Show()
    {
        Debug.Log($"Show Coin: " + gameObject.name);
        if(!PreloadFinished) Loader.Show();

        ShowAnimation();
    }

    private async Task ShowAnimation()
    {
        float lerp = transform.localScale.x;
        while (lerp <= 1f)
        {
            if (DataProvider.ApplicationQuit.IsCancellationRequested) return;
            
            if (_preloadFinished)
            {
                lerp = Mathf.Clamp(lerp + Time.deltaTime / ScaleDuration, 0f, 1f);
            }

            transform.localScale = Vector3.one * Easing.EaseInOutQuint(lerp);
            await Task.Yield();
        }
    }

    private void Update()
    {
        if (Settings != null && !Settings.CoinAnimation)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler((_hovering ? 90f : 270f), 90f, 90f), Time.deltaTime * HoverSpeed);
        }
        else
        {
            float idleRotation = Mathf.Sin(Time.realtimeSinceStartup * _idleSpeed) * IdleAngle;
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler((_hovering ? 90f : 270f) + idleRotation, 90f, 90f), Time.deltaTime * HoverSpeed);
        }

        Loader.SetWorldPosition(transform.position);
    }

    private void OnMouseEnter()
    {
        _hovering = true;
        
        AudioPlayer.Instance.PlayRandomAudioClip(HoverClips, new Vector2(0.5f, 1f), new Vector2(0.9f, 1.1f));
    }

    private void OnMouseExit()
    {
        _hovering = false;
    }

    private void OnMouseDown()
    {
        if(_thrower != null) _thrower.ThrowCoin(this);
    }

    public void Deactivate()
    {
        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;

        enabled = false;
    }
}
