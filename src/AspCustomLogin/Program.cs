namespace AspCustomLogin
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
            => Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(host =>
            {
                host.UseStartup<Startup>();
#if DEBUG
#else
                host.UseUrls("http://0.0.0.0:6133");
#endif
            });
    }
}