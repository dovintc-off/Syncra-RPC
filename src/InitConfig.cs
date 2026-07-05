// Copyright (C) 2026 Dovintc
// This file is part of Syncra RPC
// Licensed under AGPL-3.0 with No-Misattribution Addendum.
// See LICENSE.md file for details.

using System;
using System.IO;
using System.Text.Json;
using System.Collections.Generic;
using System.Globalization;

namespace SyncraRPC;

public class Config
{
    public const string StaticApplicationID = "1515830709384646776";
    string fileConfig = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json");
    string language = "";

    public Config(string FileConfig = "")
    {
        if (!(FileConfig == ""))
        {
            this.fileConfig = FileConfig;
        }
        JsonRoot conf = ReadConfig(this.fileConfig);

        if (!(conf.Changeable?.Language == null))
        {
            this.language = conf.Changeable?.Language ?? "";
        } else
        {   
            try 
            {
                SetStandardConfig(GetUserLanguage(), StaticApplicationID);
            } catch
            {
                SetStandardConfig("en", StaticApplicationID);
            }
        }
    }

    public void SetStandardConfig(
        string? newLanguage = "",
        string? newAppId = "",
        string? agreedWithHideUIButton = "",
        string? SomeBtn = "",
        string? OutputOfAdditionalInformation = ""
    )
    {
        JsonRoot config = ReadConfig(this.fileConfig);
        if (config.Changeable == null) config.Changeable = new ChangeableData();
        if (config.Changeable.WindowSettings == null) config.Changeable.WindowSettings = new WindowSettings(); 
        if (newLanguage != "") config.Changeable.Language = newLanguage;
        if (newAppId != "") config.Changeable.ApplicationID = newAppId;
        if (agreedWithHideUIButton != "") config.Changeable.AgreedWithHideUIButton = agreedWithHideUIButton;
        if (OutputOfAdditionalInformation != "") config.Changeable.WindowSettings.OutputOfAdditionalInformation = OutputOfAdditionalInformation;
        if (SomeBtn != "") config.Changeable.WindowSettings.SomeBtn = Convert.ToBoolean(SomeBtn);
        if (config.Changeable.Language != null) this.language = config.Changeable.Language;

        SaveToFile(config);
    }

    public object? GetStandardConfig(string key)
    {
        JsonRoot config = ReadConfig(this.fileConfig);
        if (config.Changeable == null || config.Changeable.WindowSettings == null) return null;

        return key switch
        {
            "Language" => config.Changeable.Language,
            "ApplicationID" => config.Changeable.ApplicationID,
            "AgreedWithHideUIButton" => config.Changeable.AgreedWithHideUIButton,
            "SomeBtn" => config.Changeable.WindowSettings.SomeBtn,
            "OutputOfAdditionalInformation" => config.Changeable.WindowSettings.OutputOfAdditionalInformation,
            _ => null
        };
    }

    public void SetUserDefinedConfig(string key, object value)
    {
        JsonRoot currentConfig = ReadConfig(this.fileConfig);

        if (currentConfig.UserDefined == null) {
            currentConfig.UserDefined = new Dictionary<string, object>();
        }

        currentConfig.UserDefined[key] = value;
        SaveToFile(currentConfig);
    }
        
    private JsonRoot ReadConfig(string PathToFile)
    {   
        if (!File.Exists(PathToFile)) {
            return new JsonRoot();
        }

        using FileStream stream = File.OpenRead(PathToFile);
        JsonRoot data = JsonSerializer.Deserialize<JsonRoot>(stream)!;
        return data;
    }

    private void SaveToFile(JsonRoot config)
    {
        var options = new JsonSerializerOptions { WriteIndented = true};
        string jsonString = JsonSerializer.Serialize(config, options);
        File.WriteAllText(this.fileConfig, jsonString);
    }

    private static string GetUserLanguage()
    {
        CultureInfo cultureInfo = CultureInfo.CurrentCulture;
        RegionInfo curentRegion = new RegionInfo(cultureInfo.Name);
        string countryCode = curentRegion.TwoLetterISORegionName; 
        return countryCode;
    }
}

public class JsonRoot
{
    public ChangeableData? Changeable {get; set;}

    [System.Text.Json.Serialization.JsonPropertyName("User-defined")]
    public Dictionary<string, object>? UserDefined {get; set;}
}

public class ChangeableData
{
    public string? Language {get; set;}
    public string? AgreedWithHideUIButton {get; set;}
    public string? ApplicationID {get; set;} = Config.StaticApplicationID;
    public WindowSettings? WindowSettings {get; set;}
}

public class WindowSettings
{
    public bool? SomeBtn {get; set;}
    public string? OutputOfAdditionalInformation {get; set;}
}