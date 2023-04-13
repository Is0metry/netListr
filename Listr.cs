using System;

namespace Listr.Base
{
    public class ListrLike
    {
        public Guid Id {get; protected set;}
        public Guid Parent { get; protected set; }

        public Guid User {get; protected set;}
        public string Task { get; protected set; }
        public bool Completed { get; protected set; }

        public ListrLike(Guid id, Guid parent, Guid user, string task, bool completed)
        {
            this.Parent = parent;
            this.Task = task;
            this.Completed = completed;
        }
        

}