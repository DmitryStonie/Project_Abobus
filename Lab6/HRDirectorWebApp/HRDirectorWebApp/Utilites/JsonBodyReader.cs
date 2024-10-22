namespace HRManagerWebApp;

public class JsonBodyReader
{
    public async Task<String> ReadJsonBody(HttpRequest request)
    {
        request.EnableBuffering();
        request.Body.Position = 0;
        using var stream = new StreamReader(request.Body);
        var bodyStr = await stream.ReadToEndAsync();
        return bodyStr;
    }
}