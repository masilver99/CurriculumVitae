using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace CV.data
{
    public class StringArrayHandler : SqlMapper.TypeHandler<List<string>>
    {
        public override List<string> Parse(object value)
        {
            return (value as string).Split('|').ToList();
        }

        public override void SetValue(IDbDataParameter parameter, List<string> value)
        {
            //throw new NotImplementedException();
        }
    }
}
