using Leipzig;
using UnityEngine;

public class CoinData
{
    public ManifestDeserialized manifest;
    public Texture2D front;
    public Texture2D back;
    public string[] information;

    public CoinData(ManifestDeserialized manifest, Texture2D front, Texture2D back, string[] information)
    {
        this.manifest = manifest;
        this.front = front;
        this.back = back;
        this.information = information;
    }
}