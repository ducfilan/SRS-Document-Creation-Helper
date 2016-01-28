using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Excel = Microsoft.Office.Interop.Excel;
using System.Globalization;
using System.Windows.Forms;
using System.Diagnostics;

namespace SpecHelper
{
    public class ImportExportExcelHelper
    {
        private static ImportExportExcelHelper _instance;
        private const string _xlsExtension = ".xls";
        private const string _xlsxExtension = ".xlsx";
        private const string _nameClassHeaderColumn = "File Name";
        public static ImportExportExcelHelper Instance { get { return _instance ?? (_instance = new ImportExportExcelHelper()); } }

        public List<ClassItem> GetClassDataFromExcel(string filePath)
        {
            var result = new List<ClassItem>();
            ClassItem classItem = null;
            // var spCollection = new Dictionary<string, List<string>>();
            // Create localized tables and columns list
            if (File.Exists(filePath)
                && (filePath.Trim().EndsWith(_xlsExtension)
                    || filePath.Trim().EndsWith(_xlsxExtension)))
            {
                Excel.Workbook wkb = null;
                Excel.Application excel = null;
                try
                {
                    excel = new Excel.Application();
                    wkb = excel.Workbooks.Open(filePath);
                    var sheet = wkb.Sheets[2] as Excel.Worksheet;

                    //
                    // Take the used range of the sheet. Finally, get an object array of all
                    // of the cells in the sheet (their values). You can do things with those
                    // values. See notes about compatibility.
                    //
                    if (sheet != null)
                    {
                        Excel.Range excelRange = sheet.UsedRange;

                        for (var r = 1; r <= excelRange.Rows.Count; r++)
                        {
                            var className = ((Excel.Range)excelRange.Cells[r, 1]).Value2;
                            var spName = ((Excel.Range)excelRange.Cells[r, 5]).Value2 + string.Empty;

                            if (className != null || classItem == null)
                            {
                                if (_nameClassHeaderColumn.Equals(className + string.Empty)) { continue; }
                                classItem = new ClassItem { Name = className + string.Empty };
                                result.Add(classItem);
                            }

                            spName = spName.ToLowerInvariant().Trim().Trim('[', ']', '.');

                            // Remove 'dbo'
                            if (spName.StartsWith("dbo"))
                            {
                                spName = spName.Substring(3);
                                spName = spName.Trim('[', ']', '.');
                            }

                            if (!string.IsNullOrEmpty(spName.Trim()))
                            { classItem.AddProgramItem(spName); }
                        }
                    }
                }
                catch (Exception ex)
                {
                    //if you need to handle stuff
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                    //
                    // Clean up.
                    //
                    if (wkb != null)
                    {
                        wkb.Close();
                        Marshal.FinalReleaseComObject(wkb);
                        Marshal.FinalReleaseComObject(excel);
                    }
                }
            }

            return result;
        }

