using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using REORGCHART.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;

namespace REORGCHART.Helper
{
    public class ExcelAPI
    {
        private Cell InsertCellInWorksheet(string columnName, uint rowIndex, WorksheetPart worksheetPart)
        {
            Worksheet worksheet = worksheetPart.Worksheet;
            SheetData sheetData = worksheet.GetFirstChild<SheetData>();
            string cellReference = columnName + rowIndex;

            // If the worksheet does not contain a row with the specified row index, insert one.
            Row row;
            if (sheetData.Elements<Row>().Where(r => r.RowIndex == rowIndex).Count() != 0)
            {
                row = sheetData.Elements<Row>().Where(r => r.RowIndex == rowIndex).First();
            }
            else
            {
                row = new Row() { RowIndex = rowIndex };
                sheetData.Append(row);
            }

            // If there is not a cell with the specified column name, insert one. 
            if (row.Elements<Cell>().Where(c => c.CellReference.Value == columnName + rowIndex).Count() > 0)
            {
                return row.Elements<Cell>().Where(c => c.CellReference.Value == cellReference).First();
            }
            else
            {

                // Cells must be in sequential order according to CellReference. Determine where to insert the new cell.
                Cell refCell = null;
                Cell newCell = new Cell() { CellReference = cellReference };
                row.InsertBefore(newCell, refCell);
                worksheet.Save();

                return newCell;
            }
        }

        private int InsertSharedStringItem(string text, SharedStringTablePart shareStringPart)
        {

            // If the part does not contain a SharedStringTable, create one.
            if (shareStringPart.SharedStringTable == null)
            {
                shareStringPart.SharedStringTable = new SharedStringTable();
            }
            int idy = 0;

            // Iterate through all the items in the SharedStringTable. If the text already exists, return its index.
            foreach (SharedStringItem item in shareStringPart.SharedStringTable.Elements<SharedStringItem>())
            {
                if (item.InnerText == text)
                {
                    return idy;
                }
                idy++;
            }

            // The text does not exist in the part. Create the SharedStringItem and return its index.
            shareStringPart.SharedStringTable.AppendChild(new SharedStringItem(new DocumentFormat.OpenXml.Spreadsheet.Text(text)));
            shareStringPart.SharedStringTable.Save();
            return idy;
        }

        // Creates new Excel sheet.
        public void CreateSpreadsheetWorkbook(string filepath, DataTable dt)
        {
            // Create a spreadsheet document by supplying the filepath.
            // By default, AutoSave = true, Editable = true, and Type = xlsx.
            FileInfo fileExists = new FileInfo(filepath);
            if (fileExists.Exists) fileExists.Delete();

            using (SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Create(filepath, SpreadsheetDocumentType.Workbook))
            {

                // Add a WorkbookPart to the document.
                WorkbookPart workbookpart = spreadsheetDocument.AddWorkbookPart();
                workbookpart.Workbook = new Workbook();

                // Add a WorksheetPart to the WorkbookPart.
                WorksheetPart worksheetPart = workbookpart.AddNewPart<WorksheetPart>();
                worksheetPart.Worksheet = new Worksheet(new SheetData());

                // Add Sheets to the Workbook.
                Sheets sheets = spreadsheetDocument.WorkbookPart.Workbook.AppendChild<Sheets>(new Sheets());

                // Append a new worksheet and associate it with the workbook.
                Sheet sheet = new Sheet() { Id = spreadsheetDocument.WorkbookPart.GetIdOfPart(worksheetPart), SheetId = 1, Name = "Sheet1" };
                sheets.Append(sheet);

                // save worksheet
                spreadsheetDocument.WorkbookPart.WorksheetParts.First().Worksheet.Save();

                string cl = "";
                uint row = 2;
                int index;
                Cell cell;
                foreach (DataRow dr in dt.Rows)
                {
                    for (int idx = 0; idx < dt.Columns.Count; idx++)
                    {
                        if (idx >= 26)
                            cl = "A" + Convert.ToString(Convert.ToChar(65 + idx - 26));
                        else
                            cl = Convert.ToString(Convert.ToChar(65 + idx));
                        SharedStringTablePart shareStringPart;
                        if (spreadsheetDocument.WorkbookPart.GetPartsOfType<SharedStringTablePart>().Count() > 0)
                        {
                            shareStringPart = spreadsheetDocument.WorkbookPart.GetPartsOfType<SharedStringTablePart>().First();
                        }
                        else
                        {
                            shareStringPart = spreadsheetDocument.WorkbookPart.AddNewPart<SharedStringTablePart>();
                        }
                        if (row == 2)
                        {
                            index = InsertSharedStringItem(dt.Columns[idx].ColumnName, shareStringPart);
                            cell = InsertCellInWorksheet(cl, row - 1, worksheetPart);
                            cell.CellValue = new CellValue(index.ToString());
                            cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
                        }

                        // Insert the text into the SharedStringTablePart.
                        index = InsertSharedStringItem(Convert.ToString(dr[idx]), shareStringPart);
                        cell = InsertCellInWorksheet(cl, row, worksheetPart);
                        cell.CellValue = new CellValue(index.ToString());
                        cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
                    }
                    row++;
                }

                spreadsheetDocument.WorkbookPart.Workbook.Save();
            }
        }

