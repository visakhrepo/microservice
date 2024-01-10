using System.Threading.Tasks;

namespace Ordering.Application.Contracts.Infrastructure
{
    internal interface IEmailService
    {
        Task<bool> SendEmail(Models.Email email);
    }
}
