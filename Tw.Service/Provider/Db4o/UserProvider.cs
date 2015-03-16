using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tw.Service.Provider.Db4o;
using Entity = Tw.Model.Entity;

namespace Tw.Service.Provider.Db4o
{
    class UserProvider : BaseProvider
    {
        public UserProvider(): base("user")
        {
        }
    }
}
