using CommandsService.Models;

namespace CommandsService.Data
{
    public interface ICommandRepository
    {
        bool SaveChanges();

        // All about platforms
        IEnumerable<Platform> GetAllPlatforms();
        void CreatePlatform(Platform platfrom);
        bool PlatformExists(int platfromId);
        bool ExternalPlatformExists(int externalPlatfromId);
        
        // All about commands
        IEnumerable<Command> GetCommandsForPlatform(int platfromId);
        Command GetCommand(int platfromId, int commandId);
        void CreateCommand(int platfromId, Command command);
        int? DeleteCommand(int commandId);
    }
}