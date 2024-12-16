namespace ADXAPI
{
    public interface IJWTGenerator
    {
        string GenerateToken(string email, bool adxAccess);
    }
}
