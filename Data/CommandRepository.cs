using CommandsService.Models;

namespace CommandsService.Data
{
    public class CommandRepository : ICommandRepository
    {
        private readonly AppDbContext _context;

        public CommandRepository(AppDbContext context)
        {
            _context = context;
        }

        public bool SaveChanges()
        {
            return (_context.SaveChanges() >= 0);
        }

        public bool PlatformExists(int platfromId)
        {
            return _context.Platforms.Any(p => p.Id == platfromId);
        }

        public IEnumerable<Command> GetCommandsForPlatform(int platfromId)
        {
            return _context.Commands
                .Where(c => c.PlatfromId == platfromId)
                .OrderBy(c => c.Platform.Name).ToList();
        }

        public Command GetCommand(int platfromId, int commandId)
        {
            return _context.Commands
                .Where(c => c.PlatfromId == platfromId && c.Id == commandId)
                .FirstOrDefault()!;
        }

        public void CreatePlatform(Platform platfrom)
        {
            if (platfrom == null)            
                throw new ArgumentNullException(nameof(platfrom));            

            _context.Add(platfrom);            
        }

        public IEnumerable<Platform> GetAllPlatforms()
        {
            return _context.Platforms.OrderBy(p => p.Name).ToList();
        }

        public void CreateCommand(int platfromId, Command command)
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));

            command.PlatfromId = platfromId;
            _context.Commands.Add(command);
        }
    
        public int? DeleteCommand(int commandId)
        {
            var commandToDelete = _context.Commands.FirstOrDefault(c => c.Id == commandId);

            if (commandToDelete == null)
                return null;

            _context.Remove(commandToDelete);  
           
            return commandToDelete.Id;
        }
     

        

        

        

    }
}