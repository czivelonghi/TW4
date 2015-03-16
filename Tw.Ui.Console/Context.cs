using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tw.Service.Common;

namespace Tw.Ui.Console
{
    public class Context
    {
        #region variables/classes
        class ObjectContext
        {
            public string Parent { set; get; }
            public string Context { set; get; }
            public string Name { set; get; }
            public object Object { set; get; }
        }

        public class Node
        {
            public bool Active { get; set; }
            public bool Nav { get; set; }
            public string Name { get; set; }
            public string Parent { get; set; }

        }

        List<Node> Nodes = new List<Node>();
        List<ObjectContext> objcontext = new List<ObjectContext>();

        #endregion

        #region public

        public Context()
        {
            Nodes.Add(new Node() { Nav = false, Active = false, Name = "root" });
        }

        public bool NodeExists(string name)
        {
            return objcontext.Exists(x => x.Name == name);
        }

        public void AddNode(string name, string parent = "root", bool active = false)
        {
            Nodes.Add(new Node() { Nav = true, Active = active, Name = name, Parent = parent });
        }

        public void Create<T>(string context, string name)
        {
            var o = objcontext.FirstOrDefault(x => x.Context == context);
            var new_obj = CreateObject<T>(name);

            if (o != null)
            {
                o.Name = name;
                o.Object = new_obj;
            }
            else
                objcontext.Add(new ObjectContext() { Context = context, Name = name, Object = new_obj });

        }

        private static void CopyTo(object S, object T)
        {
            foreach (var pS in S.GetType().GetProperties())
            {
                foreach (var pT in T.GetType().GetProperties().Where(x => ((x.Name.ToLower() != "name") && (x.Name.ToLower() != "id"))))
                {
                    if (pT.Name != pS.Name) continue;
                    (pT.GetSetMethod()).Invoke(T, new object[] { pS.GetGetMethod().Invoke(S, null) });
                }
            };
        }

        public void Copy<T>(string context, string name)
        {
            var obj_ctx = objcontext.FirstOrDefault(x => x.Context == context);

            if (obj_ctx != null)
            {
                var new_obj = CreateObject<T>(name);
                CopyTo(obj_ctx.Object, new_obj);
                obj_ctx.Name = name;
                obj_ctx.Object = new_obj;
            }

        }

        //load object
        public bool Load(string context, object obj)
        {
            bool success = false;

            var i = objcontext.Find(element => element.Context == context);

            if (obj != null)
            {
                if (i != null)
                    i.Object = obj;
                else
                    objcontext.Add(new ObjectContext() { Context = context, Name = "", Object = obj });
            }
            return success;
        }

        //cd entry
        public bool Navigate(string cmd)
        {
            if (cmd == "..")
                return DownNode();
            else
                return UpNode(cmd);
        }


        public void Clear()
        {
            Nodes.Clear();
            objcontext.Clear();
        }

        public void Clear(string context)
        {
            var nc = ChildNodeChain();//clear all previous children

            foreach (var c in nc)
            {
                var i = objcontext.FindIndex(element => element.Context == c);
                if (i > -1)
                {
                    objcontext.RemoveAt(i);
                }
            }

        }

        public void ClearActiveContext()
        {
            Clear(ActiveNodeName());
        }

        public string GetContextName(string context)
        {
            var o = objcontext.FirstOrDefault(x => x.Context == context);
            if (o != null)
                return Utility.Attribute.Value(o.Object, "Name").ToString();
            else
                return null;
        }

        public T GetObject<T>(string context)
        {
            T obj = default(T);

            var o = objcontext.FirstOrDefault(x => x.Context == context);
            if (o != null) obj = (T)o.Object;

            return obj;

        }

        public object GetActiveObject()
        {
            object obj = objcontext.FirstOrDefault(x => x.Context == ActiveNodeName());
            if (obj != null)
                return (obj as ObjectContext).Object;
            else
                return null;
        }

        public List<String> NodeChain()
        {
            List<String> c = new List<string>();
            var n = ActiveNodeName();
            while (n != string.Empty)
            {
                c.Add(n);
                n = Parent(n);
            }
            c.Reverse();
            return c;
        }

        //list child objects from current node
        public List<String> ChildNodeChain()
        {
            List<String> c = new List<string>();
            var n = ActiveNodeName();

            while (n != string.Empty)
            {
                c.Add(n);
                n = Child(n);
            }
            c.Reverse();
            return c;
        }
        public string ActiveNodeName()
        {
            var n = ActiveNode();
            if (n != null)
                return n.Name;
            else
                return "";
        }

        public string ActiveContextName()
        {
            return GetContextName(ActiveNodeName());
        }

        public Node ActiveNode()
        {
            return Nodes.FirstOrDefault(x => x.Active == true);
        }
        #endregion

        #region private
        T CreateObject<T>(params object[] args)
        {
            return (T)Activator.CreateInstance(typeof(T), args);
        }

        void DeactiveNode()
        {
            var n = Nodes.FirstOrDefault(x => x.Active);
            if (n != null) n.Active = false;
        }

        bool UpNode(string nodename)
        {
            bool nav = false;
            var an = ActiveNodeName();
            if (nodename != an)
            {
                var n = Nodes.FirstOrDefault(x => ((x.Parent == an) || (x.Parent == "root")) && (x.Name == nodename) && (x.Nav == true));
                if (n != null)
                {
                    DeactiveNode();
                    n.Active = true;
                    nav = true;
                }
            }
            else
            {
                nav = true;
            }
            return nav;
        }
        //cd ..
        bool DownNode()
        {
            bool nav = false;
            var an = ActiveNode();
            var n = Nodes.FirstOrDefault(x => x.Name == an.Parent && x.Nav == true);
            if (n != null)
            {
                ClearActiveContext();
                DeactiveNode();
                n.Active = true;
                nav = true;
            }
            return nav;
        }

        string Parent(string child)
        {
            var n = Nodes.FirstOrDefault(x => x.Name == child && x.Nav == true);
            var p = Nodes.FirstOrDefault(x => x.Name == n.Parent && x.Nav == true);
            if (p != null)
                return p.Name;
            else
                return "";

        }

        string Child(string name)
        {
            var c = "";
            var p = Nodes.FirstOrDefault(x => x.Parent == name && x.Nav == true);
            if (p != null) c = p.Name;
            return c;
        }
        #endregion
    }
}
