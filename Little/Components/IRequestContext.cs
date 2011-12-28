namespace Little
{
   public interface IRequestContext
   {
      string ApiVersion{get;}
      string Secret { get; }
      string Key { get; }
   }
}