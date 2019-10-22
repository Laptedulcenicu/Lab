using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Threading;

namespace NetworkApp
{
    class Program
    {
        public static List<JObject> Result = new List<JObject> ( );
        public static int isDone = 0;

        static void Main ( string[] args )
        {
            string access_token = "";
            using (var httpClient = new HttpClient ( ))
            {
                string responseBody = httpClient.GetStringAsync ( "http://localhost:5000/register" ).Result;
                JObject json = JObject.Parse ( responseBody );
                access_token = json["access_token"].ToString ( );
                Console.WriteLine ( "Access token: " + json["access_token"] );
                SendRequest ( "/home", access_token );
            }
            Console.ReadLine ( );
         
        }

        public static void SendRequest ( string route, string access_token )
        {
            using (var httpClient = new HttpClient ( ))
            {
                Console.WriteLine ( "Sending request to " + route );
                httpClient.DefaultRequestHeaders.Add ( "X-Access-Token", access_token );
                string responseBody = httpClient.GetStringAsync ( "http://localhost:5000" + route ).Result;
                JObject jsonResponse = JObject.Parse ( responseBody );
                if (jsonResponse["link"] != null)
                {
                    JObject array = (JObject)jsonResponse["link"];
                    foreach (var item in array.Children ( ))
                    {
                        SendRequestWithNewThread ( item.First.ToString ( ), access_token );
                    }
                }
                if (jsonResponse["data"] != null)
                {
                    Result.Add ( Functions.GetData ( jsonResponse ) );
                }
            }
        }

        public static void SendRequestWithNewThread ( string route, string access_token )
        {
            var thread = new System.Threading.Thread ( ( ) =>
            {
                isDone++;
                SendRequest ( route, access_token );
                isDone--;
                if (isDone == 0)
                {
                   
                    PrintData ( );
                    MultiThreadedServer.Ceva ( );


                }
            } );
            thread.Start ( );

        }

        public static void PrintData ( )
        {  
            Result.ForEach ( x => Console.WriteLine ( x + "\n--------------------------------" ) );
            Console.Write ( "Process done." );
        }

        public static string SearchColumn(string key )
        {

            string result = "";
            string newline = ", ";
           
            for(int i = 0; i < Result.Count ; i++)
            {
                if (Result[i][key] != null)
                {

                    result += Result[i][key].ToString ( );
                      result+= newline;

                }
            }
            return result; 
        }


    }
}
