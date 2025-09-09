namespace CemSys2.Interface
{
    public interface IPdfService
    {
        Task<byte[]> GeneratePdfAsync(string viewName, object model, HttpContext httpContext);
    }
}
