namespace FinalProject.Repository.Helpers
{
    public class QueryParameters
    {
        public List<string> SelectedColumns { get; } = new List<string>();
        public List<WhereClause> WhereClauses { get; } = new List<WhereClause>();
        public List<OrderByClause> OrderByClause { get; } = new List<OrderByClause>();
        public List<UpdateField> UpdateFields { get; } = new List<UpdateField>();
        public int? Skip { get; set; }
        public int? Take { get; set; }

        public QueryParameters AddColumns(params string[] columns)
        {
            if (columns != null && columns.Any())
            {
                SelectedColumns.AddRange(columns);
            }
            return this;
        }

        public QueryParameters AddWhere(string column, object value, string @operator = "=")
        {
            WhereClauses.Add(new WhereClause(column, value, @operator));
            return this;
        }

        public QueryParameters AddOrderBy(string column, bool ascending = true)
        {
            OrderByClause.Add(new OrderByClause(column, ascending));
            return this;
        }

        public QueryParameters AddUpdateField(string column, object value)
        {
            UpdateFields.Add(new UpdateField(column, value));
            return this;
        }

        public QueryParameters AddPagination(int skip, int take)
        {
            Skip = skip;
            Take = take;
            return this;
        }

        public Dictionary<string, object> GetParameters()
        {
            var parameters = new Dictionary<string, object>();

            // Add WHERE clause parameters with indices
            for (int i = 0; i < WhereClauses.Count; i++)
            {
                var clause = WhereClauses[i];
                parameters[$"{clause.Column}_{i}"] = clause.Value;
            }

            // Add UPDATE field parameters with indices
            for (int i = 0; i < UpdateFields.Count; i++)
            {
                var field = UpdateFields[i];
                parameters[$"{field.Column}_{i}"] = field.Value;
            }

            return parameters;
        }
    }

    public class WhereClause
    {
        public string Column { get; }
        public object Value { get; }
        public string Operator { get; }

        public WhereClause(string column, object value, string @operator = "=")
        {
            Column = column;
            Value = value;
            Operator = @operator;
        }
    }

    public class OrderByClause
    {
        public string Column { get; }
        public bool Ascending { get; }

        public OrderByClause(string column, bool ascending = true)
        {
            Column = column;
            Ascending = ascending;
        }
    }

    public class UpdateField
    {
        public string Column { get; }
        public object Value { get; }

        public UpdateField(string column, object value)
        {
            Column = column;
            Value = value;
        }
    }
}