using AutoMapper;
using Customer_Managerment.CustomerManagement.Application.Interfaces;
using Customer_Managerment.CustomerManagement.Domain.Entities;
using Customer_Managerment.CustomerManagement.Infrastructure.Data;
using Customer_Managerment.CustomerManagement.Infrastructure.Data.Entities;
using Microsoft.EntityFrameworkCore;
using SendGrid.Helpers.Errors.Model;

namespace Customer_Managerment.CustomerManagement.Infrastructure.Repositories
{
    public class CampaignRepository : ICampaignRepository
    {
        private readonly IDbContextFactory<CustomerManagementDbContext> _contextFactory;
        private readonly IMapper _mapper;

        public CampaignRepository(IDbContextFactory<CustomerManagementDbContext> contextFactory, IMapper mapper)
        {
            _contextFactory = contextFactory;
            _mapper = mapper;
        }

        public async Task<Campaign> AddCampaignAsync(CampaignDomain campaignDomain)
        {
            await using var context = _contextFactory.CreateDbContext();
            var campaign = _mapper.Map<Campaign>(campaignDomain);

            campaign.IdCampaign = Guid.NewGuid();
            campaign.Status = "Active";
            campaign.CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);

            await context.Campaigns.AddAsync(campaign);
            await context.SaveChangesAsync();
            return campaign;
        }

        public async Task<List<Campaign>> GetListCampaignsAsync(Guid idUser)
        {
            await using var context = _contextFactory.CreateDbContext();
            return await context.Campaigns
                .AsNoTracking()
                .Where(x => x.IdUser == idUser)
                .ToListAsync();
        }

        public async Task<Campaign> GetCampaignByIdAsync(Guid idCampaign)
        {
            await using var context = _contextFactory.CreateDbContext();
            var c = await context.Campaigns.AsNoTracking().FirstOrDefaultAsync(x => x.IdCampaign == idCampaign);
            if (c == null) throw new NotFoundException("Không tìm thấy chiến dịch!");
            return c;
        }

        public async Task<Campaign> GetExistingCampaignAsync(Guid idCampaign)
        {
            await using var context = _contextFactory.CreateDbContext();
            return await context.Campaigns
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.IdCampaign == idCampaign);
        }

        public async Task<Campaign> UpdateCampaignAsync(CampaignDomain campaignDomain, Campaign campaign)
        {
            await using var context = _contextFactory.CreateDbContext();
            context.Attach(campaign);
            _mapper.Map(campaignDomain, campaign);
            context.Entry(campaign).State = EntityState.Modified;
                    
            await context.SaveChangesAsync();
            return campaign;
        }

        public async Task DeleteCampaignAsync(Guid idCampaign)
        {
            await using var context = _contextFactory.CreateDbContext();
            var campaign = await context.Campaigns.FindAsync(idCampaign);
            if (campaign == null) 
                throw new NotFoundException("Không tìm thấy chiến dịch!");

            context.Campaigns.Remove(campaign);
            await context.SaveChangesAsync();
        }
    }
}
