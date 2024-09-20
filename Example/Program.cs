﻿// Copyright (c) 2013-2022 Cemalettin Dervis, MIT License.
// https://github.com/cemderv/SharpConfig

using SharpConfig;

class SomeClass
{
  public string SomeString { get; set; }

  public int SomeInt { get; set; }

  public int[] SomeInts { get; set; }

  public DateTime SomeDate { get; set; }

  // This field will be ignored by SharpConfig
  // when creating sections from objects and vice versa.
  [SharpConfig.Ignore]
  public int SomeIgnoredField;

  // Same for this property.
  [SharpConfig.Ignore]
  public int SomeIgnoredProperty { get; set; }
}

internal static class Program
{
  public static void Main()
  {
    // Call the methods in this file here to see their effect.

    //HowToLoadAConfig();
    //HowToCreateAConfig();
    //HowToSaveAConfig(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "TestCfg.ini"));
    //HowToCreateObjectsFromSections();
    //HowToCreateSectionsFromObjects();
    //HowToHandleArrays();

    Console.ReadLine();
  }

  /// <summary>
  /// Shows how to load a configuration from a string (our sample config).
  /// The same works for loading files and streams. Just call the appropriate method
  /// such as LoadFromFile and LoadFromStream, respectively.
  /// </summary>
  private static void HowToLoadAConfig()
  {
    // Read our example config.
    Configuration cfg = Configuration.LoadFromFile("SampleCfg.txt");

    // Just print all sections and their settings.
    PrintConfig(cfg);
  }

  /// <summary>
  /// Shows how to create a configuration in memory.
  /// </summary>
  private static void HowToCreateAConfig()
  {
    var cfg = new Configuration();

    cfg["SomeStructure"]["SomeString"].StringValue = "foobar";
    cfg["SomeStructure"]["SomeInt"].IntValue = 2000;
    cfg["SomeStructure"]["SomeInts"].IntValueArray = new[] { 1, 2, 3 };
    cfg["SomeStructure"]["SomeDate"].DateTimeValue = DateTime.Now;

    // We can obtain the values directly as strings, ints, floats, or any other (custom) type,
    // as long as the string value of the setting can be converted to the type you wish to obtain.
    string nameValue = cfg["SomeStructure"]["SomeString"].StringValue;

    int ageValue = cfg["SomeStructure"]["SomeInt"].IntValue;

    DateTime dateValue = cfg["SomeStructure"]["SomeDate"].DateTimeValue;

    // Print our config just to see that it works.
    PrintConfig(cfg);
  }

  /// <summary>
  /// Shows how to save a configuration to a file.
  /// </summary>
  /// <param name="filename">The destination filename.</param>
  private static void HowToSaveAConfig(string filename)
  {
    var cfg = new Configuration();

    cfg["SomeStructure"]["SomeString"].StringValue = "foobar";
    cfg["SomeStructure"]["SomeInt"].IntValue = 2000;
    cfg["SomeStructure"]["SomeInts"].IntValueArray = new[] { 1, 2, 3 };
    cfg["SomeStructure"]["SomeDate"].DateTimeValue = DateTime.Now;

    cfg.SaveToFile(filename);

    Console.WriteLine("The config has been saved to {0}!", filename);
  }

  /// <summary>
  /// Shows how to create C#/.NET objects directly from sections.
  /// </summary>
  private static void HowToCreateObjectsFromSections()
  {
    var cfg = new Configuration();

    // Create the section.
    cfg["SomeStructure"]["SomeString"].StringValue = "foobar";
    cfg["SomeStructure"]["SomeInt"].IntValue = 2000;
    cfg["SomeStructure"]["SomeInts"].IntValueArray = new[] { 1, 2, 3 };
    cfg["SomeStructure"]["SomeDate"].DateTimeValue = DateTime.Now;

    // Now create an object from it.
    var p = cfg["SomeStructure"].ToObject<SomeClass>();

    // Test.
    Console.WriteLine("SomeString:   " + p.SomeString);
    Console.WriteLine("SomeInt:      " + p.SomeInt);
    PrintArray("SomeInts", p.SomeInts);
    Console.WriteLine("SomeDate:     " + p.SomeDate);
  }

  /// <summary>
  /// Shows how to create sections directly from C#/.NET objects.
  /// </summary>
  private static void HowToCreateSectionsFromObjects()
  {
    var cfg = new Configuration();

    // Create an object.
    var p = new SomeClass
    {
      SomeString = "foobar",
      SomeInt = 2000,
      SomeInts = new[] { 1, 2, 3 },
      SomeDate = DateTime.Now
    };

    // Now create a section from it.
    cfg.Add(Section.FromObject("SomeStructure", p));

    // Print the config to see that it worked.
    PrintConfig(cfg);
  }

  /// <summary>
  /// Shows the usage of arrays in SharpConfig.
  /// </summary>
  private static void HowToHandleArrays()
  {
    var cfg = new Configuration();

    cfg["GeneralSection"]["SomeInts"].IntValueArray = new[] { 1, 2, 3 };

    // Get the array back.
    int[] someIntValuesBack = cfg["GeneralSection"]["SomeInts"].GetValueArray<int>();
    float[] sameValuesButFloats = cfg["GeneralSection"]["SomeInts"].GetValueArray<float>();
    string[] sameValuesButStrings = cfg["GeneralSection"]["SomeInts"].GetValueArray<string>();

    // There is also a non-generic variant of GetValueArray:
    object[] sameValuesButObjects = cfg["GeneralSection"]["SomeInts"].GetValueArray(typeof(int));

    PrintArray("someIntValuesBack", someIntValuesBack);
    PrintArray("sameValuesButFloats", sameValuesButFloats);
    PrintArray("sameValuesButStrings", sameValuesButStrings);
    PrintArray("sameValuesButObjects", sameValuesButObjects);
  }

  /// <summary>
  /// Prints all sections and their settings to the console.
  /// </summary>
  /// <param name="cfg">The configuration to print.</param>
  private static void PrintConfig(Configuration cfg)
  {
    foreach (Section section in cfg)
    {
      Console.WriteLine("[{0}]", section.Name);

      foreach (Setting setting in section)
      {
        Console.Write("  ");

        if (setting.IsArray)
          Console.Write("[Array, {0} elements] ", setting.ArraySize);

        Console.WriteLine(setting.ToString());
      }

      Console.WriteLine();
    }
  }

  // Small helper method for printing objects of an arbitrary type.
  private static void PrintArray<T>(string arrName, IReadOnlyList<T> arr)
  {
    Console.Write(arrName + " = { ");

    for (int i = 0; i < arr.Count - 1; i++)
      Console.Write(arr[i] + ", ");

    Console.Write(arr[^1]!.ToString());
    Console.WriteLine(" }");
  }
}
