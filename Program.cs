// See https://aka.ms/new-console-template for more information
using Microsoft.Extensions.Configuration;
using ListrApp.Base;
using ListrApp.MySQL;
namespace ListrApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
            .AddUserSecrets<Program>();
            var configuration = builder.Build();
            var connectionString = $"server={configuration["server"]};uid={configuration["MySQLUsername"]};pwd={configuration["MySQLPassword"]};database={configuration["database"]};";
            var dbLister = new Listr(Guid.Parse("3e2cc6bd-d972-11ed-9cba-0242ac110002"),connectionString);
            Console.WriteLine(dbLister);
            dbLister.AddChild("This is a test of AddChild");
            ListrLike l = new ListrApp.Base.ListrLike(null, "This is a root task", false);
            Console.WriteLine(l);
            string[] newTasks = {"this is a new task","this is a second new task", "this is the third task"};
            foreach (string s in newTasks )
            {
                l.AddChild(s);
            }
            Console.WriteLine(l);
            ListrLike lchild = l.GetChild(1);
            Console.WriteLine(lchild);
            lchild = lchild.GetParent();
            Console.WriteLine($"{lchild}\n\n{l}");

        }
    }
}