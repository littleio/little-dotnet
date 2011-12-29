
namespace Little
{
   public interface IDriverData
   {
      string Url{get;}
   }

   public class DriverData : IDriverData
   {
      public const string APIURL = "https://api.little.io/api/";
      private string _url;

      public string Url
      {
         get { return _url ?? APIURL; }
         set { _url = value; }
      }

      public void Reset()
      {
         _url = null;
      }
   }
}