        public void ExportProgramUpdateResultToExcel(string path, ListBox.ObjectCollection successList, ListBox.ObjectCollection failList, ListBox.ObjectCollection noChangeList, ListBox.ObjectCollection middleList, ListBox.ObjectCollection localizedList)
        {
            Excel.Workbook wkb = null;
            Excel.Application excel = null;
            try
            {
                excel = new Excel.Application();
                wkb = excel.Workbooks.Add();
                var sheet = wkb.Sheets[1] as Excel.Worksheet;
                var successListRunable = true;
                var failListRunable = true;
                var noChangeListRunable = true;
                var middleListRunable = true;
                var localizedListRunable = true;
                sheet.Range[sheet.Cells[1, 1], sheet.Cells[1, 1]].Value = "Success";
                sheet.Range[sheet.Cells[1, 2], sheet.Cells[1, 2]].Value = "Fail";
                sheet.Range[sheet.Cells[1, 3], sheet.Cells[1, 3]].Value = "No Change";
                sheet.Range[sheet.Cells[1, 4], sheet.Cells[1, 4]].Value = "Middle SP";
                sheet.Range[sheet.Cells[1, 5], sheet.Cells[1, 5]].Value = "Localized";
                for (var cursor = 2; successListRunable || failListRunable || noChangeListRunable || middleListRunable || localizedListRunable; cursor++)
                {
                    try
                    {
                        if (cursor - 2 < successList.Count)
                        {
                            sheet.Range[sheet.Cells[cursor, 1], sheet.Cells[cursor, 1]].Value = successList[cursor - 2];
                        }
                        else
                        { successListRunable = false; }
                        if (cursor - 2 < failList.Count)
                        {
                            sheet.Range[sheet.Cells[cursor, 2], sheet.Cells[cursor, 2]].Value = failList[cursor - 2];
                        }
                        else
                        { failListRunable = false; }
                        if (cursor - 2 < noChangeList.Count)
                        {
                            sheet.Range[sheet.Cells[cursor, 3], sheet.Cells[cursor, 3]].Value = noChangeList[cursor - 2];
                        }
                        else
                        { noChangeListRunable = false; }
                        if (cursor - 2 < middleList.Count)
                        {
                            sheet.Range[sheet.Cells[cursor, 4], sheet.Cells[cursor, 4]].Value = middleList[cursor - 2];
                        }
                        else
                        { middleListRunable = false; }
                        if (cursor - 2 < localizedList.Count)
                        {
                            sheet.Range[sheet.Cells[cursor, 5], sheet.Cells[cursor, 5]].Value = localizedList[cursor - 2];
                        }
                        else
                        { localizedListRunable = false; }
                    }
                    catch (Exception ex) { }
                }

                wkb.SaveAs(Path.Combine(path, "ProgramItemsUpdateResult.xlsx"));
            }
            catch (Exception e) { }
            finally
            {
                //
                // Clean up.
                //
                if (wkb != null)
                {
                    wkb.Close();
                    Marshal.FinalReleaseComObject(wkb);
                    Marshal.FinalReleaseComObject(excel);
                }
            }
        }

        public void ExportDataToExcel(string path, List<ClassItem> classItems)
        {

            Excel.Workbook wkb = null;
            Excel.Application excel = null;
            try
            {
                excel = new Excel.Application();
                wkb = excel.Workbooks.Add();
                var sheet = wkb.Sheets[1] as Excel.Worksheet;

                foreach (var classItem in classItems)
                {
                    try
                    {
                        if (sheet == null) { continue; }
                        var name = classItem.Name;
                        // Check name of sheet before setting.
                        if (name.Length > 31)
                        {
                            name = name.Substring(0, 27) + "...";
                        }
                        sheet.Name = name;
                        ExportExcel(classItem, sheet);
                        sheet = (Excel.Worksheet)wkb.Sheets.Add(Type.Missing, sheet);
                    }
                    catch (Exception ex) { }
                }

                wkb.SaveAs(Path.Combine(path, "BackendResult.xlsx"));
            }
            catch (Exception e) { }
            finally
            {
                //
                // Clean up.
                //
                if (wkb != null)
                {
                    wkb.Close();
                    Marshal.FinalReleaseComObject(wkb);
                    Marshal.FinalReleaseComObject(excel);
                }
            }
        }

