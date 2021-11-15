using D2Hunt.App.Data.Models;
using System.Collections.ObjectModel;

namespace D2Hunt.App.Data.ViewModels;

public class MainWindowViewModel : BaseObservable
{
    private string region = "--";
    private string hotIp = string.Empty;

    public ObservableCollection<GameInfo> GamesHistory { get; set; } = new ObservableCollection<GameInfo>();
        
    public string Region { get => region; set => SetField(ref region, value); }

    public string HotIp { get => hotIp; set => SetField(ref hotIp, value); }
}