namespace Service
{
    class Program
    {
        private static async System.Threading.Tasks.Task Main(string[] args) => await BuidHost(args).RunAsync(args);

        private static ServiceHost BuidHost(string[] args) => new ServiceHost(args);
    }
}
