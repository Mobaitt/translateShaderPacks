using CommunityToolkit.Mvvm.ComponentModel;
using translateShaderPacks.Models;
using translateShaderPacks.Services;

namespace translateShaderPacks.ViewModels;

public partial class SettingsWindowViewModel : ViewModelBase
{
    [ObservableProperty]
    private AppConfig _config = ConfigService.Load();

    public void SaveConfig()
    {
        ConfigService.Save(Config);
    }
}