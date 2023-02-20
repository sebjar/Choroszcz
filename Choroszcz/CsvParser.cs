using System.Text;

namespace Choroszcz
{
    public class CsvParser : IDisposable
    {
        private Dictionary<string, List<string>> dummyPrimitive;
        public List<string[]> RawFields;
        public int Pointer = -1;
        public int ItemsCount = 0;
        public CsvParser(string filePath, bool enclosedFields = true, string delimeter = ";")
        {
            Program.Print(PrintType.Debug, "Initializing CsvParser");

            dummyPrimitive = new();
            RawFields = new();
            ParseCsv(filePath, enclosedFields, delimeter);
        }
        private void ParseCsv(string filePath, bool enclosedFields = true, string delimeter = ";")
        {
            Program.Print(PrintType.Info, $"Starting csv parsing procces for \"{filePath}\"");
            Microsoft.VisualBasic.FileIO.TextFieldParser csvParser = new(filePath, Encoding.UTF8);
            csvParser.SetDelimiters(delimeter);
            csvParser.HasFieldsEnclosedInQuotes = enclosedFields;
            foreach (var header in csvParser.ReadFields())
                dummyPrimitive.Add(header, new List<string>());
                
            for (; !csvParser.EndOfData; ItemsCount++)
            {
                string[] fields = csvParser.ReadFields();
                for (int i = 0; i < dummyPrimitive.Keys.Count; i++)
                    dummyPrimitive.ElementAt(i).Value.Add(fields[i]);
                RawFields.Add(fields);
            }
            Program.Print(PrintType.Success, $"Finished parsing file \"{filePath}\"");
        }
        public bool EndOfData => ItemsCount <= ++Pointer;
        public string this[string key]
        {
            get => dummyPrimitive[key][Pointer];
        }
        public void Reset() => Pointer = -1;
        public void Dispose()
        {
            dummyPrimitive.Clear();
            dummyPrimitive = null;
            Pointer = default;
            ItemsCount = default;
            Program.Print(PrintType.Debug, "CsvParser was disposed");
        }
    }
}
