using System;
using Leipzig;
using UnityEngine;

[Serializable]
public class CoinData
{
    public Texture2D FrontTex;
    public Texture2D BackTex;
    
    private string[] _information;
    public string[] Information
    {
        get => _information ??= new string[] { };
        set => _information = value;
    }
    
    public float Diameter;
    public float Weight;
    public CoinMaterial CoinMat;
    public ManifestDeserialized OriginalManifest;

    public CoinData(ManifestDeserialized originalManifest, Texture2D frontTex, Texture2D backTex, string[] information)
    {
        this.OriginalManifest = originalManifest;
        this.FrontTex = frontTex;
        this.BackTex = backTex;
        this.Information = information;
        this.Diameter = 1f;
        this.Weight = 0f;
        this.CoinMat = CoinMaterial.Default;
    }
}