        private string ColumnName(int ColumnIndexer)
        {
            string cl = "";

            if ((ColumnIndexer - 1) >= 26)
                cl = "A" + Convert.ToString(Convert.ToChar(65 + (ColumnIndexer - 1) - 26));
            else
                cl = Convert.ToString(Convert.ToChar(65 + (ColumnIndexer - 1)));

            return cl;
        }

        // Creates Excel sheet with exisiting Template
        public void PopulateDataTableToExcelTemplate(string FilePath, DataTable dt)
        {
            try
            {
                // Will not overwrite if the destination file already exists.
                string SourcePath = System.Web.HttpContext.Current.Server.MapPath("~/Content/Dashboards/ExcelPackageTemplate.xlsx");
                File.Copy(SourcePath, FilePath);
                Thread.Sleep(5000);
            }
            // Catch exception if the file was already copied.
            catch (IOException copyError)
            {
                Console.WriteLine(copyError.Message);
            }
            using (SpreadsheetDocument ssd = SpreadsheetDocument.Open(FilePath, true))
            {
                WorkbookPart wbPart = ssd.WorkbookPart;
                WorksheetPart worksheetPart = wbPart.WorksheetParts.First();

                SheetData sheetdata = worksheetPart.Worksheet.GetFirstChild<SheetData>();
                string[] headerColumns = new string[] { dt.Columns[0].ColumnName, dt.Columns[1].ColumnName, dt.Columns[2].ColumnName };
                DocumentFormat.OpenXml.Spreadsheet.Row r = new DocumentFormat.OpenXml.Spreadsheet.Row();
                DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                int RowIndexer = 1;
                int ColumnIndexer = 1;

                r.RowIndex = (UInt32)RowIndexer;
                foreach (DataColumn dc in dt.Columns)
                {
                    cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cell.CellReference = ColumnName(ColumnIndexer) + RowIndexer;
                    cell.DataType = CellValues.InlineString;
                    cell.InlineString = new InlineString(new Text(dc.ColumnName.ToString()));
                    // consider using cell.CellValue. Then you don't need to use InlineString.
                    // Because it seems you're not using any rich text so you're just bloating up
                    // the XML.

                    r.AppendChild(cell);

                    ColumnIndexer++;
                }
                // here's the missing part you needed
                sheetdata.Append(r);

                RowIndexer = 2;
                foreach (DataRow dr in dt.Rows)
                {
                    r = new DocumentFormat.OpenXml.Spreadsheet.Row();
                    r.RowIndex = (UInt32)RowIndexer;
                    // this follows the same starting column index as your column header.
                    // I'm assuming you start with column 1. Change as you see fit.
                    ColumnIndexer = 1;
                    foreach (object value in dr.ItemArray)
                    {
                        cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        // I moved it here so it's consistent with the above part
                        // Also, the original code was using the row index to calculate
                        // the column name, which is weird.
                        cell.CellReference = ColumnName(ColumnIndexer) + RowIndexer;
                        cell.DataType = CellValues.InlineString;
                        cell.InlineString = new InlineString(new Text(value.ToString()));

                        r.AppendChild(cell);
                        ColumnIndexer++;
                    }
                    RowIndexer++;

                    // missing part
                    sheetdata.Append(r);
                }

                worksheetPart.Worksheet.Save();
                wbPart.Workbook.Save();
            }
        }

