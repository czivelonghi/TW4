using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

//http://discusscode.blogspot.com/2012/03/solving-potentially-dangerous.html
namespace Tw.Model.Entity
{
    //[AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    //sealed class Expression : ValidationAttribute, IMetadataAware
    //{
    //    public Expression()
    //        : base("no script tags please..!")
    //    {
    //    }

    //    public override bool IsValid(object value)
    //    {
    //        if (value.ToString().ToLower().Contains("<script>"))
    //        {
    //            return false;
    //        }
    //        return true;
    //    }

    //    public void OnMetadataCreated(ModelMetadata metadata)
    //    {
    //        metadata.RequestValidationEnabled = false;
    //    }

    //}

    [AttributeUsage(AttributeTargets.All)]
    public class PersistanceAttribute : System.Attribute
    {
        public PersistanceAttribute()
        {
        }

        private string _keycolumn = "";
        public string KeyColumn
        {
            get { return _keycolumn; }
            set { _keycolumn = value; }
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class BindingAttribute : System.Attribute
    {
        public BindingAttribute()
        {
        }
    }
}
