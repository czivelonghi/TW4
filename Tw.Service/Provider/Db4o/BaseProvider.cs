using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using Db4objects.Db4o;
using Db4objects.Db4o.Query;
using Db4objects.Db4o.Linq;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Config.Encoding;
using Db4objects.Db4o.Diagnostic;
using Db4objects.Db4o.IO;
using Db4objects.Db4o.Reflect;
using Db4objects.Db4o.Typehandlers;
using Tw.Model;

namespace Tw.Service.Provider.Db4o
{
    public class BaseProvider :  IDisposable
    {
        public IObjectContainer Container;

        public BaseProvider() { }

        public string Path { get; set; }

        public bool Paging=false;

        public BaseProvider(string db)
        {
            Connect(db);
        }

        private void Connect(string db)
        {

            var path = Tw.Service.Common.Utility.Configuration.Connection(db);

            if (Paging)
            {
                var config = Db4oEmbedded.NewConfiguration();
                var memory = new PagingMemoryStorage();
                config.File.Storage = memory;
                Container = Db4oEmbedded.OpenFile(config, path);
            }
            else
            {
                //var config = Db4oEmbedded.NewConfiguration();
                //var memory = new MemoryStorage();
                //config.File.Storage = memory;
                //Container = Db4oEmbedded.OpenFile(config, path);
                Container = Db4oEmbedded.OpenFile(path);
            }
        }

        public IEnumerable<T> Query<T>()
        {
            var results = (from T o
                           in Container
                           select o);

            return results;
        }

        public T Query<T>(string key, object value)
        {
            IQuery query = Container.Query();
            query.Constrain(typeof(T));
            query.Descend(key).Constrain(value);
            IObjectSet result = query.Execute();
            T found = default(T);
            if (result.Count > 0)
            {
                found = (T)result.Next();
            }

            return found;
        }

        //http://community.versant.com/documentation/reference/db4o-7.12/java/reference/html/content/object_lifecycle/querying/soda_query/soda_query_api.html
        public bool Save<T>(string key, object keyValue, T obj)
        {
            bool success = false;

            IQuery query = Container.Query();
            query.Constrain(typeof(T));
            query.Descend(key).Constrain(keyValue);
            IObjectSet result = query.Execute();

            if (result.Count > 0)
            {
                T found = (T)result.Next();
                Copy(ref found, obj);
                Container.Store(found);
                success = true;
            }
            else
            {
                Container.Store(obj);
                success = true;
            }

            return success;
        }

        public int Insert<T>(List<T> obj)
        {
            int count=0;

            foreach(var o in obj)
            {
                 Container.Store(o);
                 count +=1;
            }

            return count;
        }

        private void Copy<T>(ref T oldObj, T newObject)
        {
            var prop = typeof(T).GetProperties();

            foreach (var p in prop)
            {
                var value = p.GetValue(newObject, null);
                p.SetValue(oldObj, value, null);
            }
        }

        public bool Delete<T>(T obj)
        {
            bool success = false;

            IObjectSet result = Container.QueryByExample(obj);

            if (result.Count > 0)
            {
                T found = (T)result.Next();
                Container.Delete(found);
                success = true;
            }

            return success;
        }

        //public bool Delete<T>(string key, object keyValue, T obj)
        //{
        //    bool success = false;

        //    IQuery query = Container.Query();
        //    query.Constrain(typeof(T));
        //    query.Descend(key).Constrain(keyValue);
        //    IObjectSet result = query.Execute();

        //    if (result.Count > 0)
        //    {
        //        Container.Delete(result[0]);
        //        success = true;
        //    }

        //    return success;
        //}

        public int Delete<T>(string key, object value)
        {
            
            int count = 0;

            IQuery query = Container.Query();
            query.Constrain(typeof(T));
            query.Descend(key).Constrain(value);
            IObjectSet result = query.Execute();
            
            foreach(var o in result)
            {
                Container.Delete(o);
                count +=1;
            }

            return count;
        }

        public bool Truncate<T>()
        {
            bool success = false;

            IQuery query = Container.Query();
            query.Constrain(typeof(T));
            IObjectSet result = query.Execute();

            foreach(var o in result)
            {
                Container.Delete(o);
                success = true;
            }

            return success;
        }

        public void CreateIndex<T>(string column)
        {
            var col = "_" + column.ToLower();
            Db4oFactory.Configure().ObjectClass(typeof(T)).ObjectField(col).Indexed(true);
        }

        public void DropIndex<T>(string column)
        {
            var col = "_" + column.ToLower();
            Db4oFactory.Configure().ObjectClass(typeof(T)).ObjectField(column).Indexed(false);
        }

        public void Dispose()
        {
            if (Container != null)
            {
                Container.Dispose();
                GC.SuppressFinalize(Container);
            }
        }
    }
    
}