        // Create multiple sheet from Dataset
        public void ExportDSToExcel(string filepath, DataSet ds)
        {
            using (var workbook = SpreadsheetDocument.Create(filepath, DocumentFormat.OpenXml.SpreadsheetDocumentType.Workbook))
            {
                var workbookPart = workbook.AddWorkbookPart();
                workbook.WorkbookPart.Workbook = new DocumentFormat.OpenXml.Spreadsheet.Workbook();
                workbook.WorkbookPart.Workbook.Sheets = new DocumentFormat.OpenXml.Spreadsheet.Sheets();

                uint sheetId = 1;

                foreach (DataTable table in ds.Tables)
                {
                    var sheetPart = workbook.WorkbookPart.AddNewPart<WorksheetPart>();
                    var sheetData = new DocumentFormat.OpenXml.Spreadsheet.SheetData();
                    sheetPart.Worksheet = new DocumentFormat.OpenXml.Spreadsheet.Worksheet(sheetData);

                    DocumentFormat.OpenXml.Spreadsheet.Sheets sheets = workbook.WorkbookPart.Workbook.GetFirstChild<DocumentFormat.OpenXml.Spreadsheet.Sheets>();
                    string relationshipId = workbook.WorkbookPart.GetIdOfPart(sheetPart);

                    if (sheets.Elements<DocumentFormat.OpenXml.Spreadsheet.Sheet>().Count() > 0)
                    {
                        sheetId =
                            sheets.Elements<DocumentFormat.OpenXml.Spreadsheet.Sheet>().Select(s => s.SheetId.Value).Max() + 1;
                    }

                    DocumentFormat.OpenXml.Spreadsheet.Sheet sheet = new DocumentFormat.OpenXml.Spreadsheet.Sheet() { Id = relationshipId, SheetId = sheetId, Name = table.TableName };
                    sheets.Append(sheet);

                    DocumentFormat.OpenXml.Spreadsheet.Row headerRow = new DocumentFormat.OpenXml.Spreadsheet.Row();

                    List<String> columns = new List<string>();
                    foreach (DataColumn column in table.Columns)
                    {
                        columns.Add(column.ColumnName);

                        DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(column.ColumnName);
                        headerRow.AppendChild(cell);
                    }

                    sheetData.AppendChild(headerRow);

                    foreach (DataRow dsrow in table.Rows)
                    {
                        DocumentFormat.OpenXml.Spreadsheet.Row newRow = new DocumentFormat.OpenXml.Spreadsheet.Row();
                        foreach (String col in columns)
                        {
                            DocumentFormat.OpenXml.Spreadsheet.Cell cell = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                            cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                            cell.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(dsrow[col].ToString()); //
                            newRow.AppendChild(cell);
                        }

                        sheetData.AppendChild(newRow);
                    }
                }

                workbook.WorkbookPart.Workbook.Save();
            }
        }


