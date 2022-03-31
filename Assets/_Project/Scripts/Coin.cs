using System;
using Leipzig;
using UnityEngine;
using Random = System.Random;

public class Coin : MonoBehaviour
{
    [SerializeField] private MeshRenderer _front;
    private MeshRenderer Front => _front;
    
    [SerializeField] private MeshRenderer _back;
    private MeshRenderer Back => _back;

    private string[] _information;
    public string[] Information => _information ??= new string[]{};

    private const float HoverSpeed = 12f;
    private float _idleSpeed = 1f;
    private static Vector2 IdleSpeedRange => new Vector2(0.8f, 2.3f);
    private const float IdleAngle = 15f;
    
    private bool _hovering;
    private CoinThrower _thrower;

    [SerializeField] private ManifestDeserialized _debugManifestContent;

    public int Index { get; private set; }

    public void SetCoinConnection(CoinThrower thrower, int index)
    {
        this._thrower = thrower;
        this.Index = index;
    }

    public void SetCoinData(CoinData data)
    {
        this._debugManifestContent = data.manifest;
        
        if(data.front == data.back) FlipBack(true);
        
        if (Front != null) Front.material.SetTexture("_BaseMap", data.front);
        if (Back != null) Back.material.SetTexture("_BaseMap", data.back);

        this._information = data.information;

        _idleSpeed = UnityEngine.Random.Range(IdleSpeedRange.x, IdleSpeedRange.y);
    }

    private void FlipBack(bool flip)
    {
        Back.transform.localScale = new Vector3(flip ? -1f : 1f, 1f, 1f);
    }

    public void SetUnderwater(bool underwater)
    {
        Front.gameObject.layer = LayerMask.NameToLayer(underwater ? "Underwater" : "Default");
        Back.gameObject.layer = LayerMask.NameToLayer(underwater ? "Underwater" : "Default");
    }

    private void Update()
    {
        float idleRotation = Mathf.Sin(Time.realtimeSinceStartup * _idleSpeed) * IdleAngle;
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler((_hovering ? 90f : 270f) + idleRotation, 90f, 90f), Time.deltaTime * HoverSpeed);
    }

    private void OnMouseEnter()
    {
        _hovering = true;
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
