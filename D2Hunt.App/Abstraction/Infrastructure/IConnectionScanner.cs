namespace D2Hunt.App.Abstraction.Infrastructure;

public interface IConnectionScanner
{
    Task BeginScan(Func<string, DateTime, Task> onNewGameFound, Func<string, Task> onRegionChange, int scanInterval = 1000);
    void EndScan();
}