namespace Dreamrosia.Koin.UPbit.Infrastructure.Clients
{
    public class Error
    {
        public string Name { get; set; }

        public string Message { get; set; }
    }

    public class ErrorRoot
    {
        public Error Error { get; set; }
    }
}