using System;
using DataAccess;

namespace Amica.Data
{
    public class SqlGetRequest : GetRequest
    {
        public string DataSourcePassword { get; set; }
    }
}
