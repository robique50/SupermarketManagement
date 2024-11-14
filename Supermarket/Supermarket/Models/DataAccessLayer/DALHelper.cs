using System.Data.SqlClient;
using System.Configuration;


namespace Supermarket.Models.DataAccessLayer
{
    class DALHelper
    {
        private static readonly string connectionString = ConfigurationManager.ConnectionStrings["myConStr"].ConnectionString;

        public static SqlConnection Connection
        {
            get
            {
                return new SqlConnection(connectionString);
            }
        }
    }
}
