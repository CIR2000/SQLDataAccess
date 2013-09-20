using System;
using System.Collections.Generic;
using System.Text;
using Amica;
using Amica.Data;
using DataAccess;


namespace Amica.Data
{
    public class SqlDataReader : DataAccessBase
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

        /// <summary>
        /// Execute the specified request.
        /// </summary>
        /// <param name="request">Request.</param>
        /// <typeparam name="T">The expected return type .</typeparam>
        public override Response<T> Execute<T>(IRequest request)
        {
            return null;
        }

        /// <summary>
        /// Executes an async request.
        /// </summary>
        /// <param name="request">Request.</param>
        /// <param name="callback">Callback.</param>
        /// <typeparam name="T">The expected return type.</typeparam>
        public override void ExecuteAsync<T>(IRequest request, Action<Response<T>, IRequest> callback)
        {
            // TODO return response: not implemented
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
        protected override string ParseFilters(IList<IFilter> filters)
        {
			var s = new StringBuilder ();
            string concat = "";
            Filter ff;
            FiltersGroup fg;

            foreach (IFilter f in filters)
            {
                if (f is FiltersGroup)
                {
                    fg = (FiltersGroup)f;
					if (fg.Filters.Count > 0)
						s.Append (concat + "(" + ParseFilters (fg.Filters) + ")");
                    concat = " " + fg.Concatenator.ToString().ToUpper() + " ";
                }
                else if (f is Filter)
                {
                    ff = (Filter)f;
                    s.Append(concat);
                    s.Append(ff.Field + OpDict[ff.Comparator].Operator + (ff.Value == null ? "NULL" : FormatSQLValue(ff)));
                    concat = " " + ff.Concatenator.ToString().ToUpper() + " ";
                }
            }
			return s.ToString ();
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sorts"></param>
        /// <returns></returns>
        protected override string ParseSort(IList<Sort> sorts)
        {
			var s = new StringBuilder();

            foreach (Sort srt in sorts)
            {
                s.Append(srt.Field + " " + srt.Direction.ToString().ToUpper() + " ");
            }
			return s.ToString ();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        private string FormatSQLValue(Filter f)
        {
			if (f.Value is string)
                return "'" + OpDict[f.Comparator].Prefix + ((string)f.Value).Replace("'", "''") + OpDict[f.Comparator].Suffix + "'";
			else if (f.Value is DateTime)
                return "'" + ((DateTime)f.Value).ToString("yyyy/MM/dd hh:mm:ss") + "'";
			else if (f.Value is bool)
                return (bool)f.Value ? "TRUE" : "FALSE";
			else
                return f.Value.ToString();
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
