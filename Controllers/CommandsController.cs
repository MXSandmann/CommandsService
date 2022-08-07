using AutoMapper;
using CommandsService.Data;
using CommandsService.Dtos;
using CommandsService.Models;
using Microsoft.AspNetCore.Mvc;

namespace CommandsService.Controllers
{
    [Route("api/c/platforms/{platformId:int}/[controller]")]
    [ApiController]
    public class CommandsController : ControllerBase
    {
        private readonly ICommandRepository _repository;
        private readonly IMapper _mapper;

        public CommandsController(ICommandRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        [HttpGet]
        public ActionResult<IEnumerable<CommandReadDto>> GetCommandsForPlatform(int platformId)
        {
            Console.WriteLine($"--> Hit GetCommandsForPlatform {platformId}");

            if (!_repository.PlatformExists(platformId))
                return NotFound();
            
            var commands = _repository.GetCommandsForPlatform(platformId);

            return Ok(_mapper.Map<IEnumerable<CommandReadDto>>(commands));
        }

        [HttpGet("{commandId:int}", Name = "GetCommandForPlatform")]
        public ActionResult<CommandCreateDto> GetCommandForPlatform(int platformId, int commandId)
        {
            Console.WriteLine($"--> Hit GetCommandForPlatform, platform: {platformId}, command: {commandId}");

            if (!_repository.PlatformExists(platformId))
                return NotFound();
            
            var command = _repository.GetCommand(platformId, commandId);

            if (command == null)
                return NotFound();

            return Ok(_mapper.Map<IEnumerable<CommandReadDto>>(command));
        }

        [HttpPost]
        public ActionResult<CommandReadDto> CreateCommandForPlatform(int platformId, CommandCreateDto commandCreateDto)
        {
            Console.WriteLine($"--> Hit CreateCommandForPlatform, platform: {platformId}, command: {commandCreateDto.CommandLine}, {commandCreateDto.HowTo}");

            if (!_repository.PlatformExists(platformId))
                return NotFound();

            var command = _mapper.Map<Command>(commandCreateDto);
            _repository.CreateCommand(platformId, command);
            _repository.SaveChanges();

            var commandReadDto = _mapper.Map<CommandReadDto>(command);

            return CreatedAtRoute(nameof(GetCommandForPlatform),
                new {platformIdm = platformId, commandId = commandReadDto.Id}, commandReadDto);
        }
    }
}