using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tw.Service.Provider.Db4o;
using Entity = Tw.Model.Entity;
using Common = Tw.Service.Common;

namespace Tw.Service.Repository
{
    public class SystemRepository: BaseRepository
    {
        public SystemRepository(): base("system")
        {
        }
    }
}
