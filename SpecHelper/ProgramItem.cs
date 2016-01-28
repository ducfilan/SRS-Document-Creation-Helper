// -----------------------------------------------------------------------
// <copyright file="ProgramItem.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;

namespace SpecHelper
{


    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class ProgramItem : SqlItem
    {
        private Regex _storedProcedureHeaderRegex = new Regex(@"^\s*create\s+(procedure|proc|function)\s*.*", RegexOptions.IgnoreCase | RegexOptions.Multiline);
        private Regex _storedProcedureBodyRegex = new Regex(@"^(\s*as\s*|\s*Return\s*.*)$", RegexOptions.IgnoreCase | RegexOptions.Multiline);
        private Regex _singleLineCommentRegex = new Regex(@"^\s*--.*$", RegexOptions.IgnoreCase | RegexOptions.Multiline);
        private Regex _blockCommentRegex = new Regex(@"/\*((?!\*/).)*\*/", RegexOptions.IgnoreCase | RegexOptions.Singleline);		// Assume no nested comment.
        private Regex _paramRegex = new Regex(@"@\w+\s+", RegexOptions.IgnoreCase);

        private const string _lockRegexString = @"{0}\]?\s*(\b\w+\b)?\s*(with)?\s*\(nolock\)";
        private const string _nonAliasRegexString = @"{0}\]?\s*(\,|where|join|go|\)|--|select|order|on|group|inner)";

        public bool IsLocalized { get; set; }
        public bool IsBeingLocalized { get; set; }
        public bool IsMiddleSP { get; set; }
        public bool HasParseError { get; set; }
        public string Content { get; private set; }

        private readonly List<SqlItem> _sqlItems = new List<SqlItem>();
        public IEnumerable<SqlItem> SqlItems
        {
            get
            {
                foreach (var item in _sqlItems)
                {
                    yield return item;
                }
            }
        }

        public ProgramItem(string itemName)
            : base(itemName)
        { }

        public override void UpdateReferences()
        {
            var connection = ConnectionManager.GetConnection();

            if (connection != null)
            {
                // Update SP
                try
                {
                    var command = new SqlCommand
                                        {
                                            Connection = connection,
                                            CommandText =
                                                string.Format(
                                                    @"SELECT DISTINCT REFERENCED_ENTITY_NAME FROM SYS.DM_SQL_REFERENCED_ENTITIES ('DBO.{0}', 'OBJECT') AS RE INNER JOIN 
																			                    (SELECT NAME FROM SYS.OBJECTS WHERE TYPE IN ('FN', 'IF', 'TF')
																			                    UNION
																			                    SELECT NAME FROM SYS.PROCEDURES
                                                                                                UNION
                                                                                                SELECT NAME FROM SYS.VIEWS) PR ON PR.NAME = RE.REFERENCED_ENTITY_NAME",
                                                    this.Name)
                                        };

                    var dataTable = new DataTable();
                    var dataAdapter = new SqlDataAdapter(command);

                    dataAdapter.Fill(dataTable);

                    foreach (DataRow row in dataTable.Rows)
                    {
                        var programName = row[0].ToString();

                        var proramItem = SqlItemManager.GetRegisteredItem(programName);
                        if (proramItem != null)
                        {
                            _sqlItems.Add(proramItem);
                        }
                        else
                        {
                            proramItem = new ProgramItem(programName);
                            _sqlItems.Add(proramItem);

                            proramItem.UpdateReferences();
                        }
                    }
                }
                catch { };

                // Update Tables
                try
                {
                    var command = new SqlCommand
                    {
                        Connection = connection,
                        CommandText =
                            string.Format(
                                @"  SELECT DISTINCT REFERENCED_ENTITY_NAME FROM SYS.DM_SQL_REFERENCED_ENTITIES ('DBO.{0}', 'OBJECT') AS RE 
                                    INNER JOIN SYS.TABLES AS TB ON TB.NAME = RE.REFERENCED_ENTITY_NAME
                                    WHERE (TB.NAME LIKE 'AH_MASTER%' OR TB.NAME = 'AH_MEMBER_WELLNESS_TIPS') AND TB.NAME NOT LIKE '%_LOCALIZED' ",
                                this.Name)
                    };
                    var dataTable = new DataTable();
                    var dataAdapter = new SqlDataAdapter(command);

                    dataAdapter.Fill(dataTable);

                    foreach (DataRow row in dataTable.Rows)
                    {
                        var tableName = row[0].ToString();
                        var tableItem = SqlItemManager.GetRegisteredItem(tableName) as TableItem;
                        if (tableItem != null)
                        {
                            if (!_sqlItems.Contains(tableItem))
                            {
                                tableItem.IsReferenced = true;
                                _sqlItems.Add(tableItem);
                            }
                        }
                        else
                        {
                            tableItem = new TableItem(tableName);
                            tableItem.IsReferenced = true;
                            _sqlItems.Add(tableItem);
                            tableItem.UpdateReferences();
                        }
                    }
                }
                catch { }
            }
        }

        public override Status UpdateSchema()
        {
            var connection = ConnectionManager.GetConnection();
            if (connection != null)
            {
                string updatedContent = string.Empty;
                try
                {
                    var server = new Server(new ServerConnection(connection));
                    GetSchemaContent();
                    updatedContent = UdateSchemaContent();
                    if (IsBeingLocalized)
                    {
                        server.ConnectionContext.ExecuteNonQuery(Content);
                        return Status.Success;
                    }
                    else if (IsMiddleSP)
                    {
                        server.ConnectionContext.ExecuteNonQuery(updatedContent);
                        return Status.MiddleSP;
                    }
                    else if (IsLocalized)
                    {
                        server.ConnectionContext.ExecuteNonQuery(updatedContent);
                        return Status.Localized;
                    }
                    else if (HasParseError)
                    {
                        return Status.Fail;
                    }
                    else
                    {
                        // No change code.
                        server.ConnectionContext.ExecuteNonQuery(updatedContent);
                        return Status.NoChange;
                    }
                }
                catch
                {
                    HasParseError = true;
                    this.Content = updatedContent;
                    return Status.Fail;
                }
            }

            return Status.NoChange;
        }

        private void GetSchemaContent()
        {
            IsLocalized = false;
            IsBeingLocalized = false;
            IsMiddleSP = false;
            HasParseError = false;

            Content = string.Empty;

            var connection = ConnectionManager.GetConnection();

            if (connection != null)
            {
                try
                {
                    var command = new SqlCommand
                    {
                        Connection = connection,
                        CommandText =
                            string.Format(@"SP_HELPTEXT N'{0}'", this.Name)
                    };
                    var dataTable = new DataTable();
                    var dataAdapter = new SqlDataAdapter(command);

                    dataAdapter.Fill(dataTable);

                    foreach (DataRow row in dataTable.Rows)
                    {
                        Content += row[0].ToString();
                    }
                    Console.WriteLine("Duc");
                }
                catch (Exception exception)
                {
                    // Todo: display message.
                }
            }
        }

        private string UdateSchemaContent()
        {
            Regex rg = new Regex(@"^\s*create\s+", RegexOptions.IgnoreCase | RegexOptions.Multiline);

            // Get stored procedure header, parameter body...
            string storedProcedureHead;		// Include the "create procedure..."
            string storedProcedureBody;		// from "as..."
            string storedProcedureParam;	// between head and body

            // Only localized once
            if (Content.IndexOf("OU_CULTURE_KEY", StringComparison.InvariantCultureIgnoreCase) != -1)
            {
                IsLocalized = true;
                return rg.Replace(Content, @"alter ", 1);
            }

            var matchHeader = _storedProcedureHeaderRegex.Match(Content);
            var matchBody = _storedProcedureBodyRegex.Match(Content);

            if (matchHeader.Success)
            {
                storedProcedureHead = Content.Substring(0, matchHeader.Index + matchHeader.Value.Length);
            }
            else
            {
                HasParseError = true;
                return string.Empty;
            }

            if (matchBody.Success)
            {
                storedProcedureBody = Content.Substring(matchBody.Index);
            }
            else
            {
                HasParseError = true;
                return string.Empty;
            }

            var fieldsToSelect = GetFieldsToSelect(storedProcedureBody);

            storedProcedureParam = Content.Substring(matchHeader.Index + matchHeader.Value.Length, matchBody.Index - (matchHeader.Index + matchHeader.Value.Length));

            // Add alter
            storedProcedureHead = storedProcedureHead.ToLowerInvariant().Replace("create", "alter");

            // Remove comment from stored params
            var lineCommentMatch = _singleLineCommentRegex.Match(storedProcedureParam);

            while (lineCommentMatch.Success)
            {
                storedProcedureParam = storedProcedureParam.Remove(lineCommentMatch.Index, lineCommentMatch.Length);
                lineCommentMatch = _singleLineCommentRegex.Match(storedProcedureParam);
            }

            var blockCommentMatch = _blockCommentRegex.Match(storedProcedureParam);
            while (blockCommentMatch.Success)
            {
                storedProcedureParam = storedProcedureParam.Remove(blockCommentMatch.Index, blockCommentMatch.Length);
                blockCommentMatch = _blockCommentRegex.Match(storedProcedureParam);
            }

            storedProcedureParam = storedProcedureParam.Trim();
            if (storedProcedureParam.Length > 2 && storedProcedureParam[0] == '(')
            {
                // Remove '(' and ')'
                storedProcedureParam = storedProcedureParam.Substring(1, storedProcedureParam.Length - 2);
            }

            // Modify param to add OU_CULTURE_KEY
            if (string.IsNullOrEmpty(storedProcedureParam)
                || string.IsNullOrEmpty(storedProcedureParam.Trim()))
            {
                storedProcedureParam = "@OU_CULTURE_KEY int" + storedProcedureParam;
            }
            else
            {
                storedProcedureParam = "@OU_CULTURE_KEY int, " + storedProcedureParam;
            }

            // Re-add '('
            storedProcedureParam = '(' + storedProcedureParam + ')';

            // Get all parameters
            var paramList = new List<string>();

            var paramMatch = _paramRegex.Match(storedProcedureParam);
            while (paramMatch.Success)
            {
                paramList.Add(paramMatch.Value.Trim().ToLowerInvariant());
                paramMatch = paramMatch.NextMatch();
            }

            if (!Name.StartsWith("fn_", StringComparison.CurrentCultureIgnoreCase))
            {
                // Log code is not correct, remove for now

                // Modify content of the body
                //            // Add log
                // var asText = Regex.Match(storedProcedureBody, "\\s*as(\\s|\\n)?");
                var tempIndex = storedProcedureBody.IndexOf("as", StringComparison.CurrentCultureIgnoreCase) + 2;
                storedProcedureBody = storedProcedureBody.Insert(tempIndex, Environment.NewLine + " BEGIN TRY" + Environment.NewLine);
                storedProcedureBody +=
                @"
            		END TRY
            		BEGIN CATCH
                        DECLARE ";
                if (storedProcedureBody.IndexOf("@ErrorCode ", StringComparison.CurrentCultureIgnoreCase) == -1)
                {
                    storedProcedureBody += "@ErrorCode int, \r\n";
                }
                if (storedProcedureBody.IndexOf("@ERRORSEVERITY ", StringComparison.CurrentCultureIgnoreCase) == -1)
                {
                    storedProcedureBody += "@ERRORSEVERITY int, \r\n";
                }
                if (storedProcedureBody.IndexOf("@ERRORPROCEDURE ", StringComparison.CurrentCultureIgnoreCase) == -1)
                {
                    storedProcedureBody += "@ERRORPROCEDURE nvarchar(500), \r\n";
                }
                if (storedProcedureBody.IndexOf("@ERRORMESSAGE ", StringComparison.CurrentCultureIgnoreCase) == -1)
                {
                    storedProcedureBody += "@ERRORMESSAGE nvarchar(500), \r\n";
                }
                if (storedProcedureBody.IndexOf("@SPID ", StringComparison.CurrentCultureIgnoreCase) == -1)
                {
                    storedProcedureBody += "@SPID int, \r\n";
                }
                if (storedProcedureBody.IndexOf("@ErrorDate ", StringComparison.CurrentCultureIgnoreCase) == -1)
                {
                    storedProcedureBody += "@ErrorDate datetime = getdate(),\r\n";
                }
                if (storedProcedureBody.IndexOf("@Parameters ", StringComparison.CurrentCultureIgnoreCase) == -1)
                {
                    storedProcedureBody += "@Parameters nvarchar(max),\r\n";
                }

                storedProcedureBody = storedProcedureBody.TrimEnd('\n').Trim('\r');
                storedProcedureBody = storedProcedureBody.TrimEnd(',') + ";";
                storedProcedureBody += @"SELECT @ErrorCode=ERROR_NUMBER(),
            					@ERRORSEVERITY=ERROR_SEVERITY(),
            					@ERRORPROCEDURE=ERROR_PROCEDURE(),
            					@ERRORMESSAGE=ERROR_MESSAGE(),
            					@SPID=@@SPID,
            					@Parameters=";

                for (int index = 0; index < paramList.Count; index++)
                {
                    if (index == 0)
                    {
                        //‘Parameter1=‘+cast(@Parameter1 as nvarchar())+‘, Parameter2=‘+Cast(Parameter2 as nvarchar()),etc…	
                        storedProcedureBody += string.Format("'Parameter{0}=' + cast({1} as nvarchar(max))", index, paramList[index]);
                    }
                    else
                    {
                        storedProcedureBody += string.Format(" + ', Parameter{0}=' + cast({1} as nvarchar(max))", index, paramList[index]);
                    }
                }


                storedProcedureBody +=
    @"			-- add all parameters of this procedure here.
            		
            		-- this procedure must be created beforehand. Please check if this SP exists.
            		-- It will coexists with the table that save the data called by this SP.
            		Exec spv_AH_LOG_DATABASE_ERRORS_Add																
            			@SPID=@SPID,
            			@ErrorDate=@ErrorDate,
            			@ErrorCode=@ErrorCode,
            			@ErrorSeverity=@ErrorSeverity,
            			@ErrorMessage=@ErrorMessage,
            			@StoredProcName=@ERRORPROCEDURE,
            			@StoredProcParameters=@Parameters
            END CATCH
        ";
            }

            // Localize 
            IsBeingLocalized = false;
            foreach (var sqlItem in SqlItems)
            {
                // Skip all items except tables.
                if (sqlItem is ProgramItem || !(((TableItem)sqlItem).IsLocalized || ((TableItem)sqlItem).IsBeingLocalized))
                {
                    continue;
                }

                var nonAliasRegex = new Regex(string.Format(_nonAliasRegexString, sqlItem.Name), RegexOptions.IgnoreCase);
                var lockRegex = new Regex(string.Format(_lockRegexString, sqlItem.Name), RegexOptions.IgnoreCase);

                var startIndex = -1;
                do
                {
                    startIndex++;
                    startIndex = storedProcedureBody.IndexOf(sqlItem.Name, startIndex, StringComparison.InvariantCultureIgnoreCase);

                    // Have table
                    if (startIndex != -1)
                    {
                        // Check if we actually have a table with the correct name.
                        // This can be done using Regex
                        if ((storedProcedureBody[startIndex - 1] == '['
                               || storedProcedureBody[startIndex - 1] == '.'
                               || storedProcedureBody[startIndex - 1] == ','
                               || storedProcedureBody[startIndex - 1] == ' '
                               || storedProcedureBody[startIndex - 1] == '\t')
                           && (storedProcedureBody[startIndex + sqlItem.Name.Length] == ']'
                               || storedProcedureBody[startIndex + sqlItem.Name.Length] == ','
                               || storedProcedureBody[startIndex + sqlItem.Name.Length] == ' '
                               || storedProcedureBody[startIndex + sqlItem.Name.Length] == ')'
                               || storedProcedureBody[startIndex + sqlItem.Name.Length] == '\t'
                               || storedProcedureBody[startIndex + sqlItem.Name.Length] == '\r'
                               || storedProcedureBody[startIndex + sqlItem.Name.Length] == '\n'))
                        {
                            // Check if have alias 
                            // Suppose we do not use alias
                            var useAlias = false;
                            var lockString = "";


                            // Find index of Select, Update, Insert, Delete
                            var subContent = storedProcedureBody.Substring(0, startIndex);
                            var selectIndex = subContent.LastIndexOf("SELECT", StringComparison.InvariantCultureIgnoreCase);
                            var updateIndex = subContent.LastIndexOf("UPDATE", StringComparison.InvariantCultureIgnoreCase);
                            var insertIndex = subContent.LastIndexOf("INSERT", StringComparison.InvariantCultureIgnoreCase);
                            var deleteIndex = subContent.LastIndexOf("DELETE", StringComparison.InvariantCultureIgnoreCase);
                            var joinIndex = subContent.LastIndexOf("JOIN", StringComparison.InvariantCultureIgnoreCase);

                            selectIndex = Math.Max(selectIndex, joinIndex);

                            // Find whether we are selecting, upating, inserting or deleting
                            if (selectIndex == Math.Max(deleteIndex, Math.Max(insertIndex, Math.Max(selectIndex, updateIndex)))
                              && selectIndex != -1)
                            //if (true)
                            {
                                IsBeingLocalized = true;

                                Match lockMatch = lockRegex.Match(storedProcedureBody, startIndex);
                                if (lockMatch.Success && lockMatch.Index == startIndex)
                                {
                                    lockString = "with (nolock)";

                                    // Remove with (lock)
                                    if (lockMatch.Value.ToLowerInvariant().Contains("with"))
                                    {
                                        storedProcedureBody = storedProcedureBody.Remove(startIndex + lockMatch.Value.LastIndexOf("with", StringComparison.InvariantCultureIgnoreCase),
                                                                                        lockMatch.Length - lockMatch.Value.LastIndexOf("with", StringComparison.InvariantCultureIgnoreCase));
                                    }
                                    else
                                    {
                                        storedProcedureBody = storedProcedureBody.Remove(startIndex + lockMatch.Value.LastIndexOf("(nolock)", StringComparison.InvariantCultureIgnoreCase),
                                                                                        lockMatch.Length - lockMatch.Value.LastIndexOf("(nolock)", StringComparison.InvariantCultureIgnoreCase));
                                    }
                                }

                                Match nonAliasMatch = nonAliasRegex.Match(storedProcedureBody, startIndex);
                                if (!nonAliasMatch.Success || nonAliasMatch.Index != startIndex)
                                {
                                    useAlias = true;
                                }

                                // create join query
                                var joinQuery = " (SELECT ";

                                foreach (var columnItem in ((TableItem)sqlItem).Columns)
                                {
                                    if (columnItem.IsLocalized)
                                    {
                                        joinQuery = joinQuery + string.Format(" LocalizedTB.[{0}],\r\n", columnItem.Name);
                                    }
                                    else
                                    {
                                        joinQuery = joinQuery + string.Format(" OriginalTB.[{0}],\r\n", columnItem.Name);
                                    }
                                }

                                joinQuery = joinQuery.Trim('\n').Trim('\r').TrimEnd(',');

                                var joinCondition = string.Empty;
                                foreach (var columnItem in ((TableItem)sqlItem).Columns)
                                {
                                    if (columnItem.IsPK)
                                    {
                                        joinCondition += string.Format(" OriginalTB.[{0}] = LocalizedTB.[{0}] AND", columnItem.Name);
                                    }
                                }

                                // Remove 'and'
                                joinCondition = joinCondition.Remove((joinCondition.Length - 1) - 3);

                                /*var originalTbPartJoinQuery = string.Format(@" FROM (SELECT ");
                                var localizedTbPartJoinQuery = string.Format(@"(SELECT ");
                                foreach (var columnItem in ((TableItem)sqlItem).Columns)
                                {
                                    if (columnItem.IsLocalized)
                                    {
                                        localizedTbPartJoinQuery += string.Format(@"[{0}], ", columnItem.Name);
                                    }
                                    else
                                    {
                                        originalTbPartJoinQuery += string.Format(@"[{0}], ", columnItem.Name);
                                        if (columnItem.IsPK)
                                        {
                                            localizedTbPartJoinQuery += string.Format(@"[{0}], ", columnItem.Name);
                                        }
                                    }
                                }


                                localizedTbPartJoinQuery = localizedTbPartJoinQuery + "OU_CULTURE_KEY" + string.Format(" FROM {0}_LOCALIZED) LocalizedTB", sqlItem.Name);
                                originalTbPartJoinQuery = originalTbPartJoinQuery.Substring(0,
                                    originalTbPartJoinQuery.Length - 2) + string.Format(" FROM {0}) OriginalTB", sqlItem.Name);

                                joinQuery += originalTbPartJoinQuery + string.Format("{0} INNER JOIN ", lockString) +
                                            localizedTbPartJoinQuery + string.Format(" {0} ON {1} WHERE LocalizedTB.OU_CULTURE_KEY  = @OU_CULTURE_KEY) ", lockString, joinCondition);
                                */

                                joinQuery = joinQuery + string.Format(@" FROM {0} OriginalTB {2} 
                                            INNER JOIN {0}_LOCALIZED LocalizedTB 
                                            {2} ON {1} WHERE LocalizedTB.OU_CULTURE_KEY  = @OU_CULTURE_KEY) ", sqlItem.Name, joinCondition, lockString);

                                // Remove the dbo.
                                if (storedProcedureBody[startIndex - 1] == '.'
                                    || (storedProcedureBody[startIndex - 1] == '[' && storedProcedureBody[startIndex - 2] == '.'))
                                {
                                    var dboIndex = storedProcedureBody.IndexOf("dbo", startIndex - 7, StringComparison.InvariantCultureIgnoreCase);

                                    if (dboIndex != -1 && dboIndex < startIndex)
                                    {
                                        storedProcedureBody = storedProcedureBody.Remove(dboIndex, startIndex - dboIndex);
                                        startIndex = dboIndex;
                                    }
                                }

                                // Remove '[' and ']'
                                if (storedProcedureBody[startIndex - 1] == '[')
                                {
                                    storedProcedureBody = storedProcedureBody.Remove(startIndex - 1, 1);
                                    startIndex--;

                                    storedProcedureBody = storedProcedureBody.Remove(startIndex + sqlItem.Name.Length, 1);
                                }

                                if (useAlias)
                                {
                                    // Remove table name and Insert to original data
                                    storedProcedureBody = storedProcedureBody.Remove(startIndex, sqlItem.Name.Length);
                                    storedProcedureBody = storedProcedureBody.Insert(startIndex, joinQuery);
                                    startIndex += joinQuery.Length;
                                }
                                else
                                {
                                    // Insert to original data.
                                    storedProcedureBody = storedProcedureBody.Insert(startIndex, joinQuery);
                                    startIndex += joinQuery.Length;
                                }
                            }
                            else
                            {
                                //throw new NotSupportedException();
                                // Delete, Insert, Update
                            }
                        }
                    }
                }
                while (startIndex != -1);
            }

            if (IsBeingLocalized)
            {
                Content = storedProcedureHead + Environment.NewLine + storedProcedureParam + Environment.NewLine + storedProcedureBody;
            }
            else
            {
                foreach (var sqlItem in this.SqlItems)
                {
                    if (sqlItem is ProgramItem)
                    {
                        IsMiddleSP = true;
                        break;
                    }
                }
            }

            return rg.Replace(Content, "alter ", 1);
        }

        public string GetFieldsToSelect(string storedProcedureBody)
        {
            //select to from
            var selectStartPosition = 0;
            int indexOfSelectInComment = -1;
            do
            {
                selectStartPosition = storedProcedureBody.IndexOf("SELECT ", selectStartPosition, StringComparison.OrdinalIgnoreCase) + "SELECT ".Length;

                if (selectStartPosition - "SELECT ".Length == -1) break;

                var lastIndexOfDoubleDash = 0;
                while (storedProcedureBody.IndexOf("--", lastIndexOfDoubleDash + 2, selectStartPosition - lastIndexOfDoubleDash, StringComparison.Ordinal) != -1)
                {
                    lastIndexOfDoubleDash = storedProcedureBody.IndexOf("--", lastIndexOfDoubleDash, selectStartPosition - lastIndexOfDoubleDash, StringComparison.Ordinal);
                }
                if (lastIndexOfDoubleDash > 0)
                    indexOfSelectInComment = storedProcedureBody.IndexOf('\n', lastIndexOfDoubleDash, selectStartPosition);
            } while ("-(".IndexOf(storedProcedureBody[selectStartPosition - 1 - "SELECT ".Length]) > -1
                 || indexOfSelectInComment > -1);

            var fromStartPosition = storedProcedureBody.IndexOf("FROM", selectStartPosition, StringComparison.OrdinalIgnoreCase);
            var fieldsToSelectPart = "-1";

            if (selectStartPosition - "SELECT ".Length == -1 || fromStartPosition == -1) return fieldsToSelectPart;
            try
            {
                fieldsToSelectPart = storedProcedureBody.Substring(selectStartPosition, fromStartPosition - selectStartPosition);
            }
            catch (Exception)
            { }

            return fieldsToSelectPart;
        }
    }

    public enum Status
    {
        NoChange = 0,
        Localized = 1,
        Success = 2,
        Fail = 3,
        MiddleSP = 4
    }
}