        // Create DataTable from excel sheet
        public void CreateDataTableFromExcel(string FilePath, DataTable dt)
        {
            using (SpreadsheetDocument spreadSheetDocument = SpreadsheetDocument.Open(FilePath, false))
            {

                WorkbookPart workbookPart = spreadSheetDocument.WorkbookPart;
                IEnumerable<Sheet> sheets = spreadSheetDocument.WorkbookPart.Workbook.GetFirstChild<Sheets>().Elements<Sheet>();
                string relationshipId = sheets.First().Id.Value;
                WorksheetPart worksheetPart = (WorksheetPart)spreadSheetDocument.WorkbookPart.GetPartById(relationshipId);
                Worksheet workSheet = worksheetPart.Worksheet;
                SheetData sheetData = workSheet.GetFirstChild<SheetData>();
                IEnumerable<Row> rows = sheetData.Descendants<Row>();

                string ColumnName = "";
                foreach (Cell cell in rows.ElementAt(0))
                {
                    ColumnName = GetCellValue(spreadSheetDocument, cell);
                    dt.Columns.Add(ColumnName);
                }

                foreach (Row row in rows) //this will also include your header row...
                {
                    DataRow tempRow = dt.NewRow();

                    for (int Idx = 0; Idx < row.Descendants<Cell>().Count(); Idx++)
                    {
                        tempRow[Idx] = GetCellValue(spreadSheetDocument, row.Descendants<Cell>().ElementAt(Idx));
                    }

                    dt.Rows.Add(tempRow);
                }

            }
            dt.Rows.RemoveAt(0); //...so i'm taking it out here.

        }

        private string GetColumnDataType(string DataField, string DataType)
        {
            if (DataType != "NONE")
            {
                List<SearchField> IVs = new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<List<SearchField>>(DataType);
                foreach (SearchField sf in IVs)
                {
                    if (sf.FieldValue != "")
                    {
                        if (sf.FieldName == DataField)
                        {
                            switch (sf.FieldValue)
                            {
                                case "VARCHAR":
                                    return "";
                                    break;
                                case "INT":
                                    return "0";
                                    break;
                                case "FLOAT":
                                    return "0.0";
                                    break;
                                case "DateTime":
                                    return DateTime.Now.ToString("yyyy/MM/dd");
                                    break;
                            }
                        }
                    }
                }
            }

            return "";
        }

