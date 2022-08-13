using System.Text.Json;
using AutoMapper;
using CommandsService.Data;
using CommandsService.Dtos;
using CommandsService.Models;

namespace CommandsService.EventProcessing
{
    public class EventProcessor : IEventProcessor
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IMapper _mapper;

        public EventProcessor(IServiceScopeFactory serviceScopeFactory, IMapper mapper)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _mapper = mapper;
        }

        public void ProcessEvent(string message)
        {
            var eventType = DetermineEvent(message);

            switch (eventType)
            {
                case EventType.PlatformPublished:
                    AddPlatform(message);
                    break;
                default:
                    break;
            }
        }

        private EventType DetermineEvent(string notificationMessage)
        {
            Console.WriteLine("--> Determining Event");

            try
            {
                var eventType = JsonSerializer.Deserialize<GenericEventDto>(notificationMessage)!;                
                switch (eventType.Event)
                {
                    case "PlatformPublished" :
                        Console.WriteLine("--> Platform published event detected");
                        return EventType.PlatformPublished;
                    default:
                        Console.WriteLine("--> Could not determine the event type");
                        return EventType.Undetermined;    

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"--> Cannot determine the event {notificationMessage}: {ex.Message}");
                return EventType.Undetermined;
            }
        }

        private void AddPlatform(string publishedMessage)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var repo = scope.ServiceProvider.GetRequiredService<ICommandRepository>();

                var platformPublishedDto = JsonSerializer.Deserialize<PlatformPublishedDto>(publishedMessage);

                try
                {
                    var platform = _mapper.Map<Platform>(platformPublishedDto);
                    if (!repo.ExternalPlatformExists(platform.ExternalId))
                    {
                        repo.CreatePlatform(platform);
                        repo.SaveChanges();
                        Console.WriteLine("--> Platform added");
                        return;
                    }
                    Console.WriteLine("--> Platform already exisists!");
                }
                catch(Exception ex)
                {
                    Console.WriteLine($"--> Could not add a platform to DB: {ex.Message}");
                }
            }
        }
    }

    enum EventType
    {
        PlatformPublished,
        Undetermined
    }
}