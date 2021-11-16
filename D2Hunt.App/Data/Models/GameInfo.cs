namespace D2Hunt.App.Data.Models;

public class GameInfo : BaseObservable
{
    private int delay = 60;

    public int Id { get; set; }
    public string Address { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public int Delay { get => delay; set => SetField(ref delay, value); }
    public bool IsHot { get; set; } = false;
}