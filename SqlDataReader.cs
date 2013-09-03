using System;
using System.Collections.Generic;
using Amica;
using Amica.Data;
using DataAccess;


namespace Amica.Data
{
    public class SqlDataReader : DataReader
    {
        /// <summary>
        /// 
        /// </summary>
        private Dictionary <ComparisonOperator, OperatorInfo> OpDict = new Dictionary<ComparisonOperator,OperatorInfo>()
        {
            { ComparisonOperator.BeginsWith, new OperatorInfo {Operator=" LIKE ", Suffix="%"}},
            { ComparisonOperator.Contains, new OperatorInfo {Operator=" LIKE ", Prefix="%", Suffix="%"}},
            { ComparisonOperator.EndsWith, new OperatorInfo {Operator=" LIKE ", Prefix="%"}},
            { ComparisonOperator.NotContains, new OperatorInfo {Operator=" NOT LIKE ", Prefix="%", Suffix="%"}},
            { ComparisonOperator.Equal, new OperatorInfo {Operator=" = "}},
            { ComparisonOperator.NotEqual, new OperatorInfo {Operator=" <> "}},
            { ComparisonOperator.GreaterThan, new OperatorInfo {Operator=" > "}},
            { ComparisonOperator.GreaterThenOrEqual, new OperatorInfo {Operator=" >= "}},
            { ComparisonOperator.LessThan, new OperatorInfo {Operator=" < "}},
            { ComparisonOperator.LessThanOrEqual, new OperatorInfo {Operator=" <= "}}
        };

        /// <summary>
        /// 
        /// </summary>
        public SqlDataReader()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public string DataSourceName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Authentication Autentication { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="request"></param>
        /// <returns></returns>
        public override Response<T> Get<T>(GetRequest request)
        {
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="request"></param>
        /// <returns></returns>
        public override Response<T> Get<T>(GetRequestItem request)
        {
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filters"></param>
        /// <returns></returns>
        protected new string ParseFilters(IList<Filter> filters)
        {
            string s = "";
            string concat = "";

            foreach (Filter f in filters)
            {
                s += concat.Length > 0 ? concat : "";
                s += f.Name + OpDict[f.Operator].Operator + (f.Value == null ? "NULL" : FormatSQLValue(f));
                concat = " " + f.Concatenation.ToString().ToUpper() + " ";
                if (f.Filters != null && f.Filters.Count > 0)
                    s += concat + "(" + ParseFilters(f.Filters) + ")";
            }
            return (s).Trim();
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        private string FormatSQLValue(Filter f)
        {
            switch (f.Value.GetType().Name)
            {
                case "String": return "'" + OpDict[f.Operator].Prefix + ((string)f.Value).Replace("'", "''") + OpDict[f.Operator].Suffix + "'";
                case "DateTime": return "'" + ((DateTime)f.Value).ToString("yyyy/MM/dd hh:mm:ss") + "'";
                case "Boolean": return (bool)f.Value ? "TRUE" : "FALSE";
                default: return f.Value.ToString();
            }                
        }

        /// <summary>
        /// 
        /// </summary>
        private class OperatorInfo
        {
            /// <summary>
            /// 
            /// </summary>
            public string Operator { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string Prefix { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string Suffix { get; set; }
        }
    }
}
