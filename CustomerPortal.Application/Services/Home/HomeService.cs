using CustomerPortal.Application.Helper;
using CustomerPortal.Application.Models;
using CustomerPortal.Domain.Entities;
using CustomerPortal.Domain.IRepositories;

namespace CustomerPortal.Application.Services.Home
{
    public class HomeService : IHomeService
    {
        private readonly IHomeContentCardRepository _homeContentCardRepository;
        private readonly IUserRepository _userRepository;

        public HomeService(
            IHomeContentCardRepository homeContentCardRepository,
            IUserRepository userRepository)
        {
            _homeContentCardRepository = homeContentCardRepository;
            _userRepository = userRepository;
        }

        public async Task<ApiResponse<HomeModel.HomeResponse>> GetAsync(Guid userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user is null)
            {
                return ApiResponse<HomeModel.HomeResponse>.ErrorResponse(
                    ApplicationConstant.ResponseMessages.UserNotFound,
                    ApplicationConstant.StatusCodes.NotFound);
            }

            var cards = await _homeContentCardRepository.GetAllActiveAsync();

            return ApiResponse<HomeModel.HomeResponse>.SuccessResponse(
                new HomeModel.HomeResponse
                {
                    UserId = user.RecId,
                    FirstName = user.FirstName,
                    Cards = cards.Select(MapToCardResponse).ToList()
                },
                ApplicationConstant.ResponseMessages.HomeRetrieved);
        }

        private static HomeModel.HomeContentCardResponse MapToCardResponse(
            HomeContentCard card) => new()
            {
                RecId = card.RecId,
                Title = card.Title,
                Body = card.Body,
                ImageUrl = card.ImageUrl,
                ActionUrl = card.ActionUrl,
                DisplayOrder = card.DisplayOrder
            };
    }
}
