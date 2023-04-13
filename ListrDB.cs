using Listr.Base;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System.Text;

namespace Listr.MySQL
{
    public class Listr : ListrLike
    {
        protected class Connection
        {
            public MySqlConnection conn { get; }
            public Connection(string connString)
            {
                conn = new MySqlConnection(connString);
                conn.Open();
            }
            ~Connection()
            {
                conn.Close();
            }
        }

        private Connection conn { get; }

        private List<ListrLike> Children {get; set;}

        public Listr(Guid id, string connString) : this(id, new Connection(connString)) { }

        private Listr(ListrLike list, Connection conn) : this(list.Id, conn) { }
        private Listr(Guid id, Connection conn) : base(id, Guid.Empty, Guid.Empty, "", false)
        {
            this.conn = conn;
            MySqlConnection connex = conn.conn;
            string query = "SELECT * FROM listr WHERE id = @id";
            using (MySqlCommand cmd = new MySqlCommand(query, connex))
            {
                cmd.Parameters.AddWithValue("@id", id);
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        Guid parent = reader.GetGuid("parent");
                        Guid user = reader.GetGuid("user");
                        string task = reader.GetString("task");
                        bool completed = reader.GetBoolean("completed");
                        base.Parent = parent;
                        base.User = user;
                        base.Task = task;
                        base.Completed = completed;
                    }
                }
                this.Children = new List<ListrLike>();
                query = "SELECT * FROM listr WHERE parent = @id";
                using (MySqlCommand cmd2 = new MySqlCommand(query, connex))
                {
                    cmd2.Parameters.AddWithValue("@id", id);
                    using (MySqlDataReader reader = cmd2.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Guid childId = reader.GetGuid("id");
                            Guid childParent = reader.GetGuid("parent");
                            if (childId == childParent)
                            {
                                continue;
                            }
                            Guid childUser = reader.GetGuid("user");
                            string childTask = reader.GetString("task");
                            bool childCompleted = reader.GetBoolean("completed");
                            ListrLike child = new ListrLike(childId, childParent, childUser, childTask, childCompleted);
                            Children.Add(child);
                        }
                    }
                }
            }
        }

        public bool AddChild(string newTask)
        {
            Guid newId = Guid.NewGuid();
            using(var cmd = new MySqlCommand("INSERT INTO listr (id, parent, user, task) VALUES (@id, @parent, @user, @task)",conn.conn))
            {
                cmd.Parameters.AddWithValue("@id",newId);
                cmd.Parameters.AddWithValue("@parent",this.Id);
                cmd.Parameters.AddWithValue("@user",this.User);
                cmd.Parameters.AddWithValue("@task",newTask);
                int rowsAffected = cmd.ExecuteNonQuery();
                if (rowsAffected != 1) {
                    throw new Exception("Insert did not work");
                }
            }
            var newListr = new ListrLike(newId, this.Id, this.User, newTask, false);
            this.Children.Add(newListr);
            return true;
        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(this.Task);
            if (this.Completed)
            {
                sb.Append("✅");
            }
            foreach (var child in this.Children)
            {
                sb.Append("\n");
                sb.AppendFormat("    {0}", child.Task);
            }
            return sb.ToString();
        }


    }
}