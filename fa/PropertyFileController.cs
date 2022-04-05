using System.Text.Json;
using System.IO;

namespace PropertyFileController
{
    public class LifeProperty
    {
        public LifeProperty(int BoardWidth, int BoardHeight, int BoardCellSize, double LifeDensity)
        {
            this.BoardWidth = BoardWidth;
            this.BoardHeight = BoardHeight;
            this.BoardCellSize = BoardCellSize;
            this.LifeDensity = LifeDensity;
        }
        public int BoardWidth { get; set; }
        public int BoardHeight { get; set; }
        public int BoardCellSize { get; set; }
        public double LifeDensity { get; set; }
    }

    public class JSONController
    {
        public static void SerializeToJSON(LifeProperty theLifeProperty, string theFileName = "LifeProperty.json")
        {
            using FileStream aCreateStream = File.Create(theFileName);
            JsonSerializer.Serialize(aCreateStream, theLifeProperty);
        }

        public static void DeserializeFromJSON(out LifeProperty theLifeProperty, string theFileName = "LifeProperty.json")
        {
            string aSaveStream = File.ReadAllText(theFileName);
            theLifeProperty = JsonSerializer.Deserialize<LifeProperty>(aSaveStream)!;
        }
    }
}