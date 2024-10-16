using Hackathon;
using Hackathon.DataProviders;
using JuniorsWebApp.MassTransit;
using MassTransit;

namespace HRDirectorWebApp.MassTransit.Consumers;

public class GetJuniorWishlistConsumer :
    IConsumer<JuniorWishlistSended>
{
    readonly HrManager _hrManager;

    public GetJuniorWishlistConsumer(HrManager hrManager)
    {
        _hrManager = hrManager;
    }
    public async Task Consume(ConsumeContext<JuniorWishlistSended> context)
    {
        var junior = new Junior(0, context.Message.Name, context.Message.JuniorId, new Wishlist(context.Message.Wishlist));
        _hrManager.Juniors.Add(junior);
        Console.WriteLine("HRManager got junior");
    }
}   