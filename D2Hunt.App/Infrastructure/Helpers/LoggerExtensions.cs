namespace D2Hunt.App.Infrastructure.Helpers;

public static class LoggerExtensions
{
    public static void LogDebug(this ILogger logger, string messageTemplate, params object[] propertyValues)
    {
        logger.Debug(Message(messageTemplate), Properties(propertyValues));
    }

    public static void LogInfo(this ILogger logger, string messageTemplate, params object[] propertyValues)
    {
        logger.Information(Message(messageTemplate), Properties(propertyValues));
    }

    public static void LogError(this ILogger logger, Exception exception, string messageTemplate, params object[] propertyValues)
    {
        logger.Error(exception, Message(messageTemplate), Properties(propertyValues));
    }

    public static void LogError(this ILogger logger, string messageTemplate, params object[] propertyValues)
    {
        logger.Error(Message(messageTemplate), Properties(propertyValues));
    }

    private static string Message(string originalMessage) => string.Concat("({MachineName}) ", originalMessage);

    private static object[] Properties(object[] propertyValues)
    {
        var list = new List<object>();
        list.Add(Environment.MachineName);
        list.AddRange(propertyValues);
        return list.ToArray();   
    }
}
