using System;
using DataAccess;

namespace Amica.Data
{
    public class SqlGetRequestItem : GetRequestItem
    {
        public string DataSourcePassword { get; set; }
    }
}
