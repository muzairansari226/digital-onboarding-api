namespace CustomerPortal.Application.Models
{
    public class HomeModel
    {
        public class HomeResponse
        {
            public Guid UserId { get; set; }

            public string FirstName { get; set; } = string.Empty;

            public List<HomeContentCardResponse> Cards { get; set; } = new();
        }

        public class HomeContentCardResponse
        {
            public Guid RecId { get; set; }

            public string Title { get; set; } = string.Empty;

            public string? Body { get; set; }

            public string? ImageUrl { get; set; }

            public string? ActionUrl { get; set; }

            public int DisplayOrder { get; set; }
        }
    }
}