        // Create DataTable from excel sheet
        public string[] CreateJSONFromExcel(string FilePath, string DataType)
        {
            string[] RetValue = { "", "" };
            string RetJSON = "", RetTotalJSON = "", ValJSON = "", CurrentRow="", CurrentCol="", Warning="";
            try
            {
                using (SpreadsheetDocument spreadSheetDocument = SpreadsheetDocument.Open(FilePath, false))
                {

                    WorkbookPart workbookPart = spreadSheetDocument.WorkbookPart;
                    IEnumerable<Sheet> sheets = spreadSheetDocument.WorkbookPart.Workbook.GetFirstChild<Sheets>().Elements<Sheet>();
                    string relationshipId = sheets.First().Id.Value;
                    WorksheetPart worksheetPart = (WorksheetPart)spreadSheetDocument.WorkbookPart.GetPartById(relationshipId);
                    Worksheet workSheet = worksheetPart.Worksheet;
                    SheetData sheetData = workSheet.GetFirstChild<SheetData>();
                    IEnumerable<Row> rows = sheetData.Descendants<Row>();

                    string ColumnName = "", ColumnValue = "";
                    List<String> Columns = new List<string>();
                    List<String> DataColumns = new List<string>();
                    foreach (Cell cell in rows.ElementAt(0))
                    {
                        ColumnName = GetCellValue(spreadSheetDocument, cell);
                        ColumnName = ColumnName.Trim().ToUpper().Replace(" ", "_");
                        Columns.Add(ColumnName);
                        DataColumns.Add(ColumnName);
                    }

                    int Index = 0;
                    foreach (Row row in rows) //this will also include your header row...
                    {
                        CurrentRow = row.RowIndex.ToString();
                        if (CurrentRow == "199")
                            CurrentRow = "199";
                        if (Index > 0)
                        {
                            for (int Idx = 0; Idx <= DataColumns.Count - 1; Idx++)
                            {
                                DataColumns[Idx] = "";
                            }
                            RetJSON += ""; ValJSON = "";
                            for (int Idx = 0; Idx < row.Descendants<Cell>().Count(); Idx++)
                            {
                                Cell cell = row.Descendants<Cell>().ElementAt(Idx);
                                int actualCellIndex = CellReferenceToIndex(cell, row.RowIndex.ToString());
                                CurrentCol = actualCellIndex.ToString();
                                try
                                {
                                    ColumnValue = GetCellValue(spreadSheetDocument, row.Descendants<Cell>().ElementAt(Idx));
                                    DataColumns[actualCellIndex] = ColumnValue;
                                }
                                catch(Exception ex)
                                {
                                    DataColumns[actualCellIndex] = "";
                                    Warning += "Issue with data(null or some junk) in Row " + CurrentRow + " Column " + CurrentCol+"\n";
                                }
                            }
                            var ShowRowFlag = "N";
                            for (int Idx = 0; Idx <= DataColumns.Count - 1; Idx++)
                            {
                                if (DataColumns[Idx].Trim()!="" && DataColumns[Idx].Trim() != "0") ShowRowFlag = "Y";
                            }

                            if (ShowRowFlag == "Y")
                            {
                                for (int Idx = 0; Idx <= DataColumns.Count - 1; Idx++)
                                {
                                    ValJSON += ",\"" + Columns[Idx] + "\":\"" + (string.IsNullOrEmpty(DataColumns[Idx].Trim()) ? GetColumnDataType(Columns[Idx], DataType) : DataColumns[Idx]) + "\"";
                                }
                                RetJSON += ",{" + ValJSON.Substring(1) + "}";
                            }
                            else break;
                        }
                        Index++;
                        if (Index % 100 == 0)
                        {
                            RetTotalJSON += RetJSON;
                            RetJSON = "";
                        }
                    }

                    if (RetJSON.Length >= 1)
                    {
                        RetTotalJSON += RetJSON;
                        RetJSON = "";
                    }
                }
            }
            catch(Exception ex)
            {
                Warning += "Issue with data in Row " + CurrentRow + " Column " + CurrentCol+" ( "+ex.Message+" )";
            }

            RetValue[0] = "{ \"data\":[" + RetTotalJSON.Substring(1) + "]}";
            RetValue[1] = Warning;

            return RetValue;
        }

        public string GetCellValue(SpreadsheetDocument document, Cell cell)
        {
            SharedStringTablePart stringTablePart = document.WorkbookPart.SharedStringTablePart;
            string value = cell.CellValue.InnerXml;

            if (cell.DataType != null && cell.DataType.Value == CellValues.SharedString)
            {
                return stringTablePart.SharedStringTable.ChildElements[Int32.Parse(value)].InnerText;
            }
            else
            {
                return value;
            }
        }

        private int GetCharterIndex(char ch)
        {
            int CharIndex = 0;
            switch (ch)
            {
                case 'A':
                    CharIndex = 0;
                    break;
                case 'B':
                    CharIndex = 1;
                    break;
                case 'C':
                    CharIndex = 2;
                    break;
                case 'D':
                    CharIndex = 3;
                    break;
                case 'E':
                    CharIndex = 4;
                    break;
                case 'F':
                    CharIndex = 5;
                    break;
                case 'G':
                    CharIndex = 6;
                    break;
                case 'H':
                    CharIndex = 7;
                    break;
                case 'I':
                    CharIndex = 8;
                    break;
                case 'J':
                    CharIndex = 9;
                    break;
                case 'K':
                    CharIndex = 10;
                    break;
                case 'L':
                    CharIndex = 11;
                    break;
                case 'M':
                    CharIndex = 12;
                    break;
                case 'N':
                    CharIndex = 13;
                    break;
                case 'O':
                    CharIndex = 14;
                    break;
                case 'P':
                    CharIndex = 15;
                    break;
                case 'Q':
                    CharIndex = 16;
                    break;
                case 'R':
                    CharIndex = 17;
                    break;
                case 'S':
                    CharIndex = 18;
                    break;
                case 'T':
                    CharIndex = 19;
                    break;
                case 'U':
                    CharIndex = 20;
                    break;
                case 'V':
                    CharIndex = 21;
                    break;
                case 'W':
                    CharIndex = 22;
                    break;
                case 'X':
                    CharIndex = 23;
                    break;
                case 'Y':
                    CharIndex = 24;
                    break;
                case 'Z':
                    CharIndex = 25;
                    break;
            }

            return CharIndex;
        }

