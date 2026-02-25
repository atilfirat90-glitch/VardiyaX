using System.Collections.ObjectModel;
using System.Windows.Input;
using ShiftCraft.Mobile.Models;
using ShiftCraft.Mobile.Services;

namespace ShiftCraft.Mobile.ViewModels;

public class TeamChatViewModel : BaseViewModel
{
    private readonly ITeamMessageService _messageService;
    private readonly IAuthService _authService;
    private string _newMessage = string.Empty;
    private string _selectedChannel = "general";

    public TeamChatViewModel(ITeamMessageService messageService, IAuthService authService)
    {
        _messageService = messageService;
        _authService = authService;
        Title = "Takım Sohbeti";
        Messages = new ObservableCollection<TeamMessageModel>();
        Channels = new List<string> { "general", "managers", "announcement" };

        LoadCommand = new Command(async () => await LoadAsync());
        SendCommand = new Command(async () => await SendAsync());
    }

    public ICommand LoadCommand { get; }
    public ICommand SendCommand { get; }
    public ObservableCollection<TeamMessageModel> Messages { get; }
    public List<string> Channels { get; }

    public bool IsManager => _authService.IsManager;

    public string NewMessage
    {
        get => _newMessage;
        set => SetProperty(ref _newMessage, value);
    }

    public string SelectedChannel
    {
        get => _selectedChannel;
        set
        {
            if (SetProperty(ref _selectedChannel, value))
            {
                _ = LoadAsync();
            }
        }
    }

    public async Task LoadAsync()
    {
        if (IsBusy) return;
        IsBusy = true;

        try
        {
            Messages.Clear();
            var messages = await _messageService.GetByChannelAsync(SelectedChannel, 1);
            foreach (var msg in messages)
            {
                Messages.Add(msg);
            }
        }
        catch (UnauthorizedAccessException)
        {
            await Shell.Current.GoToAsync("//login");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[TeamChatViewModel] Load error: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task SendAsync()
    {
        if (string.IsNullOrWhiteSpace(NewMessage)) return;

        try
        {
            var request = new SendMessageModel
            {
                Content = NewMessage,
                Channel = SelectedChannel,
                BusinessId = 1
            };

            var result = await _messageService.SendMessageAsync(request);
            if (result != null)
            {
                Messages.Insert(0, result);
                NewMessage = string.Empty;
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[TeamChatViewModel] Send error: {ex.Message}");
        }
    }
}
