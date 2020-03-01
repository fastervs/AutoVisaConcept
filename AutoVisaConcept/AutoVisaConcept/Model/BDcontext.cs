using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoVisaConcept.Model
{
    class BDcontext
    {
        string connstring = "server=127.0.0.1;Port=5432;Database=test1;User Id=postgres;Password = 1; ";

        public  NpgsqlConnection conn;

        public void BDcontextinit()
        {
            conn = new NpgsqlConnection(connstring);
        }
         
        
    }
}
