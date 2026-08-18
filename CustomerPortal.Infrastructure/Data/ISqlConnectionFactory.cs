using Microsoft.Data.SqlClient;

namespace CustomerPortal.Infrastructure.Data
{
    public interface ISqlConnectionFactory
    {
        SqlConnection Create();
    }
}
