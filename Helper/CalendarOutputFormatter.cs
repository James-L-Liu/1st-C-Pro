using System.Globalization;
using System.Text;
using A2.Models;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;
public class CalendarOutputFormatter : TextOutputFormatter
{
    public CalendarOutputFormatter()
    {
        SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("text/calendar"));
        SupportedEncodings.Add(Encoding.UTF8);
    }

    public override Task WriteResponseBodyAsync(OutputFormatterWriteContext context, Encoding selectedEncoding)
    {
        Event e = (Event) context.Object;
        StringBuilder builder = new StringBuilder();
        builder.AppendLine("BEGIN:VCALENDAR");
        builder.AppendLine("VERSION:2.0");
        builder.AppendLine("PRODID:jliu613");
        builder.AppendLine("BEGIN:VEVENT");
        builder.Append("UID:").AppendLine(e.Id + "");
        builder.Append("DTSTAMP:").AppendLine(DateTime.Now.ToString("yyyyMMddTHHmmssZ") + "");
        builder.Append("DTSTART:").AppendLine(e.Start + "");
        builder.Append("DTEND:").AppendLine(e.End + "");
        builder.Append("SUMMARY:").AppendLine(e.Summary + "");
        builder.Append("DESCRIPTION:").AppendLine(e.Description + "");
        builder.Append("LOCATION:").AppendLine(e.Location + "");
        builder.AppendLine("END:VEVENT");
        builder.AppendLine("END:VCALENDAR");
        string outString = builder.ToString();
        byte[] outBytes = selectedEncoding.GetBytes(outString);
        var response = context.HttpContext.Response.Body;
        return response.WriteAsync(outBytes, 0, outBytes.Length);
    }
}