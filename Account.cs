using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemoryComp
{
    class Account
    {
        int userid;
        string username;
        public Account(int id, string uname)
        {
            userid = id;
            Username = uname;
        }
        public int Userid { get => userid; private set => userid = value; }
        public string Username { get => username; private set => username = value; }
    }
}
