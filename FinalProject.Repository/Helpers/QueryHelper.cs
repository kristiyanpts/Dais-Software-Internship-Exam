using System.Text;

namespace FinalProject.Repository.Helpers
{
    public class QueryHelper
    {
        public static string BuildSelectQuery(string tableName, IEnumerable<string> columns, QueryParameters parameters = null)
        {
            var query = new StringBuilder();
            query.Append("SELECT ");
            query.Append(columns.Any() ? string.Join(", ", columns) : "*");
            query.Append($" FROM {tableName}");

            if (parameters?.WhereClauses?.Any() == true)
            {
                query.Append(" WHERE ");

                // Group by both column and operator
                var groupedClauses = parameters.WhereClauses
                    .Select((clause, index) => new { Clause = clause, Index = index })
                    .GroupBy(x => new { x.Clause.Column, x.Clause.Operator });

                var whereConditions = new List<string>();

                foreach (var group in groupedClauses)
                {
                    if (group.Count() == 1)
                    {
                        // Single condition for this column and operator
                        var clause = group.First();
                        whereConditions.Add($"{clause.Clause.Column} {clause.Clause.Operator} @{clause.Clause.Column}_{clause.Index}");
                    }
                    else
                    {
                        // Multiple conditions for the same column and operator - use OR
                        var orConditions = group.Select(clause =>
                            $"{clause.Clause.Column} {clause.Clause.Operator} @{clause.Clause.Column}_{clause.Index}");
                        whereConditions.Add($"({string.Join(" OR ", orConditions)})");
                    }
                }

                query.Append(string.Join(" AND ", whereConditions));
            }

            if (parameters?.OrderByClause?.Any() == true)
            {
                query.Append(" ORDER BY ");
                query.Append(string.Join(", ", parameters.OrderByClause.Select(o => $"{o.Column} {(o.Ascending ? "ASC" : "DESC")}")));
            }

            if (parameters?.Take.HasValue == true)
            {
                query.Append($" OFFSET {parameters.Skip ?? 0} ROWS");
                query.Append($" FETCH NEXT {parameters.Take} ROWS ONLY");
            }

            return query.ToString();
        }

        public static string BuildUpdateQuery(string tableName, QueryParameters parameters)
        {
            if (parameters?.UpdateFields?.Any() != true)
            {
                throw new ArgumentException("Update query must have at least one field to update.");
            }

            if (parameters?.WhereClauses?.Any() != true)
            {
                throw new ArgumentException("Update query must have at least one where clause for safety.");
            }

            var query = new StringBuilder();
            query.Append($"UPDATE {tableName} SET ");
            query.Append(string.Join(", ", parameters.UpdateFields.Select((u, index) => $"{u.Column} = @{u.Column}_{index}")));

            query.Append(" WHERE ");

            // Group by both column and operator
            var groupedClauses = parameters.WhereClauses
                .Select((clause, index) => new { Clause = clause, Index = index })
                .GroupBy(x => new { x.Clause.Column, x.Clause.Operator });

            var whereConditions = new List<string>();

            foreach (var group in groupedClauses)
            {
                if (group.Count() == 1)
                {
                    // Single condition for this column and operator
                    var clause = group.First();
                    whereConditions.Add($"{clause.Clause.Column} {clause.Clause.Operator} @{clause.Clause.Column}_{clause.Index}");
                }
                else
                {
                    // Multiple conditions for the same column and operator - use OR
                    var orConditions = group.Select(clause =>
                        $"{clause.Clause.Column} {clause.Clause.Operator} @{clause.Clause.Column}_{clause.Index}");
                    whereConditions.Add($"({string.Join(" OR ", orConditions)})");
                }
            }

            query.Append(string.Join(" AND ", whereConditions));

            return query.ToString();
        }

        public static string BuildDeleteQuery(string tableName, QueryParameters parameters)
        {
            parameters ??= new QueryParameters();

            if (!parameters.WhereClauses.Any())
            {
                throw new ArgumentException("Delete query must have at least one where clause for safety.");
            }

            var query = new StringBuilder();
            query.Append($"DELETE FROM {tableName}");
            query.Append(" WHERE ");

            // Group by both column and operator
            var groupedClauses = parameters.WhereClauses
                .Select((clause, index) => new { Clause = clause, Index = index })
                .GroupBy(x => new { x.Clause.Column, x.Clause.Operator });

            var whereConditions = new List<string>();

            foreach (var group in groupedClauses)
            {
                if (group.Count() == 1)
                {
                    // Single condition for this column and operator
                    var clause = group.First();
                    whereConditions.Add($"{clause.Clause.Column} {clause.Clause.Operator} @{clause.Clause.Column}_{clause.Index}");
                }
                else
                {
                    // Multiple conditions for the same column and operator - use OR
                    var orConditions = group.Select(clause =>
                        $"{clause.Clause.Column} {clause.Clause.Operator} @{clause.Clause.Column}_{clause.Index}");
                    whereConditions.Add($"({string.Join(" OR ", orConditions)})");
                }
            }

            query.Append(string.Join(" AND ", whereConditions));

            return query.ToString();
        }

        public static string BuildInsertQuery(string tableName, IEnumerable<string> columns)
        {
            var query = new StringBuilder();
            query.Append($"INSERT INTO {tableName} (");
            query.Append(string.Join(", ", columns));
            query.Append(") VALUES (");
            query.Append(string.Join(", ", columns.Select(c => $"@{c}")));
            query.Append("); SELECT CAST(SCOPE_IDENTITY() AS INT);");

            return query.ToString();
        }
    }
}