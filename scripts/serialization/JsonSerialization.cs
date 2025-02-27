﻿using System;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;

namespace ColdMint.scripts.serialization;

/// <summary>
/// <para>JsonSerialization</para>
/// <para>Json序列化工具</para>
/// </summary>
/// <remarks>
///<para>This serializer is no longer recommended and Yaml is recommended instead of Json.</para>
///<para>此序列化器已不再推荐使用，建议用Yaml代替Json。</para>
/// </remarks>
/// <seealso cref="YamlSerialization"/>
[Obsolete("The Json serializer is out of date, we recommend yaml serialization.\nJson序列化器已过时了，我们推荐使用Yaml。", true)]
public static class JsonSerialization
{
    private static readonly JsonSerializerOptions Options = new()
    {
        //Case-insensitive attribute matching
        //不区分大小写的属性匹配
        PropertyNameCaseInsensitive = true,
        //Try to avoid escape
        //尽量避免转义
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        //Enable smart Print
        //启用漂亮打印
        WriteIndented = true
    };

    /// <summary>
    /// <para>Read a Json file to type T</para>
    /// <para>读取一个Json文件到T类型</para>
    /// </summary>
    /// <param name="path"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static async Task<T?> ReadJsonFileToObjAsync<T>(string path)
    {
        await using var openStream = File.OpenRead(path);
        return await JsonSerializer.DeserializeAsync<T>(openStream, Options);
    }

    /// <summary>
    /// <para>Serialize the object to Json</para>
    /// <para>将对象序列化为Json</para>
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static string Serialize(object obj)
    {
        return JsonSerializer.Serialize(obj, Options);
    }

    /// <summary>
    /// <para>Deserialize Json to an object</para>
    /// <para>将Json反序列化为对象</para>
    /// </summary>
    /// <param name="json"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T? Deserialize<T>(string json)
    {
        return JsonSerializer.Deserialize<T>(json, Options);
    }

    public static async Task<T?> ReadJsonFileToObjAsync<T>(Stream openStream)
    {
        return await JsonSerializer.DeserializeAsync<T>(openStream, Options);
    }
}