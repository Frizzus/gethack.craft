module Fpracticle

open System.IO
open System

let getEnvArray envUrl : Map<string, string> = 
    let file =  File.ReadLines (Directory.GetCurrentDirectory() + envUrl)
    let mutable envArray = Map []
    for line in file do
        let buffer = line.Split ':'
        // get the first part to go as a key and the second as the value
        
        envArray <- envArray.Add (buffer.[0], buffer.[1])     
    envArray