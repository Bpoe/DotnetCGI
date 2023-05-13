namespace Dotnet.Cgi
{
    using System;

    public static class Result
    {
        public static void WriteResponse(HttpResponseMessage response)
        {
            Console.WriteLine("Status: {0} {1}", (int)response.StatusCode, ReasonPhrases.GetReasonPhrase((int)response.StatusCode));

            foreach (var h in response.Headers)
            {
                Console.WriteLine("{0}: {1}", h.Key, string.Concat(h.Value));
            }

            if (response.Content != null)
            {
                foreach (var h in response.Content.Headers)
                {
                    Console.WriteLine("{0}: {1}", h.Key, string.Concat(h.Value));
                }

                Console.WriteLine();
                Console.WriteLine(response.Content.ReadAsStringAsync().Result);
            }
        }
    }
}
