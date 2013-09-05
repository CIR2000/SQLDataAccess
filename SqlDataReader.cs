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
        private Dictionary <Comparison, OperatorInfo> OpDict = new Dictionary<Comparison, OperatorInfo>()
        {
            { Comparison.BeginsWith, new OperatorInfo {Operator=" LIKE ", Suffix="%"}},
            { Comparison.Contains, new OperatorInfo {Operator=" LIKE ", Prefix="%", Suffix="%"}},
            { Comparison.EndsWith, new OperatorInfo {Operator=" LIKE ", Prefix="%"}},
            { Comparison.NotContains, new OperatorInfo {Operator=" NOT LIKE ", Prefix="%", Suffix="%"}},
            { Comparison.Equal, new OperatorInfo {Operator=" = "}},
            { Comparison.NotEqual, new OperatorInfo {Operator=" <> "}},
            { Comparison.GreaterThan, new OperatorInfo {Operator=" > "}},
            { Comparison.GreaterThenOrEqual, new OperatorInfo {Operator=" >= "}},
            { Comparison.LessThan, new OperatorInfo {Operator=" < "}},
            { Comparison.LessThanOrEqual, new OperatorInfo {Operator=" <= "}}
        };

        /// <summary>
        /// 
        /// </summary>
        public SqlDataReader()
        {
        }

        public override Response<T> Get<T>(IGetRequest request)
        {
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="request"></param>
        /// <returns></returns>
        public override Response<T> Get<T>(IGetRequestItem request)
        {
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        public string DataSourcePassword { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filters"></param>
        /// <returns></returns>
        protected new string ParseFilters(IList<IFilter> filters)
        {
            string s = "";
            string concat = "";
            Filter ff;
            FiltersGroup fg;

            foreach (IFilter f in filters)
            {
                if (f.GetType().Name == "FiltersGroup")
                {
                    fg = (FiltersGroup)f;
                    if (fg.Filters.Count > 0)
                        s += concat + "(" + ParseFilters(fg.Filters) + ")";
                    concat = " " + fg.Concatenator.ToString().ToUpper() + " ";
                }
                else if (f.GetType().Name == "Filter")
                {
                    ff = (Filter)f;
                    s += concat;
                    s += ff.Field + OpDict[ff.Comparator].Operator + (ff.Value == null ? "NULL" : FormatSQLValue(ff));
                    concat = " " + ff.Concatenator.ToString().ToUpper() + " ";
                }
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
                case "String": return "'" + OpDict[f.Comparator].Prefix + ((string)f.Value).Replace("'", "''") + OpDict[f.Comparator].Suffix + "'";
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
