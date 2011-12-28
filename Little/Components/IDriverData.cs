
namespace Little
{
   public interface IDriverData
   {
      string Url{get;}
   }

   public class DriverData : IDriverData
   {
      public const string APIURL = "http://192.168.1.102:3000/api/";
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