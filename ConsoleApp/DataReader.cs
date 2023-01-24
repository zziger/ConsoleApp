namespace ConsoleApp
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    public class DataReader
    {
        IList<ImportedObject> ImportedObjects;

        public void ImportAndPrintData(string fileToImport)
        {
            ImportedObjects = new List<ImportedObject>();

            var importedLines = File.ReadLines(fileToImport);

            foreach (var importedLine in importedLines)
            {
                var values = importedLine
                    .Split(';');
                if (values.Length < 7) continue;
                
                // clear and correct imported data
                values = values
                    .Select(e => e.Trim().Replace(" ", "").Replace(Environment.NewLine, ""))
                    .ToArray();
                
                var importedObject = new ImportedObject();
                importedObject.Type = values[0].ToUpper();
                importedObject.Name = values[1];
                importedObject.Schema = values[2];
                importedObject.ParentName = values[3];
                importedObject.ParentType = values[4];
                importedObject.DataType = values[5];
                importedObject.IsNullable = values[6];
                ImportedObjects.Add(importedObject);
            }

            // assign number of children
            foreach (var importedObject in ImportedObjects)
            {
                foreach (var impObj in ImportedObjects)
                {
                    if (impObj.ParentType != importedObject.Type || impObj.ParentName != importedObject.Name) continue;
                    importedObject.NumberOfChildren++;
                }
            }

            foreach (var database in ImportedObjects)
            {
                if (database.Type != "DATABASE") continue;
                
                Console.WriteLine($"Database '{database.Name}' ({database.NumberOfChildren} tables)");

                // print all database's tables
                foreach (var table in ImportedObjects)
                {
                    if (table.ParentType?.ToUpper() != database.Type) continue;
                    if (table.ParentName != database.Name) continue;
                    
                    Console.WriteLine($"\tTable '{table.Schema}.{table.Name}' ({table.NumberOfChildren} columns)");

                    // print all table's columns
                    foreach (var column in ImportedObjects)
                    {
                        if (column.ParentType?.ToUpper() != table.Type) continue;
                        if (column.ParentName != table.Name) continue;
                        
                        Console.WriteLine($"\t\tColumn '{column.Name}' with {column.DataType} data type {(column.IsNullable == "1" ? "accepts nulls" : "with no nulls")}");
                    }
                }
            }
        }
    }

    class ImportedObject : ImportedObjectBaseClass
    {
        public string Schema { get; set; }

        public string ParentName { get; set; }
        public string ParentType { get; set; }

        public string DataType { get; set; }
        public string IsNullable { get; set; }

        public double NumberOfChildren { get; set; }
    }

    class ImportedObjectBaseClass
    {
        public string Name { get; set; }
        public string Type { get; set; }
    }
}
