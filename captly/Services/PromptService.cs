using captly.Interfaces;
using captly.Model;
using Newtonsoft.Json;
using System.IO;

namespace captly.Services;
internal class PromptService : IPromptService
{
    public PromptService()
    {
        //get solution path 
        string appDirectory = AppDomain.CurrentDomain.BaseDirectory;
        string promptsDirectory = Path.Combine(appDirectory, "Shared");
        string promptsFile = Path.Combine(promptsDirectory, "prompts.json");

        //deserizalize prompts
        string json = File.ReadAllText(promptsFile);
        Prompts = JsonConvert.DeserializeObject<Prompts>(json) ?? new();
    }

    public Prompts Prompts { get; set; }
}
