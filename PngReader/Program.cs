using PngReader;
using PngReader.lib;
using System.Windows;

string inputFile = "test.png";
byte[] bytes = File.ReadAllBytes(inputFile);
PNGParser.ReadPNG(bytes);
//Console.WriteLine(PNGParser.IsValidPNG(bytes) ? "File is a valid PNG file, continuing..." : "The file isn't a PNG file or is an invalid PNG file.");
//var pngData = PNGParser.ReadPNGData(bytes);
//Console.WriteLine(System.Text.Encoding.UTF8.GetString(pngData.chunks[0].type));