        private Excel.Worksheet ExportExcel(ClassItem classItem, Excel.Worksheet worksheet)
        {
            var cursor = 1;
            // Create BACKEND Header with 6 columns. 
            // Merger 6 columns.
            var header = (Excel.Range)worksheet.Range[worksheet.Cells[cursor, 1], worksheet.Cells[cursor, 6]];
            header.Merge();
            header.Value2 = "BackEnd";

            // Create header cloumns.
            cursor++;
            worksheet.Range[worksheet.Cells[cursor, 1], worksheet.Cells[cursor, 1]].Value = "No.";
            worksheet.Range[worksheet.Cells[cursor, 2], worksheet.Cells[cursor, 2]].Value = "Type";
            worksheet.Range[worksheet.Cells[cursor, 3], worksheet.Cells[cursor, 3]].Value = "Line";
            worksheet.Range[worksheet.Cells[cursor, 4], worksheet.Cells[cursor, 4]].Value = "Name";
            worksheet.Range[worksheet.Cells[cursor, 5], worksheet.Cells[cursor, 5]].Value = "Related Name";
            worksheet.Range[worksheet.Cells[cursor, 6], worksheet.Cells[cursor, 6]].Value = "Note";

            // Get SP and table list
            var refSPList = new List<string>();
            var refTableList = new List<string>();

            // Create data.
            foreach (var programItem in classItem.ProgramItems)
            {
                // Add the initial item
                if (!refSPList.Contains(programItem.Name, StringComparer.Create(CultureInfo.InvariantCulture, true)))
                {
                    refSPList.Add(programItem.Name);
                    GetReferenceData(programItem, ref refSPList, ref refTableList);
                }
                break;
            }

            foreach (var programName in refSPList)
            {
                var programItem = SqlItemManager.GetRegisteredItem(programName) as ProgramItem;
                if (programItem != null)
                {
                    cursor++;
                    worksheet.Range[worksheet.Cells[cursor, 4], worksheet.Cells[cursor, 4]].Value = programItem.Name;

                    string refString = string.Empty;

                    foreach (var sqlItem in programItem.SqlItems)
                    {
                        refString += sqlItem.Name + Environment.NewLine;
                    }
                    worksheet.Range[worksheet.Cells[cursor, 6], worksheet.Cells[cursor, 6]].Value = refString;
                }
            }

            // Create header cloumns.
            cursor = cursor + 2;
            worksheet.Range[worksheet.Cells[cursor, 1], worksheet.Cells[cursor, 1]].Value = "No.";
            worksheet.Range[worksheet.Cells[cursor, 2], worksheet.Cells[cursor, 2]].Value = "Type";
            worksheet.Range[worksheet.Cells[cursor, 3], worksheet.Cells[cursor, 3]].Value = "Line";
            worksheet.Range[worksheet.Cells[cursor, 4], worksheet.Cells[cursor, 4]].Value = "Name";
            worksheet.Range[worksheet.Cells[cursor, 5], worksheet.Cells[cursor, 5]].Value = "Related Name";
            worksheet.Range[worksheet.Cells[cursor, 6], worksheet.Cells[cursor, 6]].Value = "Note";

            foreach (var tableName in refTableList)
            {
                var tableItem = SqlItemManager.GetRegisteredItem(tableName) as TableItem;
                if (tableItem != null)
                {
                    cursor++;
                    worksheet.Range[worksheet.Cells[cursor, 4], worksheet.Cells[cursor, 4]].Value = tableItem.Name;

                    foreach (var columnItem in tableItem.Columns)
                    {
                        if (columnItem.DataType == DataTypes.NVarchar)
                        {
                            cursor++;
                            worksheet.Range[worksheet.Cells[cursor, 5], worksheet.Cells[cursor, 5]].Value = columnItem.Name;
                        }
                    }
                }
            }

            return worksheet;
        }

        private void GetReferenceData(ProgramItem programItem, ref List<string> refSPList, ref List<string> refTableList)
        {
            var stringComparer = StringComparer.Create(CultureInfo.InvariantCulture, true);

            foreach (var item in programItem.SqlItems)
            {
                if (item is TableItem)
                {
                    if (!refTableList.Contains(item.Name, stringComparer))
                    {
                        refTableList.Add(item.Name);
                    }
                }

                if (item is ProgramItem)
                {
                    if (!refSPList.Contains(item.Name, stringComparer))
                    {
                        refSPList.Add(item.Name);
                        GetReferenceData(item as ProgramItem, ref refSPList, ref refTableList);
                    }
                }
            }
        }

        public void ExportDataToExcel(string path, List<TableItem> tableItems)
        {
            Excel.Workbook wkb = null;
            Excel.Application excel = null;
            try
            {
                excel = new Excel.Application();
                wkb = excel.Workbooks.Add();
                var sheet = wkb.Sheets[1] as Excel.Worksheet;

                if (sheet != null)
                {
                    int row = 0;
                    foreach (var tableItem in tableItems)
                    {
                        row++;
                        int col = 1;
                        sheet.Cells[row, col] = tableItem.Name;

                        foreach (var columnItem in tableItem.Columns)
                        {
                            if (columnItem.Name.Equals("Created_By") || columnItem.Name.Equals("Updated_By"))
                                continue;
                            if (columnItem.DataType != DataTypes.NVarchar) continue;
                            col++;
                            sheet.Cells[row, col] = columnItem.Name;
                        }
                    }
                }

                wkb.SaveAs(Path.Combine(path, "ConfirmTableResult.xlsx"));
            }
            catch (Exception e) { }
            finally
            {
                //
                // Clean up.
                //
                if (wkb != null)
                {
                    wkb.Close();
                    Marshal.FinalReleaseComObject(wkb);
                    Marshal.FinalReleaseComObject(excel);
                }
            }
        }

