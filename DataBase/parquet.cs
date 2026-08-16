using Parquet;
using Parquet.Schema;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

class ParquetClass
{

    public async Task<List<string>> GetDataFromParquet(string ParquetFilePath , string NameOfTheField)
    {

        var RowsList = new List<string>();

        using Stream stream = File.OpenRead(ParquetFilePath);

        await using var parquetreader = await ParquetReader.CreateAsync(stream);

        DataField dataField = parquetreader.Schema.GetDataFields().FirstOrDefault(S => S.Name.Equals(NameOfTheField, StringComparison.OrdinalIgnoreCase));
        int TotalRows = parquetreader.RowGroupCount;

        for (int i =0; i< TotalRows; i++)
        {


            using ParquetRowGroupReader parquetReadGroups = parquetreader.OpenRowGroupReader(i);

            string[] buffer = new string[parquetReadGroups.RowCount];

            if (dataField != null)
                await parquetReadGroups.ReadAsync(dataField,buffer);
            if(buffer != null)
            {
                if (buffer.Length > 1)
                {
                    foreach (var phrase in buffer)
                        RowsList.Add(phrase);
                }
            }

        }
        return RowsList;
    }

    public string[] TextExtractor(string DirectoryPath)
    {
        //DirectoryPath = "C:\Users\FadiSK\source\repos\textdatabase\ptb.train"
        var Directories = Directory.GetFiles(DirectoryPath);
        return Directories;

    }

    public List<string> GetTextLines(string[] directories)
    {
        
        var list = new List<string>();
        foreach(var filename in directories)
        {
            long num = 0;
            
            Console.WriteLine($"\nyou are in file named : {filename} \n");
            var File = System.IO.File.ReadAllLines(filename);
            var x = File.Length;
            foreach (var line in File)
            {

                Console.Write($"\u001b[2K\ryou are in line number :{num++} out of {x}");
                list.Add(line);
            }

        }
        return list;
    }


}




