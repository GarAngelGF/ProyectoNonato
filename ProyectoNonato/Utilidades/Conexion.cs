
using System.Data.SqlClient;

namespace ProyectoNonato.Utilidades
{
    public class Conexion
    {
        // Nota: Reemplaza "TuPassword" con la contraseña de adminprobd2026 definida en Azure
        public static string CadenaSQL = "Server=tcp:adminbdnonato26.database.windows.net,1433;Initial Catalog=EscuelaDBPADMBD;Persist Security Info=False;User ID=adminprobd2026;Password=Birriamasters2026#;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
    }
}
