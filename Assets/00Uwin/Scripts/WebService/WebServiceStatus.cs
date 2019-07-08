public class WebServiceStatus {

	public enum Status : int
	{
	    ERROR = 0,

		OK = 1,

        AUTHORIZATION_EXCEPTION = -1000,
        SERVER_EXCEPTION = -1001,
        INTERNET_ERROR = -1002,
    }
}
