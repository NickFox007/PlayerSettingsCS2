using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Core.Capabilities;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Menu;


namespace PlayerSettings;
public class PlayerSettingsCore : BasePlugin
{
    public override string ModuleName => "PlayerSettings [Core]";
    public override string ModuleVersion => "0.1";
    public override string ModuleAuthor => "Nick Fox";
    public override string ModuleDescription => "One storage for player's settings (aka ClientCookies)";

    private ISettingsApi? _api;
    private readonly PluginCapability<ISettingsApi?> _pluginCapability = new("settings:nfcore");
    public override void Load(bool hotReload)
    {
        _api = new SettingsApi();
        Capabilities.RegisterPluginCapability(_pluginCapability, () => _api);
        Storage.Init(this);
    }
}