
namespace NetworkApp
{
    using System;
    using System.IO;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading;

    public class MultiThreadedServer
    {

        private static void ProcessClientRequests ( object argument )
        {
            TcpClient client = (TcpClient)argument;
            try
            {
                StreamReader reader = new StreamReader ( client.GetStream ( ) );
                StreamWriter writer = new StreamWriter ( client.GetStream ( ) );
                string s = String.Empty;
                while (!(s = reader.ReadLine ( )).Equals ( "Exit" ) || (s == null))
                {
                    Console.WriteLine ( "From client -> " + s );
                    string result = Program.SearchColumn ( s );
                    writer.WriteLine ( "From server -> " + result );
                    writer.Flush ( );
                }
                reader.Close ( );
                writer.Close ( );
                client.Close ( );
                Console.WriteLine ( "Closing client connection!" );
            }
            catch (IOException)
            {
                Console.WriteLine ( "Problem with client communication. Exiting thread." );
            }
            finally
            {
                if (client != null)
                {
                    client.Close ( );
                }
            }
        }



        public static void Ceva ( )
        {
            TcpListener listener = null;
            try
            {
                listener = new TcpListener ( IPAddress.Parse ( "192.168.0.103" ), 8080 );
                listener.Start ( );
                Console.WriteLine ( "MultiThreadedEchoServer started..." );
                while (true)
                {
                    Console.WriteLine ( "Waiting for incoming client connections..." );
                    TcpClient client = listener.AcceptTcpClient ( );
                    Console.WriteLine ( "Accepted new client connection..." );
                    Thread t = new Thread ( ProcessClientRequests );
                    t.Start ( client );
                }
            }
            catch (Exception e)
            {
                Console.WriteLine ( e );
            }
            finally
            {
                if (listener != null)
                {
                    listener.Stop ( );
                }
            }
        } 
    } 
}
