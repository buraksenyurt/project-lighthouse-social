using System.Data;

namespace LighthouseSocial.Data;

public interface IDbConnectionFactory
{
    IDbConnection CreateConnection();
}
