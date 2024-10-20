using System.Text;
using Hackathon;
using Newtonsoft.Json;

namespace HRManagerWebApp;

public class TeamsSender(ILogger<TeamsSender> logger, IHttpClientFactory httpClientFactory)
{
    public async Task<bool> SendTeams(IEnumerable<Team> teams, string hrDirectorUri, string Guid)
    {
        while (true)
        {
            try
            {
                var json = JsonConvert.SerializeObject(new { 
                    teams = teams, 
                    guid = Guid, 
                });
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await httpClientFactory.CreateClient().PostAsync(hrDirectorUri, content);
                if (response.IsSuccessStatusCode)
                {
                    logger.LogInformation("Teams successfully loaded!");
                    return true;
                }

                logger.LogInformation($"Got response {response.StatusCode}");
            }
            catch (AggregateException ex)
            {
                logger.LogError(ex.Message);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return false;
            }
        }
    }
}