using AutoMapper;
using captly.Core;
using captly.Core.AsyncCommand;
using captly.Interfaces;
using captly.ViewModels;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;

namespace captly.Views.WhisperOptionsSetup;
public class WhisperOptionsSetupViewModel : BaseViewModel
{
    public readonly IAppConfigurationService AppConfigurationService;
    public readonly IMapper Mapper;
    public WhisperOptionsSetupViewModel(IAppConfigurationService appConfigurationService, IMapper mapper)
    {
        AppConfigurationService = appConfigurationService;
        Mapper = mapper;

        Task.Run(() => Initialize()).Wait();

        FilteredOptions = CollectionViewSource.GetDefaultView(AllOptions);
        FilteredOptions.Filter = FilterOptions;
        FilteredOptions.SortDescriptions.Add(new SortDescription(nameof(WhisperOptionViewModel.Option), ListSortDirection.Ascending));
    }

    async Task Initialize()
    {
        var whisperOptions = await AppConfigurationService.ReadWhisperOptions();

        if(whisperOptions is not null)
        {
            AllOptions = new(whisperOptions.Select(o => Mapper.Map<WhisperOptionViewModel>(o)));
        }
    }

    private string _searchText = string.Empty;
    public ObservableCollection<WhisperOptionViewModel> AllOptions { get; private set; } = [];
    public ICollectionView FilteredOptions { get; }
    public string SearchText
    {
        get => _searchText;
        set
        {
            if (SetProperty(ref _searchText, value))
            {
                FilteredOptions.Refresh();
            }
        }
    }
    private bool FilterOptions(object obj)
    {
        if (obj is WhisperOptionViewModel option)
        {
            if (string.IsNullOrWhiteSpace(SearchText))
                return true;

            var lowerSearch = SearchText.ToLower();
            return option.Option.ToLower().Contains(lowerSearch) ||
                   option.Original.ToLower().Contains(lowerSearch) ||
                   option.Description.ToLower().Contains(lowerSearch);
        }
        return false;
    }
    public IAsyncCommand SaveWhisperOptionsCommand { get; }
}
