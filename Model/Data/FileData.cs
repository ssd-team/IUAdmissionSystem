﻿namespace Model.Data
{
  public  class FileData: IData
    {
        public string Type { get; set; }

        public string FileName { get; set; }

        public string Data => throw new System.NotImplementedException();

        public IData DeserializeFromJSON(string json)
        {
            throw new System.NotImplementedException();
        }

        public string SerializeToJSON()
        {
            throw new System.NotImplementedException();
        }
    }

    public class FileDataWrapper
    {
        public FileData Data { get; set; }

        public string Bytes { get; set; }
    }
}
