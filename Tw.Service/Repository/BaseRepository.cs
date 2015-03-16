using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tw.Service.Provider.Db4o;

namespace Tw.Service.Repository
{
    public abstract class BaseRepository
    {

        private string _provider = string.Empty;
        public string Provider { get { return _provider; } set { _provider = value; } }

        public BaseRepository() { }
        public BaseRepository(string provider) { _provider = provider; }

        public virtual List<T> Query<T>()
        {
            List<T> l=new List<T>();

            using (var p = new BaseProvider(_provider))
            {
                l = p.Query<T>().ToList();
            }

            return l;
        }

        public T Query<T>(string key, object value)
        {
            T o = default(T);
            
            using (var p = new BaseProvider(_provider))
            {
                o = p.Query<T>("_" + key, value);
            }

            return o;
        }

        public bool Exist<T>(string key, object value)
        {
            var o = Query<T>(key, value);
            if (o != null)
                return true;
            else
                return false;
        }

        //http://msdn.microsoft.com/en-us/library/aa288454%28v=vs.71%29.aspx
        private string KeyColumn<T>(T obj)
        {
            string keycolumn = string.Empty;

            System.Reflection.MemberInfo info = typeof(T);
            object[] attributes = info.GetCustomAttributes(true);

            for (int i = 0; i < attributes.Length; i ++)
            {
                if(attributes[0].GetType()==typeof(Tw.Model.Entity.PersistanceAttribute))
                {
                    Tw.Model.Entity.PersistanceAttribute attr = attributes[0] as Tw.Model.Entity.PersistanceAttribute;
                    keycolumn = attr.KeyColumn;
                    break;
                }
                
            }
            return keycolumn;
        }

        private object KeyValue<T>(T obj)
        {
            var prop = typeof(T).GetProperties();
            var keycolumn=KeyColumn(obj);
            var column = prop.FirstOrDefault(x=>x.Name.ToLower()==keycolumn);

            object keyvalue = null;
 
            if(column!=null)
                keyvalue = column.GetValue(obj, null);

            return keyvalue;
        }

        private string Db4oFriendlyKeyColumn<T>(T obj)
        {
            var col = "_" + KeyColumn(obj).ToLower();
            return col;
        }

        public virtual bool Save<T>(T obj)
        {
            using (var p = new BaseProvider(_provider))
            {
                return p.Save<T>(Db4oFriendlyKeyColumn(obj), KeyValue(obj), obj);
            }
        }

        public virtual int Insert<T>(List<T> obj)
        {
            using (var p = new BaseProvider(_provider))
            {
                return p.Insert<T>(obj);
            }
        }

        public virtual int Delete<T>(T obj)
        {
            using (var p = new BaseProvider(_provider))
            {
                return p.Delete<T>(Db4oFriendlyKeyColumn(obj), KeyValue(obj));
            }
        }

        public virtual int Delete<T>(string key, string value)
        {
            using (var p = new BaseProvider(_provider))
            {
                return p.Delete<T>("_" + key, value);
            }
        }

        public virtual bool Truncate<T>()
        {
            using (var p = new BaseProvider(_provider))
            {
                return p.Truncate<T>();
            }
        }

        public virtual void Index<T>(string column,bool create=true)
        {
            using (var p = new BaseProvider(_provider))
            {
                if(create)
                    p.CreateIndex<T>(column);
                else
                    p.DropIndex<T>(column);
            }
        }
    }
}
