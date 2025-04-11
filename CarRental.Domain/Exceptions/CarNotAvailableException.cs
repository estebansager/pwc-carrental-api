namespace CarRental.Domain.Exceptions
{
    public class CarNotAvailableException : Exception
    {
        public CarNotAvailableException(string message) : base(message)
        {
                
        }

    }

}
