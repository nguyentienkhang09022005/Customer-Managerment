using AutoMapper;
using AutoMapper.QueryableExtensions;
using Customer_Managerment.CustomerManagement.Application.DTOs.Response;
using Customer_Managerment.CustomerManagement.Application.Interfaces;
using Customer_Managerment.CustomerManagement.Domain.Constant;
using Customer_Managerment.CustomerManagement.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Customer_Managerment.CustomerManagement.Api.Query
{
    [ExtendObjectType(OperationTypeNames.Query)]
    [Authorize]
    public class DealQuery
    {
        private readonly IDealRepository _dealRepository;
        private readonly ITeamMemberRepository _teamMemberRepository;
        private readonly IMapper _mapper;

        public DealQuery(IDealRepository dealRepository, ITeamMemberRepository teamMemberRepository, IMapper mapper)
        {
            _dealRepository = dealRepository;
            _teamMemberRepository = teamMemberRepository;
            _mapper = mapper;
        }

        [UseFiltering]
        [UseSorting]
        public IQueryable<DealResponse> GetDeals([Service] IHttpContextAccessor httpContextAccessor)
        {
            var currentUserRole = GetCurrentUserRole(httpContextAccessor);
            var currentUserId = GetCurrentUserId(httpContextAccessor);

            // Chỉ ADMIN mới được xem toàn bộ deals
            if (currentUserRole == "ADMIN")
            {
                return _dealRepository.GetListDeal().ProjectTo<DealResponse>(_mapper.ConfigurationProvider);
            }

            // STAFF fallback: trả deals của mình (OWNER) + deals mình là MEMBER
            return FilterDealsForStaffAsync(httpContextAccessor, currentUserId).Result;
        }

        [UseFiltering]
        [UseSorting]
        public async Task<IQueryable<DealResponse>> GetMyDeals([Service] IHttpContextAccessor httpContextAccessor)
        {
            var currentUserId = GetCurrentUserId(httpContextAccessor);
            return await FilterDealsForStaffAsync(httpContextAccessor, currentUserId);
        }

        private async Task<IQueryable<DealResponse>> FilterDealsForStaffAsync(
            IHttpContextAccessor httpContextAccessor, Guid currentUserId)
        {
            var teamMemberships = await _teamMemberRepository.GetByStaffAsync(currentUserId);

            var dealIdsWhereUserIsMember = teamMemberships
                .Where(tm => tm.EntityType == TeamEntityTypeConstant.EntityTypeDeal)
                .Select(tm => tm.EntityId)
                .ToList();

            return _dealRepository.GetListDeal()
                .Where(d => d.IdStaff == currentUserId || dealIdsWhereUserIsMember.Contains(d.IdDeal))
                .ProjectTo<DealResponse>(_mapper.ConfigurationProvider);
        }

        public async Task<IQueryable<DealResponse>> GetDealById(Guid idDeal, [Service] IHttpContextAccessor httpContextAccessor)
        {
            var currentUserId = GetCurrentUserId(httpContextAccessor);
            var currentUserRole = GetCurrentUserRole(httpContextAccessor);

            if (currentUserRole == "ADMIN")
            {
                return _dealRepository.GetListDeal()
                    .Where(d => d.IdDeal == idDeal)
                    .ProjectTo<DealResponse>(_mapper.ConfigurationProvider);
            }

            // STAFF: check if they are creator OR team member
            var teamMemberships = await _teamMemberRepository.GetByStaffAsync(currentUserId);
            var dealIdsWhereUserIsMember = teamMemberships
                .Where(tm => tm.EntityType == TeamEntityTypeConstant.EntityTypeDeal)
                .Select(tm => tm.EntityId)
                .ToList();

            return _dealRepository.GetListDeal()
                .Where(d => d.IdDeal == idDeal && (d.IdStaff == currentUserId || dealIdsWhereUserIsMember.Contains(d.IdDeal)))
                .ProjectTo<DealResponse>(_mapper.ConfigurationProvider);
        }

        public async Task<PagedResponse<DealResponse>> GetDealsPaged(int page, int pageSize, [Service] IHttpContextAccessor httpContextAccessor)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;
            if (pageSize > 200) pageSize = 200;

            var currentUserRole = GetCurrentUserRole(httpContextAccessor);
            var currentUserId = GetCurrentUserId(httpContextAccessor);

            List<Deal> items;
            int totalCount;

            if (currentUserRole == "ADMIN")
            {
                (items, totalCount) = await _dealRepository.GetListDealPagedAsync(page, pageSize);
            }
            else
            {
                var teamMemberships = await _teamMemberRepository.GetByStaffAsync(currentUserId);
                var dealIdsWhereUserIsMember = teamMemberships
                    .Where(tm => tm.EntityType == TeamEntityTypeConstant.EntityTypeDeal)
                    .Select(tm => tm.EntityId)
                    .ToList();

                // STAFF: lấy trên bảng rồi filter in-memory rồi paginate
                var allDeals = await _dealRepository.GetListDeal()
                    .Where(d => d.IdStaff == currentUserId || dealIdsWhereUserIsMember.Contains(d.IdDeal))
                    .OrderByDescending(d => d.CreatedAt)
                    .ToListAsync();
                totalCount = allDeals.Count;
                items = allDeals.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            }

            var dtoItems = _mapper.Map<List<DealResponse>>(items);
            return new PagedResponse<DealResponse> { Items = dtoItems, TotalCount = totalCount };
        }

        private Guid GetCurrentUserId(IHttpContextAccessor httpContextAccessor)
        {
            var user = httpContextAccessor.HttpContext?.User;

            // JWT middleware maps "sub" to ClaimTypes.NameIdentifier
            var userIdClaim = user?.FindFirst("sub")?.Value
                ?? user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            return Guid.TryParse(userIdClaim, out var id) ? id : Guid.Empty;
        }

        private string GetCurrentUserRole(IHttpContextAccessor httpContextAccessor)
        {
            var user = httpContextAccessor.HttpContext?.User;
            return user?.FindFirst(ClaimTypes.Role)?.Value ?? "STAFF";
        }
    }
}