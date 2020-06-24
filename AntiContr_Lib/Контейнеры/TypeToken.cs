using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntiContr_Lib
{
    public enum TypeUser
    {
        User,
        Admin,
        None        
    }

    public class TypeToken
    {
       public string token;
       public TypeUser typeUser;
    }
}
