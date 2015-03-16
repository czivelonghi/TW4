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
    public class QueryRepository: BaseRepository
    {

        public List<Entity.Query> Object = new List<Entity.Query>();

        public QueryRepository() : base("user") { }

        public void Load()
        {
            Object = base.Query<Entity.Query>();
        }

    }
}
