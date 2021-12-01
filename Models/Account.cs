using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectIboxDb.Models
{
    class Account : Dictionary<string, object>
    {
        public long Id 
        {
            get { return (long)this["Id"]; }
            set { this["Id"] = value; }
        }
        public string Username
        {
            get { return (string)this["Username"]; }
            set { this["Username"] = value; }
        }
        public string Password
        {
            get { return (string)this["Password"]; }
            set { this["Password"] = value; }
        }
        public int RoleId
        {
            get { return (int)this["RoleId"]; }
            set { this["RoleId"] = value; }
        }

    }
}
