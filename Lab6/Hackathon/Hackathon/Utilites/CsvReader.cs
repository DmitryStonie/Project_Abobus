namespace Hackathon
{
    public static class CsvReader
    {
        private const int NumberOfTokens = 2;

        public static List<T> ReadCsv<T>(string path) where T : new()
        {
            var employees = new List<T>();
            using (var reader = new StreamReader(path))
            {
                if (!reader.EndOfStream)
                {
                    reader.ReadLine();
                }

                while (!reader.EndOfStream)
                {
                    string? s = reader.ReadLine();
                    if (s is null)
                    {
                        continue;
                    }

                    string[] tokens = s.Split(';');
                    if (tokens.Length != NumberOfTokens || tokens[0] == "" || tokens[1] == "")
                        throw new FormatException("Id or name are empty.");
                    int id = int.Parse(tokens[0]);
                    if (id < 0) throw new IncorrectEmployeesDataException("Id less than zero.");
                    var employee = Activator.CreateInstance(typeof(T), id, tokens[1]);
                    if (employee != null) employees.Add((T)employee);
                }
            }

            if (employees.Count == 0) throw new EmptyFileException("File does not contain any employees data.");
            return employees;
        }
    }
}