        public List<TableItem> GetTableDataFromExcel(string filePath, int defaultSheet = 1)
        {
            var result = new List<TableItem>();
            TableItem classItem = null;

            // Create table list
            if (File.Exists(filePath)
                && (filePath.Trim().EndsWith(_xlsExtension)
                    || filePath.Trim().EndsWith(_xlsxExtension)))
            {
                Excel.Workbook wkb = null;
                Excel.Application excel = null;
                try
                {
                    excel = new Excel.Application();
                    wkb = excel.Workbooks.Open(filePath);
                    var sheet = wkb.Sheets[defaultSheet] as Excel.Worksheet;

                    //
                    // Take the used range of the sheet. Finally, get an object array of all
                    // of the cells in the sheet (their values). You can do things with those
                    // values. See notes about compatibility.
                    //
                    if (sheet != null)
                    {
                        Excel.Range excelRange = sheet.UsedRange;

                        for (var r = 1; r <= excelRange.Rows.Count; r++)
                        {
                            var tableName = ((Excel.Range)excelRange.Cells[r, 1]).Value2 + string.Empty;
                            tableName = tableName.Trim();
                            var tableItem = SqlItemManager.GetRegisteredItem(tableName) as TableItem;

                            if (tableItem == null)
                            {
                                tableItem = new TableItem(tableName);
                            }

                            tableItem.IsBeingLocalized = true;

                            for (var c = 2; c <= excelRange.Columns.Count; c++)
                            {
                                var columnName = ((Excel.Range)excelRange.Cells[r, c]).Value2 + string.Empty;
                                columnName = columnName.Trim();
                                if (!string.IsNullOrEmpty(columnName))
                                {
                                    if (tableItem.ContainColumn(columnName))
                                    {
                                        var columnItem = tableItem.GetColumn(columnName);
                                        columnItem.IsLocalized = true;
                                    }
                                    else
                                    {
                                        var columnItem = new ColumnItem(columnName);
                                        columnItem.IsLocalized = true;
                                        tableItem.Columns.Add(columnItem);
                                    }
                                }
                            }

                            if (tableItem.Columns.Count != 0)
                            {
                                result.Add(tableItem);
                            }
                            else
                            {
                                SqlItemManager.UnregisterItem(tableItem.Name);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    //if you need to handle stuff
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                    //
                    // Clean up.
                    //
                    if (wkb != null)
                    {
                        wkb.Close();
                        Marshal.FinalReleaseComObject(wkb);
                        Marshal.FinalReleaseComObject(excel);
                    }
                }
            }

            return result;
        }

        public List<string> GetIgnoredListFromExcel(string filePath, int defaultSheet = 3)
        {
            var result = new List<string>();

            // Create table list
            if (File.Exists(filePath)
                && (filePath.Trim().EndsWith(_xlsExtension)
                    || filePath.Trim().EndsWith(_xlsxExtension)))
            {
                Excel.Workbook wkb = null;
                Excel.Application excel = null;
                try
                {
                    excel = new Excel.Application();
                    wkb = excel.Workbooks.Open(filePath);
                    var sheet = wkb.Sheets[defaultSheet] as Excel.Worksheet;

                    //
                    // Take the used range of the sheet. Finally, get an object array of all
                    // of the cells in the sheet (their values). You can do things with those
                    // values. See notes about compatibility.
                    //
                    if (sheet != null)
                    {
                        Excel.Range excelRange = sheet.UsedRange;

                        for (var r = 1; r <= excelRange.Rows.Count; r++)
                        {
                            var tableName = ((Excel.Range)excelRange.Cells[r, 1]).Value2 + string.Empty;
                            tableName = tableName.Trim();

                            if (!tableName.Contains(" ") && !string.IsNullOrEmpty(tableName))
                            { result.Add(tableName); }
                        }
                    }
                }
                catch (Exception ex)
                {
                    //if you need to handle stuff
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                    //
                    // Clean up.
                    //
                    if (wkb != null)
                    {
                        wkb.Close();
                        Marshal.FinalReleaseComObject(wkb);
                        Marshal.FinalReleaseComObject(excel);
                    }
                }
            }

            return result;
        }
    }
}