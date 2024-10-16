namespace Mfm.Api.IntegrationTests.Support;
internal static class TestHelper
{
    public static async Task AssertEventuallyAsync(Func<Task> assertion, int timeoutInSeconds = 3)
    {
        var timeout = TimeSpan.FromSeconds(timeoutInSeconds);
        var interval = TimeSpan.FromMilliseconds(500);
        var startTime = DateTime.UtcNow;

        while (DateTime.UtcNow - startTime < timeout)
        {
            try
            {
                await assertion();
                return;
            }
            catch
            {
                await Task.Delay(interval);
            }
        }

        await assertion();
    }
}
