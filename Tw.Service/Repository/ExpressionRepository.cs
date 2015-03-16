using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tw.Service.Provider.Db4o;
using Entity = Tw.Model.Entity;
using Common = Tw.Service.Common;
using Utility = Tw.Service.Common.Utility;

namespace Tw.Service.Repository
{
    public class ExpressionRepository: UserRepository
    {
        public List<Entity.Expression> Expression = new List<Entity.Expression>();

        public ExpressionRepository()
        {
        }

        public void Load()
        {
            //load user funcs
            Expression = base.Query<Entity.Expression>();
            Expression.Sort((x, y) => x.Name.CompareTo(y.Name));

            //load system
            this.Provider="system";
            var builtinfuncs = base.Query<Entity.Expression>();
            Expression.AddRange(builtinfuncs);
        }
    }
}
