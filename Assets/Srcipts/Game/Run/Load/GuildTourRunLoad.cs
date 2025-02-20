using System.Threading.Tasks;
using Loxodon.Framework.Contexts;
using LP.Framework;

public class GuildTourRunLoad : IRunLoad
{

    public async Task Load()
    {
        var context = Context.GetApplicationContext();

        IGuildTourService guildTourService = new GuildTourService();

        guildTourService.Init();

        context.GetContainer().Register<IGuildTourService>(guildTourService);
    }
}
