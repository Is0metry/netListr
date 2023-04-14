using ListrApp.Base;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System.Text;

namespace ListrApp.MySQL
{
    public class Listr : ListrLike
    {
        protected class Connection
        {
            public MySqlConnection Conn { get; }
            public Connection(string connString)
            {
                Conn = new MySqlConnection(connString);
                Conn.Open();
            }
            ~Connection()
            {
                Conn.Close();
            }
        }

        private Connection conn { get; }

        public Guid Id;
        public Guid? ParentId;

        private Listr(
            Connection c,
            Listr parent,
            Guid parentId,
            MySqlDataReader reader) : base(parent, "", false)
        {
            this.Id = reader.GetGuid("id");
            this.conn = c;
            this.ParentId = parentId;
            this.Task = reader.GetString("task");
            this.Completed = reader.GetBoolean("completed");
        }
        private Listr(Guid id, Connection c, Listr? parent, Guid?h parentId, string task, bool completed)
        {
            this.Id = id;
            this.conn = c;
            this.Parent = parent;
            this.ParentId = parentId;
            this.Task = task;
            this.Completed = completed;
        }

        private Listr(Guid id, Connection conn, Listr? parent) : base(parent, "", false)
        {
            this.conn = conn;
            string query = "SELECT * FROM listr WHERE id = @id";
            using (var cmd = new MySqlCommand(query, conn.Conn))
            {
                cmd.Parameters.AddWithValue("@id", id);
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    if (!reader.Read())
                    {
                        throw new Exception($"No record found with ID: {id}");
                    }
                    Guid? parentId;
                    try
                    {
                        parentId = reader.GetGuid("parent");
                    }
                    catch (MySqlException)
                    {
                        parentId = null;
                    }
                    this.Id = id;
                    this.Parent = parent;
                    this.ParentId = parentId;
                    this.conn = conn;

                }
            }
            query = "SELECT * FROM listr WHERE parent = @parent";
            using (MySqlCommand cmd2 = new MySqlCommand(query, conn.Conn))
            {
                cmd2.Parameters.AddWithValue("@parent", id);
                using (var reader = cmd2.ExecuteReader())
                {
                    while (reader.Read())
                    { this.Children.Add(new Listr(conn, this, id, reader)); }
                }
            }

        }
        public Listr(Guid id, string connString, Listr? parent = null) : this(id, new Connection(connString), parent) { }

        public override void AddChild(string newTask)
        {
            string query = "INSERT INTO listr (id, parent, user, task) VALUES (@id,@parent,@user,@task)";
            Guid id = Guid.NewGuid();
            using (var cmd = new MySqlCommand(query,this.conn.Conn))
            {
                cmd.Parameters.AddWithValue("@id",id);
                cmd.Parameters.AddWithValue("@parent",this.Id);
                cmd.Parameters.AddWithValue("@user", Guid.Empty);
                cmd.Parameters.AddWithValue("@task",newTask);
                if (cmd.ExecuteNonQuery() != 1)
                {
                    throw new Exception("there was an error with the insertion (lol).");
                }
                this.Children.Add(new Listr(id,this.conn, this,this.ParentId,newTask,false));

            }
        }
    }
}