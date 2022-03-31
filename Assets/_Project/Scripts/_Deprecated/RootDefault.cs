using System;
using System.Collections.Generic;
using Newtonsoft.Json;

/*
[Serializable]
public class RootDefault
{
    [JsonProperty("@context")] public string Context;

    [JsonProperty("@id")] public string Id;

    [JsonProperty("@type")] public string Type;
    public string label;
    public string description;
    public List<Collection> collections;
}

[Serializable]
public class Collection
{
    [JsonProperty("@id")] public string Id;

    [JsonProperty("@type")] public string Type;
    public string label;
}

[Serializable]
public class CollectionContent
{
    [JsonProperty("@context")] public string Context;

    [JsonProperty("@id")] public string Id;

    [JsonProperty("@type")] public string Type;
    public string label;
    public string description;
    public List<Manifest> manifests;
}

[Serializable]
public class Manifest
{
    [JsonProperty("@id")] public string Id;

    [JsonProperty("@type")] public string Type;
    public string label;
}

[Serializable]
public class ManifestDeserialized
{
    [JsonProperty("@context")] public string Context;

    [JsonProperty("@id")] public string Id;

    [JsonProperty("@type")] public string Type;
    public string label;
    public List<Metadata> metadata;
    public List<Sequence> sequences;
    public string license;
    public string requiredStatement;
    public List<Rendering> rendering;
    public string seeAlso;
}

[Serializable]
public class Metadata
{
    public string label;
    public string value;
}

[Serializable]
public class Service
{
    [JsonProperty("@context")] public string Context;

    [JsonProperty("@id")] public string Id;
    public string profile;
}

[Serializable]
public class Resource
{
    [JsonProperty("@id")] public string Id;

    [JsonProperty("@type")] public string Type;
    public string format;
    public int height;
    public int width;
    public string label;
    public Service service;
}

[Serializable]
public class Image
{
    [JsonProperty("@id")] public string Id;

    [JsonProperty("@type")] public string Type;
    public string license;
    public string motivation;
    public string on;
    public Resource resource;
}

[Serializable]
public class Canvas
{
    [JsonProperty("@id")] public string Id;

    [JsonProperty("@type")] public string Type;
    public List<Image> images;
    public int width;
    public int height;
    public string label;
}

[Serializable]
public class Sequence
{
    [JsonProperty("@id")] public string Id;

    [JsonProperty("@type")] public string Type;
    public List<Canvas> canvases;
}

[Serializable]
public class Rendering
{
    public string label;
    public string format;

    [JsonProperty("@id")] public string Id;
}

*/
