using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectIboxDb.Models
{
    class Role : Dictionary<string,object>
    {
        public long Id
        {
            get { return (long)this["Id"]; }
            set { this["Id"] = value; }
        }
        public string RoleName
        {
            get { return (string)this["RoleName"]; }
            set { this["RoleName"] = value; }
        }
        public string Description
        {
            get { return (string)this["Description"]; }
            set { this["Description"] = value; }
        }

    }
}
