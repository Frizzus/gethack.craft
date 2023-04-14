using System;
using System.IO;
using System.Collections;

static class FrizzusUtils{
    public static IDictionary<string, string> getEnvArray(string envUrl){
        string[] envFile = File.ReadAllLines(Directory.GetCurrentDirectory() + envUrl);
        IDictionary<string,string> envArray = new Dictionary<string,string>();
        foreach (var line in envFile)
        {
            string[] buffer = line.Split('=');
            envArray.Add(buffer[0], buffer[1]);
        }
        return envArray;
    }
}