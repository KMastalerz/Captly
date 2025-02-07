using AutoMapper;
using captly.Core.AsyncCommand;
using captly.Models;

namespace captly.Views.WhisperOptionsSetup;
public class WhisperOptionsSetupCommands
{
    public class SaveWhisperOptionsCommand : AsyncCommand
    {
        public override bool CanExecute(object? parameter = null) => true;

        public override async Task ExecuteAsync(object? parameter = null)
        {
            if(parameter is WhisperOptionsSetupViewModel whisperOptionsSetupViewModel)
            {
                var mapper = App.GetService<IMapper>();
                await whisperOptionsSetupViewModel.AppConfigurationService.SaveWhisperOptions(whisperOptionsSetupViewModel.AllOptions.Select(o => mapper.Map<WhisperOption>(o)));
            }
        }
    }
}
