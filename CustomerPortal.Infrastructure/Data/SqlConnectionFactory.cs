using CustomerPortal.Application.Helper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace CustomerPortal.Infrastructure.Data
{
    public class SqlConnectionFactory : ISqlConnectionFactory
    {
        private readonly string _connectionString;

        public SqlConnectionFactory(IConfiguration configuration)
        {
            _connectionString = ConnectionStringResolver.Resolve(configuration);
        }

        public SqlConnection Create() => new(_connectionString);
    }

    public static class ConnectionStringResolver
    {
        // appsettings holds only the NAME of the environment variable, never the connection
        // string itself, so no credential is ever committed. Run set-connection-string.bat to
        // create the variable.
        public static string Resolve(IConfiguration configuration)
        {
            var variableName =
                configuration[ApplicationConstant.ConfigurationSections.SqlConnectionVariableName]
                ?? throw new InvalidOperationException(
                    "The SQL connection environment variable name is not configured.");

            return Environment.GetEnvironmentVariable(variableName)
                ?? throw new InvalidOperationException(
                    $"The environment variable '{variableName}' does not hold a SQL connection " +
                    "string. Run set-connection-string.bat, then restart your terminal or IDE.");
        }
    }
}
