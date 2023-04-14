using System;
using System.Text;
namespace ListrApp.Base
{
    public class ListrLike
    {
        public ListrLike? Parent { get; protected set; }
        public string Task { get; protected set; }
        public bool Completed { get; protected set; }

        protected List<ListrLike> Children {get; set;}
        public ListrLike(ListrLike? parent, string task, bool completed, List<ListrLike>? children = null)
        {
            this.Parent = parent ?? null;
            this.Task = task;
            this.Completed = completed;
            this.Children = children ?? new List<ListrLike>();
        }
        public virtual void AddChild(string newTask)
        {
            ListrLike newListr = new ListrLike(this, newTask, false, null);
            this.Children.Add(newListr);
        }

        public virtual ListrLike GetChild(int childIndex)
        {
            childIndex = childIndex -1;
            if (childIndex > this.Children.Count)
            {
                throw new IndexOutOfRangeException();
            }
            return this.Children[childIndex];
        }
        public virtual void ToggleChildCompleted(int childIndex)
        {
            ListrLike child = this.GetChild(childIndex);
            child.Completed = !child.Completed;
        }
        public virtual void DeleteChild(int childIndex)
        {
            var child = this.GetChild(childIndex);
            this.Children.Remove(child);
        }
        public virtual ListrLike GetParent()
        {
            if(this.Parent == null)
            {
                throw new Exception("Current Listr is Root");
            }
            return this.Parent;
        }
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append(this.Task);
            sb.Append((this.Completed)?"✅\n":"\n");
            for (int i = 0; i < this.Children.Count; i++)
            {
                sb.AppendFormat("     {0}. {1}{2}", i+1,this.Children[i].Task, (this.Children[i].Completed)?"✅\n":"\n");
        
            }
            return sb.ToString();
        }
        
    }
}