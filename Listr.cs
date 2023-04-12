using System;

namespace Listr.Base
{
    public class ListrLike
    {
        public ListrLike? Parent {get; protected set;}

        public List<ListrLike> Children {get; set;} = 
        public string Task { get; protected set; }
        public bool Completed { get; protected set; }
        public ListrLike(ListrLike? parent, List<ListrLike>? children, string task, bool completed)
        {
            this.Parent = parent;
            if (children == null)
            {
                children = new List<ListrLike>();
            }
            this.Children = children;
            this.Task = task;
            this.Completed = completed;
        }
        public void AddChild(string NewTask)
        {
            var NewChild = new ListrLike(this,null, NewTask, false );
            this.Children.Add(NewChild);
        }
        
        public virtual bool CompleteChild(int childIndex)
        {
            childIndex = childIndex -1;
            if (childIndex < this.Children.Count)
            {
                throw new Exception("index out of bounds");
            }
            ListrLike child = this.Children[childIndex];
            child.Completed = !child.Completed;
            return child.Completed;

        }
        public virtual ListrLike GetChild(int child) 
        {
            child = child - 1;
            if (child < this.Children.Count)
            {
                throw new Exception("index out of bounds");
            }
            return this.Children[child];
        }

    }
}