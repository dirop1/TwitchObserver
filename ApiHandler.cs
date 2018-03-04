using Newtonsoft.Json;
using System;
using System.Net;
using System.IO;
using System.Text;
using System.Collections.Generic;

public class ApiHandler

{
    public static bool DEBUG = true;
    public static String api_url = "https://api.twitch.tv/kraken", access_token = "", client_id = "";
    public ApiHandler()
    {
        readClientIDFile();
    }

    public void connect()
    {
        d("Connect Called");
    }
    private void readClientIDFile()
    {
        string currentDirectory = Directory.GetCurrentDirectory();
        string path = System.IO.Path.Combine(currentDirectory, "clientid.txt");
        Console.WriteLine(path);
        foreach (string line in File.ReadLines(@path))
        {            
            if (line != null && line != "")
            {
                client_id = line.Trim();
            }
        }
    }
    public Stream getStreamInfo(String channelName)
    {
        String url = "https://api.twitch.tv/kraken/streams/" + channelName + "?client_id=" + client_id;
        ApiHandler.d(url);
        String json = GET(url);
        RootObject rootObject = JsonConvert.DeserializeObject<RootObject>(json);
        d("Called api for " + channelName);
        return rootObject.stream;
    }

    public ChatObject getChatInfo(String channelName)
    {
        String url = "http://tmi.twitch.tv/group/user/" + channelName + "/chatters";
        String json = GET(url);
        ChatObject rootObject = JsonConvert.DeserializeObject<ChatObject>(json);
        d("Called api for chat of " + channelName);
        return rootObject;
    }



    /****WEB HANDLER ***/
    public static void d(String msg)
    {
         if(DEBUG)Console.WriteLine(msg );
    }
    public string GET(string url)
    {
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
        try
        {
            WebResponse response = request.GetResponse();
            using (System.IO.Stream responseStream = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                return reader.ReadToEnd();
            }
        }
        catch (WebException ex)
        {
            WebResponse errorResponse = ex.Response;
            using (System.IO.Stream responseStream = errorResponse.GetResponseStream())
            {
                StreamReader reader = new StreamReader(responseStream, Encoding.GetEncoding("utf-8"));
                String errorText = reader.ReadToEnd();
                // log errorText
            }
            throw;
        }
    }

    void POST(string url, string jsonContent)
    {
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
        request.Method = "POST";

        System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
        Byte[] byteArray = encoding.GetBytes(jsonContent);

        request.ContentLength = byteArray.Length;
        request.ContentType = @"application/json";

        using (System.IO.Stream dataStream = request.GetRequestStream())
        {
            dataStream.Write(byteArray, 0, byteArray.Length);
        }
        long length = 0;
        try
        {
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                length = response.ContentLength;
            }
        }
        catch (WebException ex)
        {
            // Log exception and throw as for GET example above
        }
    }
    public class Links
    {
        public string self { get; set; }
    }

    public class Preview
    {
        public string small { get; set; }
        public string medium { get; set; }
        public string large { get; set; }
        public string template { get; set; }
    }

    public class Links2
    {
        public string self { get; set; }
        public string follows { get; set; }
        public string commercial { get; set; }
        public string stream_key { get; set; }
        public string chat { get; set; }
        public string subscriptions { get; set; }
        public string editors { get; set; }
        public string teams { get; set; }
        public string videos { get; set; }
    }

    public class Channel
    {
        public bool mature { get; set; }
        public string status { get; set; }
        public string broadcaster_language { get; set; }
        public string display_name { get; set; }
        public string game { get; set; }
        public string language { get; set; }
        public int _id { get; set; }
        public string name { get; set; }
        public string created_at { get; set; }
        public string updated_at { get; set; }
        public object delay { get; set; }
        public string logo { get; set; }
        public object banner { get; set; }
        public string video_banner { get; set; }
        public object background { get; set; }
        public string profile_banner { get; set; }
        public object profile_banner_background_color { get; set; }
        public bool partner { get; set; }
        public string url { get; set; }
        public int views { get; set; }
        public int followers { get; set; }
        public Links2 _links { get; set; }
    }

    public class Stream
    {
        public long _id { get; set; }
        public string game { get; set; }
        public int viewers { get; set; }
        public string created_at { get; set; }
        public int video_height { get; set; }
        public decimal average_fps { get; set; }
        public int delay { get; set; }
        public bool is_playlist { get; set; }
        public Links _links { get; set; }
        public Preview preview { get; set; }
        public Channel channel { get; set; }
    }

    public class Links3
    {
        public string self { get; set; }
        public string channel { get; set; }
    }

    public class RootObject
    {
        public Stream stream { get; set; }
        public Links3 _links { get; set; }
    }







    /***** CHAT *****/


    public class LinksChat
    {
    }

    public class Chatters
    {
        public List<string> moderators { get; set; }
        public List<string> staff { get; set; }
        public List<object> admins { get; set; }
        public List<object> global_mods { get; set; }
        public List<string> viewers { get; set; }
    }

    public class ChatObject
    {
        public Links _links { get; set; }
        public int chatter_count { get; set; }
        public Chatters chatters { get; set; }
    }

}
