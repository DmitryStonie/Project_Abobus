using Hackathon;
using Hackathon.DataProviders;
using JuniorsWebApp.MassTransit;
using MassTransit;

namespace HRDirectorWebApp.MassTransit.Consumers;

public class GetJuniorWishlistConsumer :
    IConsumer<JuniorWishlistSended>
{
    readonly HrDirector _hrDirector;

    public GetJuniorWishlistConsumer(HrDirector hrDirector)
    {
        _hrDirector = hrDirector;
    }
    public async Task Consume(ConsumeContext<JuniorWishlistSended> context)
    {
        //убрать константу (видимо путем добавления нужного конструктора)
        var junior = new Junior(0, context.Message.Name, context.Message.JuniorId, new Wishlist(context.Message.Wishlist));
        _hrDirector.Juniors.Add(junior);
        Console.WriteLine("HRDirector got junior");

    }
}   