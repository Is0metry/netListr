// See https://aka.ms/new-console-template for more information
using Microsoft.Extensions.Configuration;

namespace Listr
{
    class Program
    {
        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
            .AddUserSecrets<Program>();
            var configuration = builder.Build();
            var connectionString = $"server={configuration["server"]};uid={configuration["MySQLUsername"]};pwd={configuration["MySQLPassword"]};database={configuration["database"]};";
            var dbLister = new Listr.MySQL.Listr(Guid.Parse("3e2cc6bd-d972-11ed-9cba-0242ac110002"),connectionString);
            Console.WriteLine(dbLister.ToString());
            dbLister.AddChild("This is a test of AddChild");
        }
    }
}