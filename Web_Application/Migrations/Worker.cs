using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using System.Threading;
using DataLayer;
using Microsoft.EntityFrameworkCore;

namespace Migrations
{
    public class Worker : IHostedService
    {
        private readonly ApplicationDataContext _context;

        public  Worker(ApplicationDataContext context)
        {
            _context = context;
        }
        
        
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _context.Database.MigrateAsync(cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}