using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemoryComp
{
    public class Account
    {
        private int userid;
        private string username;
        private int megyeid;
        public Account(int id, string uname, int megye)
        {
            userid = id;
            Username = uname;
            Megyeid = megye;
        }
        public int Userid { get => userid; private set => userid = value; }
        public string Username { get => username; private set => username = value; }
        public int Megyeid { get => megyeid; private set => megyeid = value; }
    }
}
