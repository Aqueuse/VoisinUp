using VoisinUp.Models;
using VoisinUp.Repositories;

namespace VoisinUp.Services;

public class TaverneService {
    private readonly TaverneRepository _taverneRepository;
    private readonly UserService _userService;
    
    private List<TaverneMessageDto> lastMessagesDto;
    
    public TaverneService(TaverneRepository taverneRepository, UserService userService) {
        _taverneRepository = taverneRepository;
        _userService = userService;
    }
    
    public async Task PurgeOldMessages(int olderThanDays) {
        var threshold = DateTime.UtcNow.AddDays(-olderThanDays);
        await _taverneRepository.DeleteMessagesOlderThan(threshold);
    }

    public async Task<ServiceResult> GetLastMessages() {
        var lastMessages = await _taverneRepository.GetLastMessages();
        
        lastMessagesDto = new List<TaverneMessageDto>();
        
        foreach (var taverneMessage in lastMessages) {
            lastMessagesDto.Add(new TaverneMessageDto {
                Content = taverneMessage.Content,
                Timestamp = taverneMessage.Timestamp,
                UserCard = await _userService.GetUserCard(taverneMessage.UserId)
            });
        }
        
        return new ServiceResult { StatusCode = 200, Data = lastMessagesDto };
    }

    public async Task SaveMessage(TaverneMessage taverneMessage) {
        await _taverneRepository.SaveMessage(taverneMessage);
    }
}