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
    public class FilterRepository: BaseRepository
    {
        public List<Entity.Filter> Object = new List<Entity.Filter>();

        public FilterRepository() : base("user") { }

        public void Load()
        {
            Object = base.Query<Entity.Filter>();
        }

    }
}