        private int CellReferenceToIndex(Cell cell, string RowIndex)
        {
            int Index = 0, Idx = 0; ;
            string reference = cell.CellReference.ToString().ToUpper();
            reference = reference.Substring(0, reference.IndexOf(RowIndex));
            foreach (char ch in reference)
            {
                if (Char.IsLetter(ch) && (reference.Length - 1 == Idx))
                {
                    Index += GetCharterIndex(ch);
                }
                else if (reference.Length - 1 > Idx)
                {
                    Index += (GetCharterIndex(ch)+1)*26;
                }
                Idx++;
            }

            return Index;
        }

        // Create DataTable from excel sheet
        public DataTable ImportExcel(string FilePath)
        {
            string sheetName = "";

            //Create a new DataTable.
            DataTable dtExcel = new DataTable();

            //Open the Excel file in Read Mode using OpenXml.
            using (SpreadsheetDocument doc = SpreadsheetDocument.Open(FilePath, true))
            {
                //Read the first Sheet from Excel file.
                Sheet sheet = doc.WorkbookPart.Workbook.Sheets.GetFirstChild<Sheet>();

                // Grab the sheet name each time through your loop
                sheetName = doc.WorkbookPart.Workbook.Descendants<Sheet>().ElementAt(0).Name;

                //Get the Worksheet instance.
                Worksheet worksheet = (doc.WorkbookPart.GetPartById(sheet.Id.Value) as WorksheetPart).Worksheet;

                //Fetch all the rows present in the Worksheet.
                IEnumerable<Row> rows = worksheet.GetFirstChild<SheetData>().Descendants<Row>();

                //Loop through the Worksheet rows.
                foreach (Row row in rows)
                {
                    //Use the first row to add columns to DataTable.
                    if (row.RowIndex.Value == 1)
                    {
                        foreach (Cell cell in row.Descendants<Cell>())
                        {
                            dtExcel.Columns.Add(GetCellValue(doc, cell));
                        }
                    }
                    else
                    {
                        //Add rows to DataTable.
                        dtExcel.Rows.Add();
                        int idx = 0;
                        foreach (Cell cell in row.Descendants<Cell>())
                        {
                            dtExcel.Rows[dtExcel.Rows.Count - 1][idx] = GetCellValue(doc, cell);
                            idx++;
                        }
                    }
                }
            }
            dtExcel.TableName = sheetName;

            return dtExcel;
        }

        // Get Excel sheet name
        public string GetExcelSheetName(string FilePath)
        {
            string sheetName = "";

            //Open the Excel file in Read Mode using OpenXml.
            using (SpreadsheetDocument doc = SpreadsheetDocument.Open(FilePath, true))
            {
                //Read the first Sheet from Excel file.
                Sheet sheet = doc.WorkbookPart.Workbook.Sheets.GetFirstChild<Sheet>();

                // Grab the sheet name each time through your loop
                sheetName = doc.WorkbookPart.Workbook.Descendants<Sheet>().ElementAt(0).Name;
            }
            return sheetName;
        }

    }
}
