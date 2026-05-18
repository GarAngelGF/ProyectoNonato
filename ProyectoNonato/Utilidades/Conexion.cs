
using Microsoft.Data.SqlClient;

namespace ProyectoNonato.Utilidades
{
    public class Conexion
    {
        // Nota: Reemplaza "TuPassword" con la contraseña de adminprobd2026 definida en Azure
        public static string CadenaSQLBase = "Server=tcp:adminbdnonato26.database.windows.net,1433;Initial Catalog=EscuelaDBPADMBD;Persist Security Info=False;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
       // public static string CadenaSQLBase = "Server=.\\SQLEX;Initial Catalog=EscuelaDBPADMBD;TrustServerCertificate=True;";

        public static string GenerarCadenaDinamica(string user, string pass)
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(CadenaSQLBase)
            {
                UserID = user,
                Password = pass
            };
            return builder.ConnectionString;
        }
    }
}
