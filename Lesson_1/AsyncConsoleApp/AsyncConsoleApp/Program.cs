﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace AsyncConsoleApp
{
    class Program
    {
        private static HttpClient _client;
        private static readonly Uri Uri = new Uri("https://jsonplaceholder.typicode.com/posts/");
        private static readonly string FilePath = "result.txt";
        private const int StartNumberOfPost = 4;
        private const int NumOfPosts = 10;

        static async Task Main()
        {
            try
            {
                _client = new HttpClient();

                var tasks = GetPostTasks(NumOfPosts, StartNumberOfPost, Uri);
                await Task.WhenAll(tasks);
                
                Console.WriteLine("Downloading is completed");

                var data = new List<JsonPostData>();
                for (int i = 0; i < tasks.Length; i++)
                {
                    if(tasks[i].IsCompletedSuccessfully)
                    {
                        data.Add(JsonConvert.DeserializeObject<JsonPostData>(tasks[i].Result));
                    }
                }
                
                Console.WriteLine("Parsing is completed");
                
                await WriteDataToFile(data, Path.Combine(Environment.CurrentDirectory, FilePath));
                
                Console.WriteLine("Data was written to file result.txt");
            }
            catch (Exception e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
            }
            finally
            {
                _client?.Dispose();
            }
        }

        private static Task<string>[] GetPostTasks(int numOfTasks, int startPost, Uri baseUri) =>
            Enumerable
                .Range(startPost, numOfTasks + startPost)
                .Select(i => new Uri(baseUri, i.ToString()))
                .Select(uri => _client.GetStringAsync(uri))
                .ToArray();
        
        private static async Task WriteDataToFile(List<JsonPostData> data, string filePath)
        {
            await using StreamWriter writer = new StreamWriter(File.Open(filePath, FileMode.Create));
            foreach (var postData in data)
            {
                await writer.WriteLineAsync(postData.UserId.ToString());
                await writer.WriteLineAsync(postData.Id.ToString());
                await writer.WriteLineAsync(postData.Title);
                await writer.WriteLineAsync(postData.Body);
                await writer.WriteLineAsync();
            }
        }
        
        private class JsonPostData
        {
            public int UserId { get; set; }
            public int Id { get; set; }
            public string Title { get; set; }
            public string Body { get; set; }
        }
    }
}