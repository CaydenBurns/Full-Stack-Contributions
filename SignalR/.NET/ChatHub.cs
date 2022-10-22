

using Microsoft.AspNetCore.SignalR;
using Sabio.Models.Domain;
using Sabio.Models.Domain.Messages;
using System.Threading.Tasks;

namespace Sabio.Web.Api.Hubs
{
    public interface IChatHub
    {
        Task OnMessageAdd(ChatMessage message);
        Task OnMessageUpdate(ChatMessage message);
        Task OnMessageDelete(OutboundMessageIds message);
    }

    public class ChatHub : Hub<IChatHub>
    {

    }
}