using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Core.Capabilities;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Modules.Menu;
using System.Text.Json.Serialization;
using static CounterStrikeSharp.API.Core.Listeners;


namespace PlayerSettings;

public class PluginConfig : BasePluginConfig
{
    [JsonPropertyName("DatabaseParams")] public DatabaseParams DatabaseParams { get; set; } = new DatabaseParams();

}
public class PlayerSettingsCore : BasePlugin, IPluginConfig<PluginConfig>
{
    internal static PlayerSettingsCore plugin;
    public PluginConfig Config { get; set; }

    public void OnConfigParsed(PluginConfig config)
    {
        plugin = this;
        Config = config;
        Storage.Init();
    }

    public override string ModuleName => "PlayerSettings [Core]";
    public override string ModuleVersion => "0.8";
    public override string ModuleAuthor => "Nick Fox";
    public override string ModuleDescription => "One storage for player's settings (aka ClientCookies)";

    private ISettingsApi? _api;
    private readonly PluginCapability<ISettingsApi?> _pluginCapability = new("settings:nfcore");
    public override void Load(bool hotReload)
    {
        _api = new SettingsApi();
        Capabilities.RegisterPluginCapability(_pluginCapability, () => _api);

        RegisterListener<Listeners.OnClientAuthorized>(OnClientAuthorized);        
    }

    public override void Unload(bool hotReload)
    {
        Storage.Close();
    }

    private void OnClientAuthorized(int slot, SteamID steamID)
    {
        ((SettingsApi)_api).LoadOnConnect(Utilities.GetPlayerFromSlot(slot));
    }

}

public struct DatabaseParams
{
    public string Host { get; set; }
    public string Name { get; set; }
    public string User { get; set; }
    public string Password { get; set; }

    public DatabaseParams()
    {
        Host = "127.0.0.1:3306";
        Name = "";
        User = "";
        Password = "";
    }

    public bool IsDefault()
    {
        return (Host == "127.0.0.1:3306" && Name == "" && User == "" && Password == "") || Host == "";
    }
}