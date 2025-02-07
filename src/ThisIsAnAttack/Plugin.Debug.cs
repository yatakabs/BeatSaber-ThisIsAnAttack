using System.Diagnostics;
using SiraUtil.Zenject;
using ThisIsAnAttack.Logging;

namespace ThisIsAnAttack;

public partial class Plugin
{
    [Conditional("DEBUG")]
    private static void PrintContainerCatalog(
        Zenjector zenjector,
        IPluginLogger logger)
    {
        var locations = Enum
            .GetValues(typeof(Location))
            .Cast<Location>();

        foreach (var location in locations)
        {
            logger.DebugFormat(
                "Calling zenjector.Install() with location: {0}, for catalog output.",
                location);

            zenjector.Install(
                location,
                container =>
                {
                    logger.DebugFormat(
                        "Location: {0}",
                        location);

                    foreach (var binding in container.AllContracts)
                    {
                        var type = binding.Type;
                        var identifier = binding.Identifier;

                        logger.DebugFormat(
                            "Type: {0}, Identifier: {1}",
                            type,
                            identifier);
                    }

                    container.AllContracts
                        .Select(binding => new
                        {
                            Type = binding.Type.Name,
                            Identifier = binding.Identifier
                        })
                        .Select(x => string.Join(
                            ", ",
                            location.ToString(),
                            x.Type,
                            x.Identifier))
                        .ToList()
                        .ForEach(x =>
                        {
                            logger.Debug(x);
                        });
                });
        }
    }
}

